using System.Text.Json.Serialization;

namespace JiraHelper.Core.Rest.MSTeams.Payload.Native
{
	public class Section
	{
		public Section(string text)
		{
			Text = text ?? Text;
		}


		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <example>
		/// <![CDATA[
		///		""sections"": [{{
		///			""text"": ""<img src =\\""{image}\\"" alt=\\""build-sticker\\""></img> <strong>{"img"}</ strong > ""
		///		}}]
		/// ]]>
		/// </example>
		[JsonPropertyName("text")]
		public string Text { get; set; }
	}
}
