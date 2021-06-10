namespace ZooLab.BusinessLogic.Animals.Supplies.MedicalSupplies
{
	public class AntiDepression : Medicine
	{
		public override Animal.SicknessType TreatsSickness => Animal.SicknessType.Depression;
	}
}
