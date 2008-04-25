using System;
using NUnit.Framework;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Utilities
{
	[TestFixture]
	public class ConsoleUtilityTest
	{
		private IConsoleManager _consoleStub;

		[SetUp]
		public void SetUp ()
		{
			_consoleStub = new MockRepository().Stub<IConsoleManager>();
			_consoleStub.ForegroundColor = ConsoleColor.Gray;
			_consoleStub.BackgroundColor = ConsoleColor.Black;
		}

		[Test]
		public void EnterColorScope_NullForegroundColor ()
		{
			Assert.AreEqual (ConsoleColor.Gray, _consoleStub.ForegroundColor);
			using (new ConsoleUtility.ColorScope (_consoleStub, null, ConsoleColor.Gray))
			{
				Assert.AreEqual (ConsoleColor.Gray, _consoleStub.ForegroundColor);
				_consoleStub.ForegroundColor = ConsoleColor.Yellow;
			}
			Assert.AreEqual (ConsoleColor.Yellow, _consoleStub.ForegroundColor);
		}

		[Test]
		public void EnterColorScope_ForegroundColor_SetsColor ()
		{
			Assert.AreEqual (ConsoleColor.Gray, _consoleStub.ForegroundColor);
			using (new ConsoleUtility.ColorScope (_consoleStub, ConsoleColor.Green, ConsoleColor.Gray))
			{
				Assert.AreEqual (ConsoleColor.Green, _consoleStub.ForegroundColor);
			}
		}

		[Test]
		public void EnterColorScope_ForegroundColor_RestoresColor ()
		{
			_consoleStub.ForegroundColor = ConsoleColor.Magenta;
			using (new ConsoleUtility.ColorScope (_consoleStub, ConsoleColor.Blue, ConsoleColor.Gray))
			{
				Assert.AreEqual (ConsoleColor.Blue, _consoleStub.ForegroundColor);
			}
			Assert.AreEqual (ConsoleColor.Magenta, _consoleStub.ForegroundColor);
		}

		[Test]
		public void EnterColorScope_NullBackgroundColor ()
		{
			Assert.AreEqual (ConsoleColor.Black, _consoleStub.BackgroundColor);
			using (new ConsoleUtility.ColorScope (_consoleStub, ConsoleColor.Green, null))
			{
				Assert.AreEqual (ConsoleColor.Black, _consoleStub.BackgroundColor);
				_consoleStub.BackgroundColor = ConsoleColor.Green;
			}
			Assert.AreEqual (ConsoleColor.Green, _consoleStub.BackgroundColor);
		}

		[Test]
		public void EnterColorScope_BackgroundColor_SetsColor ()
		{
			Assert.AreEqual (ConsoleColor.Black, _consoleStub.BackgroundColor);
			using (new ConsoleUtility.ColorScope (_consoleStub, ConsoleColor.Gray, ConsoleColor.Green))
			{
				Assert.AreEqual (ConsoleColor.Green, _consoleStub.BackgroundColor);
			}
		}

		[Test]
		public void EnterColorScope_BackgroundColor_RestoresColor ()
		{
			_consoleStub.BackgroundColor = ConsoleColor.Magenta;
			using (new ConsoleUtility.ColorScope (_consoleStub, ConsoleColor.Gray, ConsoleColor.Blue))
			{
				Assert.AreEqual (ConsoleColor.Blue, _consoleStub.BackgroundColor);
			}
			Assert.AreEqual (ConsoleColor.Magenta, _consoleStub.BackgroundColor);
		}

		[Test]
		public void EnterColorScope_DisposeTwiceIgnored ()
		{
			_consoleStub.ForegroundColor = ConsoleColor.White;
			_consoleStub.BackgroundColor = ConsoleColor.Red;
			IDisposable scope = new ConsoleUtility.ColorScope (_consoleStub, ConsoleColor.Green, ConsoleColor.Magenta);
			Assert.AreEqual (ConsoleColor.Green, _consoleStub.ForegroundColor, "color was set");
			Assert.AreEqual (ConsoleColor.Magenta, _consoleStub.BackgroundColor, "color was set");
			scope.Dispose();
			Assert.AreEqual (ConsoleColor.White, _consoleStub.ForegroundColor, "color was restored");
			Assert.AreEqual (ConsoleColor.Red, _consoleStub.BackgroundColor, "color was restored");
			_consoleStub.ForegroundColor = ConsoleColor.Yellow;
			_consoleStub.BackgroundColor = ConsoleColor.DarkYellow;
			scope.Dispose ();
			Assert.AreEqual (ConsoleColor.Yellow, _consoleStub.ForegroundColor, "second dispose ignored");
			Assert.AreEqual (ConsoleColor.DarkYellow, _consoleStub.BackgroundColor, "second dispose ignored");
		}
	}
}