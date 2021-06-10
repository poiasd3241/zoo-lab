using Xunit;
using ZooLab.BusinessLogic.ExtensionMethods;

namespace ZooLab.BusinessLogic.Tests.ExtensionMethods
{
	public class ObjectExtensionsTests
	{
		[Fact]
		public void ShouldReturnObjectTypeName()
		{
			object obj = new();

			Assert.Equal("Object", ObjectExtensions.GetTypeName(obj));
		}
	}
}
