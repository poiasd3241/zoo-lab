using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.Exceptions;
using ZooLab.BusinessLogic.ExtensionMethods;
using ZooLab.BusinessLogic.HireValidators;
using ZooLab.BusinessLogic.Logging;

namespace ZooLab.BusinessLogic.Zoos
{
	public class Zoo
	{
		#region Private Members

		private readonly IConsole _console;
		public List<string> AnimalNames { get; private set; } = new();

		#endregion

		#region Public Properties

		public List<Enclosure> Enclosures { get; private set; } = new();
		public List<IEmployee> Employees { get; private set; } = new();
		public string Location { get; }
		public IntIdGenerator AnimalIdGenerator { get; } = new(int.MaxValue);

		#endregion

		#region Constructor

		public Zoo(string location, IConsole console = null)
		{
			Location = location;
			_console = console;
		}

		#endregion

		#region Public Methods

		public Enclosure AddEnclosure(string name, int squareFeet)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				_console?.WriteLine("Cannot add an enclosure: the name cannot be a null or whitespace.");
				throw new ArgumentException("Cannot be null or whitespace", nameof(name));
			}

			if (Enclosures.Exists(e => e.Name == name))
			{
				_console?.WriteLine("Cannot add an enclosure: the name is taken.");
				throw new ArgumentException("Already taken", nameof(name));
			}

			if (squareFeet <= 0)
			{
				_console?.WriteLine("Cannot add an enclosure: the area (sq. ft) must be greater than 0.");
				throw new ArgumentException("Must be greater than 0", nameof(squareFeet));
			}

			var newEnclosure = new Enclosure(name, this, squareFeet, _console);
			newEnclosure.AnimalAdded += Enclosure_AnimalAdded;
			Enclosures.Add(newEnclosure);
			_console?.WriteLine($"Added an enclosure '{name}', {squareFeet} sq. ft.");
			return newEnclosure;
		}

		public Enclosure FindAvailableEnclosure(Animal animal)
		{
			// No animal.
			if (animal is null)
			{
				_console?.WriteLine("Cannot find an enclosure: the animal is not provided.");
				throw new ArgumentNullException(nameof(animal));
			}

			var suitableEnclosures = Enclosures?.Where(enclosure => enclosure.CanAddAnimal(animal));
			if (suitableEnclosures.IsNullOrEmpty())
			{
				_console?.WriteLine($"No available enclosures for the {animal.GetTypeName()}.");
				throw new NoAvailableEnclosureException();
			}

			var chosenEnclosure = suitableEnclosures.OrderByDescending(enclosure => enclosure.FreeSpaceSqFt).First();
			_console?.WriteLine($"Found a suitable enclosure '{chosenEnclosure.Name}' for {animal.GetTypeName()}. " +
				@$"{(suitableEnclosures.Count() == 1
				? "This is the only suitable enclosure."
				: $"Of all suitable enclosures, this one has the most free space at {chosenEnclosure.FreeSpaceSqFt} sq. ft.")}");
			return chosenEnclosure;
		}

		public void HireEmployee(IEmployee employee)
		{
			// No employee.
			if (employee is null)
			{
				_console?.WriteLine("Cannot hire an employee: the employee is not provided.");
				throw new ArgumentNullException(nameof(employee));
			}

			try
			{
				var hireValidator = HireValidatorProvider.GetHireValidator(employee);
				var result = hireValidator.ValidateEmployee(employee, this);

				if (result.IsValid)
				{
					Employees.Add(employee);
					_console?.WriteLine($"Hired an employee: {employee.GetTypeName()} {employee.FirstName} {employee.LastName}.");
				}
				else
				{
					// Check for the lack of animal experience.
					if (employee is IAnimalExperience animalExpEmployee)
					{
						if (result.Errors.Exists(error => error.PropertyName == nameof(animalExpEmployee.AnimalExperiences)))
						{
							var noExperience = AnimalNames.Where(animalName => animalExpEmployee.AnimalExperiences.Contains(animalName) == false);
							throw new NoNeededExperienceException($"{employee.GetTypeName()} {employee.FirstName} {employee.LastName} " +
								$"doesn't have experience with the following animals: {string.Join(", ", noExperience)}.");
						}
					}

					var sb = new StringBuilder();
					sb.Append($"The {employee.GetTypeName()} {employee.FirstName} {employee.LastName} cannot be hired:");
					foreach (var error in result.Errors)
					{
						sb.Append($"\n{error.PropertyName}: {error.ErrorMessage}");
					}
					_console?.WriteLine(sb.ToString());
				}
			}
			catch (Exception ex)
			{
				_console?.WriteLine(ex.Message);
				throw;
			}
		}

		public void FeedAnimals(DateTime now)
		{
			var resultFeedAnimalsByZooKeeper = GetAnimalsForEmployeeAction<ZooKeeper>();

			if (resultFeedAnimalsByZooKeeper.IsEmpty)
			{
				_console?.WriteLine(resultFeedAnimalsByZooKeeper.ReasonEmpty);
				return;
			}

			// Feed all animals
			foreach (var (zooKeeper, animals) in resultFeedAnimalsByZooKeeper.Data)
			{
				foreach (var animal in animals)
				{
					zooKeeper.FeedAnimal(animal, now);
				}
				_console?.WriteLine($"Zoo keeper {zooKeeper.FirstName} {zooKeeper.LastName} " +
					$"fed {animals.Count} animal(s) #({string.Join(", ", animals.Select(a => a.ID))}).");
			}
		}

		public void HealAnimals()
		{
			var resultFeedAnimalsByZooKeeper = GetAnimalsForEmployeeAction<Veterinarian>(
				(animal) => animal.IsSick, "No animals need healing.");

			if (resultFeedAnimalsByZooKeeper.IsEmpty)
			{
				_console?.WriteLine(resultFeedAnimalsByZooKeeper.ReasonEmpty);
				return;
			}

			// Heal all animals
			foreach (var (veterinarian, animals) in resultFeedAnimalsByZooKeeper.Data)
			{
				foreach (var animal in animals)
				{
					veterinarian.HealAnimal(animal);
				}
				_console?.WriteLine($"Veterinarian {veterinarian.FirstName} {veterinarian.LastName} " +
					$"healed {animals.Count} animal(s) #({string.Join(", ", animals.Select(a => a.ID))}).");
			}
		}

		#endregion

		#region Private Methods

		private void Enclosure_AnimalAdded(string animalName)
		{
			if (AnimalNames.Contains(animalName) == false)
			{
				AnimalNames.Add(animalName);
			}
		}

		private ResultItemListByObject<TEmployee, Animal> GetAnimalsForEmployeeAction<TEmployee>(
			Func<Animal, bool> animalFilter = null, string failedFilterMessage = "No animals that the action can be performed on.")
			where TEmployee : IAnimalExperience
		{
			var result = new ResultItemListByObject<TEmployee, Animal>();

			// Get all animals.
			var allAnimals = new List<Animal>();
			foreach (var enclosure in Enclosures)
			{
				allAnimals.AddRange(enclosure.Animals);
			}

			if (allAnimals.Count == 0)
			{
				result.ReasonEmpty = "No animals in the zoo.";
				return result;
			}

			var animalsForAction = animalFilter is null
					? allAnimals
					: allAnimals.Where(animalFilter).ToList();
			if (animalsForAction.Count == 0)
			{
				result.ReasonEmpty = failedFilterMessage;
				return result;
			}

			List<TEmployee> employees = Employees.Where(e => e is TEmployee).Select(e => (TEmployee)e).ToList();
			if (employees.Count == 0)
			{
				result.ReasonEmpty = $"No {typeof(TEmployee).Name} employees in the zoo.";
				return result;
			}

			AssignAnimalsByTypeToEmployeesAction(employees, allAnimals, result);
			return result;
		}

		private void AssignAnimalsByTypeToEmployeesAction<TEmployee>(
			List<TEmployee> employees, List<Animal> animals, ResultItemListByObject<TEmployee, Animal> result)
			where TEmployee : IAnimalExperience
		{
			var animalTypeThatHasExperiencedEmployeesCount = 0;
			foreach (var animalName in AnimalNames)
			{
				var animalsCurrentType = animals.Where(animal => animal.GetTypeName() == animalName).ToList();
				if (animalsCurrentType.Count == 0)
				{
					continue;
				}

				// Get employees experienced with this animal type
				var employeesWithExperience = new List<TEmployee>();
				foreach (var employee in employees)
				{
					if (employee.AnimalExperiences.Contains(animalName))
					{
						employeesWithExperience.Add(employee);
					}
				}

				if (employeesWithExperience.Count > 0)
				{
					animalTypeThatHasExperiencedEmployeesCount++;
					AssignAnimalsToEmployeesAction(employeesWithExperience, animalsCurrentType, result);
				}
			}

			if (animalTypeThatHasExperiencedEmployeesCount == 0)
			{
				result.ReasonEmpty = "No employees are experienced with the animals that need the action to be performed on.";
			}
		}

		private static void AssignAnimalsToEmployeesAction<TEmployee>(List<TEmployee> employees,
			List<Animal> animals, ResultItemListByObject<TEmployee, Animal> result)
			where TEmployee : IAnimalExperience
		{
			var employeeIndex = 0;
			var employeesCount = employees.Count;

			foreach (var animal in animals)
			{
				result.AddItemForObject(employees[employeeIndex], animal);

				if (employeeIndex == employeesCount - 1)
				{
					employeeIndex = 0;
				}
				else
				{
					employeeIndex++;
				}
			}
		}

		#endregion
	}
}
