using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.Zoos;

namespace ZooLab.BusinessLogic.HireValidators
{
	public class ZooKeeperHireValidator : HireValidator<ZooKeeper>, IHireValidator
	{
		private List<string> _requiredAnimalExperience;

		public ZooKeeperHireValidator()
		{
			RuleFor(zooKeeper => zooKeeper.AnimalExperiences)
				.Must(animalExperiences => animalExperiences.Count > 0 && animalExperiences.TrueForAll(
					animalExperience => _requiredAnimalExperience.Contains(animalExperience)))
				.WithMessage("Must have experience with all animals present in the hiring zoo.");
			RuleFor(zooKeeper => zooKeeper.LastName)
				.NotEmpty().WithMessage("Last name is required.");
		}

		public override ValidationResult ValidateEmployee(IEmployee employee, Zoo hiringZoo)
		{
			var zooKeeper = (ZooKeeper)employee;
			_requiredAnimalExperience = hiringZoo.AnimalNames;
			return Validate(zooKeeper);
		}
	}
}
