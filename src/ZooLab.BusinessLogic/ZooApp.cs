using System;
using System.Collections.Generic;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Animals.Birds;
using ZooLab.BusinessLogic.Animals.Mammals;
using ZooLab.BusinessLogic.Animals.Reptiles;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.Logging;
using ZooLab.BusinessLogic.Zoos;

namespace ZooLab
{
	public class ZooApp
	{
		#region Private Members

		private readonly IConsole _console;
		private static readonly Random _rnd = new();

		private readonly List<string> _firstNames = new()
		{
			"Wyatt",
			"Payton",
			"Maisie",
			"Montel",
			"Jordyn",
			"Kylan",
			"Jemma",
			"Enya",
			"Benito",
			"Najma"
		};
		private readonly List<string> _lastNames = new()
		{
			"Kinney",
			"Huff",
			"Reader",
			"Mohammed",
			"Cote",
			"Hayden",
			"Holcomb",
			"Wicks",
			"Hodgson",
			"Krause"
		};

		private readonly List<string> _allAnimalNames = new()
		{
			"Parrot",
			"Penguin",
			"Bison",
			"Elephant",
			"Lion",
			"Snake",
			"Turtle"
		};

		#endregion

		#region Public Properties

		public List<Zoo> Zoos { get; private set; } = new();

		#endregion

		#region Constructor

		public ZooApp(IConsole console = null)
		{
			_console = console;

			var now = DateTime.Now;
			Initialize(now);
		}

		#endregion

		#region Public Methods

		public void AddZoo(Zoo zoo)
		{
			if (zoo is null)
			{
				_console?.WriteLine("Cannot add a zoo: the zoo is not provided.");
				throw new ArgumentNullException(nameof(zoo));
			}
			if (Zoos.Exists(z => z.Location == zoo.Location))
			{
				_console?.WriteLine("Cannot add a zoo: the location is taken.");
				throw new ArgumentException("Already taken", nameof(zoo));
			}

			Zoos.Add(zoo);
		}

		#endregion

		#region Private Methods

		private void Initialize(DateTime now)
		{
			// Create 2 Zoos
			var zoo1 = GetInitializedZoo("loc1", now);
			var zoo2 = GetInitializedZoo("loc2", now);

			Zoos.Add(zoo1);
			Zoos.Add(zoo2);

			// Feed and heal all animals
			foreach (var zoo in Zoos)
			{
				zoo.FeedAnimals(now);
				zoo.HealAnimals();
			}
		}

		private Zoo GetInitializedZoo(string location, DateTime now)
		{
			var zoo = new Zoo(location, _console);
			InitializeZooWithEnclosures(zoo);
			InitializeZooWithAllAnimalTypes(zoo, now);
			InitializeZooWithZooKeepersAndVeterinarians(zoo);
			return zoo;
		}

		private static void InitializeZooWithEnclosures(Zoo zoo)
		{
			zoo.AddEnclosure("enc1", 9000);
			zoo.AddEnclosure("enc2", 5000);
			zoo.AddEnclosure("enc3", 3000);
			zoo.AddEnclosure("enc4", 1000);
			zoo.AddEnclosure("enc5", 500);
		}

		private void InitializeZooWithAllAnimalTypes(Zoo zoo, DateTime now)
		{
			var feedSchedule = new List<int>() { now.Hour, now.AddHours(12).Hour };
			var animals3ofEachType = GetAnimalsOfEveryType(feedSchedule, 3);
			foreach (var animal in animals3ofEachType)
			{
				var suitableEnclosure = zoo.FindAvailableEnclosure(animal);
				suitableEnclosure.AddAnimal(animal);
			}
		}

		private void InitializeZooWithZooKeepersAndVeterinarians(Zoo zoo)
		{
			AddZooKeeperToZoo(zoo);
			AddZooKeeperToZoo(zoo);
			AddVeterinatianToZoo(zoo);
			AddVeterinatianToZoo(zoo);
		}

		private List<Animal> GetAnimalsOfEveryType(List<int> feedSchedule, int amountEachType)
		{
			var result = new List<Animal>();

			foreach (var animalName in _allAnimalNames)
			{
				for (int i = 0; i < amountEachType; i++)
				{
					var animal = GetAnimalByName(animalName);
					animal.AddFeedSchedule(feedSchedule);
					result.Add(animal);
				}
			}
			return result;
		}

		private static Animal GetAnimalByName(string animalName) => animalName switch
		{
			"Parrot" => new Parrot(),
			"Penguin" => new Penguin(),
			"Bison" => new Bison(),
			"Elephant" => new Elephant(),
			"Lion" => new Lion(),
			"Snake" => new Snake(),
			"Turtle" => new Turtle()
		};

		private string GetRandomFirstName() => _firstNames[_rnd.Next(_firstNames.Count)];
		private string GetRandomLastName() => _lastNames[_rnd.Next(_lastNames.Count)];
		private void AddZooKeeperToZoo(Zoo zoo)
		{
			var zooKeeper = new ZooKeeper(GetRandomFirstName(), GetRandomLastName(), _console);

			foreach (var animalName in zoo.AnimalNames)
			{
				zooKeeper.AddAnimalExperience(GetAnimalByName(animalName));
			}
			zoo.HireEmployee(zooKeeper);
		}

		private void AddVeterinatianToZoo(Zoo zoo)
		{
			var veterinarian = new Veterinarian(GetRandomFirstName(), GetRandomLastName(), _console);

			foreach (var animalName in zoo.AnimalNames)
			{
				veterinarian.AddAnimalExperience(GetAnimalByName(animalName));
			}
			zoo.HireEmployee(veterinarian);
		}

		#endregion
	}
}
