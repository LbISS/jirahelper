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
	/// Send issues to channel in MS Teams with ticket template
	/// </summary>
	/// <seealso cref="Core.JiraHelper.Core.Business.Actions.IAction" />
	public class MSTeamsNotifyTicketAction : IAction
	{
		/// <summary>
		/// The webhook URI.
		/// </summary>
		public string WebhookUri { get; }

		/// <summary>
		/// The jira URI.
		/// </summary>
		protected string JiraUri { get; }

		/// <summary>
		/// The webhook service.
		/// </summary>
		protected WebHookService WebhookService { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MSTeamsNotifyTicketAction"/> class.
		/// </summary>
		/// <param name="webhookUri">The webhook URI.</param>
		/// <param name="webhookService">The webhook service.</param>
		/// <param name="jiraUri">The jira URI.</param>
		/// <exception cref="System.ArgumentNullException">
		/// webhookUri
		/// or
		/// webhookService
		/// or
		/// jiraUri
		/// </exception>
		public MSTeamsNotifyTicketAction(
			string webhookUri,
			string jiraUri,
			WebHookService webhookService)
		{
			WebhookUri = webhookUri ?? throw new ArgumentNullException(nameof(webhookUri));
			JiraUri = jiraUri ?? throw new ArgumentNullException(nameof(jiraUri));
			WebhookService = webhookService ?? throw new ArgumentNullException(nameof(webhookService));
		}

		/// <summary>
		/// Sends "new ticket" message to MS Teams.
		/// </summary>
		/// <param name="issue">The issue.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ProcessNewIssue(Issue issue, CancellationToken cancellationToken)
		{
			return WebhookService.SendPayload(new NewTicketPayload(issue, JiraUri), WebhookUri, cancellationToken);
		}

		/// <summary>
		/// Doing nothing.
		/// </summary>
		/// <param name="issue">The issue.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ProcessUpdatedIssue(Issue issue, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Sends "closed ticket" message to MS Teams.
		/// </summary>
		/// <param name="issue">The issue.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ProcessClosedIssue(Issue issue, CancellationToken cancellationToken)
		{
			return WebhookService.SendPayload(new ClosedTicketPayload(issue, JiraUri), WebhookUri, cancellationToken);
		}
	}
}
