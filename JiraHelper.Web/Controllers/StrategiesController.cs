using System.Threading;
using System.Threading.Tasks;
using JiraHelper.Core.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JiraHelper.Controllers
{
	/// <summary>
	/// Controller for test purposes.
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
	[ApiController]
	[Route("[controller]")]
	public class StrategiesController : ControllerBase
	{
#pragma warning disable IDE0052 // Remove unread private members
		/// <summary>
		/// The logger
		/// </summary>
		private readonly ILogger<StrategiesController> _logger;
#pragma warning restore IDE0052 // Remove unread private members

		/// <summary>
		/// The jira strategies manager
		/// </summary>
		private readonly JiraStrategiesManager _jiraStrategiesManager;


		/// <summary>
		/// Initializes a new instance of the <see cref="StrategiesController"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public StrategiesController(ILogger<StrategiesController> logger, JiraStrategiesManager jiraStrategiesManager)
		{
			_logger = logger;
			_jiraStrategiesManager = jiraStrategiesManager;
		}

		/// <summary>
		/// Returns HTTP_OK 200, doing nothing else.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult Get()
		{
			return Ok();
		}

		/// <summary>
		/// Runs the strategy.
		/// </summary>
		/// <param name="strategyKey">The strategy key.</param>
		/// <returns></returns>
		[HttpPost]
		[Route("{strategyKey}")]
		public async Task<IActionResult> RunStrategy(string strategyKey, CancellationToken cancellationToken)
		{
			var result = await _jiraStrategiesManager.RunStrategy(strategyKey, cancellationToken);
			return Ok(result);
		}
	}
}
