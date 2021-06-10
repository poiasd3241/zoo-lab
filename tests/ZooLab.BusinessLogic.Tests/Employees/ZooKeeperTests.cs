using System;
using System.Collections.Generic;
using Xunit;
using ZooLab.BusinessLogic.Animals.Mammals;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.Exceptions;
using ZooLab.BusinessLogic.Tests.Animals;
using ZooLab.BusinessLogic.Tests.Logging;

namespace ZooLab.BusinessLogic.Tests.Employees
{
	public class ZooKeeperTests
	{
		#region Basic

		[Fact]
		public void ShouldCreateZooKeeper()
		{
			ZooKeeper zooKeeper = new("a", "b");

			Assert.Equal("a", zooKeeper.FirstName);
			Assert.Equal("b", zooKeeper.LastName);
			Assert.Empty(zooKeeper.AnimalExperiences);
			Assert.False(zooKeeper.HasAnimalExperiece(new Lion()));
		}

		#endregion

		#region Add Animal Experience

		[Fact]
		public void ShouldFailAddAnimalExperienceByAnimalNull()
		{
			var console = new TestConsole();
			ZooKeeper zooKeeper = new("a", "b", console);

			var exception = Assert.Throws<ArgumentNullException>(() => zooKeeper.AddAnimalExperience(null));

			Assert.Equal("animal", exception.ParamName);
			Assert.Empty(zooKeeper.AnimalExperiences);
			Assert.Equal("Cannot add an animal experience: the animal is not provided.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailAddAnimalExperienceByAlreadyExperiencedWithAnimal()
		{
			var console = new TestConsole();
			ZooKeeper zooKeeper = new("a", "b", console);
			var lion1 = new Lion();
			var lion2 = new Lion();

			zooKeeper.AddAnimalExperience(lion1);
			console.Clear();
			zooKeeper.AddAnimalExperience(lion2);

			Assert.Equal(new List<string>() { "Lion" }, zooKeeper.AnimalExperiences);
			Assert.Equal("ZooKeeper a b is already experienced with Lion.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldAddAnimalExperience()
		{
			var console = new TestConsole();
			ZooKeeper zooKeeper = new("a", "b", console);
			var lion = new Lion();

			zooKeeper.AddAnimalExperience(lion);

			Assert.Equal(new List<string>() { "Lion" }, zooKeeper.AnimalExperiences);
			Assert.True(zooKeeper.HasAnimalExperiece(new Lion()));
			Assert.Equal("ZooKeeper a b is now experienced with Lion.\n", console.CurrentOutput);
		}

		#endregion

		#region Feed Animal

		[Fact]
		public void ShouldFailFeedAnimalByAnimalNull()
		{
			var console = new TestConsole();
			ZooKeeper zooKeeper = new("a", "b", console);

			var exception = Assert.Throws<ArgumentNullException>(() => zooKeeper.FeedAnimal(null, DateTime.Now));

			Assert.Equal("animal", exception.ParamName);
			Assert.Empty(zooKeeper.AnimalExperiences);
			Assert.Equal("Cannot feed an animal: the animal is not provided.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailFeedAnimalByNotExperiencedWithAnimal()
		{
			var console = new TestConsole();
			var animal = new TestAnimal();
			ZooKeeper zooKeeper = new("a", "b", console);

			var result = zooKeeper.FeedAnimal(animal, DateTime.Now);

			Assert.False(result);
			Assert.Equal("Cannot feed TestAnimal: no experience.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailFeedAnimalByFeedScheduleNullOrEmpty()
		{
			var console = new TestConsole();
			var animal = new TestAnimal();
			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);
			console.Clear();

			var exception = Assert.Throws<ArgumentInvalidPropertyException>(() => zooKeeper.FeedAnimal(animal, DateTime.Now));

			Assert.Equal("animal", exception.ParamName);
			Assert.Equal("FeedSchedule", exception.PropertyName);
			Assert.Equal("Cannot feed TestAnimal #0: feed schedule is undefined or empty.\n", console.CurrentOutput);
		}

		[Theory]
		[InlineData(22)]
		[InlineData(3)]
		public void ShouldFailFeedAnimalByFeedScheduleNotMatchingCurrentTime(int feedScheduleHour)
		{
			var console = new TestConsole();
			var animal = new TestAnimal();
			animal.AddFeedSchedule(new() { feedScheduleHour });
			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);
			// Set the feeding time to 1 hour after the schedule.
			var now = DateTime.Now.Date + new TimeSpan(feedScheduleHour + 1, 0, 0);

			console.Clear();
			var result = zooKeeper.FeedAnimal(animal, now);

			Assert.False(result);
			Assert.Equal($"Cannot feed TestAnimal #0 between {now.Hour}:00 and {now.AddHours(1).Hour}:00.\n",
				console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailFeedAnimalByDailyLimitZero()
		{
			var console = new TestConsole();
			var animal = new TestAnimal();

			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);

			animal.SetCustomMaxDailyFeedings(0);
			animal.AddFeedSchedule(new() { 4 });
			var now = GetNowWithHour(4);
			console.Clear();

			//When
			var exception = Assert.Throws<ArgumentInvalidPropertyException>(
				() => zooKeeper.FeedAnimal(animal, now));

			Assert.Equal("animal", exception.ParamName);
			Assert.Equal("MaxDailyFeedings", exception.PropertyName);
			Assert.Equal("MaxDailyFeedings: cannot be zero (Parameter 'animal')", exception.Message);
			Assert.Empty(console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailFeedAnimalByDailyLimitExcess()
		{
			var console = new TestConsole();
			var animal = new TestAnimal();

			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);
			var time = GetNowWithHour(2);
			animal.SetCustomFeedTimes(new() { new(time, zooKeeper) });

			animal.AddFeedSchedule(new() { 4 });
			var now = GetNowWithHour(4);

			console.Clear();
			var result = zooKeeper.FeedAnimal(animal, now);

			Assert.False(result);
			Assert.Equal($"Cannot feed TestAnimal more than 1 time(s) in 24 hours.\n",
				console.CurrentOutput);
		}

		public class FailFeedAnimalByFavoriteFoodNullOrEmpty : TheoryData<string[]>
		{
			public FailFeedAnimalByFavoriteFoodNullOrEmpty()
			{
				Add(null);
				Add(Array.Empty<string>());
			}
		}

		[Theory]
		[ClassData(typeof(FailFeedAnimalByFavoriteFoodNullOrEmpty))]
		public void ShouldFailFeedAnimalByFavoriteFoodNullOrEmpty(string[] favoriteFood)
		{
			var console = new TestConsole();
			var animal = new TestAnimal();

			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);

			animal.SetCustomFavoriteFood(favoriteFood);
			animal.AddFeedSchedule(new() { 4 });
			var now = GetNowWithHour(4);
			console.Clear();

			// When
			var exception = Assert.Throws<ArgumentInvalidPropertyException>(
				() => zooKeeper.FeedAnimal(animal, now));

			Assert.Equal("animal", exception.ParamName);
			Assert.Equal("FavoriteFood", exception.PropertyName);
			Assert.Equal("FavoriteFood: cannot be null or empty (Parameter 'animal')", exception.Message);
			Assert.Equal($"Cannot feed TestAnimal #0: favorite food list is undefined or empty.\n",
				console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailFeedAnimalByFavoriteFoodUnknown()
		{

			var console = new TestConsole();
			var animal = new TestAnimal();

			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);

			var unknownFoodName = "Cake";
			var expectedMessage = "Unknown food name: 'Cake'.";
			animal.SetCustomFavoriteFood(new[] { unknownFoodName });
			animal.AddFeedSchedule(new() { 4 });
			var now = GetNowWithHour(4);
			console.Clear();

			// When
			var exception = Assert.Throws<NotImplementedException>(
				() => zooKeeper.FeedAnimal(animal, now));

			Assert.Equal(expectedMessage, exception.Message);
			Assert.Equal($"{expectedMessage}\n", console.CurrentOutput);
		}

		public class FeedAnimalWithDifferentFood : TheoryData<string>
		{
			public FeedAnimalWithDifferentFood()
			{
				Add("Grass");
				Add("Vegetable");
				Add("Meat");
			}
		}

		[Theory]
		[ClassData(typeof(FeedAnimalWithDifferentFood))]
		public void ShouldFeedAnimal(string foodName)
		{
			var console = new TestConsole();
			var animal = new TestAnimal();

			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);

			animal.SetCustomFavoriteFood(new[] { foodName });
			animal.AddFeedSchedule(new() { 4 });
			var now = GetNowWithHour(4);
			console.Clear();

			// When
			var result = zooKeeper.FeedAnimal(animal, now);

			Assert.True(result);
			Assert.Equal($"ZooKeeper a b fed TestAnimal #0 with {foodName}.\n", console.CurrentOutput);
		}

		#endregion

		#region Helpers

		private static DateTime GetNowWithHour(int hour) =>
			DateTime.Now.Date + new TimeSpan(hour, 0, 0);

		#endregion
	}
}
