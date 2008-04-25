using NUnit.Framework;
using Remotion.Logging;

namespace Remotion.UnitTests.Logging
{
  [TestFixture]
  public class Log4NetLogManagerTest
  {
    [Test]
    public void GetLogger_WithNameAsString ()
    {
      ILogManager logManager = new Log4NetLogManager ();
      
      ILog log = logManager.GetLogger ("The Name");

      Assert.IsInstanceOfType (typeof (Log4NetLog), log);
      Log4NetLog log4NetLog = (Log4NetLog) log;
      Assert.AreEqual ("The Name", log4NetLog.Logger.Name);
    }

    [Test]
    public void GetLogger_WithNameFromType ()
    {
      ILogManager logManager = new Log4NetLogManager ();

      ILog log = logManager.GetLogger (typeof (SampleType));

      Assert.IsInstanceOfType (typeof (Log4NetLog), log);
      Log4NetLog log4NetLog = (Log4NetLog) log;
      Assert.AreEqual ("Remotion.UnitTests.Logging.SampleType", log4NetLog.Logger.Name);
    }
  }
}