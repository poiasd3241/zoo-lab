using System;
using Xunit;
using ZooLab.BusinessLogic.Employees;
using ZooLab.BusinessLogic.HireValidators;

namespace ZooLab.BusinessLogic.Tests.HireValidators
{
	public class HireValidatorProviderTests
	{
		public class ByHoursNullOrEmpty : TheoryData<IEmployee, Type>
		{
			public ByHoursNullOrEmpty()
			{
				Add(new ZooKeeper("a", "b"), typeof(ZooKeeperHireValidator));
				Add(new Veterinarian("a", "b"), typeof(VeterinarianHireValidator));
			}
		}

		[Theory]
		[ClassData(typeof(ByHoursNullOrEmpty))]
		public void ShouldGetHireValidator(IEmployee employee, Type hireValidatorType)
		{
			var hireValidator = HireValidatorProvider.GetHireValidator(employee);

			Assert.Equal(hireValidatorType, hireValidator.GetType());
		}

		#region Unknown Employee

		private class UnknownEmployee : IEmployee
		{
			public string FirstName { get; }
			public string LastName { get; }
		}

		[Fact]
		public void ShouldCreateUnknownEmployee()
		{
			var employee = new UnknownEmployee();

			Assert.Null(employee.FirstName);
			Assert.Null(employee.LastName);
		}

		#endregion

		[Fact]
		public void ShouldFailGetHireValidatorByEmployeeUnknown()
		{
			var exception = Assert.Throws<NotImplementedException>(
				() => HireValidatorProvider.GetHireValidator(new UnknownEmployee()));

			Assert.Equal("Unknown employee type: 'UnknownEmployee'.", exception.Message);
		}
	}
}
