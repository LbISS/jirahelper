using JiraHelper.Core.Business;
using Microsoft.Extensions.DependencyInjection;
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
			var host = JiraHelper.Core.Startup.CreateHostBuilderCommon(args).Build();

			using (var scope = host.Services.CreateScope())
			{
				var jiraManager = scope.ServiceProvider.GetRequiredService<JiraStrategiesManager>();
				jiraManager.RunAllActiveStrategies(System.Threading.CancellationToken.None);
			}

			host.Run();
		}
	}
}
