using System;
using System.Collections.Generic;
using Xunit;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Animals.Mammals;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.Tests.Animals;
using ZooLab.BusinessLogic.Tests.Logging;

namespace ZooLab.BusinessLogic.Tests.Employees
{
	public class VeterinarianTests
	{
		#region Basic

		[Fact]
		public void ShouldCreateVeterinarian()
		{
			Veterinarian veterinarian = new("a", "b");

			Assert.Equal("a", veterinarian.FirstName);
			Assert.Equal("b", veterinarian.LastName);
			Assert.Empty(veterinarian.AnimalExperiences);
			Assert.False(veterinarian.HasAnimalExperiece(new Lion()));
		}

		#endregion

		#region Add Animal Experience

		[Fact]
		public void ShouldFailAddAnimalExperienceByAnimalNull()
		{
			var console = new TestConsole();
			Veterinarian veterinarian = new("a", "b", console);

			var exception = Assert.Throws<ArgumentNullException>(() => veterinarian.AddAnimalExperience(null));

			Assert.Equal("animal", exception.ParamName);
			Assert.Empty(veterinarian.AnimalExperiences);
			Assert.Equal("Cannot add an animal experience: the animal is not provided.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailAddAnimalExperienceByAlreadyExperiencedWithAnimal()
		{
			var console = new TestConsole();
			Veterinarian veterinarian = new("a", "b", console);
			var lion1 = new Lion();
			var lion2 = new Lion();

			veterinarian.AddAnimalExperience(lion1);
			console.Clear();
			veterinarian.AddAnimalExperience(lion2);

			Assert.Equal(new List<string>() { "Lion" }, veterinarian.AnimalExperiences);
			Assert.Equal("Veterinarian a b is already experienced with Lion.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldAddAnimalExperience()
		{
			var console = new TestConsole();
			Veterinarian veterinarian = new("a", "b", console);
			var lion = new Lion();

			veterinarian.AddAnimalExperience(lion);

			Assert.Equal(new List<string>() { "Lion" }, veterinarian.AnimalExperiences);
			Assert.True(veterinarian.HasAnimalExperiece(new Lion()));
			Assert.Equal("Veterinarian a b is now experienced with Lion.\n", console.CurrentOutput);
		}

		#endregion

		#region Heal Animal

		[Fact]
		public void ShouldFailHealAnimalByAnimalNull()
		{
			var console = new TestConsole();
			Veterinarian veterinarian = new("a", "b", console);

			var exception = Assert.Throws<ArgumentNullException>(() => veterinarian.HealAnimal(null));

			Assert.Equal("animal", exception.ParamName);
			Assert.Empty(veterinarian.AnimalExperiences);
			Assert.Equal("Cannot heal an animal: the animal is not provided.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailHealAnimalByAnimalHealthy()
		{
			var console = new TestConsole();
			var animal = new TestAnimal(Animal.SicknessType.None, console);
			Veterinarian veterinarian = new("a", "b", console);

			var result = veterinarian.HealAnimal(animal);

			Assert.False(result);
			Assert.Equal("The animal is healthy and doesn't need healing.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailHealAnimalByNotExperiencedWithAnimal()
		{
			var console = new TestConsole();
			var animal = new TestAnimal(Animal.SicknessType.Inflammation, console);
			Veterinarian veterinarian = new("a", "b", console);

			var result = veterinarian.HealAnimal(animal);

			Assert.False(result);
			Assert.Equal("Cannot heal TestAnimal: no experience.\n", console.CurrentOutput);
		}

		[Fact]
		public void ShouldFailHealAnimalByAnimalSicknessUnknown()
		{
			var console = new TestConsole();
			var animal = new TestAnimal();
			var unknownSickness = (Animal.SicknessType)123;
			animal.SetCustomSickness(unknownSickness);
			Veterinarian veterinarian = new("a", "b", console);
			veterinarian.AddAnimalExperience(animal);
			var expectedMessage = "Unknown sickness: '123'.";

			console.Clear();
			var exception = Assert.Throws<NotImplementedException>(() => veterinarian.HealAnimal(animal));

			Assert.Equal(expectedMessage, exception.Message);
			Assert.Equal($"{expectedMessage}\n", console.CurrentOutput);
		}

		public class HealAnimalWithDifferentSicknesses : TheoryData<Animal.SicknessType, string>
		{
			public HealAnimalWithDifferentSicknesses()
			{
				Add(Animal.SicknessType.Infection, "Antibiotics");
				Add(Animal.SicknessType.Inflammation, "AntiInflammatory");
				Add(Animal.SicknessType.Depression, "AntiDepression");
			}
		}

		[Theory]
		[ClassData(typeof(HealAnimalWithDifferentSicknesses))]
		public void ShouldHealAnimal(Animal.SicknessType sickness, string medicineName)
		{
			var console = new TestConsole();
			var animal = new TestAnimal();
			animal.SetCustomSickness(sickness);
			Veterinarian veterinarian = new("a", "b", console);
			veterinarian.AddAnimalExperience(animal);

			console.Clear();
			var result = veterinarian.HealAnimal(animal);

			Assert.True(result);
			Assert.Equal($"Veterinarian a b healed TestAnimal #0 with {medicineName}.\n",
				console.CurrentOutput);
		}

		#endregion
	}
}
