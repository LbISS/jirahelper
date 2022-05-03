namespace JiraHelper.Core.Config
{
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
		public bool EnableTrace { get; set; } = false;
	}
}
