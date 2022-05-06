using JiraHelper.Core.Business.Storage;
using JiraHelper.Core.Business.Strategy;
using Microsoft.Extensions.Logging;

namespace JiraHelper.Example.Strategy.Iteration
{
	/// <summary>
	/// Get iteration info from storage
	/// </summary>
	/// <seealso cref="JiraHelper.Core.Business.Strategy.GetFromStorageStrategy&lt;JiraHelper.Example.Strategy.Iteration.IterationUserStoryJiraIssueDTO&gt;" />
	public class IterationGetFromStorageStrategy : GetFromStorageStrategy<IterationUserStoryJiraIssueDTO>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IterationGetFromStorageStrategy"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="storage">The storage.</param>
		/// <param name="logger">The logger.</param>
		public IterationGetFromStorageStrategy(string key, IStorage storage, ILogger<GetFromStorageStrategy<IterationUserStoryJiraIssueDTO>> logger) : base(key, storage, logger)
		{
		}
	}
}
