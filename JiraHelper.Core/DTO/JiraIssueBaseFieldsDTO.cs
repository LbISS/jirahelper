using System;
using Atlassian.Jira;

namespace JiraHelper.Core.DTO
{
	/// <summary>
	/// DTO for jira issue with base fields
	/// </summary>
	public class JiraIssueBaseFieldsDTO : JiraIssueKeyDTO
	{
		public DateTime? DueDate { get; set; }
		public IssueStatus Status { get; set; }
		public DateTime? Created { get; set; }
		public bool HasUserVoted { get; set; }
		public long? Votes { get; set; }
		public string ParentIssueKey { get; set; }
		public IssueSecurityLevel SecurityLevel { get; set; }
		public string Summary { get; set; }
		public DateTime? Updated { get; set; }
		public string Description { get; set; }
		public string Assignee { get; set; }
		public JiraUser AssigneeUser { get; set; }
		public IssueTimeTrackingData TimeTrackingData { get; set; }
		public string JiraIdentifier { get; set; }
		public IssuePriority Priority { get; set; }
		public string Project { get; set; }
		public string Reporter { get; set; }
		public JiraUser ReporterUser { get; set; }
		public IssueResolution Resolution { get; set; }
		public string Environment { get; set; }
		public DateTime? ResolutionDate { get; set; }
		public IssueType Type { get; set; }
		public IssueLabelCollection Labels { get; set; }
		public ProjectVersionCollection AffectsVersions { get; set; }
		public ProjectComponentCollection Components { get; set; }
		public ProjectVersionCollection FixVersions { get; set; }

		public string JiraUrl { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="JiraIssueKeyDTO"/> class.
		/// </summary>
		/// <param name="issue">The issue.</param>
		public JiraIssueBaseFieldsDTO(Issue issue) : base(issue)
		{
			DueDate = issue.DueDate;
			Status = issue.Status;
			Created = issue.Created;
			HasUserVoted = issue.HasUserVoted;
			Votes = issue.Votes;
			ParentIssueKey = issue.ParentIssueKey;
			SecurityLevel = issue.SecurityLevel;
			Summary = issue.Summary;
			Updated = issue.Updated;
			Description = issue.Description;
			Updated = issue.Updated;
			Assignee = issue.Assignee;
			AssigneeUser = issue.AssigneeUser;
			TimeTrackingData = issue.TimeTrackingData;
			JiraIdentifier = issue.JiraIdentifier;
			Priority = issue.Priority;
			Project = issue.Project;
			Reporter = issue.Reporter;
			ReporterUser = issue.ReporterUser;
			Resolution = issue.Resolution;
			Environment = issue.Environment;
			ResolutionDate = issue.ResolutionDate;
			Type = issue.Type;
			Labels = issue.Labels;
			AffectsVersions = issue.AffectsVersions;
			Components = issue.Components;
			FixVersions = issue.FixVersions;

			JiraUrl = issue.Jira.Url;
		}
	}
}
