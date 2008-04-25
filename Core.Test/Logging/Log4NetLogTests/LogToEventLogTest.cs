using System;
using System.Diagnostics;
using System.Reflection;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Util;
using NUnit.Framework;
using Remotion.Logging;

namespace Remotion.UnitTests.Logging.Log4NetLogTests
{
  [TestFixture]
  public class LogToEventLogTest
  {
    private static readonly string s_eventLogName = "Remotion_UnitTests";
    private static readonly string s_eventLogSource = "LogToEventLogTest_Log";
    private ILogger _logger;
    private ILog _log;
    private EventLog _testEventLog;

    [TestFixtureSetUp]
    public void SetUpFixture ()
    {
      if (!EventLog.SourceExists (s_eventLogSource))
        EventLog.CreateEventSource (s_eventLogSource, s_eventLogName);
      _testEventLog = Array.Find (EventLog.GetEventLogs(), delegate (EventLog current) { return current.Log == s_eventLogName; });
    }

    [TestFixtureTearDown]
    public void TearDownFixture ()
    {
      if (EventLog.SourceExists (s_eventLogSource))
        EventLog.DeleteEventSource (s_eventLogSource);

      if (EventLog.Exists (s_eventLogName))
        EventLog.Delete (s_eventLogName);

      _testEventLog.Dispose();
    }

    [SetUp]
    public void SetUp ()
    {
      EventLogAppender eventLogAppender = new EventLogAppender();
      eventLogAppender.LogName = s_eventLogName;
      eventLogAppender.ApplicationName = s_eventLogSource;
      eventLogAppender.SecurityContext = NullSecurityContext.Instance;
      eventLogAppender.Layout = new PatternLayout ("%m\r\n\r\n");
      BasicConfigurator.Configure (eventLogAppender);

      _logger = LoggerManager.GetLogger (Assembly.GetCallingAssembly(), "The Name");
      _log = new Log4NetLog (_logger);
      _testEventLog.Clear ();
    }

    [TearDown]
    public void TearDown ()
    {
      LoggerManager.Shutdown();
      _testEventLog.Clear ();
    }

    [Test]
    public void LogToEventLog ()
    {
      _logger.Repository.Threshold = Level.Info;

      _log.Log (LogLevel.Info, 1, (object) "The message.");
      Assert.AreEqual (1, _testEventLog.Entries.Count);
      EventLogEntry eventLogEntry = _testEventLog.Entries[0];
      Assert.AreEqual (EventLogEntryType.Information, eventLogEntry.EntryType);
      Assert.AreEqual ("The message.\r\n\r\n", eventLogEntry.Message);
      Assert.AreEqual (1, eventLogEntry.InstanceId);
    }

    [Test]
    [ExpectedException(typeof (ArgumentOutOfRangeException), ExpectedMessage = "An event id of value 65536 is not supported. Valid event ids must be within a range of 0 and 65535.\r\nParameter name: eventID")]
    public void Log_WithEventIDGreaterThan0xFFFF ()
    {
      _logger.Repository.Threshold = Level.Info;

      try
      {
        _log.Log (LogLevel.Info, 0x10000, (object) "The message.");
      }
      catch (Exception)
      {
        Assert.AreEqual (1, _testEventLog.Entries.Count);
        EventLogEntry eventLogEntry = _testEventLog.Entries[0];
        Assert.AreEqual (EventLogEntryType.Error, eventLogEntry.EntryType);
        Assert.AreEqual ("Failure during logging of message:\r\nThe message.\r\nEvent ID: 65536\r\n\r\n", eventLogEntry.Message);
        Assert.AreEqual (0xFFFF, eventLogEntry.InstanceId);

        throw;
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException), ExpectedMessage = "An event id of value -1 is not supported. Valid event ids must be within a range of 0 and 65535.\r\nParameter name: eventID")]
    public void Log_WithEventIDLessThanZero ()
    {
      _logger.Repository.Threshold = Level.Info;

      try
      {
        _log.Log (LogLevel.Info, -1, (object) "The message.");
      }
      catch (Exception)
      {
        Assert.AreEqual (1, _testEventLog.Entries.Count);
        EventLogEntry eventLogEntry = _testEventLog.Entries[0];
        Assert.AreEqual (EventLogEntryType.Error, eventLogEntry.EntryType);
        Assert.AreEqual ("Failure during logging of message:\r\nThe message.\r\nEvent ID: -1\r\n\r\n", eventLogEntry.Message);
        Assert.AreEqual (0x0, eventLogEntry.InstanceId);

        throw;
      }
    }
  }
}
