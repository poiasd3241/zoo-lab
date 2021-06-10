using Xunit;
using ZooLab.BusinessLogic.Exceptions;

namespace ZooLab.BusinessLogic.Tests.Exceptions
{
	public class NotFriendlyAnimalExceptionTests
	{
		[Fact]
		public void ShouldCreateNotFriendlyAnimalException()
		{
			var message = "hello";

			var exception = new NotFriendlyAnimalException(message);

			Assert.Equal(message, exception.Message);
		}
	}
}
