using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Logging;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class ThreadRunnerTest
  {
    private readonly ISimpleLogger _log = SimpleLogger.CreateForConsole (false);

    [Test]
    public void Run ()
    {
      bool threadRun = false;
      ThreadRunner.Run (delegate { threadRun = true; });

      Assert.That (threadRun, Is.True);
    }

    [Test]
    public void Ctor_WithTimeout ()
    {
      ThreadRunner threadRunner = new ThreadRunner (delegate { }, TimeSpan.FromSeconds (1.0));
      Assert.That (threadRunner.Timeout, Is.EqualTo (TimeSpan.FromSeconds (1.0)));
    }

    [Test]
    public void Ctor_WithoutTimeout_HasInfiniteTimeout ()
    {
      ThreadRunner threadRunner = new ThreadRunner (delegate { });
      Assert.That (threadRunner.Timeout.TotalMilliseconds, Is.EqualTo (Timeout.Infinite));
    }

    [Test]
    public void Run_CallsJoin ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      ThreadRunner threadRunnerMock = new MockRepository ().PartialMock<ThreadRunner> ((ThreadStart) delegate { }, timeout);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).Return (true);

      threadRunnerMock.Replay ();
      threadRunnerMock.Run();
      threadRunnerMock.VerifyAllExpectations ();
    }

    [Test]
    [Ignore("TODO: Fails on local build. MK")]
    public void Run_CallsJoin_WithRightThread ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      Thread threadRunnerThread = null;
      ThreadRunner threadRunnerMock = 
          new MockRepository ().PartialMock<ThreadRunner> ((ThreadStart) delegate { threadRunnerThread = Thread.CurrentThread; }, timeout);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).
          Do ( // when this expectation is reached, assert that the threadRunnerThread was passed to the method
          invocation => Assert.That (invocation.Arguments[0], Is.SameAs (threadRunnerThread))).Return (true);

      threadRunnerMock.Replay ();
      threadRunnerMock.Run ();
      threadRunnerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Run_UsesJoinResult_ToIndicateTimedOut_False ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      ThreadRunner threadRunnerMock = new MockRepository ().PartialMock<ThreadRunner> ((ThreadStart) delegate { }, timeout);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).Return (true);

      threadRunnerMock.Replay ();
      Assert.That (threadRunnerMock.Run (), Is.False);
      threadRunnerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Run_UsesJoinResult_ToIndicateTimedOut_True ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      ThreadRunner threadRunnerMock = new MockRepository ().PartialMock<ThreadRunner> ((ThreadStart) delegate { }, timeout);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).Return (false);

      threadRunnerMock.Replay ();
      Assert.That (threadRunnerMock.Run (), Is.True);
      threadRunnerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Run_WithTimedOutThread_CallsAbort ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      ThreadRunner threadRunnerMock = new MockRepository ().PartialMock<ThreadRunner> ((ThreadStart) delegate { }, timeout);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).Return (false);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "AbortThread", Arg<Thread>.Is.Anything));

      threadRunnerMock.Replay ();
      threadRunnerMock.Run ();
      threadRunnerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Run_WithTimedOutThread_CallsAbort_WithRightThread ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      Thread threadRunnerThread = null;
      ThreadRunner threadRunnerMock =
          new MockRepository ().PartialMock<ThreadRunner> ((ThreadStart) delegate { threadRunnerThread = Thread.CurrentThread; }, timeout);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).Return (false);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "AbortThread", Arg<Thread>.Is.Anything)).
          Do ( // when this expectation is reached, assert that the threadRunnerThread was passed to the method
          invocation => Assert.That (invocation.Arguments[0], Is.SameAs (threadRunnerThread)));

      threadRunnerMock.Replay ();
      threadRunnerMock.Run ();
      threadRunnerMock.VerifyAllExpectations ();
    }

    [Test]
    public void WithMillisecondsTimeout ()
    {
      ThreadRunner threadRunner = ThreadRunner.WithMillisecondsTimeout (delegate { }, 250);
      Assert.That (threadRunner.Timeout, Is.EqualTo (TimeSpan.FromMilliseconds (250)));
    }

    [Test]
    public void WithSecondsTimeout ()
    {
      ThreadRunner threadRunner = ThreadRunner.WithSecondsTimeout (delegate { }, 250);
      Assert.That (threadRunner.Timeout, Is.EqualTo (TimeSpan.FromSeconds (250)));
    }

    [Test]
    public void WithTimeout ()
    {
      ThreadRunner threadRunner = ThreadRunner.WithTimeout (delegate { }, TimeSpan.FromMinutes (250));
      Assert.That (threadRunner.Timeout, Is.EqualTo (TimeSpan.FromMinutes (250)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "xy")]
    public void Run_WithException ()
    {
      var exception = new InvalidOperationException ("xy");
      ThreadRunner.Run (() => { throw exception; });
    }

    private static void RunTimesOutEndlessLoop ()
    {
      while (true) { }
    }

    private static void RunTimesOutVeryFastFunction ()
    {
    }
  
    [Test]
    public void RunWithTimeout ()
    {
      bool timedOut = ThreadRunner.WithTimeout (RunTimesOutEndlessLoop, TimeSpan.FromSeconds (0.1)).Run();
      Assert.That (timedOut, Is.True);
    }

    [Test]
    public void RunWithoutTimeout ()
    {
      bool timedOut = ThreadRunner.WithTimeout (RunTimesOutVeryFastFunction, TimeSpan.FromDays (24.8551)).Run ();
      Assert.That (timedOut, Is.False);
    }
  }
}