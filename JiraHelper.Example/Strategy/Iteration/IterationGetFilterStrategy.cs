using JiraHelper.Core.Business.Checkers;
using JiraHelper.Core.Business.Strategy;
using JiraHelper.Core.Rest.JiraServices;
using Microsoft.Extensions.Logging;

namespace JiraHelper.Example.Strategy.Iteration
{
	/// <summary>
	/// Get all issues by the filter
	/// </summary>
	/// <seealso cref="JiraHelper.Core.Business.Strategy.GetStrategy&lt;JiraHelper.Example.Strategy.Iteration.IterationUserStoryJiraIssueDTO&gt;" />
	public class IterationGetFilterStrategy : GetStrategy<IterationUserStoryJiraIssueDTO>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IterationGetGilterStrategy"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="checker">The checker.</param>
		/// <param name="issuesService">The issues service.</param>
		/// <param name="logger">The logger.</param>
		public IterationGetFilterStrategy(string key, IChecker checker, IssuesRestService issuesService, ILogger<GetStrategy<IterationUserStoryJiraIssueDTO>> logger)
			: base(key, (i) => new IterationUserStoryJiraIssueDTO(i), checker, issuesService, logger)
		{
		}
	}
}
