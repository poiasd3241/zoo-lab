using ZooLab.BusinessLogic.Animals.Birds;
using ZooLab.BusinessLogic.Animals.Reptiles;

namespace ZooLab.BusinessLogic.Animals.Mammals
{
	public class Elephant : Mammal
	{
		private readonly string[] _favoriteFood = new[] { "Vegetable" };
		public override int RequiredSpaceSqFt => 1000;
		public override string[] FavoriteFood => _favoriteFood;
		public override bool IsFriendlyWith(Animal animal)
		{
			return animal is Elephant or
				Bison or
				Parrot or
				Turtle;
		}
	}
}
