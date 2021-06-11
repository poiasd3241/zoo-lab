using System;
using Xunit;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Animals.Birds;
using ZooLab.BusinessLogic.Animals.Mammals;
using ZooLab.BusinessLogic.Animals.Reptiles;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.Exceptions;
using ZooLab.BusinessLogic.Tests.Animals;
using ZooLab.BusinessLogic.Tests.Logging;
using ZooLab.BusinessLogic.Zoos;
using static ZooLab.BusinessLogic.Tests.Logging.TestConsole;

namespace ZooLab.BusinessLogic.Tests.Zoos
{
	public class ZooTests
	{
		[Fact]
		public void ShouldCreateZoo()
		{
			var zoo = new Zoo("x");

			Assert.Empty(zoo.AnimalNames);
			Assert.Empty(zoo.Enclosures);
			Assert.Empty(zoo.Employees);
			Assert.Equal("x", zoo.Location);
			Assert.Equal(0, zoo.AnimalIdGenerator.GetNextID());
			Assert.Equal(int.MaxValue, zoo.AnimalIdGenerator.MaxID);
		}

		#region Add Enclosure

		public class FailAddEnclosureByNameNullOrWhitespaceWithTestConsole : TheoryData<TestConsole, string>
		{
			public FailAddEnclosureByNameNullOrWhitespaceWithTestConsole()
			{
				Add(null, null);
				Add(null, "");
				Add(null, " ");
				Add(new(), null);
				Add(new(), "");
				Add(new(), " ");
			}
		}

		[Theory]
		[ClassData(typeof(FailAddEnclosureByNameNullOrWhitespaceWithTestConsole))]
		public void ShouldFailAddEnclosureByNameNullOrWhitespace(TestConsole console, string enclosureName)
		{
			var zoo = new Zoo("x", console);

			var exception = Assert.Throws<ArgumentException>(() => zoo.AddEnclosure(enclosureName, 123));

			Assert.Equal("name", exception.ParamName);
			Assert.Equal("Cannot be null or whitespace (Parameter 'name')", exception.Message);
			Assert.Empty(zoo.Enclosures);
			if (console is not null)
			{
				Assert.Equal("Cannot add an enclosure: the name cannot be a null or whitespace.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailAddEnclosureByNameTaken(TestConsole console)
		{
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("first", 123);
			console?.Clear();

			var exception = Assert.Throws<ArgumentException>(() => zoo.AddEnclosure("first", 222));

			Assert.Equal("name", exception.ParamName);
			Assert.Equal("Already taken (Parameter 'name')", exception.Message);
			var first = Assert.Single(zoo.Enclosures);
			Assert.Equal(123, first.SquareFeet);
			if (console is not null)
			{
				Assert.Equal("Cannot add an enclosure: the name is taken.\n", console.CurrentOutput);
			}
		}

		public class FailAddEnclosureBySquareFeetInvalidWithTestConsole : TheoryData<TestConsole, int>
		{
			public FailAddEnclosureBySquareFeetInvalidWithTestConsole()
			{
				Add(null, -1);
				Add(null, 0);
				Add(new(), -1);
				Add(new(), 0);
			}
		}

		[Theory]
		[ClassData(typeof(FailAddEnclosureBySquareFeetInvalidWithTestConsole))]
		public void ShouldFailAddEnclosureBySquareFeetInvalid(TestConsole console, int squareFeet)
		{
			var zoo = new Zoo("x", console);

			var exception = Assert.Throws<ArgumentException>(() => zoo.AddEnclosure("name", squareFeet));

			Assert.Equal("squareFeet", exception.ParamName);
			Assert.Equal("Must be greater than 0 (Parameter 'squareFeet')", exception.Message);
			Assert.Empty(zoo.Enclosures);
			if (console is not null)
			{
				Assert.Equal("Cannot add an enclosure: the area (sq. ft) must be greater than 0.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldAddEnclosure(TestConsole console)
		{
			var zoo = new Zoo("x", console);

			zoo.AddEnclosure("name", 123);

			var enclosure = Assert.Single(zoo.Enclosures);

			Assert.Equal("name", enclosure.Name);
			Assert.Equal(123, enclosure.SquareFeet);
			Assert.Equal(zoo, enclosure.ParentZoo);
			if (console is not null)
			{
				Assert.Equal("Added an enclosure 'name', 123 sq. ft.\n", console.CurrentOutput);
			}
		}

		#endregion

		#region Find Available Enclosure

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFindAvailableEnclosureByAnimalNull(TestConsole console)
		{
			var zoo = new Zoo("x", console);

			var exception = Assert.Throws<ArgumentNullException>(() => zoo.FindAvailableEnclosure(null));

			Assert.Equal("animal", exception.ParamName);
			if (console is not null)
			{
				Assert.Equal("Cannot find an enclosure: the animal is not provided.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFindAvailableEnclosureByEnclosuresEmpty(TestConsole console)
		{
			var zoo = new Zoo("x", console);

			Assert.Throws<NoAvailableEnclosureException>(() => zoo.FindAvailableEnclosure(new TestAnimal()));

			if (console is not null)
			{
				Assert.Equal("No available enclosures for the TestAnimal.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFindAvailableEnclosureByCannotAddAnimal(TestConsole console)
		{
			var zoo = new Zoo("x", console);

			// Animal requires more space than the enclosure can offer.
			zoo.AddEnclosure("enc", 200);
			var animal = new TestAnimal();
			animal.SetCustomRequiredSpaceSqFt(999);
			console?.Clear();

			Assert.Throws<NoAvailableEnclosureException>(() => zoo.FindAvailableEnclosure(animal));

			if (console is not null)
			{
				Assert.Equal("No available enclosures for the TestAnimal.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFindAvailableEnclosureSingleOption(TestConsole console)
		{
			var zoo = new Zoo("x", console);

			zoo.AddEnclosure("enc", 200);
			var animal = new TestAnimal();
			console?.Clear();

			var suitableEnclosure = zoo.FindAvailableEnclosure(animal);

			Assert.Equal(zoo.Enclosures[0], suitableEnclosure);
			if (console is not null)
			{
				Assert.Equal("Found a suitable enclosure 'enc' for TestAnimal. " +
					"This is the only suitable enclosure.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFindAvailableEnclosureMultipleOptions(TestConsole console)
		{
			var zoo = new Zoo("x", console);

			var animal1 = new TestAnimal();
			var animal2 = new TestAnimal();
			var animal3 = new TestAnimal();
			zoo.AddEnclosure("enc1", 100);
			zoo.AddEnclosure("enc2", 200);
			zoo.Enclosures[0].AddAnimal(animal1);
			zoo.Enclosures[1].AddAnimal(animal2);
			console?.Clear();

			var suitableEnclosure = zoo.FindAvailableEnclosure(animal3);

			// Picked the enclosure with more free space.
			Assert.Equal(zoo.Enclosures[1], suitableEnclosure);
			if (console is not null)
			{
				Assert.Equal("Found a suitable enclosure 'enc2' for TestAnimal. " +
					"Of all suitable enclosures, this one has the most free space at 199 sq. ft.\n",
					console.CurrentOutput);
			}
		}

		#endregion

		#region Hire Employee

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailHireEmployeeByEmployeeNull(TestConsole console)
		{
			var zoo = new Zoo("x", console);

			var exception = Assert.Throws<ArgumentNullException>(() => zoo.HireEmployee(null));

			Assert.Equal("employee", exception.ParamName);
			Assert.Empty(zoo.Employees);
			if (console is not null)
			{
				Assert.Equal("Cannot hire an employee: the employee is not provided.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailHireEmployeeByEmployeeLackOfAnimalExperience(TestConsole console)
		{
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			zoo.Enclosures[0].AddAnimal(new TestAnimal());

			var zooKeeper = new ZooKeeper("a", "b");
			zooKeeper.AddAnimalExperience(new Lion());
			console?.Clear();

			var exception = Assert.Throws<NoNeededExperienceException>(() => zoo.HireEmployee(zooKeeper));

			Assert.Equal("ZooKeeper a b doesn't have experience with the following animals: TestAnimal.", exception.Message);
			Assert.Empty(zoo.Employees);
			if (console is not null)
			{
				Assert.Equal("ZooKeeper a b doesn't have experience with the following animals: TestAnimal.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailHireEmployeeByEmployeeNonAnimalExperienceValidation(TestConsole console)
		{
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			zoo.Enclosures[0].AddAnimal(new TestAnimal());

			var zooKeeper = new ZooKeeper("a", "");
			zooKeeper.AddAnimalExperience(new TestAnimal());
			console?.Clear();

			zoo.HireEmployee(zooKeeper);

			Assert.Empty(zoo.Employees);
			if (console is not null)
			{
				Assert.Equal("The ZooKeeper a  cannot be hired:\n" + "LastName: Last name is required.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldHireEmployee(TestConsole console)
		{
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			zoo.Enclosures[0].AddAnimal(new TestAnimal());

			var zooKeeper = new ZooKeeper("a", "b");
			zooKeeper.AddAnimalExperience(new TestAnimal());
			console?.Clear();

			zoo.HireEmployee(zooKeeper);

			var hiredEmployee = Assert.Single(zoo.Employees);

			Assert.Equal("a", hiredEmployee.FirstName);
			Assert.Equal("b", hiredEmployee.LastName);
			Assert.True(hiredEmployee is ZooKeeper);
			if (console is not null)
			{
				Assert.Equal("Hired an employee: ZooKeeper a b.\n", console.CurrentOutput);
			}
		}

		#endregion

		#region Feed Animals

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFeedAnimalsByAnimalsEmpty(TestConsole console)
		{
			var zoo = new Zoo("x", console);
			zoo.FeedAnimals(DateTime.Now);

			if (console is not null)
			{
				Assert.Equal("No animals in the zoo.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFeedAnimalsByZooKeepersEmpty(TestConsole console)
		{
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			zoo.Enclosures[0].AddAnimal(new TestAnimal());
			var vet = new Veterinarian("a", "b");
			vet.AddAnimalExperience(new TestAnimal());
			zoo.HireEmployee(vet);
			console?.Clear();

			zoo.FeedAnimals(DateTime.Now);

			if (console is not null)
			{
				Assert.Equal("No ZooKeeper employees in the zoo.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailFeedAnimalsByZooKeepersNoExperience(TestConsole console)
		{
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			zoo.Enclosures[0].AddAnimal(new Parrot());
			var zooKeeperParrots = new ZooKeeper("a", "b");
			zooKeeperParrots.AddAnimalExperience(new Parrot());
			zoo.HireEmployee(zooKeeperParrots);

			zoo.Enclosures[0].AddAnimal(new Turtle());
			zoo.Enclosures[0].RemoveAnimal(0);

			console?.Clear();

			zoo.FeedAnimals(DateTime.Now);

			if (console is not null)
			{
				Assert.Equal("No employees are experienced with the animals that need the action to be performed on.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFeedAnimals(TestConsole console)
		{
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			var now = DateTime.Now;
			var parrot1 = new Parrot();
			var parrot2 = new Parrot();
			parrot1.AddFeedSchedule(new() { now.Hour });
			parrot2.AddFeedSchedule(new() { now.Hour });
			zoo.Enclosures[0].AddAnimal(parrot1);
			zoo.Enclosures[0].AddAnimal(parrot2);

			var zooKeeperParrots1 = new ZooKeeper("Parrot", "Pro");
			var zooKeeperParrots2 = new ZooKeeper("Parrot", "Guru");
			zooKeeperParrots1.AddAnimalExperience(new Parrot());
			zooKeeperParrots2.AddAnimalExperience(new Parrot());
			zoo.HireEmployee(zooKeeperParrots1);
			zoo.HireEmployee(zooKeeperParrots2);

			console?.Clear();

			zoo.FeedAnimals(now);

			if (console is not null)
			{
				Assert.Equal(
				"Zoo keeper Parrot Pro fed 1 animal(s) #(0).\n" +
				"Zoo keeper Parrot Guru fed 1 animal(s) #(1).\n" +
				"", console.CurrentOutput);
			}
		}

		#endregion

		#region Heal Animals

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailHealAnimalsByAnimalsEmpty(TestConsole console)
		{
			var zoo = new Zoo("x", console);
			zoo.HealAnimals();

			if (console is not null)
			{
				Assert.Equal("No animals in the zoo.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailHealAnimalsByAnimalsHealthy(TestConsole console)
		{
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			var healthyAnimal = new TestAnimal(Animal.SicknessType.None);
			zoo.Enclosures[0].AddAnimal(healthyAnimal);
			console?.Clear();

			zoo.HealAnimals();

			if (console is not null)
			{
				Assert.Equal("No animals need healing.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailHealAnimalsByVeterinariansEmpty(TestConsole console)
		{
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			var sickAnimal = new TestAnimal(Animal.SicknessType.Infection);
			zoo.Enclosures[0].AddAnimal(sickAnimal);
			console?.Clear();

			zoo.HealAnimals();

			if (console is not null)
			{
				Assert.Equal("No Veterinarian employees in the zoo.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailHealAnimalsByVeterinariansNoExperience(TestConsole console)
		{
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);

			zoo.Enclosures[0].AddAnimal(new Parrot());
			var vetParrots = new Veterinarian("a", "b");
			vetParrots.AddAnimalExperience(new Parrot());
			zoo.HireEmployee(vetParrots);

			Turtle sickTurtle;
			do
			{
				sickTurtle = new Turtle();
			} while (sickTurtle.IsSick == false);
			Assert.True(sickTurtle.IsSick);
			zoo.Enclosures[0].AddAnimal(sickTurtle);
			zoo.Enclosures[0].RemoveAnimal(0);

			console?.Clear();

			zoo.HealAnimals();

			if (console is not null)
			{
				Assert.Equal("No employees are experienced with the animals that need the action to be performed on.\n",
				console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldHealAnimals(TestConsole console)
		{
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			var now = DateTime.Now;
			var sickAnimal1 = new TestAnimal(Animal.SicknessType.Infection);
			var sickAnimal2 = new TestAnimal(Animal.SicknessType.Depression);
			sickAnimal1.AddFeedSchedule(new() { now.Hour });
			sickAnimal2.AddFeedSchedule(new() { now.Hour });
			zoo.Enclosures[0].AddAnimal(sickAnimal1);
			zoo.Enclosures[0].AddAnimal(sickAnimal2);

			var vetTestAnimals = new Veterinarian("a", "b");
			vetTestAnimals.AddAnimalExperience(new TestAnimal());
			zoo.HireEmployee(vetTestAnimals);

			console?.Clear();

			zoo.HealAnimals();

			if (console is not null)
			{
				Assert.Equal("Veterinarian a b healed 2 animal(s) #(0, 1).\n", console.CurrentOutput);
			}
		}

		#endregion
	}
}
