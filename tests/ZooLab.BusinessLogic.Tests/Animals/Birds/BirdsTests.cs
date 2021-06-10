using Xunit;
using ZooLab.BusinessLogic.Animals.Birds;
using ZooLab.BusinessLogic.Animals.Mammals;
using ZooLab.BusinessLogic.Animals.Reptiles;

namespace ZooLab.BusinessLogic.Tests.Animals.Birds
{
	public class BirdsTests
	{
		[Fact]
		public void ShouldCreateParrot()
		{
			Parrot parrot = new();

			Assert.Equal(new[] { "Vegetable" }, parrot.FavoriteFood);
			Assert.Equal(5, parrot.RequiredSpaceSqFt);
			Assert.Equal(2, parrot.MaxDailyFeedings);
			Assert.True(parrot.IsFriendlyWith(new Parrot()));
			Assert.True(parrot.IsFriendlyWith(new Bison()));
			Assert.True(parrot.IsFriendlyWith(new Elephant()));
			Assert.True(parrot.IsFriendlyWith(new Turtle()));
			Assert.False(parrot.IsFriendlyWith(new Lion()));
		}

		[Fact]
		public void ShouldCreatePenguin()
		{
			Penguin penguin = new();

			Assert.Equal(new[] { "Meat" }, penguin.FavoriteFood);
			Assert.Equal(10, penguin.RequiredSpaceSqFt);
			Assert.Equal(2, penguin.MaxDailyFeedings);
			Assert.True(penguin.IsFriendlyWith(new Penguin()));
			Assert.False(penguin.IsFriendlyWith(new Parrot()));
		}
	}
}
