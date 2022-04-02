using System;
using Atlassian.Jira;

namespace JiraHelper.Example.Strategy.Iteration
{
	public class IterationTaskJiraIssueDTO
	{
		private const int SECONDS_IN_HOUR = 3600;

		public string Key { get; set; }
		public string IssueType { get; set; }
		public string Status { get; set; }
		public DateTimeOffset? Created { get; set; }
		public string Summary { get; set; }
		public string Assignee { get; set; }
		public string Priority { get; set; }
		public long EstimatedHours { get; set; }
		public long TimeSpentHours { get; set; }
		public long RemainingHours { get; set; }

		/// <summary>
		/// For deserializer
		/// Initializes a new instance of the <see cref="IterationTaskJiraIssueDTO"/> class.
		/// </summary>
		/// <param name="issue">The issue.</param>
		public IterationTaskJiraIssueDTO()
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="IterationTaskJiraIssueDTO"/> class.
		/// </summary>
		/// <param name="issue">The issue.</param>
		public IterationTaskJiraIssueDTO(Issue issue)
		{
			Key = issue.Key.Value;
			IssueType = issue.Type.Name;
			Status = issue.Status.Name;
			Created = issue.Created;
			Summary = issue.Summary;
			Assignee = issue.Assignee;
			Priority = issue.Priority.Name;
			EstimatedHours = (issue.TimeTrackingData.OriginalEstimateInSeconds ?? 0) / SECONDS_IN_HOUR;
			TimeSpentHours = (issue.TimeTrackingData.TimeSpentInSeconds ?? 0) / SECONDS_IN_HOUR;
			RemainingHours = (issue.TimeTrackingData.RemainingEstimateInSeconds ?? 0) / SECONDS_IN_HOUR;
		}
	}
}
