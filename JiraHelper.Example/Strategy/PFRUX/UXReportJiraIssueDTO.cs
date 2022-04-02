using Atlassian.Jira;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JiraHelper.Example.Strategy.PFRUX
{
	public class UXReportJiraIssueDTO
	{
		public string Key { get; set; }
		public string Status { get; set; }
		public DateTime? Created { get; set; }
		public string Summary { get; set; }
		public string Assignee { get; set; }
		public string Priority { get; set; }
		public string UXReview { get; set; }
		public string RnDOwner { get; set; }
		public string UXDesign { get; set; }
		public string ResponsibleUX { get; set; }
		public string RelatedGCUXIssues { get; set; }

		public UXReportJiraIssueDTO(Issue issue)
		{
			Key = issue.Key.Value;
			Status = issue.Status.Name;
			Created = issue.Created;
			Summary = issue.Summary;
			Assignee = issue.Assignee;
			Priority = issue.Priority.Name;
			UXReview = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_13660", StringComparison.OrdinalIgnoreCase))?.Values?[0];
			RnDOwner = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_10040", StringComparison.OrdinalIgnoreCase))?.Values?[0];
			UXDesign = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_13760", StringComparison.OrdinalIgnoreCase))?.Values?[0];
			ResponsibleUX = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_18471", StringComparison.OrdinalIgnoreCase))?.Values?[0];

			// Introducers "https://jira/rest/api/2/issueLinkType/10140"
			var relatedIssues = issue.GetIssueLinksAsync(new List<string> { "Introducers" }).Result;
			RelatedGCUXIssues = relatedIssues?
								.Where(w => w.OutwardIssue.Project.Equals("GCUX", StringComparison.OrdinalIgnoreCase))
								.Aggregate(String.Empty, (curr, next) => (curr != String.Empty ? $"{curr},{GetGCUXLinkString(next)}" : GetGCUXLinkString(next)));
		}
		/*	"customfield_13660", // UX Review
			"customfield_10040", // R&D Owner
			"customfield_13760", // UX Design
			"customfield_10010", // Rough Dev Estimate, md
			"customfield_18471", // Responsible UX(s)
		*/

		private string GetGCUXLinkString(IssueLink issueLink)
		{
			return $"{issueLink.OutwardIssue.Key.Value}:{issueLink.OutwardIssue.Status}";
		}
	}

}


