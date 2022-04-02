using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using CsvHelper;

namespace JiraHelper.Core.Business.Storage
{
	public class CsvFileStorage : IStorage
	{
		protected JsonSerializerOptions SerializerSettings = new JsonSerializerOptions
		{ };

		protected CultureInfo Culture
		{
			get
			{
				var culture = CultureInfo.InvariantCulture;
				return culture;
			}
		}

		public string FilePath { get; }

		public CsvFileStorage(
			string filePath)
		{
			FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}

		public void SaveIssues<T>(List<T> issues)
		{
			if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
			}

			using (var writer = new StreamWriter(FilePath))
			using (var csv = new CsvWriter(writer, Culture))
			{
				csv.WriteHeader<T>();
				csv.NextRecord();
				foreach (var record in issues)
				{
					csv.WriteRecord(record);
					csv.NextRecord();
				}
			}
		}

		public List<T> GetIssues<T>()
		{
			if (!File.Exists(FilePath))
			{
				return new List<T>();
			}

			using (var reader = new StreamReader(FilePath))
			using (var csv = new CsvReader(reader, Culture))
			{
				return csv.GetRecords<T>().ToList();
			}
		}


	}
}
