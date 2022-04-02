using CommandLine;

namespace JiraHelper.Core
{
	/// <summary>
	/// Arguments that could be passed to executable file
	/// </summary>
	internal class StartupArgs
	{
		/// <summary>
		/// The configuration file for the tool.
		/// </summary>
		[Option('c', "config", Required = false, HelpText = "Path to config file.")]
		public string ConfigFile { get; set; }
	}
}
