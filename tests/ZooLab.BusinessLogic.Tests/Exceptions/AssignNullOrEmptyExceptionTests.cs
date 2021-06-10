using Xunit;
using ZooLab.BusinessLogic.Exceptions;

namespace ZooLab.BusinessLogic.Tests.Exceptions
{
	public class AssignNullOrEmptyExceptionTests
	{
		[Fact]
		public void ShouldCreateAssignNullOrEmptyExceptionDefault()
		{
			var notNullNorEmptyObjectName = "prop";

			var exception = new AssignNullOrEmptyException(notNullNorEmptyObjectName);

			Assert.Equal("Cannot assign 'null' or empty to prop", exception.Message);
		}

		[Fact]
		public void ShouldCreateAssignNullOrEmptyException()
		{
			var notNullNorEmptyObjectName = "prop";

			var exception = new AssignNullOrEmptyException(notNullNorEmptyObjectName, true);

			Assert.Equal("Cannot assign 'null', empty or whitespace to prop", exception.Message);
		}
	}
}
