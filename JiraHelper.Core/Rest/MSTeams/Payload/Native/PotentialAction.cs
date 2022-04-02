using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JiraHelper.Core.Rest.MSTeams.Payload.Native
{
	/// <summary>
	/// 
	/// </summary>
	/// <example>
	/// <![CDATA[
	/// 	""potentialAction"": [{{
	///			""@type"": ""OpenUri"",
	///			""name"": "",
	///			""targets"": [
	///				{{ ""os"": ""default"", ""uri"": ""https://jirauri/browse/{issue.Key.Value}"" }}
	///			]
	///		}}]
	/// ]]>
	/// </example>
	/// <seealso cref="JiraHelper.Core.Rest.MSTeams.Payload.Native.ITeamsElement" />
	public class PotentialAction : ITeamsElement
	{
		public PotentialAction(string name, string uri)
		{
			Name = name ?? Name;
			if (uri != null)
			{
				Targets.Add(new Target(uri));
			}
		}

		[JsonPropertyName("@type")]
		public string Type { get; set; } = "OpenUri";

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("targets")]
		public List<Target> Targets { get; set; } = new List<Target>();
	}
}
