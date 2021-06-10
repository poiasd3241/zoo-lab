using FluentValidation;
using FluentValidation.Results;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.Zoos;

namespace ZooLab.BusinessLogic.HireValidators
{
	public abstract class HireValidator<TEmployee> : AbstractValidator<TEmployee>
	{
		public abstract ValidationResult ValidateEmployee(IEmployee employee, Zoo hiringZoo);
	}
}
