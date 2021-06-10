namespace ZooLab.BusinessLogic.Animals.Birds
{
	public class Penguin : Bird
	{
		private readonly string[] _favoriteFood = new[] { "Meat" };
		public override int RequiredSpaceSqFt => 10;
		public override string[] FavoriteFood => _favoriteFood;
		public override bool IsFriendlyWith(Animal animal)
		{
			return animal is Penguin;
		}
	}
}
