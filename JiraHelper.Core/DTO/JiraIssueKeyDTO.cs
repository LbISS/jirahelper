using Atlassian.Jira;

namespace JiraHelper.Core.DTO
{
	/// <summary>
	/// DTO for jira issue containing only key
	/// </summary>
	public class JiraIssueKeyDTO
	{
		/// <summary>
		/// The key.
		/// </summary>
		public ComparableString Key { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="JiraIssueKeyDTO"/> class.
		/// </summary>
		public JiraIssueKeyDTO()
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="JiraIssueKeyDTO"/> class.
		/// </summary>
		/// <param name="issue">The issue.</param>
		public JiraIssueKeyDTO(Issue issue)
		{
			//TODO: Possibly AutoMapper
			Key = issue.Key;
		}
	}
}
