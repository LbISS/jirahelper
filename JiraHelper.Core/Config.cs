using System.Collections.Generic;

namespace JiraHelper.Core
{
	internal class Config
	{
		public JiraConfig Jira { get; set; } = new JiraConfig();

		public List<StrategyConfig> Strategies { get; set; } = new List<StrategyConfig>();
	}

	internal class JiraConfig
	{
		/// <summary>
		/// The jira URI
		/// </summary>
		public string Uri { get; set; }
		/// <summary>
		/// The jira user
		/// </summary>
		public string User { get; set; }
		/// <summary>
		/// The jira password
		/// </summary>
		public string Password { get; set; }
		public bool EnableTrace { get; set; } = false;
	}

	internal class StrategyConfig
	{
		public string Mode { get; set; }
		public string Type { get; set; }
		public string Key { get; set; }
		public IStrategyConfigParameters Parameters { get; set; }
	}

	public interface IStrategyConfigParameters
	{
	}
}
