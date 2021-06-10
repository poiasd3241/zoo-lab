using System;

namespace ZooLab.BusinessLogic.Exceptions
{
	public class AssignNullException : Exception
	{
		public AssignNullException(string notNullObjectName)
			: base(GetMessageForNotNullObject(notNullObjectName))
		{ }

		private static string GetMessageForNotNullObject(string notNullObjectName) =>
			$"Cannot assign 'null' to {notNullObjectName}";
	}
}
