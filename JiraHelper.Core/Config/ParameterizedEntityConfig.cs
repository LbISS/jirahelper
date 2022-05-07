using System.Text.Json.Nodes;

namespace JiraHelper.Core.Config
{
	/// <summary>
	/// Config for entity with incoming parameters - strategy, storage, actions, etc.
	/// </summary>
	public class ParameterizedEntityConfig
	{
		/// <summary>
		/// The mode switcher.
		/// </summary>
		/// <example>
		/// JiraHelper.Core.Business.Strategy.IActiveStrategy
		/// JiraHelper.Core.Business.Strategy.IBackgroundStrategy
		/// </example>
		public string Mode { get; set; }
		/// <summary>
		/// The type - class name which will be resolved.
		/// </summary>
		public string Type { get; set; }
		/// <summary>
		/// The unique key. Should be alphanumeric.
		/// </summary>
		public string Key { get; set; }
		/// <summary>
		/// The parameters node.
		/// </summary>
		public JsonObject Parameters { get; set; }
	}
}
