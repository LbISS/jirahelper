using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace JiraHelper.Core.Business.Storage
{
	public class JsonFileStorage : IStorage
	{
		protected JsonSerializerOptions SerializerSettings = new JsonSerializerOptions
		{ };

		public string FilePath { get; }

		public JsonFileStorage(
			string filePath)
		{
			FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}

		public void SaveIssues<T>(List<T> issues)
		{
			if(!Directory.Exists(Path.GetDirectoryName(FilePath))) {
				Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
			}
			File.WriteAllText(FilePath, JsonSerializer.Serialize(issues, SerializerSettings));
		}

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
