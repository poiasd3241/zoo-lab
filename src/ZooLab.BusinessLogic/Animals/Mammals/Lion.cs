namespace ZooLab.BusinessLogic.Animals.Mammals
{
	public class Lion : Mammal
	{
		private readonly string[] _favoriteFood = new[] { "Meat" };
		public override int RequiredSpaceSqFt => 1000;
		public override string[] FavoriteFood => _favoriteFood;
		public override bool IsFriendlyWith(Animal animal)
		{
			return animal is Lion;
		}
	}
}
