using System.Collections.Generic;
using System.Linq;

namespace ZooLab.BusinessLogic.ExtensionMethods
{
	public static class IEnumerableExtensions
	{
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
		{
			return enumerable?.Count() > 0 == false;
		}
	}
}
