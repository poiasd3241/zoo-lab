using System;
using System.Collections.Generic;
using Xunit;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Animals.Mammals;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.Exceptions;
using ZooLab.BusinessLogic.Tests.Animals;
using ZooLab.BusinessLogic.Tests.Logging;
using static ZooLab.BusinessLogic.Tests.Logging.TestConsole;

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

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailAddAnimalExperienceByAnimalNull(TestConsole console)
		{
			ZooKeeper zooKeeper = new("a", "b", console);

			var exception = Assert.Throws<ArgumentNullException>(() => zooKeeper.AddAnimalExperience(null));

			Assert.Equal("animal", exception.ParamName);
			Assert.Empty(zooKeeper.AnimalExperiences);
			if (console is not null)
			{
				Assert.Equal("Cannot add an animal experience: the animal is not provided.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailAddAnimalExperienceByAlreadyExperiencedWithAnimal(TestConsole console)
		{
			ZooKeeper zooKeeper = new("a", "b", console);
			var lion1 = new Lion();
			var lion2 = new Lion();

			zooKeeper.AddAnimalExperience(lion1);
			console?.Clear();
			zooKeeper.AddAnimalExperience(lion2);

			Assert.Equal(new List<string>() { "Lion" }, zooKeeper.AnimalExperiences);
			if (console is not null)
			{
				Assert.Equal("ZooKeeper a b is already experienced with Lion.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldAddAnimalExperience(TestConsole console)
		{
			ZooKeeper zooKeeper = new("a", "b", console);
			var lion = new Lion();

			zooKeeper.AddAnimalExperience(lion);

			Assert.Equal(new List<string>() { "Lion" }, zooKeeper.AnimalExperiences);
			Assert.True(zooKeeper.HasAnimalExperiece(new Lion()));
			if (console is not null)
			{
				Assert.Equal("ZooKeeper a b is now experienced with Lion.\n", console.CurrentOutput);
			}
		}

		#endregion

		#region Feed Animal

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFeedAnimalByAnimalNull(TestConsole console)
		{
			ZooKeeper zooKeeper = new("a", "b", console);

			var exception = Assert.Throws<ArgumentNullException>(() => zooKeeper.FeedAnimal(null, DateTime.Now));

			Assert.Equal("animal", exception.ParamName);
			Assert.Empty(zooKeeper.AnimalExperiences);
			if (console is not null)
			{
				Assert.Equal("Cannot feed an animal: the animal is not provided.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFeedAnimalByNotExperiencedWithAnimal(TestConsole console)
		{
			var animal = new TestAnimal();
			ZooKeeper zooKeeper = new("a", "b", console);

			var result = zooKeeper.FeedAnimal(animal, DateTime.Now);

			Assert.False(result);
			if (console is not null)
			{
				Assert.Equal("Cannot feed TestAnimal: no experience.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFeedAnimalByFeedScheduleNullOrEmpty(TestConsole console)
		{
			var animal = new TestAnimal();
			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);
			console?.Clear();

			var exception = Assert.Throws<ArgumentInvalidPropertyException>(() => zooKeeper.FeedAnimal(animal, DateTime.Now));

			Assert.Equal("animal", exception.ParamName);
			Assert.Equal("FeedSchedule", exception.PropertyName);
			if (console is not null)
			{
				Assert.Equal("Cannot feed TestAnimal #0: feed schedule is undefined or empty.\n", console.CurrentOutput);
			}
		}

		public class FailFeedAnimalByFeedScheduleNotMatchingCurrentTimeWithTestConsole : TheoryData<TestConsole, int>
		{
			public FailFeedAnimalByFeedScheduleNotMatchingCurrentTimeWithTestConsole()
			{
				Add(null, 22);
				Add(null, 3);
				Add(new(), 22);
				Add(new(), 3);
			}
		}

		[Theory]
		[ClassData(typeof(FailFeedAnimalByFeedScheduleNotMatchingCurrentTimeWithTestConsole))]
		public void ShouldFailFeedAnimalByFeedScheduleNotMatchingCurrentTime(TestConsole console, int feedScheduleHour)
		{
			var animal = new TestAnimal();
			animal.AddFeedSchedule(new() { feedScheduleHour });
			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);
			// Set the feeding time to 1 hour after the schedule.
			var now = DateTime.Now.Date + new TimeSpan(feedScheduleHour + 1, 0, 0);

			console?.Clear();
			var result = zooKeeper.FeedAnimal(animal, now);

			Assert.False(result);
			if (console is not null)
			{
				Assert.Equal($"Cannot feed TestAnimal #0 between {now.Hour}:00 and {now.AddHours(1).Hour}:00.\n",
				console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFeedAnimalByDailyLimitZero(TestConsole console)
		{
			var animal = new TestAnimal();

			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);

			animal.SetCustomMaxDailyFeedings(0);
			animal.AddFeedSchedule(new() { 4 });
			var now = GetNowWithHour(4);
			console?.Clear();

			//When
			var exception = Assert.Throws<ArgumentInvalidPropertyException>(
				() => zooKeeper.FeedAnimal(animal, now));

			Assert.Equal("animal", exception.ParamName);
			Assert.Equal("MaxDailyFeedings", exception.PropertyName);
			Assert.Equal("MaxDailyFeedings: cannot be zero (Parameter 'animal')", exception.Message);
			if (console is not null)
			{
				Assert.Empty(console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFeedAnimalByDailyLimitExcess(TestConsole console)
		{
			var animal = new TestAnimal();

			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);
			var timeToday = GetNowWithHour(2);
			var timeYesterday = GetNowWithHour(2).AddDays(-1);
			animal.SetCustomFeedTimes(new() { new(timeYesterday, zooKeeper), new(timeToday, zooKeeper) });

			animal.AddFeedSchedule(new() { 4 });
			var now = GetNowWithHour(4);

			console?.Clear();
			var result = zooKeeper.FeedAnimal(animal, now);

			Assert.False(result);
			if (console is not null)
			{
				Assert.Equal($"Cannot feed TestAnimal more than 1 time(s) in 24 hours.\n", console.CurrentOutput);
			}
		}

		public class FailFeedAnimalByFavoriteFoodNullOrEmptyWithTestConsole : TheoryData<TestConsole, string[]>
		{
			public FailFeedAnimalByFavoriteFoodNullOrEmptyWithTestConsole()
			{
				Add(null, null);
				Add(null, Array.Empty<string>());
				Add(new(), null);
				Add(new(), Array.Empty<string>());
			}
		}

		[Theory]
		[ClassData(typeof(FailFeedAnimalByFavoriteFoodNullOrEmptyWithTestConsole))]
		public void ShouldFailFeedAnimalByFavoriteFoodNullOrEmpty(TestConsole console, string[] favoriteFood)
		{
			var animal = new TestAnimal();

			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);

			animal.SetCustomFavoriteFood(favoriteFood);
			animal.AddFeedSchedule(new() { 4 });
			var now = GetNowWithHour(4);
			console?.Clear();

			// When
			var exception = Assert.Throws<ArgumentInvalidPropertyException>(
				() => zooKeeper.FeedAnimal(animal, now));

			Assert.Equal("animal", exception.ParamName);
			Assert.Equal("FavoriteFood", exception.PropertyName);
			Assert.Equal("FavoriteFood: cannot be null or empty (Parameter 'animal')", exception.Message);
			if (console is not null)
			{
				Assert.Equal($"Cannot feed TestAnimal #0: favorite food list is undefined or empty.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFeedAnimalByFavoriteFoodUnknown(TestConsole console)
		{
			var animal = new TestAnimal();

			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);

			var unknownFoodName = "Cake";
			var expectedMessage = "Unknown food name: 'Cake'.";
			animal.SetCustomFavoriteFood(new[] { unknownFoodName });
			animal.AddFeedSchedule(new() { 4 });
			var now = GetNowWithHour(4);
			console?.Clear();

			// When
			var exception = Assert.Throws<NotImplementedException>(
				() => zooKeeper.FeedAnimal(animal, now));

			Assert.Equal(expectedMessage, exception.Message);
			if (console is not null)
			{
				Assert.Equal($"{expectedMessage}\n", console.CurrentOutput);
			}
		}

		public class FeedAnimalWithDifferentFoodWithTestConsole : TheoryData<TestConsole, string>
		{
			public FeedAnimalWithDifferentFoodWithTestConsole()
			{
				Add(null, "Grass");
				Add(null, "Vegetable");
				Add(null, "Meat");
				Add(new(), "Grass");
				Add(new(), "Vegetable");
				Add(new(), "Meat");
			}
		}

		[Theory]
		[ClassData(typeof(FeedAnimalWithDifferentFoodWithTestConsole))]
		public void ShouldFeedAnimal(TestConsole console, string foodName)
		{
			var animal = new TestAnimal();

			ZooKeeper zooKeeper = new("a", "b", console);
			zooKeeper.AddAnimalExperience(animal);

			animal.SetCustomFavoriteFood(new[] { foodName });
			animal.AddFeedSchedule(new() { 4 });
			var now = GetNowWithHour(4);
			console?.Clear();

			// When
			var result = zooKeeper.FeedAnimal(animal, now);

			Assert.True(result);
			if (console is not null)
			{
				Assert.Equal($"ZooKeeper a b fed TestAnimal #0 with {foodName}.\n", console.CurrentOutput);
			}
		}

		#endregion

		#region Helpers

		private static DateTime GetNowWithHour(int hour) =>
			DateTime.Now.Date + new TimeSpan(hour, 0, 0);

		#endregion
	}
}
