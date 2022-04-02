using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JiraHelper.Core.Business.Strategy;
using Microsoft.Extensions.Logging;

namespace JiraHelper.Core.Business
{
	/// <summary>
	/// Manager composing available jira strategies
	/// </summary>
	public class JiraStrategiesManager
	{
		/// <summary>
		/// The logger
		/// </summary>
		private readonly ILogger<JiraStrategiesManager> _logger;
		/// <summary>
		/// The background strategies
		/// </summary>
		private readonly IEnumerable<IBackgroundStrategy> _bgStrategies;
		/// <summary>
		/// The active strategies
		/// </summary>
		private readonly IEnumerable<IActiveStrategy> _activeStrategies;

		/// <summary>
		/// Initializes a new instance of the <see cref="JiraStrategiesManager"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="strategies">The strategies.</param>
		/// <exception cref="System.ArgumentNullException">logger</exception>
		public JiraStrategiesManager(
			ILogger<JiraStrategiesManager> logger,
			IEnumerable<IBackgroundStrategy> bgStrategies,
			IEnumerable<IActiveStrategy> activeStrategies)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_bgStrategies = bgStrategies ?? new List<IBackgroundStrategy>();
			_activeStrategies = activeStrategies ?? new List<IActiveStrategy>();
		}

		/// <summary>
		/// Running all available strategies.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		public void RunAllBackgroundStrategies(CancellationToken cancellationToken)
		{
			_logger.LogDebug($"Start JiraCheckersManager.RunAllBackgroundStrategies");
			if (!cancellationToken.IsCancellationRequested)
			{
				var tasks = new ConcurrentBag<Task>();
				foreach (var strategy in _bgStrategies)
				{
					tasks.Add(strategy.Run(cancellationToken));
					_logger.LogDebug($"Scheduling {strategy}");
				}

				_logger.LogDebug($"Awaiting all");
				Task.WaitAll(tasks.ToArray(), cancellationToken);
			}
			_logger.LogDebug($"Finish JiraCheckersManager.RunAllBackgroundStrategies");
		}

		/// <summary>
		/// Running all available strategies.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		public Task<object> RunStrategy(string key, CancellationToken cancellationToken)
		{
			var strategy = _activeStrategies.SingleOrDefault(w => w.Key == key);

			if(strategy == null)
			{
				throw new KeyNotFoundException($"Strategy with key '{key}' has not found or not implementing IActiveStrategy interface.");
			}

			if (!cancellationToken.IsCancellationRequested)
			{
				return strategy.Run(cancellationToken);
			}

			return Task.FromResult(default(object));
		}
	}
}
