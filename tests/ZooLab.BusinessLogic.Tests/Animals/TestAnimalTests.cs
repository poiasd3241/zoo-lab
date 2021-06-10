using System;
using System.Collections.Generic;
using Xunit;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Animals.Birds;
using ZooLab.BusinessLogic.Employees;

namespace ZooLab.BusinessLogic.Tests.Animals
{
	public class TestAnimalTests
	{
		#region Animal

		[Fact]
		public void ShouldCreateTestAnimal()
		{
			TestAnimal animal = new();

			Assert.Equal(new[] { "Pizza", "Meat" }, animal.FavoriteFood);
			Assert.Equal(1, animal.RequiredSpaceSqFt);
			Assert.True(animal.IsFriendlyWith(new TestAnimal()));
			Assert.False(animal.IsFriendlyWith(new Penguin()));
			Assert.Equal(1, animal.MaxDailyFeedings);

			Assert.Empty(animal.FeedTimes);
			Assert.Empty(animal.FeedSchedule);
			Assert.False(animal.IsAssignedID);
			Assert.Equal(0, animal.ID);
		}

		[Fact]
		public void ShouldCreateTestAnimalWithDefinedSickness()
		{
			TestAnimal animal = new(Animal.SicknessType.Depression);

			Assert.True(animal.Sickness == Animal.SicknessType.Depression);
		}

		[Fact]
		public void ShouldCreateRandomlySickTestAnimal()
		{
			TestAnimal animal;
			do
			{
				animal = new();
			} while (animal.IsSick == false);

			var possibleSicknesses = new List<Animal.SicknessType>() {
				Animal.SicknessType.Inflammation,
				Animal.SicknessType.Depression,
				Animal.SicknessType.Infection
			};

			Assert.True(animal.IsSick);
			Assert.Contains(animal.Sickness, possibleSicknesses);
		}

		[Fact]
		public void ShouldCreateRandomlyHealthyTestAnimal()
		{
			TestAnimal animal;
			do
			{
				animal = new();
			} while (animal.IsSick);

			Assert.False(animal.IsSick);
			Assert.Equal(Animal.SicknessType.None, animal.Sickness);
		}

		[Fact]
		public void ShouldAssignID()
		{
			TestAnimal animal = new();

			var result = animal.AssignID(123);

			Assert.True(result);
			Assert.True(animal.IsAssignedID);
			Assert.Equal(123, animal.ID);
		}

		[Fact]
		public void ShouldNotAssignIdMoreThanOnce()
		{
			TestAnimal animal = new();

			animal.AssignID(123);
			var result = animal.AssignID(456);

			Assert.False(result);
			Assert.True(animal.IsAssignedID);
			Assert.Equal(123, animal.ID);
		}

		#endregion

		#region Test Animal

		[Fact]
		public void ShouldSetCustomSickness()
		{
			var animal = new TestAnimal(Animal.SicknessType.None);

			animal.SetCustomSickness(Animal.SicknessType.Infection);

			Assert.True(animal.Sickness == Animal.SicknessType.Infection);
		}

		[Fact]
		public void ShouldSetCustomFeedTimes()
		{
			var animal = new TestAnimal();
			var feedTimes = new List<FeedTime>() { new FeedTime(DateTime.Now, new ZooKeeper("a", "b")) };

			animal.SetCustomFeedTimes(feedTimes);

			Assert.Equal(feedTimes, animal.FeedTimes);
		}

		[Fact]
		public void ShouldSetCustomFavoriteFood()
		{
			var animal = new TestAnimal();
			var favoriteFood = new string[] { "Apple" };

			animal.SetCustomFavoriteFood(favoriteFood);

			Assert.Equal(favoriteFood, animal.FavoriteFood);
		}

		[Fact]
		public void ShouldSetCustomRequiredSpaceSqFt()
		{
			var animal = new TestAnimal();
			var requiredSpaceSqFt = 123;

			animal.SetCustomRequiredSpaceSqFt(requiredSpaceSqFt);

			Assert.Equal(requiredSpaceSqFt, animal.RequiredSpaceSqFt);
		}

		[Fact]
		public void ShouldSetCustomMaxDailyFeedings()
		{
			var animal = new TestAnimal();
			var maxDailyFeedings = 123;

			animal.SetCustomMaxDailyFeedings(maxDailyFeedings);

			Assert.Equal(maxDailyFeedings, animal.MaxDailyFeedings);
		}

		#endregion
	}
}
