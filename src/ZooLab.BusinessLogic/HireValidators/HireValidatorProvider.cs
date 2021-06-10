using System;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.ExtensionMethods;

namespace ZooLab.BusinessLogic.HireValidators
{
	public class HireValidatorProvider
	{
		public static IHireValidator GetHireValidator(IEmployee employee)
		{
			return employee switch
			{
				ZooKeeper => new ZooKeeperHireValidator(),
				Veterinarian => new VeterinarianHireValidator(),
				_ => throw new NotImplementedException($"Unknown employee type: '{employee.GetTypeName()}'.")
			};
		}
	}
}
