using Xunit;
using ZooLab.BusinessLogic.Animals.Mammals;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.HireValidators;
using ZooLab.BusinessLogic.Logging;
using ZooLab.BusinessLogic.Tests.Animals;
using ZooLab.BusinessLogic.Zoos;

namespace ZooLab.BusinessLogic.Tests.HireValidators
{
	public class VeterinarianHireValidatorTests
	{
		[Fact]
		public void ShouldValidateEmployee()
		{
			var hiringZoo = GetZooWithTestAnimals("x");

			var hireValidator = new VeterinarianHireValidator();
			var veterinarian = new Veterinarian("a", "b");
			veterinarian.AddAnimalExperience(new TestAnimal());

			var result = hireValidator.ValidateEmployee(veterinarian, hiringZoo);

			Assert.True(result.IsValid);
		}

		[Fact]
		public void ShouldInvalidateEmployeeLastName()
		{
			var hiringZoo = GetZooWithTestAnimals("x");

			var hireValidator = new VeterinarianHireValidator();
			var veterinarian = new Veterinarian("a", "");
			veterinarian.AddAnimalExperience(new TestAnimal());

			var result = hireValidator.ValidateEmployee(veterinarian, hiringZoo);

			Assert.False(result.IsValid);
			var error = Assert.Single(result.Errors);

			Assert.Equal("LastName", error.PropertyName);
			Assert.Equal("Last name is required.", error.ErrorMessage);
		}


		[Fact]
		public void ShouldInvalidateEmployeeExperienceEmpty()
		{
			var hiringZoo = GetZooWithTestAnimals("x");

			var hireValidator = new VeterinarianHireValidator();
			var veterinarian = new Veterinarian("a", "b");

			var result = hireValidator.ValidateEmployee(veterinarian, hiringZoo);

			Assert.False(result.IsValid);
			var error = Assert.Single(result.Errors);

			Assert.Equal("AnimalExperiences", error.PropertyName);
			Assert.Equal("Must have experience with all animals present in the hiring zoo.", error.ErrorMessage);
		}

		[Fact]
		public void ShouldInvalidateEmployeeExperienceDifferentAnimal()
		{
			var hiringZoo = GetZooWithTestAnimals("x");

			var hireValidator = new VeterinarianHireValidator();
			var veterinarian = new Veterinarian("a", "b");
			veterinarian.AddAnimalExperience(new Lion());

			var result = hireValidator.ValidateEmployee(veterinarian, hiringZoo);

			Assert.False(result.IsValid);
			var error = Assert.Single(result.Errors);

			Assert.Equal("AnimalExperiences", error.PropertyName);
			Assert.Equal("Must have experience with all animals present in the hiring zoo.", error.ErrorMessage);
		}

		private static Zoo GetZooWithTestAnimals(string location, IConsole console = null)
		{
			var zoo = new Zoo(location, console);
			zoo.AddEnclosure("1", 99999);
			zoo.Enclosures[0].AddAnimal(new TestAnimal());
			zoo.Enclosures[0].AddAnimal(new TestAnimal());

			return zoo;
		}
	}
}
