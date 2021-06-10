using System;

namespace ZooLab.BusinessLogic.Exceptions
{
	public class NoAvailableSpaceException : Exception
	{
		public NoAvailableSpaceException(string message) : base(message)
		{
		}
	}
}
