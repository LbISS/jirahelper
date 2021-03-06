using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraHelper.Core.Business.Checkers;
using JiraHelper.Core.Rest.JiraServices;
using Microsoft.Extensions.Logging;

namespace JiraHelper.Core.Business.Strategy
{
	/// <summary>
	/// Strategy saving info about jira issues in storage
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <seealso cref="JiraHelper.Core.Business.Strategy.IActiveStrategy" />
	/// <seealso cref="JiraHelper.Core.Business.Strategy.AbstractStrategy" />
	/// <seealso cref="JiraHelper.Core.Business.Strategy.IBackgroundStrategy" />
	public class GetStrategy<T> : AbstractStrategy, IActiveStrategy
	{
		/// <summary>
		/// The jira issues rest service.
		/// </summary>
		protected IssuesRestService IssuesService { get; }
		/// <summary>
		/// The logger.
		/// </summary>
		protected ILogger<GetStrategy<T>> Logger { get; }

		/// <summary>
		/// The checker - gets jira issues.
		/// </summary>
		protected IChecker Checker { get; }


		/// <summary>
		/// Constructor for object which will be saved.
		/// </summary>
		protected Func<Issue, T> ObjectToSaveConstructor { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GetStrategy{T}"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="objectToSaveConstructor">The object to save constructor.</param>
		/// <param name="checker">The checker.</param>
		/// <param name="issuesService">The issues service.</param>
		/// <param name="logger">The logger.</param>
		/// <exception cref="System.ArgumentNullException">
		/// checker
		/// or
		/// issuesService
		/// or
		/// logger
		/// </exception>
		public GetStrategy(
			string key,
			Func<Issue, T> objectToSaveConstructor,

			IChecker checker,

			IssuesRestService issuesService,
			ILogger<GetStrategy<T>> logger) : base(key)
		{
			ObjectToSaveConstructor = objectToSaveConstructor;

			Checker = checker ?? throw new ArgumentNullException(nameof(checker));

			IssuesService = issuesService ?? throw new ArgumentNullException(nameof(issuesService));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Runs the specified strategy - composing checkers/storage/action and returns the result.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<object> Run(CancellationToken cancellationToken)
		{
			List<Issue> issuesFromServer = await Checker.GetIssues(cancellationToken);
			Logger.LogDebug($"Got {issuesFromServer.Count} issues from jira: {string.Join(",", issuesFromServer.Select(s => s.Key.Value)) }");

			var issuesToSave = issuesFromServer.AsParallel()
					.Select(s =>
					{
						Logger.LogDebug($"Issue {s.Key.Value} is saved.");
						return ObjectToSaveConstructor(s);
					})
					.ToList();

			Logger.LogDebug($"Issues from server received.");

			return issuesToSave;
		}
	}
}
