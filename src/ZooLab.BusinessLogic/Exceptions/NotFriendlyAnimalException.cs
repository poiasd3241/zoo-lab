using System;

namespace ZooLab.BusinessLogic.Exceptions
{
	public class NotFriendlyAnimalException : Exception
	{
		public NotFriendlyAnimalException(string message) : base(message)
		{
		}
	}
}
