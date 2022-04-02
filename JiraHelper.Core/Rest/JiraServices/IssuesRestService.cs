using Atlassian.Jira;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JiraHelper.Core.Rest.JiraServices
{
	/// <summary>
	/// Rest service for getting jira issues
	/// </summary>
	public class IssuesRestService
	{
		/// <summary>
		/// The maximum issues per request
		/// </summary>
		protected const int MAX_ISSUES_PER_REQUEST = 1000;

		/// <summary>
		/// The jira connection
		/// </summary>
		protected Jira _jiraConn;

		/// <summary>
		/// Initializes a new instance of the <see cref="IssuesRestService"/> class.
		/// </summary>
		/// <param name="jiraConn">The jira connection.</param>
		/// <exception cref="System.ArgumentNullException">jiraConn</exception>
		public IssuesRestService(Jira jiraConn)
		{
			_jiraConn = jiraConn ?? throw new ArgumentNullException(nameof(jiraConn));
		}

		/// <summary>
		/// Gets the issues by filter.
		/// </summary>
		/// <param name="id">The filter identifier.</param>
		/// <returns></returns>
		public async Task<List<Issue>> GetFilterIssues(String id, CancellationToken cancellationToken)
		{
			JiraFilter filter = await _jiraConn.Filters.GetFilterAsync(id, cancellationToken);
			IPagedQueryResult<Issue> issues = await _jiraConn.Filters.GetIssuesFromFavoriteAsync(filter.Name, MAX_ISSUES_PER_REQUEST, default, cancellationToken);
			return new List<Issue>(issues);
		}

		/// <summary>
		/// Gets the issues by ids.
		/// </summary>
		/// <param name="ids">The ids.</param>
		/// <returns></returns>
		public async Task<List<Issue>> GetIssues(List<string> ids, CancellationToken cancellationToken)
		{
			var issues = await _jiraConn.Issues.GetIssuesAsync(ids, cancellationToken);
			return new List<Issue>(issues.Values);
		}
	}
}
