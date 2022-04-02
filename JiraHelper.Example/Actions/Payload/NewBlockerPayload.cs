using JiraHelper.Core.Rest.MSTeams.Payload;
using JiraHelper.Core.Rest.MSTeams.Payload.Native;
using System.Collections.Generic;

namespace JiraHelper.Example.Actions.Payload
{
	/// <summary>
	/// Payload for new blocker
	/// </summary>
	/// <seealso cref="JiraHelper.Core.Rest.MSTeams.Payload.IMSTeamsPayload" />
	public class NewBlockerPayload : IMSTeamsPayload
	{
		/// <summary>
		/// The content
		/// </summary>
		protected MessageCard Content;
		/// <summary>
		/// Initializes a new instance of the <see cref="NewBlockerPayload"/> class.
		/// </summary>
		/// <param name="issue">The issue.</param>
		public NewBlockerPayload(Atlassian.Jira.Issue issue, string jiraUri)
		{
			var title = $"New Blocker: {issue.Key.Value}";
			var color = "#f66";
			var image = "https://iili.io/BBUEWN.jpg";

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
