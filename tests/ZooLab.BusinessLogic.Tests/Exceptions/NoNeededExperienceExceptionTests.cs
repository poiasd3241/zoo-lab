using Xunit;
using ZooLab.BusinessLogic.Exceptions;

namespace ZooLab.BusinessLogic.Tests.Exceptions
{
	public class NoNeededExperienceExceptionTests
	{
		[Fact]
		public void ShouldCreateNoNeededExperienceException()
		{
			var message = "hello";

			var exception = new NoNeededExperienceException(message);

			Assert.Equal(message, exception.Message);
		}
	}
}
