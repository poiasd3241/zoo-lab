using System;
using System.Collections.Generic;
using System.Linq;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Animals.Supplies.FoodSupplies;
using ZooLab.BusinessLogic.Exceptions;
using ZooLab.BusinessLogic.ExtensionMethods;
using ZooLab.BusinessLogic.Logging;

namespace ZooLab.BusinessLogic.Employees
{
	public class ZooKeeper : IEmployee, IAnimalExperience
	{

		#region Private Members

		private readonly IConsole _console;
		private string Name => GetType().Name;
		private static readonly Random _rnd = new();

		#endregion

		#region Public Properties

		public string FirstName { get; private set; }
		public string LastName { get; private set; }
		public List<string> AnimalExperiences { get; private set; } = new();

		#endregion

		#region Constructor

		public ZooKeeper(string firstName, string lastName, IConsole console = null)
		{
			FirstName = firstName;
			LastName = lastName;
			_console = console;
		}

		#endregion

		#region Public Methods

		public void AddAnimalExperience(Animal animal)
		{
			if (animal is null)
			{
				_console?.WriteLine("Cannot add an animal experience: the animal is not provided.");
				throw new ArgumentNullException(nameof(animal));
			}

			var animalName = animal.GetTypeName();

			if (HasAnimalExperiece(animal))
			{
				_console?.WriteLine($"{Name} {FirstName} {LastName} is already experienced with {animalName}.");
				return;
			}
			else
			{
				AnimalExperiences.Add(animalName);
				_console?.WriteLine($"{Name} {FirstName} {LastName} is now experienced with {animalName}.");
			}
		}

		public bool HasAnimalExperiece(Animal animal) =>
			AnimalExperiences.Contains(animal.GetTypeName());

		public bool FeedAnimal(Animal animal, DateTime now)
		{
			if (CanFeed(animal, now))
			{
				try
				{
					var food = GetRandomFromFavoriteFood(animal.FavoriteFood);
					animal.Feed(food, this, now);
					_console?.WriteLine($"{Name} {FirstName} {LastName} fed {animal.GetTypeName()} #{animal.ID} with {food.GetTypeName()}.");
					return true;
				}
				catch (Exception ex)
				{
					_console?.WriteLine(ex.Message);
					throw;
				}
			}

			return false;
		}

		#endregion

		#region Private Methods

		private bool CanFeed(Animal animal, DateTime now)
		{
			// No animal.
			if (animal is null)
			{
				_console?.WriteLine("Cannot feed an animal: the animal is not provided.");
				throw new ArgumentNullException(nameof(animal));
			}

			// No experience.
			if (HasAnimalExperiece(animal) == false)
			{
				_console?.WriteLine($"Cannot feed {animal.GetTypeName()}: no experience.");
				return false;
			}

			// Bad feed schedule / daily feeding limit reached.
			if (CanFeedByFeedSchedule(animal, now) == false ||
				CanFeedByDailyLimit(animal, now) == false)
			{
				return false;
			}

			// No favorite food.
			var favoriteFood = animal.FavoriteFood;
			if (favoriteFood.IsNullOrEmpty())
			{
				_console?.WriteLine($"Cannot feed {animal.GetTypeName()} #{animal.ID}: favorite food list is undefined or empty.");
				throw new ArgumentInvalidPropertyException(nameof(animal), nameof(animal.FavoriteFood), "cannot be null or empty");
			}

			return true;
		}

		private bool CanFeedByFeedSchedule(Animal animal, DateTime now)
		{
			// No feed schedule.
			var feedSchedule = animal.FeedSchedule;
			if (feedSchedule.IsNullOrEmpty())
			{
				_console?.WriteLine($"Cannot feed {animal.GetTypeName()} #{animal.ID}: feed schedule is undefined or empty.");
				throw new ArgumentInvalidPropertyException(nameof(animal), nameof(animal.FeedSchedule), "cannot be null or empty");
			}

			// Feed schedule violation.
			var hour = now.Hour;
			if (animal.FeedSchedule.Contains(hour) == false)
			{
				var nextHour = hour == 23 ? 0 : hour + 1;
				_console?.WriteLine($"Cannot feed {animal.GetTypeName()} #{animal.ID} between {hour}:00 and {nextHour}:00.");
				return false;
			}

			return true;
		}

		private bool CanFeedByDailyLimit(Animal animal, DateTime now)
		{
			if (animal.MaxDailyFeedings == 0)
			{
				throw new ArgumentInvalidPropertyException(nameof(animal), nameof(animal.MaxDailyFeedings), "cannot be zero");
			}

			var yesterday = now.AddDays(-1);
			var feedTimesLast24h = animal.FeedTimes.Count
				(feedTime => feedTime.Time >= yesterday && feedTime.Time < now);

			if (feedTimesLast24h >= animal.MaxDailyFeedings)
			{
				_console?.WriteLine($"Cannot feed {animal.GetTypeName()} more than {animal.MaxDailyFeedings} time(s) in 24 hours.");
				return false;
			}

			return true;
		}

		private static Food GetRandomFromFavoriteFood(string[] favoriteFood)
		{
			var favFoodName = favoriteFood[_rnd.Next(favoriteFood.Length)];
			return favFoodName switch
			{
				nameof(Grass) => new Grass(),
				nameof(Vegetable) => new Vegetable(),
				nameof(Meat) => new Meat(),
				_ => throw new NotImplementedException($"Unknown food name: '{favFoodName}'."),
			};
		}

		#endregion
	}
}
