using Microsoft.Extensions.Hosting;

namespace ConsoleRunner
{
	/// <summary>
	/// Console runner for app
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Defines the entry point of the application.
		/// </summary>
		/// <param name="args">The arguments.</param>
		public static void Main(string[] args)
		{
			JiraHelper.Core.Startup.CreateHostBuilderCommon(args).Build().Run();
		}
	}
}
