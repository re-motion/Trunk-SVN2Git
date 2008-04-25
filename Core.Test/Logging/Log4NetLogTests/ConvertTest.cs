using System;
using log4net.Core;
using NUnit.Framework;
using Remotion.Logging;

namespace Remotion.UnitTests.Logging.Log4NetLogTests
{
  [TestFixture]
  public class ConvertTest
  {
    [Test]
    public void Test_Info ()
    {
      Assert.AreEqual (Level.Info, Log4NetLog.Convert (LogLevel.Info));
    }

    [Test]
    public void Test_Debug ()
    {
      Assert.AreEqual (Level.Debug, Log4NetLog.Convert (LogLevel.Debug));
    }

    [Test]
    public void Test_Warn ()
    {
      Assert.AreEqual (Level.Warn, Log4NetLog.Convert (LogLevel.Warn));
    }

    [Test]
    public void Test_Error ()
    {
      Assert.AreEqual (Level.Error, Log4NetLog.Convert (LogLevel.Error));
    }

    [Test]
    public void Test_Fatal ()
    {
      Assert.AreEqual (Level.Fatal, Log4NetLog.Convert (LogLevel.Fatal));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "LogLevel does not support value 10.\r\nParameter name: logLevel")]
    public void Test_InvalidLevel ()
    {
      Log4NetLog.Convert ((LogLevel) 10);
    }
  }
}