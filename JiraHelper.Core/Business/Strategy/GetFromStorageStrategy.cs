using System;
using System.Threading;
using System.Threading.Tasks;
using JiraHelper.Core.Business.Storage;
using Microsoft.Extensions.Logging;

namespace JiraHelper.Core.Business.Strategy
{
	/// <summary>
	/// Strategy saving infor about jira issues in storage
	/// </summary>
	/// <seealso cref="JiraHelper.Core.Business.Strategy.AbstractStrategy" />
	/// <seealso cref="JiraHelper.Core.Business.Strategy.IBackgroundStrategy" />
	public class GetFromStorageStrategy<T> : AbstractStrategy, IActiveStrategy
	{
		/// <summary>
		/// The logger.
		/// </summary>
		protected ILogger<GetFromStorageStrategy<T>> Logger { get; }

		/// <summary>
		/// The storage - used to cache/save results from other services.
		/// </summary>
		protected IStorage Storage { get; }

		public GetFromStorageStrategy(
			string key,
			IStorage storage,
			ILogger<GetFromStorageStrategy<T>> logger) : base(key)
		{
			Storage = storage ?? throw new ArgumentNullException(nameof(storage));

			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Runs the specified strategy - composing checkers/storage/action and returns the result.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		public Task<object> Run(CancellationToken cancellationToken)
		{
			return Task.FromResult((object)Storage.GetIssues<T>());
		}
	}
}
