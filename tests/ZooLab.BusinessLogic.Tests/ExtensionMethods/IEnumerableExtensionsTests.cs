using System.Collections.Generic;
using Xunit;
using ZooLab.BusinessLogic.ExtensionMethods;

namespace ZooLab.BusinessLogic.Tests.ExtensionMethods
{
	public class IEnumerableExtensionsTests
	{
		[Fact]
		public void ShouldValidateIsNullOrEmpty()
		{
			List<string> nullList = null;
			List<string> emptyList = new();

			Assert.True(IEnumerableExtensions.IsNullOrEmpty(nullList));
			Assert.True(IEnumerableExtensions.IsNullOrEmpty(emptyList));
		}

		[Fact]
		public void ShouldInvalidateIsNullOrEmpty()
		{
			List<string> list = new() { "one" };

			Assert.False(IEnumerableExtensions.IsNullOrEmpty(list));
		}
	}
}
