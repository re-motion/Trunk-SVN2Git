// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class ThreadRunnerTest
  {
    private MockRepository _mockRepository;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
    }

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
      var threadRunner = new ThreadRunner (delegate { }, TimeSpan.FromSeconds (1.0));
      Assert.That (threadRunner.Timeout, Is.EqualTo (TimeSpan.FromSeconds (1.0)));
    }

    [Test]
    public void Ctor_WithoutTimeout_HasInfiniteTimeout ()
    {
      var threadRunner = new ThreadRunner (delegate { });
      Assert.That (threadRunner.Timeout.TotalMilliseconds, Is.EqualTo (Timeout.Infinite));
    }

    [Test]
    public void Run_CallsJoin ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner> ((ThreadStart) delegate { }, timeout);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).Return (true);

      threadRunnerMock.Replay ();
      threadRunnerMock.Run();
      threadRunnerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Run_CallsJoin_WithRightThread ()
    {
      using (var waitHandle = new ManualResetEvent (false))
      {
        Thread threadRunnerThread = null;
        var threadMethod = (ThreadStart) delegate { threadRunnerThread = Thread.CurrentThread; waitHandle.Set (); };

        TimeSpan timeout = TimeSpan.MaxValue;
        var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner> (threadMethod, timeout);

        threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).
            Do (
            // when this expectation is reached, assert that the threadRunnerThread was passed to the method
            invocation =>
            {
              waitHandle.WaitOne ();
              Assert.That (invocation.Arguments[0], Is.SameAs (threadRunnerThread));
            }).Return (true);

        threadRunnerMock.Replay();
        threadRunnerMock.Run();
        threadRunnerMock.VerifyAllExpectations();
      }
    }

    [Test]
    public void Run_UsesJoinResult_ToIndicateTimedOut_False ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner> ((ThreadStart) delegate { }, timeout);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).Return (true);

      threadRunnerMock.Replay ();
      Assert.That (threadRunnerMock.Run (), Is.False);
      threadRunnerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Run_UsesJoinResult_ToIndicateTimedOut_True ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner> ((ThreadStart) delegate { }, timeout);
      threadRunnerMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "JoinThread", Arg<Thread>.Is.Anything)).Return (false);

      threadRunnerMock.Replay ();
      Assert.That (threadRunnerMock.Run (), Is.True);
      threadRunnerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Run_WithTimedOutThread_CallsAbort ()
    {
      TimeSpan timeout = TimeSpan.FromSeconds (1.0);
      var threadRunnerMock = _mockRepository.PartialMock<ThreadRunner> ((ThreadStart) delegate { }, timeout);
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
      var threadRunnerMock =
          _mockRepository.PartialMock<ThreadRunner> ((ThreadStart) delegate { threadRunnerThread = Thread.CurrentThread; }, timeout);
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

    [Test]
    public void RunWithTimeout ()
    {
      bool timedOut = ThreadRunner.WithTimeout (RunTimesOutEndlessLoop, TimeSpan.FromSeconds (0.1)).Run();
      Assert.That (timedOut, Is.True);
    }

    [Test]
    public void RunWithoutTimeout ()
    {
      bool timedOut = ThreadRunner.WithTimeout (RunTimesOutVeryFastFunction, TimeSpan.FromMilliseconds (int.MaxValue)).Run ();
      Assert.That (timedOut, Is.False);
    }

    private static void RunTimesOutEndlessLoop ()
    {
      while (true) { }
    }

    private static void RunTimesOutVeryFastFunction ()
    {
    }
  }
}