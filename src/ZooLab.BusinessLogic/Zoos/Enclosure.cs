using System;
using System.Collections.Generic;
using System.Linq;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Exceptions;
using ZooLab.BusinessLogic.ExtensionMethods;
using ZooLab.BusinessLogic.Logging;

namespace ZooLab.BusinessLogic.Zoos
{
	public class Enclosure
	{
		#region Private Members

		private readonly IConsole _console;

		#endregion

		#region Public Properties
		public string Name { get; private set; }
		public List<Animal> Animals { get; private set; } = new();
		public Zoo ParentZoo { get; private set; }
		public int SquareFeet { get; private set; }
		public int FreeSpaceSqFt { get; private set; }

		#endregion

		#region Public Events

		public event Action<string> AnimalAdded;

		#endregion


		#region Constructor

		public Enclosure(string name, Zoo parentZoo, int squareFeet, IConsole console = null)
		{
			Name = name;
			ParentZoo = parentZoo;
			SquareFeet = squareFeet;
			_console = console;
			FreeSpaceSqFt = SquareFeet;
		}

		#endregion

		#region Public Methods

		public void AddAnimal(Animal animal)
		{
			try
			{
				if (ValidateCanAddAnimal(animal))
				{
					var id = ParentZoo.AnimalIdGenerator.GetNextID();
					if (animal.AssignID(id))
					{
						Animals.Add(animal);
						OnAnimalAdded(animal);
						_console?.WriteLine($"Added {animal.GetTypeName()} #{animal.ID}.");
					}
					else
					{
						throw new InvalidOperationException("The animal already has an ID assigned to it.");
					}
				}

			}
			catch (Exception ex)
			{
				_console?.WriteLine(ex.Message);
				throw;
			}
		}

		public bool CanAddAnimal(Animal animal) =>
			animal is not null &&
			Animals.Any(existingAnimal => existingAnimal.IsFriendlyWith(animal) == false) == false &&
			animal.RequiredSpaceSqFt <= FreeSpaceSqFt;


		public void RemoveAnimal(int animalID)
		{
			var animal = Animals.Find(a => a.ID == animalID);
			if (animal == null)
			{
				_console?.WriteLine($"Cannot remove an animal #{animalID}: doesn't exist.");
			}
			else
			{
				Animals.Remove(animal);
				_console?.WriteLine($"Removed the animal #{animalID}.");
			}
		}

		#endregion

		#region Private Methods

		private bool ValidateCanAddAnimal(Animal animal)
		{
			// No animal.
			if (animal is null)
			{
				throw new ArgumentNullException(nameof(animal));
			}

			// Not friendly animal.
			Animals.ForEach(existingAnimal =>
			{
				if (existingAnimal.IsFriendlyWith(animal) == false)
				{
					throw new NotFriendlyAnimalException($"Cannot place {animal.GetTypeName()} together with {existingAnimal.GetTypeName()}.");
				}
			});

			// No available space
			if (animal.RequiredSpaceSqFt > FreeSpaceSqFt)
			{
				throw new NoAvailableSpaceException($"Cannot add {animal.GetTypeName()}: " +
					$"required space - {animal.RequiredSpaceSqFt} sq. ft, available - {FreeSpaceSqFt} sq. ft.");
			}

			return true;
		}

		private void OnAnimalAdded(Animal animal)
		{
			FreeSpaceSqFt -= animal.RequiredSpaceSqFt;
			AnimalAdded?.Invoke(animal.GetTypeName());
		}

		#endregion
	}
}
