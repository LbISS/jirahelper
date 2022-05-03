using System.Collections.Generic;

namespace JiraHelper.Core.Config
{
	public class Config
	{
		public JiraConfig Jira { get; set; } = new JiraConfig();

		public List<ParameterizedEntityConfig> Strategies { get; set; } = new List<ParameterizedEntityConfig>();
	}
}
