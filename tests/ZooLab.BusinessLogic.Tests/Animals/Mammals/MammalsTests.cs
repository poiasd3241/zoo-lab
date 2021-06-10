using Xunit;
using ZooLab.BusinessLogic.Animals.Birds;
using ZooLab.BusinessLogic.Animals.Mammals;
using ZooLab.BusinessLogic.Animals.Reptiles;

namespace ZooLab.BusinessLogic.Tests.Animals.Mammals
{
	public class MammalsTests
	{
		[Fact]
		public void ShouldCreateLion()
		{
			Lion lion = new();

			Assert.Equal(new[] { "Meat" }, lion.FavoriteFood);
			Assert.Equal(1000, lion.RequiredSpaceSqFt);
			Assert.Equal(2, lion.MaxDailyFeedings);
			Assert.True(lion.IsFriendlyWith(new Lion()));
			Assert.False(lion.IsFriendlyWith(new Parrot()));
		}

		[Fact]
		public void ShouldCreateBison()
		{
			Bison bison = new();

			Assert.Equal(new[] { "Grass" }, bison.FavoriteFood);
			Assert.Equal(1000, bison.RequiredSpaceSqFt);
			Assert.Equal(2, bison.MaxDailyFeedings);
			Assert.True(bison.IsFriendlyWith(new Bison()));
			Assert.True(bison.IsFriendlyWith(new Elephant()));
			Assert.False(bison.IsFriendlyWith(new Parrot()));
		}

		[Fact]
		public void ShouldCreateElephant()
		{
			Elephant elephant = new();

			Assert.Equal(new[] { "Vegetable" }, elephant.FavoriteFood);
			Assert.Equal(1000, elephant.RequiredSpaceSqFt);
			Assert.Equal(2, elephant.MaxDailyFeedings);
			Assert.True(elephant.IsFriendlyWith(new Elephant()));
			Assert.True(elephant.IsFriendlyWith(new Bison()));
			Assert.True(elephant.IsFriendlyWith(new Parrot()));
			Assert.True(elephant.IsFriendlyWith(new Turtle()));
			Assert.False(elephant.IsFriendlyWith(new Lion()));
		}
	}
}
