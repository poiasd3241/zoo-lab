using Xunit;
using ZooLab.BusinessLogic.Exceptions;

namespace ZooLab.BusinessLogic.Tests.Exceptions
{
	public class AssignNullExceptionTests
	{
		[Fact]
		public void ShouldCreateAssignNullExceptionException()
		{
			var notNullObjectName = "prop";

			var exception = new AssignNullException(notNullObjectName);

			Assert.Equal("Cannot assign 'null' to prop", exception.Message);
		}
	}
}
