using JiraHelper.Core.Business.Checkers;
using JiraHelper.Core.Business.Storage;
using JiraHelper.Core.Business.Strategy;
using JiraHelper.Core.Rest.JiraServices;
using Microsoft.Extensions.Logging;

namespace JiraHelper.Example.Strategy.Iteration
{
	public class IterationSaveStrategy : SaveStrategy<IterationUserStoryJiraIssueDTO>
	{
		public IterationSaveStrategy(string key, IChecker checker, IStorage storage, IssuesRestService issuesService, ILogger<SaveStrategy<IterationUserStoryJiraIssueDTO>> logger)
			: base(key, (i) => new IterationUserStoryJiraIssueDTO(i), checker, storage, issuesService, logger)
		{
		}
	}
}
