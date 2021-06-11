using System;
using Xunit;
using ZooLab.BusinessLogic.Animals.Birds;
using ZooLab.BusinessLogic.Animals.Mammals;
using ZooLab.BusinessLogic.Exceptions;
using ZooLab.BusinessLogic.Tests.Animals;
using ZooLab.BusinessLogic.Tests.Logging;
using ZooLab.BusinessLogic.Zoos;
using static ZooLab.BusinessLogic.Tests.Logging.TestConsole;

namespace ZooLab.BusinessLogic.Tests.Zoos
{
	public class EnclosureTests
	{
		[Fact]
		public void ShouldCreateEnclosure()
		{
			var zoo = new Zoo("x");
			var enclosure = new Enclosure("enc", zoo, 999);

			Assert.Equal("enc", enclosure.Name);
			Assert.Empty(enclosure.Animals);
			Assert.Equal(zoo, enclosure.ParentZoo);
			Assert.Equal(999, enclosure.SquareFeet);
			Assert.Equal(999, enclosure.FreeSpaceSqFt);
		}

		#region Can Add Animal

		[Fact]
		public void ShouldFailCanAddAnimalByAnimalNull()
		{
			var zoo = new Zoo("x");
			var enclosure = new Enclosure("enc", zoo, 999);

			Assert.False(enclosure.CanAddAnimal(null));
		}

		[Fact]
		public void ShouldFailCanAddAnimalByAnimalNotFriendly()
		{
			var zoo = new Zoo("x");
			var enclosure = new Enclosure("enc", zoo, 999999);
			enclosure.AddAnimal(new Lion());

			Assert.False(enclosure.CanAddAnimal(new Parrot()));
		}

		[Fact]
		public void ShouldFailCanAddAnimalByAnimalRequiredSpace()
		{
			var zoo = new Zoo("x");
			var enclosure = new Enclosure("enc", zoo, 1);

			Assert.False(enclosure.CanAddAnimal(new Lion()));
		}

		[Fact]
		public void ShouldCanAddAnimal()
		{
			var zoo = new Zoo("x");
			var enclosure = new Enclosure("enc", zoo, 1);

			Assert.False(enclosure.CanAddAnimal(new Lion()));
		}

		#endregion

		#region Add Animal

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailAddAnimalByAnimalNull(TestConsole console)
		{
			var zoo = new Zoo("x");
			var enclosure = new Enclosure("enc", zoo, 999, console);

			var exception = Assert.Throws<ArgumentNullException>(() => enclosure.AddAnimal(null));
			Assert.Equal("animal", exception.ParamName);
			Assert.Empty(enclosure.Animals);
			if (console is not null)
			{
				Assert.Equal("Value cannot be null. (Parameter 'animal')\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailAddAnimalByAnimalNotFriendly(TestConsole console)
		{
			var zoo = new Zoo("x");
			var enclosure = new Enclosure("enc", zoo, 99999, console);
			enclosure.AddAnimal(new Lion());
			console?.Clear();

			var exception = Assert.Throws<NotFriendlyAnimalException>(() => enclosure.AddAnimal(new Parrot()));
			Assert.Equal("Cannot place Parrot together with Lion.", exception.Message);
			Assert.Single(enclosure.Animals);
			if (console is not null)
			{
				Assert.Equal("Cannot place Parrot together with Lion.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailAddAnimalByAnimalRequiredSpace(TestConsole console)
		{
			var zoo = new Zoo("x");
			var enclosure = new Enclosure("enc", zoo, 5, console);
			console?.Clear();

			var exception = Assert.Throws<NoAvailableSpaceException>(() => enclosure.AddAnimal(new Lion()));
			Assert.Equal("Cannot add Lion: required space - 1000 sq. ft, available - 5 sq. ft.",
				exception.Message);
			Assert.Empty(enclosure.Animals);
			if (console is not null)
			{
				Assert.Equal("Cannot add Lion: required space - 1000 sq. ft, available - 5 sq. ft.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailAddAnimalByAnimalAlreadyAssignedID(TestConsole console)
		{
			var zoo = new Zoo("x");
			var enclosure = new Enclosure("enc", zoo, 5, console);
			var testAnimal = new TestAnimal();
			testAnimal.AssignID(2);
			console?.Clear();

			var exception = Assert.Throws<InvalidOperationException>(() => enclosure.AddAnimal(testAnimal));
			Assert.Equal("The animal already has an ID assigned to it.", exception.Message);
			Assert.Empty(enclosure.Animals);
			if (console is not null)
			{
				Assert.Equal("The animal already has an ID assigned to it.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldAddAnimal(TestConsole console)
		{
			var zoo = new Zoo("x");
			var enclosure = new Enclosure("enc", zoo, 9999, console);
			var animal = new TestAnimal();
			console?.Clear();

			enclosure.AddAnimal(animal);

			var addedAnimal = Assert.Single(enclosure.Animals);
			Assert.Equal(animal, addedAnimal);
			if (console is not null)
			{
				Assert.Equal("Added TestAnimal #0.\n", console.CurrentOutput);
			}
		}

		#endregion

		#region Remove Animal

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldFailRemoveAnimalByAnimalNotPresent(TestConsole console)
		{
			var zoo = new Zoo("x");
			var enclosure = new Enclosure("enc", zoo, 9999, console);
			var animal = new TestAnimal();
			enclosure.AddAnimal(animal);
			console?.Clear();

			enclosure.RemoveAnimal(123);

			var addedAnimal = Assert.Single(enclosure.Animals);
			Assert.Equal(animal, addedAnimal);
			if (console is not null)
			{
				Assert.Equal("Cannot remove an animal #123: doesn't exist.\n", console.CurrentOutput);
			}
		}

		[Theory]
		[ClassData(typeof(TestConsoleOrNull))]
		public void ShouldRemoveAnimal(TestConsole console)
		{
			var zoo = new Zoo("x");
			var enclosure = new Enclosure("enc", zoo, 9999, console);
			var animal = new TestAnimal();
			enclosure.AddAnimal(animal);
			console?.Clear();

			enclosure.RemoveAnimal(0);

			Assert.Empty(enclosure.Animals);
			if (console is not null)
			{
				Assert.Equal("Removed the animal #0.\n", console.CurrentOutput);
			}
		}

		#endregion
	}
}
