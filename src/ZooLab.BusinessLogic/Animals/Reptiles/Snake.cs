namespace ZooLab.BusinessLogic.Animals.Reptiles
{
	public class Snake : Reptile
	{
		private readonly string[] _favoriteFood = new[] { "Meat" };
		public override int RequiredSpaceSqFt => 2;
		public override string[] FavoriteFood => _favoriteFood;
		public override bool IsFriendlyWith(Animal animal)
		{
			return animal is Snake;
		}
	}
}
