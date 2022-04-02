using JiraHelper.Core.Business.Checkers;
using JiraHelper.Core.Business.Storage;
using JiraHelper.Core.Business.Strategy;
using JiraHelper.Core.Rest.JiraServices;
using Microsoft.Extensions.Logging;

namespace JiraHelper.Example.Strategy.PFRUX
{
	public class PFRUXSaveStrategy : SaveStrategy<UXReportJiraIssueDTO>
	{
		public PFRUXSaveStrategy(string key, IChecker checker, IStorage storage, IssuesRestService issuesService, ILogger<SaveStrategy<UXReportJiraIssueDTO>> logger) 
			: base(key, (i) => new UXReportJiraIssueDTO(i), checker, storage, issuesService, logger)
		{
		}
	}

}


