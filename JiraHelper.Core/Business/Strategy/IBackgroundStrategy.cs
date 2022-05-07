using System.Threading;
using System.Threading.Tasks;

namespace JiraHelper.Core.Business.Strategy
{
	/// <summary>
	/// Common interface for strategies which will be run in backdround periodically
	/// </summary>
	public interface IBackgroundStrategy : IStrategy
	{
		/// <summary>
		/// Runs the specified strategy - composing checkers/storage/action and returns the result.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task Run(CancellationToken cancellationToken);
	}
}
