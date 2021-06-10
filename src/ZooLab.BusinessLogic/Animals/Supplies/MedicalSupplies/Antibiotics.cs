namespace ZooLab.BusinessLogic.Animals.Supplies.MedicalSupplies
{
	public class Antibiotics : Medicine
	{
		public override Animal.SicknessType TreatsSickness => Animal.SicknessType.Infection;
	}
}
