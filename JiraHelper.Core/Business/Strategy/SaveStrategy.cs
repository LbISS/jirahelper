using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraHelper.Core.Business.Checkers;
using JiraHelper.Core.Business.Storage;
using JiraHelper.Core.Rest.JiraServices;
using Microsoft.Extensions.Logging;

namespace JiraHelper.Core.Business.Strategy
{
	/// <summary>
	/// Strategy saving info about jira issues in storage
	/// </summary>
	/// <seealso cref="JiraHelper.Core.Business.Strategy.AbstractStrategy" />
	/// <seealso cref="JiraHelper.Core.Business.Strategy.IBackgroundStrategy" />
	public class SaveStrategy<T> : AbstractStrategy, IBackgroundStrategy, IActiveStrategy
	{
		/// <summary>
		/// The jira issues rest service.
		/// </summary>
		protected IssuesRestService IssuesService { get; }
		/// <summary>
		/// The logger.
		/// </summary>
		protected ILogger<SaveStrategy<T>> Logger { get; }

		/// <summary>
		/// The checker - gets jira issues.
		/// </summary>
		protected IChecker Checker { get; }
		/// <summary>
		/// The storage - used to cache/save results from other services.
		/// </summary>
		protected IStorage Storage { get; }


		protected Func<Issue, T> ObjectToSaveConstructor { get; }

		public SaveStrategy(
			string key,
			Func<Issue, T> objectToSaveConstructor,

			IChecker checker,
			IStorage storage,

			IssuesRestService issuesService,
			ILogger<SaveStrategy<T>> logger) : base(key)
		{
			ObjectToSaveConstructor = objectToSaveConstructor;

			Checker = checker ?? throw new ArgumentNullException(nameof(checker));
			Storage = storage ?? throw new ArgumentNullException(nameof(storage));

			IssuesService = issuesService ?? throw new ArgumentNullException(nameof(issuesService));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Runs the specified strategy - composing checkers/storage/action and returns the result.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		public async Task Run(CancellationToken cancellationToken)
		{
			List<Issue> issuesFromServer = await Checker.GetIssues(cancellationToken);
			Logger.LogDebug($"Got {issuesFromServer.Count} issues from jira: {string.Join(",", issuesFromServer.Select(s => s.Key.Value)) }");

			var issuesToSave = issuesFromServer.AsParallel()
					.Select(s =>
					{
						Logger.LogDebug($"Issue {s.Key.Value} is saved.");
						return ObjectToSaveConstructor(s);
					})
					.ToList();

			Storage.SaveIssues(issuesToSave);
			Logger.LogDebug($"Issues from server saved.");
		}

		async Task<object> IActiveStrategy.Run(CancellationToken cancellationToken)
		{
			await this.Run(cancellationToken);
			return Task.FromResult(default(object));
		}
	}
}
