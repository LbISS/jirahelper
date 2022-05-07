using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace JiraHelper.Core.Business.Storage
{
	/// <summary>
	/// Storage providing methods to work with json files.
	/// </summary>
	/// <seealso cref="JiraHelper.Core.Business.Storage.IStorage" />
	public class JsonFileStorage : IStorage
	{
		/// <summary>
		/// The serializer settings
		/// </summary>
		protected JsonSerializerOptions SerializerSettings = new JsonSerializerOptions
		{ };

		/// <summary>
		/// The file path.
		/// </summary>
		public string FilePath { get; }

		public JsonFileStorage(
			string filePath)
		{
			FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}

		/// <summary>
		/// Saves the jira issues to json file.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="issues">The issues.</param>
		public void SaveIssues<T>(List<T> issues)
		{
			if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
			}
			File.WriteAllText(FilePath, JsonSerializer.Serialize(issues, SerializerSettings));
		}

		/// <summary>
		/// Gets the jira issues from json file.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public List<T> GetIssues<T>()
		{
			if (!File.Exists(FilePath))
			{
				return new List<T>();
			}

			return JsonSerializer.Deserialize<List<T>>(File.ReadAllText(FilePath), SerializerSettings);
		}
	}
}
