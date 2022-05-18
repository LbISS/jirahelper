using Atlassian.Jira;
using CommandLine;
using JiraHelper.Core.Business;
using JiraHelper.Core.Config;
using JiraHelper.Core.Rest.JiraServices;
using JiraHelper.Core.Rest.MSTeams;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JiraHelper.Core
{
	/// <summary>
	/// Handling common stratup logic for both console and web application
	/// </summary>
	public static class Startup
	{
		/// <summary>
		/// The default configuration file name
		/// </summary>
		private const string DEFAULT_CONFIG = "config.json";
		/// <summary>
		/// The folder name to search for custom strategies
		/// </summary>
		private const string STRATEGIES_FOLDER = "strategies";

		private static readonly object _logLevelLock = new object();
		private static LogLevel? _logLevel;

		private static string AppPath
		{
			get
			{
				return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
		}

		/// <summary>
		/// The log level for stratup operations.
		/// </summary>
		private static LogLevel LogLevel
		{
			get
			{
				if (_logLevel == null)
				{
					lock (_logLevelLock)
					{
						if (_logLevel == null)
						{
							// W/A for static logger on host build phase.
							var isDev = string.Equals(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"), "development", StringComparison.InvariantCultureIgnoreCase);

							var builder = new ConfigurationBuilder()
								.SetBasePath(Directory.GetCurrentDirectory())
								.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
								.AddJsonFile($"appsettings.{(isDev ? "Development" : "Production")}.json", optional: true);

							IConfiguration config = builder.Build();

							_logLevel = config.GetSection("Logging")?.GetSection("Console")?.GetSection("LogLevel")?.GetValue<LogLevel>("Default") ?? LogLevel.Information;

						}
					}
				}
				return _logLevel ?? LogLevel.Information;
			}
		}

		private static readonly object _loggerLock = new object();
		private static ILogger _logger;
		/// <summary>
		/// Gets the logger for startup operations.
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
								builder.SetMinimumLevel(LogLevel);
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
			var startupArgs = ParseArguments(args);

			var config = LoadConfigFile(startupArgs);

			return Host.CreateDefaultBuilder(args)
				.ConfigureServices((host, services) =>
				{
					//Add non-configurable services
					services = services
						.AddScoped<IssuesRestService, IssuesRestService>()
						.AddSingleton<WebHookService, WebHookService>()
						.AddScoped<JiraStrategiesManager, JiraStrategiesManager>()
						.AddHostedService<JiraBackgroundJob>();

					//Add Jira client
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

					// Load strategies
					List<Assembly> stratAssemblies = LoadAssembliesForSpecificStrategies();
					stratAssemblies.Add(AppDomain.CurrentDomain.GetAssemblies().First(f => f.GetName().Name.Equals("JiraHelper.Core")));

					var configResolver = new ConfigResolver(Logger, stratAssemblies);

					foreach (var stratConfig in config.Strategies)
					{
						try
						{
							services = services.AddScoped(
										Type.GetType(stratConfig.Mode),
										(serviceProvider) =>
										{
											var resolution = configResolver.ResolveConstructor(stratConfig, serviceProvider);
											if (resolution.Item1)
											{
												return resolution.Item2;
											}
											else
											{
												throw new TypeLoadException($"No appropriate constructor have been found for strategy '{stratConfig.Type}'.");
											}
										});
						}
						catch (Exception exc)
						{
							Logger.LogError($"Error loading strategy '{stratConfig.Key}': ", exc);
						}

					}
				});
		}

		/// <summary>
		/// Parses the arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		private static StartupArgs ParseArguments(string[] args)
		{
			var startupArgs = Parser.Default.ParseArguments<StartupArgs>(args)
				.MapResult(runnerArgs => runnerArgs, errors =>
				{
					Logger.LogWarning($"Errors parsing parameters: ", errors);
					return new StartupArgs();
				});

			startupArgs.ConfigFile = String.IsNullOrWhiteSpace(startupArgs.ConfigFile) ? Path.Combine(AppPath, DEFAULT_CONFIG) : startupArgs.ConfigFile;
			return startupArgs;
		}

		/// <summary>
		/// Loads the configuration file.
		/// </summary>
		/// <param name="startupArgs">The startup arguments.</param>
		/// <returns></returns>
		private static Config.Config LoadConfigFile(StartupArgs startupArgs)
		{
			var config = new Config.Config();

			try
			{
				if (File.Exists(startupArgs.ConfigFile))
				{
					config = System.Text.Json.JsonSerializer.Deserialize<Config.Config>(File.ReadAllText(startupArgs.ConfigFile));
				}
			}
			catch (NotSupportedException exc)
			{
				Logger.LogError($"Error during parsing config file {exc.Message}", exc);
			}

			return config;
		}

		/// <summary>
		/// Loads the assemblies for dynamic specific strategies.
		/// </summary>
		/// <returns></returns>
		private static List<Assembly> LoadAssembliesForSpecificStrategies()
		{
			List<Assembly> allAssemblies = new List<Assembly>();
			string strategiesPath = Path.Combine(AppPath, STRATEGIES_FOLDER);

			if (!Directory.Exists(strategiesPath))
			{
				Logger.LogInformation($"No custom strategies will be loaded - '{strategiesPath}' folder not found.");
				return new List<Assembly>();
			}

			foreach (string dll in Directory.GetFiles(strategiesPath, "*.dll"))
			{
				try
				{
					allAssemblies.Add(Assembly.LoadFile(dll));
					Logger.LogInformation($"Strategies from dll '{Path.GetFileName(dll)}' have been successfully loaded.");
				}
				catch (Exception exc)
				{
					Logger.LogError($"Error loading strategies from dll '{Path.GetFileName(dll)}'.", exc);
				}
			}

			return allAssemblies;
		}
	}
}
