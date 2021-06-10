using System;
using Xunit;
using ZooLab.BusinessLogic.Zoos;

namespace ZooLab.BusinessLogic.Tests.Zoos
{
	public class IntIdGeneratorTests
	{
		[Fact]
		public void ShouldCreateIntIdGenerator()
		{
			var idGen = new IntIdGenerator(2);

			Assert.Equal(0, idGen.NextID);
			Assert.Equal(0, idGen.GetNextID());
			Assert.Equal(1, idGen.NextID);
			Assert.Equal(1, idGen.GetNextID());
		}

		[Fact]
		public void ShouldFailCreateIntIdGeneratorByMaxIdInvalid()
		{
			var exception = Assert.Throws<ArgumentException>(() => new IntIdGenerator(-1));

			Assert.Equal("Max ID must be greater than 0.", exception.Message);
		}

		[Fact]
		public void ShouldFailGetNextIdByOverflow()
		{
			var idGen = new IntIdGenerator(1);
			idGen.GetNextID();

			var exception = Assert.Throws<OverflowException>(() => idGen.GetNextID());

			Assert.Equal("Reached maximum value of the generated ID (1).", exception.Message);
		}
	}
}
