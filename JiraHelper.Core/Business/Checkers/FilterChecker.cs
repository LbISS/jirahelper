using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraHelper.Core.Rest.JiraServices;

namespace JiraHelper.Core.Business.Checkers
{
	/// <summary>
	/// Gets the issue by jira filter
	/// </summary>
	/// <seealso cref="JiraHelper.Core.Business.Checkers.IChecker" />
	public class FilterChecker : IChecker
	{
		/// <summary>
		/// The filter identifier.
		/// </summary>
		public string FilterId { get; }

		/// <summary>
		/// The issues rest service.
		/// </summary>
		protected IssuesRestService IssuesRestService { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterChecker"/> class.
		/// </summary>
		/// <param name="filterId">The filter identifier.</param>
		/// <param name="issuesRestService">The issues rest service.</param>
		/// <exception cref="System.ArgumentNullException">
		/// filterId
		/// or
		/// issuesRestService
		/// </exception>
		public FilterChecker(string filterId, IssuesRestService issuesRestService)
		{
			FilterId = filterId ?? throw new ArgumentNullException(nameof(filterId));
			IssuesRestService = issuesRestService ?? throw new ArgumentNullException(nameof(issuesRestService));
		}

		/// <summary>
		/// Gets the issues from jira by instance criterias.
		/// </summary>
		/// <returns></returns>
		public Task<List<Issue>> GetIssues(CancellationToken cancellationToken)
		{
			return IssuesRestService.GetFilterIssues(FilterId, cancellationToken);
		}
	}
}
