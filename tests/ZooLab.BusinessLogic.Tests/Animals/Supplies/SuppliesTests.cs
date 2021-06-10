using Xunit;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Animals.Supplies.MedicalSupplies;

namespace ZooLab.BusinessLogic.Tests.Animals.Supplies
{
	public class SuppliesTests
	{
		[Fact]
		public void ShouldCreateAntiInflammatory()
		{
			AntiInflammatory antiInflammatory = new();

			Assert.Equal(Animal.SicknessType.Inflammation, antiInflammatory.TreatsSickness);
		}

		[Fact]
		public void ShouldCreateAntiDepression()
		{
			AntiDepression antiDepression = new();

			Assert.Equal(Animal.SicknessType.Depression, antiDepression.TreatsSickness);
		}

		[Fact]
		public void ShouldCreateAntibiotics()
		{
			Antibiotics antibiotics = new();

			Assert.Equal(Animal.SicknessType.Infection, antibiotics.TreatsSickness);
		}
	}
}
