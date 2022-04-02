using Atlassian.Jira;
using JiraHelper.Core.Business.Actions;
using JiraHelper.Core.Rest.MSTeams;
using JiraHelper.Example.Actions.Payload;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JiraHelper.Example.Actions
{
	/// <summary>
	/// Send issues to channel in MS Teams with blocker template
	/// </summary>
	/// <seealso cref="Core.JiraHelper.Core.Business.Actions.IAction" />
	public class MSTeamsNotifyBlockerAction : IAction
	{
		/// <summary>
		/// The webhook URI.
		/// </summary>
		public string WebhookUri { get; }

		/// <summary>
		/// The webhook service.
		/// </summary>
		protected WebHookService WebhookService { get; }
		/// <summary>
		/// The jira URI.
		/// </summary>
		protected string JiraUri { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MSTeamsNotifyBlockerAction" /> class.
		/// </summary>
		/// <param name="webhookUri">The webhook URI.</param>
		/// <param name="webhookService">The webhook service.</param>
		/// <param name="jiraUri">The jira URI.</param>
		/// <exception cref="System.ArgumentNullException">webhookUri
		/// or
		/// webhookService</exception>
		public MSTeamsNotifyBlockerAction(
			string webhookUri,
			WebHookService webhookService,
			string jiraUri)
		{
			WebhookUri = webhookUri ?? throw new ArgumentNullException(nameof(webhookUri));
			WebhookService = webhookService ?? throw new ArgumentNullException(nameof(webhookService));
			JiraUri = jiraUri ?? throw new ArgumentNullException(nameof(jiraUri));
		}

		/// <summary>
		/// Sends "new blocker" message to MS Teams.
		/// </summary>
		/// <param name="issue">The issue.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ProcessNewIssue(Issue issue, CancellationToken cancellationToken)
		{
			return WebhookService.SendPayload(new NewBlockerPayload(issue, JiraUri), WebhookUri, cancellationToken);
		}

		/// <summary>
		/// Doing nothing
		/// </summary>
		/// <param name="issue">The issue.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ProcessUpdatedIssue(Issue issue, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Sends "blocker closed" message to MS Teams.
		/// </summary>
		/// <param name="issue">The issue.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ProcessClosedIssue(Issue issue, CancellationToken cancellationToken)
		{
			return WebhookService.SendPayload(new ClosedBlockerPayload(issue, JiraUri), WebhookUri, cancellationToken);
		}
	}
}
