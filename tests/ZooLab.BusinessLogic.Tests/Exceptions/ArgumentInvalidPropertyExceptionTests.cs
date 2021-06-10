using Xunit;
using ZooLab.BusinessLogic.Exceptions;

namespace ZooLab.BusinessLogic.Tests.Exceptions
{
	public class ArgumentInvalidPropertyExceptionTests
	{
		[Fact]
		public void ShouldCreateArgumentInvalidPropertyExceptionDefault()
		{
			var paramName = "person";
			var propertyName = "Email";

			var exception = new ArgumentInvalidPropertyException(paramName, propertyName);

			Assert.Equal(paramName, exception.ParamName);
			Assert.Equal(propertyName, exception.PropertyName);
			Assert.Equal("Email: invalid (Parameter 'person')", exception.Message);
		}

		[Fact]
		public void ShouldCreateArgumentInvalidPropertyException()
		{
			var paramName = "person";
			var propertyName = "Email";
			var validRuleMessage = "must be good";

			var exception = new ArgumentInvalidPropertyException(paramName, propertyName, validRuleMessage);

			Assert.Equal(paramName, exception.ParamName);
			Assert.Equal(propertyName, exception.PropertyName);
			Assert.Equal("Email: must be good (Parameter 'person')", exception.Message);
		}
	}
}
