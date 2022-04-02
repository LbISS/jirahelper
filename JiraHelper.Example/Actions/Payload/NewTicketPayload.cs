using JiraHelper.Core.Rest.MSTeams.Payload;
using JiraHelper.Core.Rest.MSTeams.Payload.Native;
using System.Collections.Generic;

namespace JiraHelper.Example.Actions.Payload
{
	/// <summary>
	/// Payload for new ticket
	/// </summary>
	/// <seealso cref="JiraHelper.Core.Rest.MSTeams.Payload.IMSTeamsPayload" />
	public class NewTicketPayload : IMSTeamsPayload
	{
		/// <summary>
		/// The content
		/// </summary>
		protected MessageCard Content;
		/// <summary>
		/// Initializes a new instance of the <see cref="NewTicketPayload"/> class.
		/// </summary>
		/// <param name="issue">The issue.</param>
		public NewTicketPayload(Atlassian.Jira.Issue issue, string jiraUri)
		{
			var title = $"New Ticket: {issue.Key.Value}";
			var color = "#ff6";
			var image = "https://iili.io/BBU1xp.png";

			this.Content = new MessageCard(title, color, image,
				new List<KeyValuePair<string, string>> {
					new KeyValuePair<string, string>("Open Jira", $"{jiraUri}browse/{issue.Key.Value}")
						}
				);
		}

		/// <summary>
		/// Gets the content.
		/// </summary>
		/// <returns></returns>
		public MessageCard GetContent()
		{
			return this.Content;
		}
	}
}
