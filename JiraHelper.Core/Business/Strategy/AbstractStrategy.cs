using System;
using System.Text.RegularExpressions;

namespace JiraHelper.Core.Business.Strategy
{
	/// <summary>
	/// Abstract strategy
	/// </summary>
	public abstract class AbstractStrategy
	{
		/// <summary>
		/// The regex limiting values posible in key
		/// </summary>
		public static readonly Regex KEY_REGEX = new Regex("[a-zA-Z0-9]+");

		/// <summary>
		/// The key
		/// </summary>
		protected string _key;

		/// <summary>
		/// The key.
		/// </summary>
		public string Key => _key;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractStrategy"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <exception cref="System.ArgumentNullException">key</exception>
		/// <exception cref="System.ArgumentException">Key '{key}' not allowed by rule '{KEY_REGEX}' - key</exception>
		protected AbstractStrategy(string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			if (!KEY_REGEX.IsMatch(key))
				throw new ArgumentException($"Key '{key}' not allowed by rule '{KEY_REGEX}'", nameof(key));

			_key = key;
		}
	}
}
