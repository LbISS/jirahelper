using JiraHelper.Core.Rest.MSTeams.Payload.Native;
using System.Collections.Generic;

namespace JiraHelper.Core.Rest.MSTeams.Payload
{
	/// <summary>
	/// Simple card for sending into Teams
	/// </summary>
	/// <seealso cref="JiraHelper.Core.Rest.MSTeams.Payload.IMSTeamsPayload" />
	public class SimpleMessageCard : IMSTeamsPayload
	{
		/// <summary>
		/// The content
		/// </summary>
		protected MessageCard Content;
		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleMessageCard" /> class.
		/// </summary>
		/// <param name="issue">The issue.</param>
		/// <param name="jiraUri">The jira URI.</param>
		public SimpleMessageCard(Atlassian.Jira.Issue issue, string jiraUri, SimpleMessageCardOptions options)
		{
			this.Content = new MessageCard(options.Title, options.Color, options.Image,
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
