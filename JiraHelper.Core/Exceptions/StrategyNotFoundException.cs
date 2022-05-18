using System;

namespace JiraHelper.Core.Exceptions
{
	/// <summary>
	/// Exception when requested strategy not found
	/// </summary>
	public class StrategyNotFoundException : Exception
	{
		public StrategyNotFoundException(string message) : base(message)
		{
		}

		public StrategyNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
