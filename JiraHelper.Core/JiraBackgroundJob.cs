using JiraHelper.Core.Business;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JiraHelper
{
	/// <summary>
	/// Job running periodic checks
	/// </summary>
	/// <seealso cref="Microsoft.Extensions.Hosting.BackgroundService" />
	public class JiraBackgroundJob : BackgroundService
	{
		private readonly IServiceProvider _services;
		private const int MS_REQUEST_DELAY = 5 * 60 * 1000; //5 minutes
		private readonly ILogger<JiraBackgroundJob> _logger;
		private bool firstRun = true;

		public JiraBackgroundJob(IServiceProvider services, ILogger<JiraBackgroundJob> logger)
		{
			_services = services;
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				using (var scope = _services.CreateScope())
				using (_logger.BeginScope($"Scope {Guid.NewGuid()}"))
				{
					_logger.LogDebug($"Job start");
					var scopedProcessingService =
						scope.ServiceProvider
							.GetRequiredService<JiraStrategiesManager>();

					scopedProcessingService.RunAllBackgroundStrategies(cancellationToken);
				}

				_logger.LogDebug($"Job end. Next run scheduled on {DateTime.Now.AddMilliseconds(MS_REQUEST_DELAY)}");

				if (firstRun)
				{
					_logger.LogInformation($"First job has been run. The program will run silently further until found some changes in the issues.");
					firstRun = false;
				}
				await Task.Delay(MS_REQUEST_DELAY, cancellationToken);
			}
		}
	}
}
