namespace ZooLab.BusinessLogic.Animals.Mammals
{
	public class Bison : Mammal
	{
		private readonly string[] _favoriteFood = new[] { "Grass" };
		public override int RequiredSpaceSqFt => 1000;
		public override string[] FavoriteFood => _favoriteFood;
		public override bool IsFriendlyWith(Animal animal)
		{
			return animal is Bison or
				Elephant;
		}
	}
}
