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

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public void ShouldFailAddEnclosureByNameNullOrWhitespace(string enclosureName)
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);

			var exception = Assert.Throws<ArgumentException>(() => zoo.AddEnclosure(enclosureName, 123));

			Assert.Equal("name", exception.ParamName);
			Assert.Equal("Cannot be null or whitespace (Parameter 'name')", exception.Message);
			Assert.Empty(zoo.Enclosures);
			Assert.Equal("Cannot add an enclosure: the name cannot be a null or whitespace.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailAddEnclosureByNameTaken()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("first", 123);
			console.Clear();

			var exception = Assert.Throws<ArgumentException>(() => zoo.AddEnclosure("first", 222));

			Assert.Equal("name", exception.ParamName);
			Assert.Equal("Already taken (Parameter 'name')", exception.Message);
			var first = Assert.Single(zoo.Enclosures);
			Assert.Equal(123, first.SquareFeet);
			Assert.Equal("Cannot add an enclosure: the name is taken.\n", console.CurrentOutput);
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		public void ShouldFailAddEnclosureBySquareFeetInvalid(int squareFeet)
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);

			var exception = Assert.Throws<ArgumentException>(() => zoo.AddEnclosure("name", squareFeet));

			Assert.Equal("squareFeet", exception.ParamName);
			Assert.Equal("Must be greater than 0 (Parameter 'squareFeet')", exception.Message);
			Assert.Empty(zoo.Enclosures);
			Assert.Equal("Cannot add an enclosure: the area (sq. ft) must be greater than 0.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldAddEnclosure()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);

			zoo.AddEnclosure("name", 123);

			var enclosure = Assert.Single(zoo.Enclosures);

			Assert.Equal("name", enclosure.Name);
			Assert.Equal(123, enclosure.SquareFeet);
			Assert.Equal(zoo, enclosure.ParentZoo);
			Assert.Equal("Added an enclosure 'name', 123 sq. ft.\n", console.CurrentOutput);
		}

		#endregion

		#region Find Available Enclosure

		[Fact]
		public void ShouldFailFindAvailableEnclosureByAnimalNull()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);

			var exception = Assert.Throws<ArgumentNullException>(() => zoo.FindAvailableEnclosure(null));

			Assert.Equal("animal", exception.ParamName);
			Assert.Equal("Cannot find an enclosure: the animal is not provided.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailFindAvailableEnclosureByEnclosuresEmpty()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);

			Assert.Throws<NoAvailableEnclosureException>(() => zoo.FindAvailableEnclosure(new TestAnimal()));

			Assert.Equal("No available enclosures for the TestAnimal.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailFindAvailableEnclosureByCannotAddAnimal()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);

			// Animal requires more space than the enclosure can offer.
			zoo.AddEnclosure("enc", 200);
			var animal = new TestAnimal();
			animal.SetCustomRequiredSpaceSqFt(999);
			console.Clear();

			Assert.Throws<NoAvailableEnclosureException>(() => zoo.FindAvailableEnclosure(animal));

			Assert.Equal("No available enclosures for the TestAnimal.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFindAvailableEnclosureSingleOption()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);

			zoo.AddEnclosure("enc", 200);
			var animal = new TestAnimal();
			console.Clear();

			var suitableEnclosure = zoo.FindAvailableEnclosure(animal);

			Assert.Equal(zoo.Enclosures[0], suitableEnclosure);
			Assert.Equal("Found a suitable enclosure 'enc' for TestAnimal. " +
				"This is the only suitable enclosure.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFindAvailableEnclosureMultipleOptions()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);

			var animal1 = new TestAnimal();
			var animal2 = new TestAnimal();
			var animal3 = new TestAnimal();
			zoo.AddEnclosure("enc1", 100);
			zoo.AddEnclosure("enc2", 200);
			zoo.Enclosures[0].AddAnimal(animal1);
			zoo.Enclosures[1].AddAnimal(animal2);
			console.Clear();

			var suitableEnclosure = zoo.FindAvailableEnclosure(animal3);

			// Picked the enclosure with more free space.
			Assert.Equal(zoo.Enclosures[1], suitableEnclosure);
			Assert.Equal("Found a suitable enclosure 'enc2' for TestAnimal. " +
				"Of all suitable enclosures, this one has the most free space at 199 sq. ft.\n", console.CurrentOutput);
		}

		#endregion

		#region Hire Employee

		[Fact]
		public void ShouldFailHireEmployeeByEmployeeNull()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);

			var exception = Assert.Throws<ArgumentNullException>(() => zoo.HireEmployee(null));

			Assert.Equal("employee", exception.ParamName);
			Assert.Empty(zoo.Employees);
			Assert.Equal("Cannot hire an employee: the employee is not provided.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailHireEmployeeByEmployeeLackOfAnimalExperience()
		{
			var zoo = new Zoo("x");
			zoo.AddEnclosure("enc", 999);
			zoo.Enclosures[0].AddAnimal(new TestAnimal());

			var zooKeeper = new ZooKeeper("a", "b");
			zooKeeper.AddAnimalExperience(new Lion());

			var exception = Assert.Throws<NoNeededExperienceException>(() => zoo.HireEmployee(zooKeeper));

			Assert.Equal("ZooKeeper a b doesn't have experience with the following animals: TestAnimal.", exception.Message);
			Assert.Empty(zoo.Employees);
		}

		[Fact]
		public void ShouldFailHireEmployeeByEmployeeNonAnimalExperienceValidation()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			zoo.Enclosures[0].AddAnimal(new TestAnimal());

			var zooKeeper = new ZooKeeper("a", "");
			zooKeeper.AddAnimalExperience(new TestAnimal());
			console.Clear();

			zoo.HireEmployee(zooKeeper);

			Assert.Empty(zoo.Employees);
			Assert.Equal("The ZooKeeper a  cannot be hired:\n" +
				"LastName: Last name is required.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldHireEmployee()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			zoo.Enclosures[0].AddAnimal(new TestAnimal());

			var zooKeeper = new ZooKeeper("a", "b");
			zooKeeper.AddAnimalExperience(new TestAnimal());
			console.Clear();

			zoo.HireEmployee(zooKeeper);

			var hiredEmployee = Assert.Single(zoo.Employees);

			Assert.Equal("a", hiredEmployee.FirstName);
			Assert.Equal("b", hiredEmployee.LastName);
			Assert.True(hiredEmployee is ZooKeeper);
			Assert.Equal("Hired an employee: ZooKeeper a b.\n", console.CurrentOutput);
		}

		#endregion

		#region Feed Animals

		[Fact]
		public void ShouldFailFeedAnimalsByAnimalsEmpty()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);
			zoo.FeedAnimals(DateTime.Now);

			Assert.Equal("No animals in the zoo.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailFeedAnimalsByZooKeepersEmpty()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			zoo.Enclosures[0].AddAnimal(new TestAnimal());
			var vet = new Veterinarian("a", "b");
			vet.AddAnimalExperience(new TestAnimal());
			zoo.HireEmployee(vet);
			console.Clear();

			zoo.FeedAnimals(DateTime.Now);

			Assert.Equal("No ZooKeeper employees in the zoo.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailFeedAnimalsByZooKeepersNoExperience()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			zoo.Enclosures[0].AddAnimal(new Parrot());
			var zooKeeperParrots = new ZooKeeper("a", "b");
			zooKeeperParrots.AddAnimalExperience(new Parrot());
			zoo.HireEmployee(zooKeeperParrots);

			zoo.Enclosures[0].AddAnimal(new Turtle());
			zoo.Enclosures[0].RemoveAnimal(0);

			console.Clear();

			zoo.FeedAnimals(DateTime.Now);

			Assert.Equal("No employees are experienced with the animals that need the action to be performed on.\n",
				console.CurrentOutput);
		}

		[Fact]
		public void ShouldFeedAnimals()
		{
			var console = new TestConsole();
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

			console.Clear();

			zoo.FeedAnimals(now);

			Assert.Equal(
				"Zoo keeper Parrot Pro fed 1 animal(s) #(0).\n" +
				"Zoo keeper Parrot Guru fed 1 animal(s) #(1).\n" +
				"", console.CurrentOutput);
		}

		#endregion

		#region Heal Animals

		[Fact]
		public void ShouldFailHealAnimalsByAnimalsEmpty()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);
			zoo.HealAnimals();

			Assert.Equal("No animals in the zoo.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailHealAnimalsByAnimalsHealthy()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			var healthyAnimal = new TestAnimal(Animal.SicknessType.None);
			zoo.Enclosures[0].AddAnimal(healthyAnimal);
			console.Clear();

			zoo.HealAnimals();

			Assert.Equal("No animals need healing.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailHealAnimalsByVeterinariansEmpty()
		{
			var console = new TestConsole();
			var zoo = new Zoo("x", console);
			zoo.AddEnclosure("enc", 999);
			var sickAnimal = new TestAnimal(Animal.SicknessType.Infection);
			zoo.Enclosures[0].AddAnimal(sickAnimal);
			console.Clear();

			zoo.HealAnimals();

			Assert.Equal("No Veterinarian employees in the zoo.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailHealAnimalsByVeterinariansNoExperience()
		{
			var console = new TestConsole();
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

			console.Clear();

			zoo.HealAnimals();

			Assert.Equal("No employees are experienced with the animals that need the action to be performed on.\n",
				console.CurrentOutput);
		}

		[Fact]
		public void ShouldHealAnimals()
		{
			var console = new TestConsole();
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

			console.Clear();

			zoo.HealAnimals();

			Assert.Equal("Veterinarian a b healed 2 animal(s) #(0, 1).\n", console.CurrentOutput);
		}

		#endregion
	}
}
