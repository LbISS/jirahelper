using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraHelper.Core.Business.Actions
{
	/// <summary>
	/// Interface for services doing smth. on jira issues.
	/// </summary>
	public interface IAction
	{
		/// <summary>
		/// Processes the new issue.
		/// </summary>
		/// <param name="issue">The issue.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ProcessNewIssue(Issue issue, CancellationToken cancellationToken);
		/// <summary>
		/// Processes the updated issue.
		/// </summary>
		/// <param name="issue">The issue.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ProcessUpdatedIssue(Issue issue, CancellationToken cancellationToken);
		/// <summary>
		/// Processes the closed issue.
		/// </summary>
		/// <param name="issue">The issue.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ProcessClosedIssue(Issue issue, CancellationToken cancellationToken);
	}
}
