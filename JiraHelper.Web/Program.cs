using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace JiraHelper
{
	/// <summary>
	/// The starting class for asp.net webapi app
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Defines the entry point of the application.
		/// </summary>
		/// <param name="args">The arguments.</param>
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		/// <summary>
		/// Creates the host builder specific for web app.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Core.Startup.CreateHostBuilderCommon(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
