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
		/// Running all available background strategies.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		public void RunAllBackgroundStrategies(CancellationToken cancellationToken)
		{
			_logger.LogDebug($"Start JiraCheckersManager.RunAllBackgroundStrategies");
			if (!cancellationToken.IsCancellationRequested)
			{
				try
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
				catch (AggregateException aexc)
				{
					aexc.Handle((exc) =>
					{
						_logger.LogError(exc, "");
						return true;
					});

				}
				catch (Exception exc)
				{
					_logger.LogError(exc, "");
				}
			}
			_logger.LogDebug($"Finish JiraCheckersManager.RunAllBackgroundStrategies");
		}

		/// <summary>
		/// Running specific active strategy by key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">Strategy with key '{key}' has not found or not implementing IActiveStrategy interface.</exception>
		public Task<object> RunStrategy(string key, CancellationToken cancellationToken)
		{
			var strategy = _activeStrategies.Where(w => w.Key == key).ToList();

			if (strategy == null || strategy.Count == 0)
			{
				throw new KeyNotFoundException($"Strategy with key '{key}' has not found or it's not implementing IActiveStrategy interface.");
			}

			if (strategy.Count > 1)
			{
				throw new InvalidOperationException($"Cannot determine which strategy to run - multiple strategies found with key '{key}'.");
			}

			if (!cancellationToken.IsCancellationRequested)
			{
				return strategy[0].Run(cancellationToken);
			}

			return Task.FromResult(default(object));
		}
	}
}
