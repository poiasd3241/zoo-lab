using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Animals.Supplies.FoodSupplies;
using ZooLab.BusinessLogic.Animals.Supplies.MedicalSupplies;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.Exceptions;
using ZooLab.BusinessLogic.Tests.Logging;

namespace ZooLab.BusinessLogic.Tests.Animals
{
	public partial class AnimalTests : BeforeAfterTestAttribute
	{
		#region Feed

		[Fact]
		public void ShouldFailFeedByNullFood()
		{
			var console = new TestConsole();
			TestAnimal animal = new(console);

			var exception = Assert.Throws<ArgumentNullException>(() => animal.Feed(null, new ZooKeeper("a", "b"), DateTime.Now));

			Assert.Empty(animal.FeedTimes);
			Assert.Equal("food", exception.ParamName);
			Assert.Equal("Cannot feed without food.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailFeedByNullZooKeeper()
		{
			var console = new TestConsole();
			TestAnimal animal = new(console);

			var exception = Assert.Throws<ArgumentNullException>(() => animal.Feed(new Grass(), null, DateTime.Now));

			Assert.Empty(animal.FeedTimes);
			Assert.Equal("zooKeeper", exception.ParamName);
			Assert.Equal("Feeding requires a zoo keeper.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailFeedByNotFavoriteFood()
		{
			var console = new TestConsole();
			TestAnimal animal = new(console);

			Assert.Empty(animal.FeedTimes);
			animal.Feed(new Grass(), new ZooKeeper("a", "b"), DateTime.Now);
			Assert.Equal("Cannot feed TestAnimal with Grass.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFeed()
		{
			var console = new TestConsole();
			TestAnimal animal = new(console);
			var now = DateTime.Now;
			var zooKeeper = new ZooKeeper("a", "b");
			var expectedFeedTimes = new List<FeedTime>() { new FeedTime(now, zooKeeper) };

			animal.Feed(new Meat(), zooKeeper, now);

			var actualFeedTimes = animal.FeedTimes;


			var actualFeedTime = Assert.Single(actualFeedTimes);
			Assert.True(DateTime.Equals(expectedFeedTimes[0].Time, actualFeedTime.Time));
			Assert.Equal(expectedFeedTimes[0].FedByZooKeeper, actualFeedTime.FedByZooKeeper);
			Assert.Equal("Fed TestAnimal #0 with Meat.\n", console.CurrentOutput);
		}

		#endregion

		#region Add Feed Schedule

		public class FailAddFeedSchedule
		{
			public class ByHoursNullOrEmpty : TheoryData<List<int>>
			{
				public ByHoursNullOrEmpty()
				{
					Add(null);
					Add(new());
				}
			}
			public class ByHoursOutOfRange : TheoryData<List<int>>
			{
				public ByHoursOutOfRange()
				{
					Add(new() { 25 });
					Add(new() { -1 });
				}
			}
		}

		[Theory]
		[ClassData(typeof(FailAddFeedSchedule.ByHoursNullOrEmpty))]
		public void ShouldFailAddFeedScheduleByHoursNullOrEmpty(List<int> hours)
		{
			var console = new TestConsole();
			TestAnimal animal = new(console);

			var exception = Assert.Throws<ArgumentNullOrEmptyException>(() => animal.AddFeedSchedule(hours));

			Assert.Empty(animal.FeedSchedule);
			Assert.Equal("hours", exception.ParamName);
			Assert.Equal("Cannot add an unspecified feed schedule.\n", console.CurrentOutput);
		}

		[Theory]
		[ClassData(typeof(FailAddFeedSchedule.ByHoursOutOfRange))]
		public void ShouldFailAddFeedScheduleByHoursOutOfRange(List<int> hours)
		{
			var console = new TestConsole();
			TestAnimal animal = new(console);

			animal.AddFeedSchedule(hours);

			Assert.Empty(animal.FeedSchedule);
			Assert.Equal("Please specify the feeding schedule values as the hour from 0 to 23.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldNotAddFeedScheduleByHoursAlreadyDefined()
		{
			var console = new TestConsole();
			TestAnimal animal = new(console);
			var feedSchedule = new List<int>() { 2 };

			animal.AddFeedSchedule(feedSchedule);
			console.Clear();
			animal.AddFeedSchedule(feedSchedule);

			Assert.Equal(feedSchedule, animal.FeedSchedule);
			Assert.Equal("TestAnimal #0 already has the feeding schedule 2:00 - 3:00 defined.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldAddFeedSchedule()
		{
			var console = new TestConsole();
			TestAnimal animal = new(console);
			var feedSchedule = new List<int>() { 2 };

			animal.AddFeedSchedule(feedSchedule);

			Assert.Equal(feedSchedule, animal.FeedSchedule);
			Assert.Equal("Added feeding schedule for the TestAnimal #0: 2:00 - 3:00.\n", console.CurrentOutput);
		}

		#endregion

		#region Heal

		[Fact]
		public void ShouldFailHealByHealthy()
		{
			var console = new TestConsole();
			var animal = new TestAnimal(Animal.SicknessType.None, console);
			var medicine = new AntiInflammatory();

			animal.Heal(medicine);

			Assert.Equal("The TestAnimal #0 is not sick and therefore doesn't need healing.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailHealByMedicineNull()
		{
			var console = new TestConsole();
			var animal = new TestAnimal(Animal.SicknessType.Infection, console);

			var exception = Assert.Throws<ArgumentNullException>(() => animal.Heal(null));

			Assert.Equal("medicine", exception.ParamName);
			Assert.Equal("Cannot heal without medicine.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailHealByMedicineWrong()
		{
			var console = new TestConsole();
			var animal = new TestAnimal(Animal.SicknessType.Inflammation, console);
			var medicine = new AntiDepression();

			animal.Heal(medicine);

			Assert.Equal("Cannot heal Inflammation with AntiDepression.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldHeal()
		{
			var console = new TestConsole();
			var animal = new TestAnimal(Animal.SicknessType.Inflammation, console);
			var medicine = new AntiInflammatory();

			animal.Heal(medicine);

			Assert.Equal(Animal.SicknessType.None, animal.Sickness);
			Assert.Equal("Healed TestAnimal #0's Inflammation with AntiInflammatory.\n", console.CurrentOutput);
		}

		#endregion

	}
}
