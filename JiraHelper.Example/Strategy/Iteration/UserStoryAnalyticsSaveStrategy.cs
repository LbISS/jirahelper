using JiraHelper.Core.Business.Checkers;
using JiraHelper.Core.Business.Storage;
using JiraHelper.Core.Business.Strategy;
using JiraHelper.Core.Rest.JiraServices;
using Microsoft.Extensions.Logging;

namespace JiraHelper.Example.Strategy.Iteration
{
	public class UserStoryAnalyticsSaveStrategy : SaveStrategy<UserStoryAnalyticsJiraIssueDTO>
	{
		public UserStoryAnalyticsSaveStrategy(string key, IChecker checker, IStorage storage, IssuesRestService issuesService, ILogger<SaveStrategy<UserStoryAnalyticsJiraIssueDTO>> logger)
			: base(key, (i) => new UserStoryAnalyticsJiraIssueDTO(i), checker, storage, issuesService, logger)
		{
		}
	}
}
