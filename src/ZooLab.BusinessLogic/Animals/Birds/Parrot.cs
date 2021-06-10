using ZooLab.BusinessLogic.Animals.Mammals;
using ZooLab.BusinessLogic.Animals.Reptiles;

namespace ZooLab.BusinessLogic.Animals.Birds
{
	public class Parrot : Bird
	{
		private readonly string[] _favoriteFood = new[] { "Vegetable" };
		public override int RequiredSpaceSqFt => 5;
		public override string[] FavoriteFood => _favoriteFood;
		public override bool IsFriendlyWith(Animal animal)
		{
			return animal is Parrot or
				Bison or
				Elephant or
				Turtle;
		}
	}
}
