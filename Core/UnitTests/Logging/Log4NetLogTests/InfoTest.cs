/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using log4net.Core;
using NUnit.Framework;

namespace Remotion.UnitTests.Logging.Log4NetLogTests
{
  [TestFixture]
  public class InfoTest : BaseTest
  {
    [Test]
    public void IsEnabled_WithLevelDebug ()
    {
      SetLoggingThreshold (Level.Debug);
      Assert.IsTrue (Log.IsInfoEnabled);
    }

    [Test]
    public void IsEnabled_WithLevelInfo ()
    {
      SetLoggingThreshold (Level.Info);
      Assert.IsTrue (Log.IsInfoEnabled);
    }

    [Test]
    public void IsEnabled_WithLevelWarn ()
    {
      SetLoggingThreshold (Level.Error);
      Assert.IsFalse (Log.IsInfoEnabled);
    }

    [Test]
    public void Logger_Log ()
    {
      SetLoggingThreshold (Level.Info);
      Logger.Log (GetType (), Level.Info, "The message.", null);
      
      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreEqual (Level.Info, events[0].Level);
      Assert.AreEqual ("The message.", events[0].MessageObject);
    }

    [Test]
    public void Test_WithMessageEventIDAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Info);

      Log.Info (2, (object) "The message.", exception);

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

      Log.Info (1, (object) "The message.");

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

      Log.Info ((object) "The message.", exception);

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

      Log.Info ((object) "The message.");

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
    public void Test_WithLogLevelNone ()
    {
      Logger.Repository.Threshold = Level.Off;

      Log.Info (1, (object) "The message.");

      Assert.IsEmpty (GetLoggingEvents ());
    }

    [Test]
    public void Test_FormatWithMessageAndEventIDAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Info);

      Log.InfoFormat (1, exception, "{0} {1}", "The", "message.");

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
    public void Test_FormatWithMessageAndEventID ()
    {
      SetLoggingThreshold (Level.Info);

      Log.InfoFormat (1, "{0} {1}", "The", "message.");

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
    public void Test_FormatWithMessageAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Info);

      Log.InfoFormat (exception, "{0} {1}", "The", "message.");

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
    public void Test_FormatWithMessage ()
    {
      SetLoggingThreshold (Level.Info);

      Log.InfoFormat ("{0} {1}", "The", "message.");

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
    public void Test_FormatWithEnumAndException ()
    {
      Exception exception = new Exception ();
      SetLoggingThreshold (Level.Info);

      Log.InfoFormat (LogMessages.TheMessage, exception, "First", "Second");

      LoggingEvent[] events = GetLoggingEvents ();
      Assert.AreEqual (1, events.Length);
      LoggingEvent loggingEvent = events[0];
      Assert.AreEqual (Level.Info, loggingEvent.Level);
      Assert.AreEqual ("The message with First and Second.", loggingEvent.MessageObject.ToString ());
      Assert.AreEqual ((int) LogMessages.TheMessage, loggingEvent.Properties["EventID"]);
      Assert.AreSame (exception, loggingEvent.ExceptionObject);
      Assert.AreSame (Logger.Repository, loggingEvent.Repository);
      Assert.AreEqual (Logger.Name, loggingEvent.LoggerName);
    }

    [Test]
    public void Test_FormatWithEnum ()
    {
      SetLoggingThreshold (Level.Info);

      Log.InfoFormat (LogMessages.TheMessage, "First", "Second");

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
    public void Test_FormatWithLogLevelNone ()
    {
      Logger.Repository.Threshold = Level.Off;

      Log.InfoFormat (1, "{0} {1}", "The", "message.");

      Assert.IsEmpty (GetLoggingEvents ());
    }
  }
}
