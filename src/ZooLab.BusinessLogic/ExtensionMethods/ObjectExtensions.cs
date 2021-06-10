namespace ZooLab.BusinessLogic.ExtensionMethods
{
	public static class ObjectExtensions
	{
		public static string GetTypeName(this object obj) => obj.GetType().Name;
	}
}
