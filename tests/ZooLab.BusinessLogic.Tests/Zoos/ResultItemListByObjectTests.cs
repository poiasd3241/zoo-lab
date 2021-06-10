using Xunit;
using ZooLab.BusinessLogic.Exceptions;
using ZooLab.BusinessLogic.Zoos;

namespace ZooLab.BusinessLogic.Tests.Zoos
{
	public class ResultItemListByObjectTests
	{
		[Fact]
		public void ShouldCreateResultItemListByObject()
		{
			var result = new ResultItemListByObject<string, int>();

			Assert.Empty(result.Data);
			Assert.True(result.IsEmpty);
			Assert.Equal("Reason not specified.", result.ReasonEmpty);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public void ShouldFailSetReasonEmptyToNullOrWhitespace(string reasonEmpty)
		{
			var result = new ResultItemListByObject<string, int>();

			var exception = Assert.Throws<AssignNullOrEmptyException>(() =>
				result.ReasonEmpty = reasonEmpty);

			Assert.Equal("Cannot assign 'null', empty or whitespace to ReasonEmpty", exception.Message);
			Assert.Equal("Reason not specified.", result.ReasonEmpty);
		}

		[Fact]
		public void ShouldSetReasonEmpty()
		{
			var result = new ResultItemListByObject<string, int>();

			result.ReasonEmpty = "because";

			Assert.Equal("because", result.ReasonEmpty);
		}

		[Fact]
		public void ShouldAddItemForObject()
		{
			var result = new ResultItemListByObject<string, int>();
			result.AddItemForObject("one", 123);
			result.AddItemForObject("one", 222);

			Assert.False(result.IsEmpty);
			var saved = Assert.Single(result.Data);
			Assert.Equal("one", saved.Key);
			Assert.Equal(2, saved.Value.Count);
			Assert.Equal(123, saved.Value[0]);
			Assert.Equal(222, saved.Value[1]);
		}

	}
}
