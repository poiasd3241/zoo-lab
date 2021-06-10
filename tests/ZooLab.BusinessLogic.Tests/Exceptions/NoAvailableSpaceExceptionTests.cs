using Xunit;
using ZooLab.BusinessLogic.Exceptions;

namespace ZooLab.BusinessLogic.Tests.Exceptions
{
	public class NoAvailableSpaceExceptionTests
	{
		[Fact]
		public void ShouldCreateNoAvailableSpaceException()
		{
			var message = "hello";

			var exception = new NoAvailableSpaceException(message);

			Assert.Equal(message, exception.Message);
		}
	}
}
