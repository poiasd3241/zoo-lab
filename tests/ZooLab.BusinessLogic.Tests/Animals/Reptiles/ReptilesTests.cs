using Xunit;
using ZooLab.BusinessLogic.Animals.Birds;
using ZooLab.BusinessLogic.Animals.Mammals;
using ZooLab.BusinessLogic.Animals.Reptiles;

namespace ZooLab.BusinessLogic.Tests.Animals.Reptiles
{
	public class ReptilesTests
	{
		[Fact]
		public void ShouldCreateSnake()
		{
			Snake snake = new();

			Assert.Equal(new[] { "Meat" }, snake.FavoriteFood);
			Assert.Equal(2, snake.RequiredSpaceSqFt);
			Assert.Equal(2, snake.MaxDailyFeedings);
			Assert.True(snake.IsFriendlyWith(new Snake()));
			Assert.False(snake.IsFriendlyWith(new Parrot()));
		}

		[Fact]
		public void ShouldCreateTurtle()
		{
			Turtle turtle = new();

			Assert.Equal(new[] { "Vegetable" }, turtle.FavoriteFood);
			Assert.Equal(5, turtle.RequiredSpaceSqFt);
			Assert.Equal(2, turtle.MaxDailyFeedings);
			Assert.True(turtle.IsFriendlyWith(new Turtle()));
			Assert.True(turtle.IsFriendlyWith(new Bison()));
			Assert.True(turtle.IsFriendlyWith(new Elephant()));
			Assert.True(turtle.IsFriendlyWith(new Parrot()));
			Assert.False(turtle.IsFriendlyWith(new Lion()));
		}
	}
}
