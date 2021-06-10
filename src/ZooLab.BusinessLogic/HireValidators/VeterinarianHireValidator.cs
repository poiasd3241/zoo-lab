using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.Zoos;

namespace ZooLab.BusinessLogic.HireValidators
{
	public class VeterinarianHireValidator : HireValidator<Veterinarian>, IHireValidator
	{
		private List<string> _requiredAnimalExperience;

		public VeterinarianHireValidator()
		{
			RuleFor(veterinarian => veterinarian.AnimalExperiences)
				.Must(animalExperiences => animalExperiences.Count > 0 && animalExperiences.TrueForAll(
					animalExperience => _requiredAnimalExperience.Contains(animalExperience)))
				.WithMessage("Must have experience with all animals present in the hiring zoo.");
			RuleFor(zooKeeper => zooKeeper.LastName)
				.NotEmpty().WithMessage("Last name is required.");
		}

		public override ValidationResult ValidateEmployee(IEmployee employee, Zoo hiringZoo)
		{
			var veterinarian = (Veterinarian)employee;
			_requiredAnimalExperience = hiringZoo.AnimalNames;
			return Validate(veterinarian);
		}
	}
}