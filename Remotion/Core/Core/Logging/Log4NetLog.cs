// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Globalization;
using log4net.Core;
using log4net.Util;
using Remotion.Globalization;
using Remotion.Globalization.Implementation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Logging
{
  /// <summary>
  /// Implementation of interface <see cref="ILog"/> for <b>log4net</b>.
  /// </summary>
  /// <remarks>
  /// Use <see cref="LogManager"/> to instantiate <see cref="Log4NetLog"/> via <see cref="Remotion.Logging.LogManager.GetLogger(string)"/>.
  /// <note type="warning">
  /// <see cref="Log4NetLog"/> does not allow event ids outside the range of unsigned 16-bit integers (0 - 65535) and will throw an
  /// <see cref="ArgumentOutOfRangeException"/> if an event id outside this range is encountered. The original message will be logged using a 
  /// truncated event id before the exception is thrown.
  /// </note>
  /// </remarks>
  public class Log4NetLog : LogImpl, ILog
  {
    private static readonly DoubleCheckedLockingContainer<IEnumerationGlobalizationService> s_globalizationService =
        new DoubleCheckedLockingContainer<IEnumerationGlobalizationService> (
            () => SafeServiceLocator.Current.GetInstance<IEnumerationGlobalizationService>());
    
    /// <summary>
    /// Converts <see cref="LogLevel"/> to <see cref="Level"/>.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/> to be converted.</param>
    /// <returns>Corresponding <see cref="Level"/> needed for logging to the <b>log4net </b> <see cref="log4net.ILog"/> interface.</returns>
    public static Level Convert (LogLevel logLevel)
    {
      switch (logLevel)
      {
        case LogLevel.Debug:
          return Level.Debug;
        case LogLevel.Info:
          return Level.Info;
        case LogLevel.Warn:
          return Level.Warn;
        case LogLevel.Error:
          return Level.Error;
        case LogLevel.Fatal:
          return Level.Fatal;
        default:
          throw new ArgumentException (string.Format ("LogLevel does not support value {0}.", logLevel), "logLevel");
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Log4NetLog"/> class 
    /// using the specified <see cref="log4net.Core.ILogger"/>.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> the log messages are written to.</param>
    public Log4NetLog (ILogger logger)
      : base (logger)
    {
    }

    /// <overloads><inheritdoc cref="ILog.Log(LogLevel, object)"/></overloads>
    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void Log (LogLevel logLevel, int? eventID, object message, Exception exceptionObject)
    {
      LogToLog4Net (Convert (logLevel), eventID, message, exceptionObject);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void Log (LogLevel logLevel, int eventID, object message)
    {
      Log (logLevel, eventID, message, null);
    }

    /// <inheritdoc />
    public void Log (LogLevel logLevel, object message, Exception exceptionObject)
    {
      Log (logLevel, (int?) null, message, exceptionObject);
    }

    /// <inheritdoc />
    public void Log (LogLevel logLevel, object message)
    {
      Log (logLevel, (int?) null, message, (Exception) null);
    }

    /// <overloads><inheritdoc cref="ILog.LogFormat(LogLevel, string, object[])"/></overloads>
    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void LogFormat (LogLevel logLevel, int? eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogToLog4NetFormat (Convert (logLevel), eventID, format, args, exceptionObject);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void LogFormat (LogLevel logLevel, int eventID, string format, params object[] args)
    {
      LogFormat (logLevel, eventID, (Exception) null, format, args);
    }

    /// <inheritdoc />
    public void LogFormat (LogLevel logLevel, string format, params object[] args)
    {
      LogFormat (logLevel, (int?) null, (Exception) null, format, args);
    }

    /// <inheritdoc />
    public void LogFormat (LogLevel logLevel, Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (logLevel, (int?) null, exceptionObject, format, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer. </exception>
    public void LogFormat (LogLevel logLevel, Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("messageEnum", messageEnum);

      LogFormat (
          logLevel,
          System.Convert.ToInt32 (messageEnum),
          exceptionObject,
          s_globalizationService.Value.GetEnumerationValueDisplayName (messageEnum),
          args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer. </exception>
    public void LogFormat (LogLevel logLevel, Enum messageEnum, params object[] args)
    {
      LogFormat (logLevel, messageEnum, (Exception) null, args);
    }

    /// <overloads><inheritdoc cref="ILog.Debug(object)"/></overloads>
    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void Debug (int eventID, object message, Exception exceptionObject)
    {
      Log (LogLevel.Debug, eventID, message, exceptionObject);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void Debug (int eventID, object message)
    {
      Log (LogLevel.Debug, eventID, message, null);
    }

    /// <overloads><inheritdoc cref="ILog.DebugFormat(string, object[])"/></overloads>
    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void DebugFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (LogLevel.Debug, eventID, exceptionObject, format, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void DebugFormat (int eventID, string format, params object[] args)
    {
      LogFormat (LogLevel.Debug, eventID, (Exception) null, format, args);
    }

    /// <inheritdoc />
    public void DebugFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (LogLevel.Debug, null, exceptionObject, format, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer. </exception>
    public void DebugFormat (Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("messageEnum", messageEnum);
      LogFormat (LogLevel.Debug, messageEnum, exceptionObject, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer. </exception>
    public void DebugFormat (Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("messageEnum", messageEnum);
      LogFormat (LogLevel.Debug, messageEnum, (Exception) null, args);
    }

    /// <overloads><inheritdoc cref="ILog.Info(object)"/></overloads>
    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void Info (int eventID, object message, Exception exceptionObject)
    {
      Log (LogLevel.Info, eventID, message, exceptionObject);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void Info (int eventID, object message)
    {
      Log (LogLevel.Info, eventID, message, null);
    }

    /// <overloads><inheritdoc cref="ILog.InfoFormat(string, object[])"/></overloads>
    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void InfoFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (LogLevel.Info, eventID, exceptionObject, format, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void InfoFormat (int eventID, string format, params object[] args)
    {
      LogFormat (LogLevel.Info, eventID, (Exception) null, format, args);
    }

    /// <inheritdoc />
    public void InfoFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (LogLevel.Info, (int?) null, exceptionObject, format, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer. </exception>
    public void InfoFormat (Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("messageEnum", messageEnum);
      LogFormat (LogLevel.Info, messageEnum, exceptionObject, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer. </exception>
    public void InfoFormat (Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("messageEnum", messageEnum);
      LogFormat (LogLevel.Info, messageEnum, (Exception) null, args);
    }


    /// <overloads><inheritdoc cref="ILog.Warn(object)"/></overloads>
    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void Warn (int eventID, object message, Exception exceptionObject)
    {
      Log (LogLevel.Warn, eventID, message, exceptionObject);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void Warn (int eventID, object message)
    {
      Log (LogLevel.Warn, eventID, message, (Exception) null);
    }

    /// <overloads><inheritdoc cref="ILog.WarnFormat(string, object[])"/></overloads>
    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void WarnFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (LogLevel.Warn, eventID, exceptionObject, format, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void WarnFormat (int eventID, string format, params object[] args)
    {
      LogFormat (LogLevel.Warn, eventID, (Exception) null, format, args);
    }

    /// <inheritdoc />
    public void WarnFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (LogLevel.Warn, (int?) null, exceptionObject, format, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer. </exception>
    public void WarnFormat (Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("messageEnum", messageEnum);
      LogFormat (LogLevel.Warn, messageEnum, exceptionObject, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer. </exception>
    public void WarnFormat (Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("messageEnum", messageEnum);
      LogFormat (LogLevel.Warn, messageEnum, (Exception) null, args);
    }


    /// <overloads><inheritdoc cref="ILog.Error(object)"/></overloads>
    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void Error (int eventID, object message, Exception exceptionObject)
    {
      Log (LogLevel.Error, eventID, message, exceptionObject);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void Error (int eventID, object message)
    {
      Log (LogLevel.Error, eventID, message, (Exception) null);
    }

    /// <overloads><inheritdoc cref="ILog.ErrorFormat(string, object[])"/></overloads>
    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void ErrorFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (LogLevel.Error, eventID, exceptionObject, format, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void ErrorFormat (int eventID, string format, params object[] args)
    {
      LogFormat (LogLevel.Error, eventID, (Exception) null, format, args);
    }

    /// <inheritdoc />
    public void ErrorFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (LogLevel.Error, (int?) null, exceptionObject, format, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer. </exception>
    public void ErrorFormat (Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("messageEnum", messageEnum);
      LogFormat (LogLevel.Error, messageEnum, exceptionObject, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer. </exception>
    public void ErrorFormat (Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("messageEnum", messageEnum);
      LogFormat (LogLevel.Error, messageEnum, (Exception) null, args);
    }


    /// <overloads><inheritdoc cref="ILog.Fatal(object)"/></overloads>
    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void Fatal (int eventID, object message, Exception exceptionObject)
    {
      Log (LogLevel.Fatal, eventID, message, exceptionObject);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void Fatal (int eventID, object message)
    {
      Log (LogLevel.Fatal, eventID, message, null);
    }

    /// <overloads><inheritdoc cref="ILog.FatalFormat(string, object[])"/></overloads>
    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void FatalFormat (int eventID, Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (LogLevel.Fatal, eventID, exceptionObject, format, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="eventID"/> is outside the range of an unsigned 16-bit integer. </exception>
    public void FatalFormat (int eventID, string format, params object[] args)
    {
      LogFormat (LogLevel.Fatal, eventID, (Exception) null, format, args);
    }

    /// <inheritdoc />
    public void FatalFormat (Exception exceptionObject, string format, params object[] args)
    {
      LogFormat (LogLevel.Fatal, (int?) null, exceptionObject, format, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer. </exception>
    public void FatalFormat (Enum messageEnum, Exception exceptionObject, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("messageEnum", messageEnum);
      LogFormat (LogLevel.Fatal, messageEnum, exceptionObject, args);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="messageEnum"/>'s underlying value is outside the range of an unsigned 16-bit integer. </exception>
    public void FatalFormat (Enum messageEnum, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("messageEnum", messageEnum);
      LogFormat (LogLevel.Fatal, messageEnum, (Exception) null, args);
    }

    /// <inheritdoc />
    public bool IsEnabled (LogLevel logLevel)
    {
      return base.Logger.IsEnabledFor (Convert (logLevel));
    }

    private void LogToLog4NetFormat (Level level, int? eventID, string format, object[] args, Exception exceptionObject)
    {
      if (Logger.IsEnabledFor (level))
        LogToLog4Net (level, eventID, new SystemStringFormat (CultureInfo.InvariantCulture, format, args), exceptionObject);
    }

    private void LogToLog4Net (Level level, int? eventID, object message, Exception exceptionObject)
    {
      if (Logger.IsEnabledFor (level))
        Logger.Log (CreateLoggingEvent (level, eventID, message, exceptionObject));
    }

    private LoggingEvent CreateLoggingEvent (Level level, int? eventID, object message, Exception exceptionObject)
    {
      LoggingEvent loggingEvent = new LoggingEvent (typeof (Log4NetLog), null, Logger.Name, level, message, exceptionObject);

      if (eventID.HasValue)
      {
        if (eventID < 0 || eventID > 0xFFFF)
        {
          LogLoggingError (eventID.Value, exceptionObject, message);

          throw new ArgumentOutOfRangeException (
             "eventID", string.Format ("An event id of value {0} is not supported. Valid event ids must be within a range of 0 and 65535.", eventID));
        }

        loggingEvent.Properties["EventID"] = eventID;
      }

      return loggingEvent;
    }

    private void LogLoggingError (int eventID, Exception exceptionObject, object message)
    {
      int safeEventID;
      if (eventID < 0)
        safeEventID  = 0;
      else if (eventID > 0xFFFF)
        safeEventID  = 0xFFFF;
      else
        safeEventID = eventID;
          
      LogToLog4NetFormat (
          Level.Error,
          safeEventID,
          "Failure during logging of message:\r\n{0}\r\nEvent ID: {1}",
          new object[] { message.ToString (), eventID },
          exceptionObject);
    }
  }
}
