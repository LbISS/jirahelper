namespace JiraHelper.Core.Config
{
	/// <summary>
	/// The jira configuration parameters.
	/// </summary>
	public class JiraConfig
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
		/// <summary>
		/// Gets or sets a value indicating whether enable trace logs for request to this jira instance.
		/// </summary>
		public bool EnableTrace { get; set; } = false;
	}
}
