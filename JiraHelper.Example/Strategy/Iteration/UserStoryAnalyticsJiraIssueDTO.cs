using Atlassian.Jira;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;

namespace JiraHelper.Example.Strategy.Iteration
{
	public class UserStoryAnalyticsJiraIssueDTO
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
		public DateTimeOffset? DesignReviewDate { get; set; }
		public DateTimeOffset? DevDueDate { get; set; }
		[JsonPropertyName("qaDueDate")]
		public DateTimeOffset? QADueDate { get; set; }

		public string DevStartDateStr { get; set; }
		public string DevCompleteDateStr { get; set; }
		public string QACompleteDateStr { get; set; }

		public List<IterationTaskJiraIssueDTO> SubTasks { get; set; }

		public long EstimatedHours => this.SubTasks.Sum(s => s.EstimatedHours);
		public long TimeSpentHours => this.SubTasks.Sum(s => s.TimeSpentHours);
		public long RemainingHours => this.SubTasks.Sum(s => s.RemainingHours);


		/// <summary>
		/// For deserializer
		/// Initializes a new instance of the <see cref="IterationUserStoryJiraIssueDTO"/> class.
		/// </summary>
		public UserStoryAnalyticsJiraIssueDTO()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IterationUserStoryJiraIssueDTO"/> class.
		/// </summary>
		/// <param name="issue">The issue.</param>
		public UserStoryAnalyticsJiraIssueDTO(Issue issue)
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

			IFormatProvider provider = CultureInfo.InvariantCulture.DateTimeFormat;
			var drDate = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_14260", StringComparison.OrdinalIgnoreCase))?.Values?[0];
			DesignReviewDate = drDate != null ? DateTimeOffset.ParseExact(drDate, "yyyy-MM-dd", provider) : null;

			var dueDate = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_17165", StringComparison.OrdinalIgnoreCase))?.Values?[0];
			DevDueDate = dueDate != null ? DateTimeOffset.ParseExact(dueDate, "yyyy-MM-dd", provider) : null;

			var qaDate = issue.CustomFields.FirstOrDefault(f => f.Id.Equals("customfield_17166", StringComparison.OrdinalIgnoreCase))?.Values?[0];
			QADueDate = qaDate != null ? DateTimeOffset.ParseExact(qaDate, "yyyy-MM-dd", provider) : null;

			// Subtasks
			SubTasks = issue.GetSubTasksAsync().Result
				.Select(s => new IterationTaskJiraIssueDTO(s))
				.ToList();

			// History
			var changeLogs = issue.GetChangeLogsAsync().Result;

			foreach (var changeLog in changeLogs.OrderBy(o => o.CreatedDate))
			{
				foreach (var changeLogItem in changeLog.Items)
				{
					if (changeLogItem.FieldType.Equals("jira", StringComparison.OrdinalIgnoreCase)
						|| changeLogItem.FieldName.Equals("status", StringComparison.OrdinalIgnoreCase))
					{
						if (changeLogItem.ToValue?.Equals("Tech Design", StringComparison.OrdinalIgnoreCase) == true
							&& DevStartDateStr == null)
						{
							DevStartDateStr = changeLog.CreatedDate.ToString("dd.MM.yyyy");
						}
						else if (changeLogItem.ToValue?.Equals("QA", StringComparison.OrdinalIgnoreCase) == true)
						{
							DevCompleteDateStr = changeLog.CreatedDate.ToString("dd.MM.yyyy");
						}
						else if (
							(
								changeLogItem.ToValue?.Equals("Ready For Demo", StringComparison.OrdinalIgnoreCase) == true
								|| changeLogItem.ToValue?.Equals("Done", StringComparison.OrdinalIgnoreCase) == true
							)
							&& QACompleteDateStr == null)
						{
							QACompleteDateStr = changeLog.CreatedDate.ToString("dd.MM.yyyy");
						}
					}
				}
			}
		}
	}
}
