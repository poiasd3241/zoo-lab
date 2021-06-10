using System;
using System.Collections.Generic;
using Xunit;
using ZooLab.BusinessLogic.Tests.Logging;
using ZooLab.BusinessLogic.Zoos;

namespace ZooLab.BusinessLogic.Tests.Zoos
{
	public class ZooAppTests
	{
		[Fact]
		public void ShouldCreateInitializedZooApp()
		{
			var zooApp = new ZooApp();
			Assert.Equal(2, zooApp.Zoos.Count);

			var allAnimalNames = new List<string>
			{
				"Parrot",
				"Penguin",
				"Bison",
				"Elephant",
				"Lion",
				"Snake",
				"Turtle"
			};

			foreach (var zoo in zooApp.Zoos)
			{
				Assert.Equal(5, zoo.Enclosures.Count);
				Assert.Equal(4, zoo.Employees.Count);
				Assert.Equal(21, zoo.AnimalIdGenerator.NextID);
				Assert.Equal(7, zoo.AnimalNames.Count);
				foreach (var animalName in allAnimalNames)
				{
					Assert.Contains(animalName, zoo.AnimalNames);
				}

				foreach (var enclosure in zoo.Enclosures)
				{
					foreach (var animal in enclosure.Animals)
					{
						Assert.Single(animal.FeedTimes);
						Assert.False(animal.IsSick);
					}
				}
			}
		}

		[Fact]
		public void ShouldFailAddZooByZooNull()
		{
			var console = new TestConsole();
			var zooApp = new ZooApp(console);
			console.Clear();

			var exception = Assert.Throws<ArgumentNullException>(() => zooApp.AddZoo(null));

			Assert.Equal("zoo", exception.ParamName);
			Assert.Equal("Cannot add a zoo: the zoo is not provided.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailAddZooByZooLocationTaken()
		{
			var console = new TestConsole();
			var zooApp = new ZooApp(console);
			var zoo1 = new Zoo("x");
			var zoo2 = new Zoo("x");
			zooApp.AddZoo(zoo1);
			console.Clear();

			var exception = Assert.Throws<ArgumentException>(() => zooApp.AddZoo(zoo2));

			Assert.Equal("zoo", exception.ParamName);
			Assert.Equal("Already taken (Parameter 'zoo')", exception.Message);
			Assert.Equal("Cannot add a zoo: the location is taken.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldAddZoo()
		{
			var console = new TestConsole();
			var zooApp = new ZooApp(console);
			var zoo1 = new Zoo("x");
			console.Clear();

			zooApp.AddZoo(zoo1);

			Assert.Equal(zoo1, zooApp.Zoos[2]);
		}
	}
}
