namespace JiraHelper.Core.Rest.MSTeams.Payload
{
	/// <summary>
	/// Options for sending simple MessageCard into Teams channel
	/// </summary>
	public class SimpleMessageCardOptions
	{
		/// <summary>
		/// The title.
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// The color.
		/// </summary>
		public string Color { get; set; }
		/// <summary>
		/// The image.
		/// </summary>
		public string Image { get; set; }
	}
}
