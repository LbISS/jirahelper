using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JiraHelper.Core.Rest.MSTeams.Payload.Native
{
	public class MessageCard : ITeamsMessage, ITeamsElement
	{
		public MessageCard(string title, string color, string image, List<KeyValuePair<string, string>> linkUris)
		{
			Summary = title ?? Summary;
			Title = title ?? Title;
			ThemeColor = color ?? ThemeColor;
			if(image != null)
			{
				Sections.Add(new Section($@"<img src=""{image}"" alt=""notify-sticker""></img>"));
			}
			if(linkUris != null)
			{
				foreach (var linkUri in linkUris)
				{
					PotentialAction.Add(new PotentialAction(linkUri.Key, linkUri.Value));
				}
			}
		}

		[JsonPropertyName("@type")]
		public string Type { get; set; } = "MessageCard";

		[JsonPropertyName("@context")]
		public string Context { get; set; } = "http://schema.org/extensions";

		[JsonPropertyName("summary")]
		public string Summary { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("themeColor")]
		public string ThemeColor { get; set; }

		[JsonPropertyName("sections")]
		public List<Section> Sections { get; set; } = new List<Section>();

		[JsonPropertyName("potentialAction")]
		public List<PotentialAction> PotentialAction { get; set; } = new List<PotentialAction>();
	}
}
