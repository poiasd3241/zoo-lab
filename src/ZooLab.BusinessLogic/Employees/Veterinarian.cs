using System;
using System.Collections.Generic;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Animals.Supplies.MedicalSupplies;
using ZooLab.BusinessLogic.ExtensionMethods;
using ZooLab.BusinessLogic.Logging;

namespace ZooLab.BusinessLogic.Employees
{
	public class Veterinarian : IEmployee, IAnimalExperience
	{

		#region Private Members

		private readonly IConsole _console;
		private string Name => GetType().Name;

		#endregion

		#region Public Properties

		public string FirstName { get; private set; }
		public string LastName { get; private set; }
		public List<string> AnimalExperiences { get; private set; } = new();

		#endregion

		#region Constructor

		public Veterinarian(string firstName, string lastName, IConsole console = null)
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

		public bool HealAnimal(Animal animal)
		{
			if (CanHeal(animal))
			{
				try
				{
					var medicine = GetMedicine(animal.Sickness);
					animal.Heal(medicine);
					_console?.WriteLine($"{Name} {FirstName} {LastName} healed {animal.GetTypeName()} #{animal.ID} with {medicine.GetTypeName()}.");
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

		private static Medicine GetMedicine(Animal.SicknessType sickness) => sickness switch
		{
			Animal.SicknessType.Infection => new Antibiotics(),
			Animal.SicknessType.Depression => new AntiDepression(),
			Animal.SicknessType.Inflammation => new AntiInflammatory(),
			_ => throw new NotImplementedException($"Unknown sickness: '{sickness}'.")
		};

		private bool CanHeal(Animal animal)
		{
			// No animal.
			if (animal is null)
			{
				_console?.WriteLine("Cannot heal an animal: the animal is not provided.");
				throw new ArgumentNullException(nameof(animal));
			}

			// Not sick.
			if (animal.IsSick == false)
			{
				_console?.WriteLine("The animal is healthy and doesn't need healing.");
				return false;
			}

			// No experience.
			if (HasAnimalExperiece(animal) == false)
			{
				_console?.WriteLine($"Cannot heal {animal.GetTypeName()}: no experience.");
				return false;
			}

			return true;
		}

		#endregion
	}
}
