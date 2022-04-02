using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraHelper.Core.Business.Checkers
{
	/// <summary>
	/// Interface for jira checkers
	/// </summary>
	public interface IChecker
	{
		/// <summary>
		/// Checks jira and gets the issues from jira by instance criterias.
		/// </summary>
		/// <returns></returns>
		public Task<List<Issue>> GetIssues(CancellationToken cancellationToken);
	}
}
