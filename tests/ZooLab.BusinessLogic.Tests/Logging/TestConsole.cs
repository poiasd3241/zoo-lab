using System.Text;
using Xunit;
using ZooLab.BusinessLogic.Logging;

namespace ZooLab.BusinessLogic.Tests.Logging
{
	public class TestConsole : IConsole
	{
		private readonly StringBuilder _currentOutputBuilder;
		public string CurrentOutput => _currentOutputBuilder.ToString();
		public void Clear()
		{
			_currentOutputBuilder.Clear();
		}
		public TestConsole()
		{
			_currentOutputBuilder = new();
		}

		public void WriteLine(string text)
		{
			_currentOutputBuilder.Append($"{text}\n");
		}

		public class TestConsoleOrNull : TheoryData<TestConsole>
		{
			public TestConsoleOrNull()
			{
				Add(null);
				Add(new TestConsole());
			}
		}
	}
}
