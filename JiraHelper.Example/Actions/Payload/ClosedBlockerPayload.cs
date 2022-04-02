using JiraHelper.Core.Rest.MSTeams.Payload;
using JiraHelper.Core.Rest.MSTeams.Payload.Native;
using System.Collections.Generic;

namespace JiraHelper.Example.Actions.Payload
{
	/// <summary>
	/// Payload for closed blocker
	/// </summary>
	/// <seealso cref="JiraHelper.Core.Rest.MSTeams.Payload.IMSTeamsPayload" />
	public class ClosedBlockerPayload : IMSTeamsPayload
	{
		/// <summary>
		/// The content
		/// </summary>
		protected MessageCard Content;
		/// <summary>
		/// Initializes a new instance of the <see cref="ClosedBlockerPayload"/> class.
		/// </summary>
		/// <param name="issue">The issue.</param>
		public ClosedBlockerPayload(Atlassian.Jira.Issue issue, string jiraUri)
		{
			var title = $"Blocker closed: {issue.Key.Value}";
			var color = "#6f6";
			var image = "https://iili.io/BCqSgS.png";

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
