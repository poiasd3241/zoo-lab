using ZooLab.BusinessLogic.Animals.Birds;
using ZooLab.BusinessLogic.Animals.Mammals;

namespace ZooLab.BusinessLogic.Animals.Reptiles
{
	public class Turtle : Reptile
	{
		private readonly string[] _favoriteFood = new[] { "Vegetable" };
		public override int RequiredSpaceSqFt => 5;
		public override string[] FavoriteFood => _favoriteFood;
		public override bool IsFriendlyWith(Animal animal)
		{
			return animal is Turtle or
				Bison or
				Elephant or
				Parrot;
		}
	}
}
