using System;
using ZooLab.BusinessLogic.Employees;

namespace ZooLab.BusinessLogic.Animals
{
	public class FeedTime
	{
		public DateTime Time { get; }
		public ZooKeeper FedByZooKeeper { get; }

		public FeedTime(DateTime time, ZooKeeper fedByZooKeeper)
		{
			Time = time;
			FedByZooKeeper = fedByZooKeeper;
		}
	}
}
