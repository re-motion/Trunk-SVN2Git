using NUnit.Framework;
using Remotion.Logging;

namespace Remotion.UnitTests.Logging
{
  [TestFixture]
  public class LogManagerTest
  {
    [Test]
    public void GetLogger_WithNameAsString ()
    {
      ILog log = LogManager.GetLogger ("The Name");

      Assert.IsInstanceOfType (typeof (Log4NetLog), log);
      Log4NetLog log4NetLog = (Log4NetLog) log;
      Assert.AreEqual ("The Name", log4NetLog.Logger.Name);
    }

    [Test]
    public void GetLogger_WithNameFromType ()
    {
      ILog log = LogManager.GetLogger (typeof (SampleType));

      Assert.IsInstanceOfType (typeof (Log4NetLog), log);
      Log4NetLog log4NetLog = (Log4NetLog) log;
      Assert.AreEqual ("Remotion.UnitTests.Logging.SampleType", log4NetLog.Logger.Name);
    }

  }
}