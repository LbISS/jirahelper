using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JiraHelper.Core.Rest
{
	/// <summary>
	/// Simple rest client to send requests
	/// </summary>
	public class RestClient
	{
		/// <summary>
		/// Sends GET request to the URI.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <returns></returns>
		public async Task<string> GetAsync(string uri)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				return await reader.ReadToEndAsync();
			}
		}

		/// <summary>
		/// Sends GET request to the URI.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="uri">The URI.</param>
		/// <returns></returns>
		public async Task<T> GetAsync<T>(string uri)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
			using (Stream stream = response.GetResponseStream())
			{
				return await JsonSerializer.DeserializeAsync<T>(stream);
			}
		}

		/// <summary>
		/// Sends POST request to the URI.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="uri">The URI.</param>
		/// <param name="data">The data (HTTP body).</param>
		/// <param name="contentType">Type of the content.</param>
		/// <param name="method">The method, could be possibly changed to PUT/Other.</param>
		/// <returns></returns>
		public async Task<string> PostAsync<T>(string uri, T data, string contentType, string method = "POST")
		{
			var serializedData = JsonSerializer.Serialize(data);
			byte[] dataBytes = Encoding.UTF8.GetBytes(serializedData);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			request.ContentLength = dataBytes.Length;
			request.ContentType = contentType;
			request.Method = method;

			using (Stream requestBody = request.GetRequestStream())
			{
				await requestBody.WriteAsync(dataBytes.AsMemory(0, dataBytes.Length));
			}

			using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				return await reader.ReadToEndAsync();
			}
		}
	}
}
