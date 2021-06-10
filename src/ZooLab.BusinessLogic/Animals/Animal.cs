using System;
using System.Collections.Generic;
using System.Linq;
using ZooLab.BusinessLogic.Animals.Supplies.FoodSupplies;
using ZooLab.BusinessLogic.Animals.Supplies.MedicalSupplies;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.Exceptions;
using ZooLab.BusinessLogic.ExtensionMethods;
using ZooLab.BusinessLogic.Logging;

namespace ZooLab.BusinessLogic.Animals
{
	public abstract class Animal
	{
		/// <summary>
		/// The type of sickness an <see cref="Animal"/> can have.<br/>
		/// <see cref="None"/> is the default value and should be used to indicate a healthy state.
		/// </summary>
		public enum SicknessType
		{
			None = 0,
			Infection = 1,
			Depression = 2,
			Inflammation = 3
		}

		#region Private Members

		/// <summary>
		/// The current type name.
		/// </summary>
		private string Name => GetType().Name;
		private readonly IConsole _console;
		private static readonly Random _rnd = new();

		#endregion

		#region Public Properties

		public abstract int RequiredSpaceSqFt { get; }
		public abstract string[] FavoriteFood { get; }
		public abstract int MaxDailyFeedings { get; }
		public List<FeedTime> FeedTimes { get; protected set; } = new();
		public List<int> FeedSchedule { get; protected set; } = new();
		public bool IsSick => Sickness != SicknessType.None;
		public int ID { get; protected set; }
		public bool IsAssignedID { get; protected set; } = false;
		public SicknessType Sickness { get; protected set; }

		#endregion

		#region Constructor

		public Animal(IConsole console = null)
		{
			_console = console;

			// Randomize sickness.
			Sickness = _rnd.Next(2) == 1
				? (SicknessType)_rnd.Next(1, Enum.GetValues(typeof(SicknessType)).Length)
				: SicknessType.None;
		}

		#endregion

		#region Public Methods

		public bool AssignID(int id)
		{
			if (IsAssignedID)
			{
				return false;
			}

			ID = id;
			IsAssignedID = true;
			return true;
		}

		public abstract bool IsFriendlyWith(Animal animal);

		public void Feed(Food food, ZooKeeper zooKeeper, DateTime now)
		{
			if (food is null)
			{
				_console?.WriteLine("Cannot feed without food.");
				throw new ArgumentNullException(nameof(food));
			}
			if (zooKeeper is null)
			{
				_console?.WriteLine("Feeding requires a zoo keeper.");
				throw new ArgumentNullException(nameof(zooKeeper));
			}

			var foodName = food.GetType().Name;
			if (FavoriteFood.Contains(foodName))
			{
				FeedTimes.Add(new FeedTime(now, zooKeeper));
				_console?.WriteLine($"Fed {Name} #{ID} with {foodName}.");
			}
			else
			{
				_console?.WriteLine($"Cannot feed {Name} with {foodName}.");
			}
		}

		public void AddFeedSchedule(List<int> hours)
		{
			if (hours.IsNullOrEmpty())
			{
				_console?.WriteLine("Cannot add an unspecified feed schedule.");
				throw new ArgumentNullOrEmptyException(nameof(hours));
			}

			if (hours.Any(h => h is < 0 or > 23))
			{
				_console?.WriteLine("Please specify the feeding schedule values as the hour from 0 to 23.");
				return;
			}

			hours.ForEach(h =>
			{
				var scheduleEndHour = h == 23 ? 0 : h + 1;

				if (FeedSchedule.Contains(h))
				{
					_console?.WriteLine($"{Name} #{ID} already has the feeding schedule {h}:00 - {scheduleEndHour}:00 defined.");
				}
				else
				{
					FeedSchedule.Add(h);
					_console?.WriteLine($"Added feeding schedule for the {Name} #{ID}: " +
						$"{h}:00 - {scheduleEndHour}:00.");
				}
			});
		}

		public void Heal(Medicine medicine)
		{
			// Not sick.
			if (IsSick == false)
			{
				_console?.WriteLine($"The {Name} #{ID} is not sick and therefore doesn't need healing.");
				return;
			}

			// No medicine.
			if (medicine is null)
			{
				_console?.WriteLine("Cannot heal without medicine.");
				throw new ArgumentNullException(nameof(medicine));
			}

			// Wrong medicine.
			var sickness = Sickness;
			if (medicine.TreatsSickness != Sickness)
			{
				_console?.WriteLine($"Cannot heal {Sickness} with {medicine.GetType().Name}.");
				return;
			}

			Sickness = SicknessType.None;
			_console?.WriteLine($"Healed {Name} #{ID}'s {sickness} with {medicine.GetType().Name}.");
		}

		#endregion
	}
}
