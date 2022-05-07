using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using CsvHelper;

namespace JiraHelper.Core.Business.Storage
{
	/// <summary>
	/// Storage providing methods to work with csv files.
	/// </summary>
	/// <seealso cref="JiraHelper.Core.Business.Storage.IStorage" />
	public class CsvFileStorage : IStorage
	{
		/// <summary>
		/// The serializer settings
		/// </summary>
		protected JsonSerializerOptions SerializerSettings = new JsonSerializerOptions
		{ };

		/// <summary>
		/// The culture.
		/// </summary>
		protected CultureInfo Culture
		{
			get
			{
				var culture = CultureInfo.InvariantCulture;
				return culture;
			}
		}

		/// <summary>
		/// The file path.
		/// </summary>
		public string FilePath { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CsvFileStorage"/> class.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <exception cref="System.ArgumentNullException">filePath</exception>
		public CsvFileStorage(
			string filePath)
		{
			FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		}

		/// <summary>
		/// Saves the jira issues to csv file.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="issues">The issues.</param>
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

		/// <summary>
		/// Gets the jira issues from csv file.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
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
