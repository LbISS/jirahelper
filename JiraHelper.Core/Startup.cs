using Atlassian.Jira;
using CommandLine;
using JiraHelper.Core.Business;
using JiraHelper.Core.Rest.JiraServices;
using JiraHelper.Core.Rest.MSTeams;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace JiraHelper.Core
{
	public static class Startup
	{
		private const string DEFAULT_CONFIG = "config.json";
		private const string STRATEGIES_FOLDER = "strategies";

		private static readonly object _loggerLock = new Object();
		private static ILogger _logger;
		/// <summary>
		/// Gets the logger.
		/// </summary>
		private static ILogger Logger
		{
			get
			{
				if (_logger == null)
				{
					lock (_loggerLock)
					{
						if (_logger == null)
						{

							using var loggerFactory = LoggerFactory.Create(builder =>
							{
								builder.SetMinimumLevel(LogLevel.Information);
								builder.AddConsole();
							});
							_logger = loggerFactory.CreateLogger("Startup");
						}
					}
				}
				return _logger;
			}
		}

		/// <summary>
		/// Initializes common host builder for console/web/etc apps.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public static IHostBuilder CreateHostBuilderCommon(string[] args)
		{
			var startupArgs = Parser.Default.ParseArguments<StartupArgs>(args)
				.MapResult(runnerArgs => runnerArgs, errors =>
				{
					Logger.LogWarning($"Errors parsing parameters: ", errors);
					return new StartupArgs();
				});

			startupArgs.ConfigFile = String.IsNullOrWhiteSpace(startupArgs.ConfigFile) ? DEFAULT_CONFIG : startupArgs.ConfigFile;

			var config = new Config();

			try
			{
				if (File.Exists(startupArgs.ConfigFile))
				{
					config = System.Text.Json.JsonSerializer.Deserialize<Config>(File.ReadAllText(startupArgs.ConfigFile));
				}
			}
			catch (NotSupportedException exc)
			{
				Logger.LogError($"Error during parsing config file {exc.Message}", exc);
			}


			return Host.CreateDefaultBuilder(args)
				.ConfigureServices((host, services) =>
				{
					services = services
						.AddScoped<IssuesRestService, IssuesRestService>()
						.AddSingleton<WebHookService, WebHookService>()
						.AddScoped<JiraStrategiesManager, JiraStrategiesManager>()
						.AddHostedService<JiraBackgroundJob>();

					if (String.IsNullOrWhiteSpace(config.Jira.Uri))
					{
						Logger.LogInformation($"Empty jira Uri provided");
					}

					services = services.AddScoped<Jira, Jira>(
						(serviceProvider) => Jira.CreateRestClient(
												config.Jira.Uri,
												config.Jira.User,
												config.Jira.Password,
												new JiraRestClientSettings { EnableRequestTrace = config.Jira.EnableTrace }
						)
					 );

					LoadAssembliesForSpecificStrategies();

					foreach (var stratConfig in config.Strategies)
					{
						services = services.AddScoped(
										Type.GetType(stratConfig.Mode),
										(serviceProvider) => Type.GetType(stratConfig.Type).GetConstructor(BindingFlags.Public, Type.EmptyTypes)
									);
					}
				});
		}

		private static List<Assembly> LoadAssembliesForSpecificStrategies()
		{
			List<Assembly> allAssemblies = new List<Assembly>();
			string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string strategiesPath = Path.Combine(currentPath, STRATEGIES_FOLDER);

			foreach (string dll in Directory.GetFiles(strategiesPath, "*.dll"))
				allAssemblies.Add(Assembly.LoadFile(dll));

			return allAssemblies;
		}
	}
}
