namespace ZooLab.BusinessLogic.Animals.Supplies.MedicalSupplies
{
	public class AntiInflammatory : Medicine
	{
		public override Animal.SicknessType TreatsSickness => Animal.SicknessType.Inflammation;
	}
}
