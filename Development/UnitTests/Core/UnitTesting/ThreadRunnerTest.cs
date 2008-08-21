using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Logging;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class ThreadRunnerTest
  {
    private ISimpleLogger log = SimpleLogger.Create (true);

    [Test]
    public void Run ()
    {
      bool threadRun = false;
      ThreadRunner.Run (delegate { threadRun = true; });

      Assert.That (threadRun, Is.True);
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
      //Console.WriteLine ("RunTimesOutVeryFastFunction");
    }

    private static bool IsInRelativeRangeAround (double center, double relativeDelta, double test)
    {
      return (center*(1.0 - relativeDelta) <= test) && (center*(1.0 + relativeDelta) >= test);
    }

    [Test]
    public void RunTimesOut ()
    {
      var stopwatch = new Stopwatch ();
      stopwatch.Start ();
      bool timedOut = ThreadRunner.RunTimesOut (RunTimesOutEndlessLoop, new TimeSpan (0, 0, 0, 0, 100));
      stopwatch.Stop ();
      Assert.That (timedOut, Is.True);
      Assert.That (IsInRelativeRangeAround (100, 0.5, stopwatch.ElapsedMilliseconds), Is.True);
    }

    [Test]
    public void RunTimesOut2 ()
    {
      bool timedOut = ThreadRunner.RunTimesOut (RunTimesOutVeryFastFunction, new TimeSpan (0, 0, 0,100));
      Assert.That (timedOut, Is.False);
    }

    [Test]
    public void RunTimesOutAfterMilliseconds ()
    {
      var stopwatch = new Stopwatch ();
      stopwatch.Start ();
      bool timedOut = ThreadRunner.RunTimesOutAfterMilliseconds (RunTimesOutEndlessLoop, 100);
      stopwatch.Stop ();
      Assert.That (timedOut, Is.True);
      Assert.That (IsInRelativeRangeAround (100, 0.5, stopwatch.ElapsedMilliseconds), Is.True);
    }

    [Test]
    public void RunTimesOutAfterSeconds ()
    {
      var stopwatch = new Stopwatch ();
      stopwatch.Start ();
      bool timedOut = ThreadRunner.RunTimesOutAfterSeconds (RunTimesOutEndlessLoop, 0.1);
      stopwatch.Stop ();
      Assert.That (timedOut, Is.True);
      Assert.That (IsInRelativeRangeAround (100, 0.5, stopwatch.ElapsedMilliseconds), Is.True);
    }

  }
}