using Atlassian.Jira;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;

namespace JiraHelper.Example.Strategy.Iteration
{
	public class IterationUserStoryJiraIssueDTO
	{
		public string Key { get; set; }
		public string IssueType { get; set; }
		public string Status { get; set; }
		public DateTimeOffset? Created { get; set; }
		public string Summary { get; set; }
		public string Assignee { get; set; }
		public string Priority { get; set; }
		[JsonPropertyName("rndOwner")]
		public string RnDOwner { get; set; }
		public string ResponsibleQA { get; set; }
		public string DesignReview { get; set; }
		public string TechDesignURL { get; set; }
		public DateTimeOffset? DesignReviewDate { get; set; }
		public DateTimeOffset? DevDueDate { get; set; }
		[JsonPropertyName("qaDueDate")]
		public DateTimeOffset? QADueDate { get; set; }


		public string IntroducedByPFR { get; set; }

		public List<IterationTaskJiraIssueDTO> SubTasks { get; set; }

		public long EstimatedHours => this.SubTasks.Sum(s => s.EstimatedHours);
		public long TimeSpentHours => this.SubTasks.Sum(s => s.TimeSpentHours);
		public long RemainingHours => this.SubTasks.Sum(s => s.RemainingHours);


		/// <summary>
		/// For deserializer
		/// Initializes a new instance of the <see cref="IterationUserStoryJiraIssueDTO"/> class.
		/// </summary>
		public IterationUserStoryJiraIssueDTO()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IterationUserStoryJiraIssueDTO"/> class.
		/// </summary>
		/// <param name="issue">The issue.</param>
		public IterationUserStoryJiraIssueDTO(Issue issue)
		{
			Key = issue.Key.Value;
			IssueType = issue.Type.Name;
			Status = issue.Status.Name;
			Created = issue.Created;
			Summary = issue.Summary;
			Assignee = issue.Assignee;
			Priority = issue.Priority.Name;
			RnDOwner = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_10040", StringComparison.OrdinalIgnoreCase))?.Values?[0];
			ResponsibleQA = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_10009", StringComparison.OrdinalIgnoreCase))?.Values?[0];
			DesignReview = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_10050", StringComparison.OrdinalIgnoreCase))?.Values?[0];
			TechDesignURL = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_15360", StringComparison.OrdinalIgnoreCase))?.Values?[0];

			IFormatProvider provider = CultureInfo.InvariantCulture.DateTimeFormat;
			var drDate = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_14260", StringComparison.OrdinalIgnoreCase))?.Values?[0];
			DesignReviewDate = drDate != null ? DateTimeOffset.ParseExact(drDate, "yyyy-MM-dd", provider) : null;

			var dueDate = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_17165", StringComparison.OrdinalIgnoreCase))?.Values?[0];
			DevDueDate = dueDate != null ? DateTimeOffset.ParseExact(dueDate, "yyyy-MM-dd", provider) : null;

			var qaDate = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_17166", StringComparison.OrdinalIgnoreCase))?.Values?[0];
			QADueDate = qaDate != null ? DateTimeOffset.ParseExact(qaDate, "yyyy-MM-dd", provider) : null;

			//Rank - customfield_14760

			// Subtasks
			SubTasks = issue.GetSubTasksAsync().Result
				.Select(s => new IterationTaskJiraIssueDTO(s))
				.ToList();

			// Introducers "https://jira/rest/api/2/issueLinkType/10140"
			var relatedIssues = issue.GetIssueLinksAsync(new List<string> { "Introducers" }).Result;
			IntroducedByPFR = relatedIssues?
								.Where(w => w.InwardIssue.Project.Equals("PFR", StringComparison.OrdinalIgnoreCase))
								.Aggregate(String.Empty, (curr, next) => (curr != String.Empty ? $"{curr},{GetPFRString(next)}" : GetPFRString(next)));
		}

		private string GetPFRString(IssueLink issueLink)
		{
			return $"{issueLink.OutwardIssue.Key.Value}";   //:{issueLink.OutwardIssue.Status}
		}
	}
}
