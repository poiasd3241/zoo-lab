using Xunit;

namespace ZooLab.BusinessLogic.Tests.Logging
{
	public class TestConsoleTests
	{
		[Fact]
		public void ShouldCreateTestConsole()
		{
			TestConsole testConsole = new();

			Assert.Equal("", testConsole.CurrentOutput);
		}

		[Fact]
		public void ShouldWriteLine()
		{
			TestConsole testConsole = new();
			testConsole.WriteLine("line");

			Assert.Equal("line\n", testConsole.CurrentOutput);
		}

		[Fact]
		public void ShouldClear()
		{
			TestConsole testConsole = new();
			testConsole.WriteLine("line");

			testConsole.Clear();

			Assert.Equal("", testConsole.CurrentOutput);
		}
	}
}
