using System.Text.Json.Nodes;

namespace JiraHelper.Core.Config
{
	public class ParameterizedEntityConfig
	{
		public string Mode { get; set; }
		public string Type { get; set; }
		public string Key { get; set; }
		public JsonObject Parameters { get; set; }
	}
}
