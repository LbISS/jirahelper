using System.Collections.Generic;

namespace JiraHelper.Core.Business.Storage
{
	/// <summary>
	/// Interface for storage for data - filesystem/db/etc
	/// </summary>
	public interface IStorage
	{
		/// <summary>
		/// Saves the jira issues to storage.
		/// </summary>
		/// <param name="issues">The issues.</param>
		void SaveIssues<T>(List<T> issues);
		/// <summary>
		/// Gets the jira issues from storage.
		/// </summary>
		/// <returns></returns>
		List<T> GetIssues<T>();
	}
}
