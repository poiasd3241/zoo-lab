using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Animals.Supplies.FoodSupplies;
using ZooLab.BusinessLogic.Animals.Supplies.MedicalSupplies;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.Exceptions;
using ZooLab.BusinessLogic.Logging;
using ZooLab.BusinessLogic.Tests.Logging;
using static ZooLab.BusinessLogic.Tests.Logging.TestConsole;

namespace ZooLab.BusinessLogic.Tests.Animals
{
	public partial class AnimalTests
	{
		#region Feed

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFeedByNullFood(TestConsole console)
		{
			TestAnimal animal = new(console);

			var exception = Assert.Throws<ArgumentNullException>(() => animal.Feed(null, new ZooKeeper("a", "b"), DateTime.Now));

			Assert.Empty(animal.FeedTimes);
			Assert.Equal("food", exception.ParamName);
			if (console is not null)
			{
				Assert.Equal("Cannot feed without food.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFeedByNullZooKeeper(TestConsole console)
		{
			TestAnimal animal = new(console);

			var exception = Assert.Throws<ArgumentNullException>(() => animal.Feed(new Grass(), null, DateTime.Now));

			Assert.Empty(animal.FeedTimes);
			Assert.Equal("zooKeeper", exception.ParamName);
			if (console is not null)
			{
				Assert.Equal("Feeding requires a zoo keeper.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFeedByNotFavoriteFood(TestConsole console)
		{
			TestAnimal animal = new(console);

			Assert.Empty(animal.FeedTimes);
			animal.Feed(new Grass(), new ZooKeeper("a", "b"), DateTime.Now);
			if (console is not null)
			{
				Assert.Equal("Cannot feed TestAnimal with Grass.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFeed(TestConsole console)
		{
			TestAnimal animal = new(console);
			var now = DateTime.Now;
			var zooKeeper = new ZooKeeper("a", "b");
			var expectedFeedTimes = new List<FeedTime>() { new FeedTime(now, zooKeeper) };

			animal.Feed(new Meat(), zooKeeper, now);

			var actualFeedTimes = animal.FeedTimes;


			var actualFeedTime = Assert.Single(actualFeedTimes);
			Assert.True(DateTime.Equals(expectedFeedTimes[0].Time, actualFeedTime.Time));
			Assert.Equal(expectedFeedTimes[0].FedByZooKeeper, actualFeedTime.FedByZooKeeper);
			if (console is not null)
			{
				Assert.Equal("Fed TestAnimal #0 with Meat.\n", console.CurrentOutput);
			}
		}

		#endregion

		#region Add Feed Schedule

		public class FailAddFeedSchedule
		{
			public class ByHoursNullOrEmptyWithTestConsole : TheoryData<TestConsole, List<int>>
			{
				public ByHoursNullOrEmptyWithTestConsole()
				{
					Add(null, null);
					Add(null, new());
					Add(new(), null);
					Add(new(), new());
				}
			}
			public class ByHoursOutOfRangeWithTestConsole : TheoryData<TestConsole, List<int>>
			{
				public ByHoursOutOfRangeWithTestConsole()
				{
					Add(null, new() { 25 });
					Add(null, new() { -1 });
					Add(new(), new() { 25 });
					Add(new(), new() { -1 });
				}
			}
		}

		[Theory]
		[ClassData(typeof(FailAddFeedSchedule.ByHoursNullOrEmptyWithTestConsole))]
		public void ShouldFailAddFeedScheduleByHoursNullOrEmpty(TestConsole console, List<int> hours)
		{
			TestAnimal animal = new(console);

			var exception = Assert.Throws<ArgumentNullOrEmptyException>(() => animal.AddFeedSchedule(hours));

			Assert.Empty(animal.FeedSchedule);
			Assert.Equal("hours", exception.ParamName);
			if (console is not null)
			{
				Assert.Equal("Cannot add an unspecified feed schedule.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(FailAddFeedSchedule.ByHoursOutOfRangeWithTestConsole))]
		public void ShouldFailAddFeedScheduleByHoursOutOfRange(TestConsole console, List<int> hours)
		{
			TestAnimal animal = new(console);

			animal.AddFeedSchedule(hours);

			Assert.Empty(animal.FeedSchedule);
			if (console is not null)
			{
				Assert.Equal("Please specify the feeding schedule values as the hour from 0 to 23.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldNotAddFeedScheduleByHoursAlreadyDefined(TestConsole console)
		{
			TestAnimal animal = new(console);
			var feedSchedule = new List<int>() { 2 };

			animal.AddFeedSchedule(feedSchedule);
			console?.Clear();
			animal.AddFeedSchedule(feedSchedule);

			Assert.Equal(feedSchedule, animal.FeedSchedule);
			if (console is not null)
			{
				Assert.Equal("TestAnimal #0 already has the feeding schedule 2:00 - 3:00 defined.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldAddFeedSchedule(TestConsole console)
		{
			TestAnimal animal = new(console);
			var feedSchedule = new List<int>() { 2 };

			animal.AddFeedSchedule(feedSchedule);

			Assert.Equal(feedSchedule, animal.FeedSchedule);
			if (console is not null)
			{
				Assert.Equal("Added feeding schedule for the TestAnimal #0: 2:00 - 3:00.\n", console.CurrentOutput);
			}
		}

		#endregion

		#region Heal

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailHealByHealthy(TestConsole console)
		{
			var animal = new TestAnimal(Animal.SicknessType.None, console);
			var medicine = new AntiInflammatory();

			animal.Heal(medicine);

			if (console is not null)
			{
				Assert.Equal("The TestAnimal #0 is not sick and therefore doesn't need healing.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailHealByMedicineNull(TestConsole console)
		{
			var animal = new TestAnimal(Animal.SicknessType.Infection, console);

			var exception = Assert.Throws<ArgumentNullException>(() => animal.Heal(null));

			Assert.Equal("medicine", exception.ParamName);
			if (console is not null)
			{
				Assert.Equal("Cannot heal without medicine.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailHealByMedicineWrong(TestConsole console)
		{
			var animal = new TestAnimal(Animal.SicknessType.Inflammation, console);
			var medicine = new AntiDepression();

			animal.Heal(medicine);

			if (console is not null)
			{
				Assert.Equal("Cannot heal Inflammation with AntiDepression.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldHeal(TestConsole console)
		{
			var animal = new TestAnimal(Animal.SicknessType.Inflammation, console);
			var medicine = new AntiInflammatory();

			animal.Heal(medicine);

			Assert.Equal(Animal.SicknessType.None, animal.Sickness);
			if (console is not null)
			{
				Assert.Equal("Healed TestAnimal #0's Inflammation with AntiInflammatory.\n", console.CurrentOutput);
			}
		}

		#endregion

	}
}
