using System.Collections.ObjectModel;
using Atlassian.Jira;

namespace JiraHelper.Core.DTO
{
	/// <summary>
	/// DTO for jira issue with base and custom fields
	/// </summary>
	public class JiraIssueCustomFieldsDTO : JiraIssueBaseFieldsDTO
	{
		public ReadOnlyCollection<CustomFieldValue> CustomFields { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="JiraIssueCustomFieldsDTO"/> class.
		/// </summary>
		/// <param name="issue">The issue.</param>
		public JiraIssueCustomFieldsDTO(Issue issue, ReadOnlyCollection<CustomFieldValue> customFields) : base(issue)
		{
			this.CustomFields = customFields;
		}
	}
}
