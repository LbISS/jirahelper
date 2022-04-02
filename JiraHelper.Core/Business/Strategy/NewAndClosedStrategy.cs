using Atlassian.Jira;
using JiraHelper.Core.Business.Actions;
using JiraHelper.Core.Business.Checkers;
using JiraHelper.Core.Business.Storage;
using JiraHelper.Core.DTO;
using JiraHelper.Core.Rest.JiraServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JiraHelper.Core.Business.Strategy
{
	/// <summary>
	/// Strategy sending info about new & closed issues
	/// </summary>
	/// <seealso cref="JiraHelper.Core.Business.Strategy.AbstractStrategy" />
	/// <seealso cref="JiraHelper.Core.Business.Strategy.IBackgroundStrategy" />
	public class NewAndClosedStrategy : AbstractStrategy, IBackgroundStrategy
	{
		/// <summary>
		/// The jira issues rest service.
		/// </summary>
		protected IssuesRestService IssuesService { get; }
		/// <summary>
		/// The logger.
		/// </summary>
		protected ILogger<NewAndClosedStrategy> Logger { get; }

		/// <summary>
		/// The checker - gets jira issues.
		/// </summary>
		protected IChecker Checker { get; }
		/// <summary>
		/// The action - applied to filtered jira issues.
		/// </summary>
		protected IAction Action { get; }
		/// <summary>
		/// The storage - used to cache/save results from other services.
		/// </summary>
		protected IStorage Storage { get; }

		public NewAndClosedStrategy(
			string key,

			IChecker checker,
			IStorage storage,
			IAction action,

			IssuesRestService issuesService,
			ILogger<NewAndClosedStrategy> logger) : base(key)
		{
			Checker = checker ?? throw new ArgumentNullException(nameof(checker));
			Storage = storage ?? throw new ArgumentNullException(nameof(storage));
			Action = action ?? throw new ArgumentNullException(nameof(action));

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

			var existingIssues = Storage.GetIssues<JiraIssueKeyDTO>();
			Logger.LogDebug($"Got {existingIssues.Count} existing issues from storage: {string.Join(",", existingIssues.Select(s => s.Key.Value)) }");

			var newIssues = issuesFromServer.Where(w => !existingIssues.Any(a => w.Key != a.Key)).ToList();
			var tasks = new List<Task>();

			foreach (var issue in newIssues)
			{
				tasks.Add(Action.ProcessNewIssue(issue, cancellationToken));
				Logger.LogInformation($"Scheduled sending new issue {issue.Key.Value}");
			}

			var closedIssuesDTOs = existingIssues.Where(w => !issuesFromServer.Any(a => w.Key != a.Key)).ToList();
			var closedIssues = await IssuesService.GetIssues(closedIssuesDTOs.Select(s => s.Key.Value).ToList(), cancellationToken);

			foreach (var issue in closedIssues)
			{
				tasks.Add(Action.ProcessClosedIssue(issue, cancellationToken));
				Logger.LogInformation($"Scheduled sending closed issue {issue.Key.Value}");
			}

			Storage.SaveIssues(issuesFromServer.Select(s => new JiraIssueKeyDTO(s)).ToList());
			Logger.LogDebug($"Issues from server saved.");

			await Task.WhenAll(tasks);
		}
	}
}
