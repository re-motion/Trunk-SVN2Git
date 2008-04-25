using System;
using log4net.Core;
using NUnit.Framework;
using Remotion.Logging;

namespace Remotion.UnitTests.Logging.Log4NetLogTests
{
  [TestFixture]
  public class LogTest : BaseTest
  {
    [Test]
    public void Test_WithMessageEventIDAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Info);

      Log.Log (LogLevel.Info, 2, (object) "The message.", exception);

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Info, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.AreEqual (2, loggingEvent.Properties["EventID"]);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithMessageAndEventID ()
    {
      SetLoggingThreshold (Level.Info);

      Log.Log (LogLevel.Info, 1, (object) "The message.");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Info, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.AreEqual (1, loggingEvent.Properties["EventID"]);
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithMessageAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Info);

      Log.Log (LogLevel.Info, (object) "The message.", exception);

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Info, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithMessage ()
    {
      SetLoggingThreshold (Level.Info);

      Log.Log (LogLevel.Info, (object) "The message.");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Info, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject);
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithEnumAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Info);

      Log.LogFormat (LogLevel.Info, LogMessages.TheMessage, exception, "First", "Second");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Info, loggingEvent.Level);
      Assert.AreEqual ("The message with First and Second.", loggingEvent.MessageObject.ToString ());
      Assert.AreEqual ((int)LogMessages.TheMessage, loggingEvent.Properties["EventID"]);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithEnum ()
    {
      SetLoggingThreshold (Level.Info);

      Log.LogFormat (LogLevel.Info, LogMessages.TheMessage, "First", "Second");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Info, loggingEvent.Level);
      Assert.AreEqual ("The message with First and Second.", loggingEvent.MessageObject.ToString ());
      Assert.AreEqual ((int) LogMessages.TheMessage, loggingEvent.Properties["EventID"]);
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_WithLogLevelNone ()
    {
      Logger.Repository.Threshold = Level.Off;

      Log.Log (LogLevel.Info, 1, (object) "The message.");

      Assert.IsEmpty (GetLoggingEvents ());
    }

    [Test]
    public void Test_FormatWithMessageAndEventIDAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Info);

      Log.LogFormat (LogLevel.Info, 1, exception, "{0} {1}", "The", "message.");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Info, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject.ToString ());
      Assert.AreEqual (1, loggingEvent.Properties["EventID"]);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithMessageAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Info);

      Log.LogFormat (LogLevel.Info, exception, "{0} {1}", "The", "message.");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Info, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject.ToString ());
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithMessageAndEventID ()
    {
      SetLoggingThreshold (Level.Info);

      Log.LogFormat (LogLevel.Info, 1, "{0} {1}", "The", "message.");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Info, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject.ToString ());
      Assert.AreEqual (1, loggingEvent.Properties["EventID"]);
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithMessage ()
    {
      SetLoggingThreshold (Level.Info);

      Log.LogFormat (LogLevel.Info, "{0} {1}", "The", "message.");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Info, loggingEvent.Level);
      Assert.AreEqual ("The message.", loggingEvent.MessageObject.ToString ());
      Assert.IsNull (loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithLogLevelNone ()
    {
      Logger.Repository.Threshold = Level.Off;

      Log.LogFormat (LogLevel.Info, 1, "{0} {1}", "The", "message.");

      Assert.IsEmpty (GetLoggingEvents ());
    }
  }
}