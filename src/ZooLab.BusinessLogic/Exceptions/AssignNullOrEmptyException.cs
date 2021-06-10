using System;

namespace ZooLab.BusinessLogic.Exceptions
{
	public class AssignNullOrEmptyException : Exception
	{
		public AssignNullOrEmptyException(string notNullNorEmptyObjectName, bool noWhitespace = false)
			: base(GetMessageForNotNullNorEmptyObject(notNullNorEmptyObjectName, noWhitespace))
		{ }

		private static string GetMessageForNotNullNorEmptyObject(string notNullNorEmptyObjectName, bool noWhitespace) =>
			$"Cannot assign 'null'{(noWhitespace ? ", empty or whitespace" : " or empty")} to {notNullNorEmptyObjectName}";
	}
}
