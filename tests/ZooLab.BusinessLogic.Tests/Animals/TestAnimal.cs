using System.Collections.Generic;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.ExtensionMethods;
using ZooLab.BusinessLogic.Logging;

namespace ZooLab.BusinessLogic.Tests.Animals
{
	public class TestAnimal : Animal
	{
		private int _requiredSpaceSqFt = 1;
		public override int RequiredSpaceSqFt => _requiredSpaceSqFt;

		private string[] _favoriteFood = new[] { "Pizza", "Meat" };
		public override string[] FavoriteFood => _favoriteFood;

		private int _maxDailyFeedings = 1;
		public override int MaxDailyFeedings => _maxDailyFeedings;

		private readonly List<string> _friendlyAnimals = new() { "TestAnimal" };
		public override bool IsFriendlyWith(Animal animal) =>
			_friendlyAnimals.Contains(animal.GetTypeName());

		public TestAnimal(IConsole console = null) : base(console)
		{
		}
		public TestAnimal(SicknessType sickness, IConsole console = null) : base(console)
		{
			SetCustomSickness(sickness);
		}

		public void SetCustomSickness(SicknessType sickness)
		{
			Sickness = sickness;
		}
		public void SetCustomFeedTimes(List<FeedTime> feedTimes)
		{
			FeedTimes = feedTimes;
		}
		public void SetCustomFavoriteFood(string[] favoriteFood)
		{
			_favoriteFood = favoriteFood;
		}
		public void SetCustomRequiredSpaceSqFt(int requiredSpaceSqFt)
		{
			_requiredSpaceSqFt = requiredSpaceSqFt;
		}
		public void SetCustomMaxDailyFeedings(int maxDailyFeedings)
		{
			_maxDailyFeedings = maxDailyFeedings;
		}
	}
}
