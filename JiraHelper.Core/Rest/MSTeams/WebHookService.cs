using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JiraHelper.Core.Rest.MSTeams.Payload;

namespace JiraHelper.Core.Rest.MSTeams
{
	/// <summary>
	/// Service for sending MS Teams webhook requests.
	/// </summary>
	public class WebHookService
	{
		/// <summary>
		/// The client
		/// </summary>
		protected HttpClient _client = new HttpClient();

		/// <summary>
		/// Initializes a new instance of the <see cref="WebHookService"/> class.
		/// </summary>
		public WebHookService()
		{}

		/// <summary>
		/// Sends the payload.
		/// </summary>
		/// <param name="payload">The payload.</param>
		/// <param name="webhookUrl">The webhook URL.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		public async Task SendPayload(IMSTeamsPayload payload, string webhookUrl, CancellationToken cancellationToken)
		{
			var jsonString = JsonSerializer.Serialize(payload.GetContent());

			var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

			var response = await _client.PostAsync(webhookUrl, content, cancellationToken);

			var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
			if(!response.IsSuccessStatusCode)
			{
				Console.WriteLine(responseString);
			}
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		public void Dispose()
		{
			_client.Dispose();
		}
	}
}
