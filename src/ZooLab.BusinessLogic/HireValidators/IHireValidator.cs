using FluentValidation.Results;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.Zoos;

namespace ZooLab.BusinessLogic.HireValidators
{
	public interface IHireValidator
	{
		public ValidationResult ValidateEmployee(IEmployee employee, Zoo hiringZoo);
	}
}