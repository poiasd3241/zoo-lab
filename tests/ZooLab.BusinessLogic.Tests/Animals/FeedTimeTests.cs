using System;
using Xunit;
using ZooLab.BusinessLogic.Animals;
using ZooLab.BusinessLogic.Employees;

namespace ZooLab.BusinessLogic.Tests.Animals
{
	public class FeedTimeTests
	{
		[Fact]
		public void ShouldCreateFeedTime()
		{
			var time = DateTime.Now;
			var zooKeeper = new ZooKeeper("a", "b");

			FeedTime feedTime = new(time, zooKeeper);

			Assert.True(DateTime.Equals(time, feedTime.Time));
			Assert.Equal(zooKeeper, feedTime.FedByZooKeeper);
		}
	}
}
