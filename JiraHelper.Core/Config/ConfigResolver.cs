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
	/// <summary>
	/// Responsible for resolving all services and parameters from passed config for further regitration in hostbuilder.
	/// </summary>
	public class ConfigResolver
	{
		protected readonly ILogger Logger;
		protected readonly List<Assembly> Assemblies;
		protected readonly Dictionary<string, Type> AssembliesTypes;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigResolver"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="assemblies">The assemblies.</param>
		/// <exception cref="System.ArgumentNullException">
		/// logger
		/// or
		/// assemblies
		/// </exception>
		public ConfigResolver(ILogger logger, List<Assembly> assemblies)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
			AssembliesTypes = Assemblies.SelectMany(s => s.ExportedTypes).ToDictionary(k => k.FullName, v => v);
		}

		/// <summary>
		/// Resolves the constructor for registration.
		/// </summary>
		/// <param name="jsonConfig">The json configuration.</param>
		/// <param name="serviceProvider">The service provider.</param>
		/// <returns></returns>
		public Tuple<bool, object> ResolveConstructor(ParameterizedEntityConfig jsonConfig, IServiceProvider serviceProvider)
		{
			Logger.LogDebug($"Finding constructor for '{jsonConfig.Type}'.");
			var constructors = FindType(jsonConfig.Type).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
			Logger.LogDebug($"Found {constructors.Length} in total for '{jsonConfig.Type}'. Resolving parameter values...");

			foreach (var constructor in constructors)
			{
				try
				{
					var parameters = constructor.GetParameters();
					Logger.LogDebug($"Resolving {parameters.Length} parameters for constructor.");
					var parameterValues = new List<object>(parameters.Length);

					foreach (var parameter in parameters)
					{
						var paramResolution = ResolveParameter(parameter, jsonConfig, serviceProvider);
						if (!paramResolution.Item1)
							break;

						parameterValues.Add(paramResolution.Item2);
					}

					if (parameters.Length != parameterValues.Count)
					{
						Logger.LogDebug($"Constructor '{jsonConfig.Type}' is not resolved. " +
							$"Resolved only {parameterValues.Count} parameters out of {parameters.Length}.");
						return new Tuple<bool, object>(false, null);
					}

					var result = constructor.Invoke(parameterValues.ToArray());
					Logger.LogDebug($"Resolved constructor '{jsonConfig.Type}' with '{result}'.");
					return new Tuple<bool, object>(true, result);
				}
				catch (Exception exc)
				{
					Logger.LogWarning($"Error on resolving constructor '{jsonConfig.Type}'.", exc);
				}
			}

			return new Tuple<bool, object>(false, null);
		}

		/// <summary>
		/// Resolves the parameter.
		/// </summary>
		/// <param name="parameterInfo">The parameter information.</param>
		/// <param name="entityConfig">The parameter configuration.</param>
		/// <param name="serviceProvider">The service provider.</param>
		/// <returns></returns>
		public Tuple<bool, object> ResolveParameter(ParameterInfo parameterInfo, ParameterizedEntityConfig entityConfig, IServiceProvider serviceProvider)
		{
			Logger.LogDebug($"Getting value for parameter '{parameterInfo.Name}' with type '{parameterInfo.ParameterType}'.");

			object value = null;
			bool resolved = false;

			if (parameterInfo.Name.Equals("Key", StringComparison.OrdinalIgnoreCase))
			{
				value = entityConfig.Key;
				resolved = !string.IsNullOrEmpty(entityConfig.Key);
			}
			else if (parameterInfo.ParameterType == typeof(string))
			{
				JsonNode proposedParameter = GetParameterValue(entityConfig.Parameters, parameterInfo.Name);
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
				var constructorResolution = ResolveConstructor(GetEntityConfig(GetParameterValue(entityConfig.Parameters, parameterInfo.Name)), serviceProvider);
				value = constructorResolution.Item2;
				resolved = constructorResolution.Item1;
			}
			else if (parameterInfo.ParameterType == typeof(JsonNode)
					|| parameterInfo.ParameterType == typeof(JsonObject))
			{
				value = entityConfig.Parameters;
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

		/// <summary>
		/// Finds the type in all assemblies by name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		private Type FindType(string name)
		{
			return AssembliesTypes[name];
		}

		/// <summary>
		/// Gets the entity configuration.
		/// </summary>
		/// <param name="node">The JsonNode.</param>
		/// <returns></returns>
		private static ParameterizedEntityConfig GetEntityConfig(JsonNode node)
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

		/// <summary>
		/// Gets the parameter value from JsonNode by name.
		/// </summary>
		/// <param name="node">The JsonNode.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		private static JsonNode GetParameterValue(JsonObject node, string name)
		{
			KeyValuePair<string, JsonNode> param = node.FirstOrDefault(w => w.Key.Equals(name, StringComparison.OrdinalIgnoreCase));
			return !default(KeyValuePair<string, JsonNode>).Equals(param) ? param.Value : null;
		}
	}
}
