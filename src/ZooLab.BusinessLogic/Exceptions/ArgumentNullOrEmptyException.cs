using System;

namespace ZooLab.BusinessLogic.Exceptions
{
	public class ArgumentNullOrEmptyException : ArgumentNullException
	{
		public ArgumentNullOrEmptyException(string paramName) : base(paramName)
		{
		}
	}
}
