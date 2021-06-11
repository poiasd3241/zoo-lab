using System;
using System.Collections.Generic;
using Xunit;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Animals.Birds;
using ZooLab.BusinessLogic.Animals.Mammals;
using ZooLab.BusinessLogic.Animals.Reptiles;
using ZooLab.BusinessLogic.Tests.Logging;
using ZooLab.BusinessLogic.Zoos;
using static ZooLab.BusinessLogic.Tests.Logging.TestConsole;

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

		#region Get Animal By Name

		[Fact]
		public void ShouldFailGetAnimalByNameUnknown()
		{
			var exception = Assert.Throws<NotImplementedException>(() => ZooApp.GetAnimalByName("z"));

			Assert.Equal("Unknown animal name: 'z'.", exception.Message);
		}

		public class GetAnimalByName : TheoryData<string, Type>
		{
			public GetAnimalByName()
			{
				Add("Parrot", typeof(Parrot));
				Add("Penguin", typeof(Penguin));
				Add("Bison", typeof(Bison));
				Add("Elephant", typeof(Elephant));
				Add("Lion", typeof(Lion));
				Add("Snake", typeof(Snake));
				Add("Turtle", typeof(Turtle));
			}
		}

		[Theory]
		[ClassData(typeof(GetAnimalByName))]
		public void ShouldGetAnimalByName(string animalName, Type animalType)
		{
			var actualAnimal = ZooApp.GetAnimalByName(animalName);

			Assert.Equal(animalType, actualAnimal.GetType());
		}

		#endregion

		#region Add Zoo

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailAddZooByZooNull(TestConsole console)
		{
			var zooApp = new ZooApp(console);
			console?.Clear();

			var exception = Assert.Throws<ArgumentNullException>(() => zooApp.AddZoo(null));

			Assert.Equal("zoo", exception.ParamName);
			Assert.Equal(2, zooApp.Zoos.Count);

			if (console is not null)
			{
				Assert.Equal("Cannot add a zoo: the zoo is not provided.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailAddZooByZooLocationTaken(TestConsole console)
		{
			var zooApp = new ZooApp(console);
			var zoo1 = new Zoo("x");
			var zoo2 = new Zoo("x");
			zooApp.AddZoo(zoo1);
			console?.Clear();

			var exception = Assert.Throws<ArgumentException>(() => zooApp.AddZoo(zoo2));

			Assert.Equal("zoo", exception.ParamName);
			Assert.Equal("Already taken (Parameter 'zoo')", exception.Message);
			Assert.Equal(3, zooApp.Zoos.Count);

			if (console is not null)
			{
				Assert.Equal("Cannot add a zoo: the location is taken.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldAddZoo(TestConsole console)
		{
			var zooApp = new ZooApp(console);
			var zoo1 = new Zoo("x");
			console?.Clear();

			zooApp.AddZoo(zoo1);

			Assert.Equal(3, zooApp.Zoos.Count);

			if (console is not null)
			{
				Assert.Equal(zoo1, zooApp.Zoos[2]);
			}
		}

		#endregion
	}
}
