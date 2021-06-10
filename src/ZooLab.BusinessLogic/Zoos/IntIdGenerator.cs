using System;

namespace ZooLab.BusinessLogic.Zoos
{
	public class IntIdGenerator
	{
		public int NextID { get; private set; } = 0;
		public int MaxID { get; }

		public IntIdGenerator(int maxID)
		{
			if (maxID <= 0)
			{
				throw new ArgumentException("Max ID must be greater than 0.");
			}
			MaxID = maxID;
		}

		public int GetNextID()
		{
			if (NextID == MaxID)
			{
				throw new OverflowException($"Reached maximum value of the generated ID ({MaxID}).");
			}
			return NextID++;
		}
	}
}
