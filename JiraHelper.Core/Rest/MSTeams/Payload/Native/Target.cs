using System.Text.Json.Serialization;

namespace JiraHelper.Core.Rest.MSTeams.Payload.Native
{
	public class Target
	{
		public Target(string uri)
		{
			URI = uri ?? URI;
		}

		[JsonPropertyName("os")]
		public string OS { get; set; } = "default";

		[JsonPropertyName("uri")]
		public string URI { get; set; }
	}
}
