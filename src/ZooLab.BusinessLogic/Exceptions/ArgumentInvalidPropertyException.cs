using System;

namespace ZooLab.BusinessLogic.Exceptions
{
	/// <summary>
	/// Throw when a parameter's property (the argument) is invalid.
	/// </summary>
	public class ArgumentInvalidPropertyException : ArgumentException
	{
		public virtual string PropertyName { get; private set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="paramName">The name of the parameter that hold the invalid property.</param>
		/// <param name="propertyName">The invalid property's name.</param>
		/// <param name="validRuleMessage">The message describing why the property is invalid.</param>
		public ArgumentInvalidPropertyException(string paramName, string propertyName, string validRuleMessage = null)
			: base(EnrichPropertyValidRuleMessage(propertyName, validRuleMessage), paramName)
		{
			PropertyName = propertyName;
		}

		private static string EnrichPropertyValidRuleMessage(string propertyName, string validRuleMessage = null)
		{
			return $"{propertyName}: {validRuleMessage ?? "invalid"}";
		}
	}
}
