using Atlassian.Jira;
using JiraHelper.Core.Rest.MSTeams;
using JiraHelper.Core.Rest.MSTeams.Payload;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JiraHelper.Core.Business.Actions
{
	/// <summary>
	/// Send issues to channel in MS Teams with template
	/// </summary>
	/// <seealso cref="Core.JiraHelper.Core.Business.Actions.IAction" />
	public class MSTeamsSimpleNotifyAction : IAction
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
		/// The card options.
		/// </summary>
		protected List<SimpleMessageCardOptions> CardOptions { get; }

		/// <summary>
		/// The webhook service.
		/// </summary>
		protected WebHookService WebhookService { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MSTeamsSimpleNotifyAction" /> class.
		/// </summary>
		/// <param name="webhookUri">The webhook URI.</param>
		/// <param name="jiraUri">The jira URI.</param>
		/// <param name="cardOptions">
		/// The list of card options. 
		/// 1st will be used for sending message about new issues.
		/// 2nd - for updated issues.
		/// 3rd - for closed/deleted issues. 
		/// If the title is empty - template will be skipped.
		/// </param>
		/// <param name="webhookService">The webhook service.</param>
		/// <exception cref="System.ArgumentNullException">webhookUri
		/// or
		/// webhookService</exception>
		public MSTeamsSimpleNotifyAction(
			string webhookUri,
			string jiraUri,
			List<SimpleMessageCardOptions> cardOptions,
			WebHookService webhookService)
		{
			WebhookUri = webhookUri ?? throw new ArgumentNullException(nameof(webhookUri));
			JiraUri = jiraUri ?? throw new ArgumentNullException(nameof(jiraUri));
			CardOptions = cardOptions == null || cardOptions.Count == 0 ? throw new ArgumentNullException(nameof(cardOptions)) : cardOptions;
			WebhookService = webhookService ?? throw new ArgumentNullException(nameof(webhookService));
		}

		/// <summary>
		/// Sends "new blocker" message to MS Teams.
		/// </summary>
		/// <param name="issue">The issue.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ProcessNewIssue(Issue issue, CancellationToken cancellationToken)
		{
			if (CardOptions.Count < 1 || String.IsNullOrEmpty(CardOptions[0].Title))
				return Task.CompletedTask;

			return WebhookService.SendPayload(new SimpleMessageCard(issue, JiraUri, CardOptions[0]), WebhookUri, cancellationToken);
		}

		/// <summary>
		/// Doing nothing
		/// </summary>
		/// <param name="issue">The issue.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ProcessUpdatedIssue(Issue issue, CancellationToken cancellationToken)
		{
			if (CardOptions.Count < 2 || String.IsNullOrEmpty(CardOptions[1].Title))
				return Task.CompletedTask;

			return WebhookService.SendPayload(new SimpleMessageCard(issue, JiraUri, CardOptions[1]), WebhookUri, cancellationToken);
		}

		/// <summary>
		/// Sends "blocker closed" message to MS Teams.
		/// </summary>
		/// <param name="issue">The issue.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ProcessClosedIssue(Issue issue, CancellationToken cancellationToken)
		{
			if (CardOptions.Count < 3 || String.IsNullOrEmpty(CardOptions[2].Title))
				return Task.CompletedTask;

			return WebhookService.SendPayload(new SimpleMessageCard(issue, JiraUri, CardOptions[2]), WebhookUri, cancellationToken);

		}
	}
}
