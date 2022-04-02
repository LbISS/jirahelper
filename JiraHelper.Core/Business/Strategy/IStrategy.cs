namespace JiraHelper.Core.Business.Strategy
{
	/// <summary>
	/// Most common interface for all strategies
	/// </summary>
	public interface IStrategy
	{
		/// <summary>
		/// The unique key.
		/// </summary>
		public string Key { get; }
	}
}
