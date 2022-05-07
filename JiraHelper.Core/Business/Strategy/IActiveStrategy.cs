using System.Threading;
using System.Threading.Tasks;

namespace JiraHelper.Core.Business.Strategy
{

	/// <summary>
	/// Common interface for strategies which will be called using API
	/// </summary>
	public interface IActiveStrategy : IStrategy
	{
		/// <summary>
		/// Runs the specified strategy - composing checkers/storage/action and returns the result.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task<object> Run(CancellationToken cancellationToken);
	}
}
