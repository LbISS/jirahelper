using System.Collections.Generic;

namespace JiraHelper.Core.Config
{
	/// <summary>
	/// Config file class for deserialization from json
	/// </summary>
	public class Config
	{
		/// <summary>
		/// The jira configuration parameters.
		/// </summary>
		public JiraConfig Jira { get; set; } = new JiraConfig();

		/// <summary>
		/// The strategies description.
		/// </summary>
		public List<ParameterizedEntityConfig> Strategies { get; set; } = new List<ParameterizedEntityConfig>();
	}
}
