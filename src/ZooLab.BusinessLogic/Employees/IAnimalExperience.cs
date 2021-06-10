using System.Collections.Generic;
using ZooLab.BusinessLogic.Animals;

namespace ZooLab.BusinessLogic.Employees
{
	/// <summary>
	/// Represents experience with animals (<see cref="Animal"/>) functionality.
	/// </summary>
	public interface IAnimalExperience
	{
		/// <summary>
		/// The list of <see cref="Animal"/> names.<br/>
		/// Represents the animals that the object has the experience with.
		/// </summary>
		List<string> AnimalExperiences { get; }
		void AddAnimalExperience(Animal animal);
		bool HasAnimalExperiece(Animal animal);
	}
}