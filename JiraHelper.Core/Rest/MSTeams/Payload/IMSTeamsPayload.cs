using JiraHelper.Core.Rest.MSTeams.Payload.Native;

namespace JiraHelper.Core.Rest.MSTeams.Payload
{
	/// <summary>
	/// Common interface for MS Teams message payloads.
	/// </summary>
	public interface IMSTeamsPayload
	{
		/// <summary>
		/// Gets the content.
		/// </summary>
		/// <returns></returns>
		MessageCard GetContent();
	}
}
