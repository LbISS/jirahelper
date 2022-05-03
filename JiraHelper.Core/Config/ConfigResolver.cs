using JiraHelper.Core.Business.Actions;
using JiraHelper.Core.Business.Checkers;
using JiraHelper.Core.Business.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;

namespace JiraHelper.Core.Config
{
	public class ConfigResolver
	{
		protected readonly ILogger Logger;
		protected readonly List<Assembly> Assemblies;
		protected readonly Dictionary<string, Type> AssembliesTypes;

		public ConfigResolver(ILogger logger, List<Assembly> assemblies)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
			AssembliesTypes = Assemblies.SelectMany(s => s.ExportedTypes).ToDictionary(k => k.FullName, v => v);
		}

		/// <summary>
		/// Gets the constructor.
		/// </summary>
		/// <param name="jsonConfig">The strat configuration.</param>
		/// <returns></returns>
		/// <exception cref="System.TypeLoadException">No appropriate constructor</exception>
		public Tuple<bool, object> ResolveConstructor(ParameterizedEntityConfig jsonConfig, IServiceProvider serviceProvider)
		{
			Logger.LogDebug($"Finding constructor for '{jsonConfig.Type}'.");
			var constructors = FindType(jsonConfig.Type).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
			Logger.LogDebug($"Found {constructors.Length} in total for '{jsonConfig.Type}'. Resolving parameter values...");

			foreach (var constuctor in constructors)
			{
				try
				{
					var parameters = constuctor.GetParameters();
					Logger.LogDebug($"Resolving constructor with {parameters.Length} parameters.");
					var parameterValues = new List<object>(parameters.Length);

					foreach (var parameter in parameters)
					{
						var paramResolution = ResolveParameter(parameter, jsonConfig, serviceProvider);
						if (!paramResolution.Item1)
							break;

						parameterValues.Add(paramResolution.Item2);
					}

					if (parameters.Length == parameterValues.Count)
					{
						var result = constuctor.Invoke(parameterValues.ToArray());
						Logger.LogDebug($"Constructor '{jsonConfig.Type}' resolved with '{result}'.");
						return new Tuple<bool, object>(true, result);
					}

					Logger.LogDebug($"Constructor '{jsonConfig.Type}' is not resolved.");
					return new Tuple<bool, object>(false, null);

				}
				catch (Exception exc)
				{
					Logger.LogWarning($"Error on resolving constructor '{jsonConfig.Type}'", exc);
				}
			}

			return new Tuple<bool, object>(false, null);
		}

		public Tuple<bool, object> ResolveParameter(ParameterInfo parameterInfo, ParameterizedEntityConfig jsonTypeConfig, IServiceProvider serviceProvider)
		{
			Logger.LogDebug($"Getting value for parameter '{parameterInfo.Name}' with type '{parameterInfo.ParameterType}'.");

			object value = null;
			bool resolved = false;

			if (parameterInfo.Name.Equals("Key", StringComparison.OrdinalIgnoreCase))
			{
				value = jsonTypeConfig.Key;
				resolved = !string.IsNullOrEmpty(jsonTypeConfig.Key);
			}
			else if (parameterInfo.ParameterType == typeof(string))
			{
				JsonNode proposedParameter = GetParameterValue(jsonTypeConfig.Parameters, parameterInfo.Name);
				if (proposedParameter != null)
				{
					value = proposedParameter.GetValue<string>();
					resolved = true;
				}
			}
			else if (parameterInfo.ParameterType.IsAssignableTo(typeof(IChecker))
				|| parameterInfo.ParameterType.IsAssignableTo(typeof(IStorage))
				|| parameterInfo.ParameterType.IsAssignableTo(typeof(IAction)))
			{
				var constructorResolution = ResolveConstructor(GetJsonTypeConfig(GetParameterValue(jsonTypeConfig.Parameters, parameterInfo.Name)), serviceProvider);
				value = constructorResolution.Item2;
				resolved = constructorResolution.Item1;
			}
			else if (parameterInfo.ParameterType == typeof(JsonNode)
					|| parameterInfo.ParameterType == typeof(JsonObject))
			{
				value = jsonTypeConfig.Parameters;
				resolved = true;
			}
			else
			{
				value = serviceProvider.GetService(parameterInfo.ParameterType);
				resolved = true;
			}

			if (resolved)
			{
				Logger.LogDebug($"Parameter '{parameterInfo.Name}' resolved with value '{value}'.");
			}
			else
			{
				Logger.LogDebug($"Parameter '{parameterInfo.Name}' is not resolved.");
			}

			return new Tuple<bool, object>(resolved, value);
		}

		private Type FindType(string name)
		{
			return AssembliesTypes[name];
		}

		private static ParameterizedEntityConfig GetJsonTypeConfig(JsonNode node)
		{
			var result = new ParameterizedEntityConfig
			{
				Key = node["Key"]?.ToString(),
				Mode = node["Mode"]?.ToString(),
				Type = node["Type"]?.ToString(),
				Parameters = node["Parameters"]?.AsObject()
			};
			return result;
		}

		private static JsonNode GetParameterValue(JsonObject node, string name)
		{
			KeyValuePair<string, JsonNode> param = node.FirstOrDefault(w => w.Key.Equals(name, StringComparison.OrdinalIgnoreCase));
			return !default(KeyValuePair<string, JsonNode>).Equals(param) ? param.Value : null;
		}
	}
}
