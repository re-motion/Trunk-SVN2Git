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

namespace Remotion.Logging
{
  /// <summary>
  /// The <see cref="ILog"/> interface declares methods for logging messages.
  /// </summary>
  /// <include file='doc\include\Logging\ILog.xml' path='ILog/Class/remarks' />
  public interface ILog
  {
    /// <overloads>Log a message object with the specified <paramref name="logLevel"/>.</overloads>
    /// <summary>
    /// Log a message object with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' 
    ///     path='ILog/Log/param[@name="logLevel" or @name="eventID" or @name="message" or @name="exceptionObject"]' />
    void Log (LogLevel logLevel, int eventID, object message, Exception exceptionObject);

    /// <summary>
    /// Log a message object with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="logLevel" or @name="eventID" or @name="message"]' />
    void Log (LogLevel logLevel, int eventID, object message);

    /// <summary>
    /// Log a message object with the specified <paramref name="logLevel"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="logLevel" or @name="message" or @name="exceptionObject"]' />
    void Log (LogLevel logLevel, object message, Exception exceptionObject);

    /// <summary>
    /// Log a message object with the specified <paramref name="logLevel"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="logLevel" or @name="message"]' />
    void Log (LogLevel logLevel, object message);

    /// <overloads>Log a formatted string with the specified <paramref name="logLevel"/>.</overloads>
    /// <summary>
    /// Log a formatted string with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="logLevel" or @name="eventID" or @name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void LogFormat (LogLevel logLevel, int eventID, Exception exceptionObject, string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="logLevel" or @name="eventID"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void LogFormat (LogLevel logLevel, int eventID, string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the specified <paramref name="logLevel"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="logLevel"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void LogFormat (LogLevel logLevel, string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the specified <paramref name="logLevel"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="logLevel" or @name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void LogFormat (LogLevel logLevel, Exception exceptionObject, string format, params object[] args);

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the specified <paramref name="logLevel"/>, including the stack 
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="logLevel" or @name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormatWithEnum/remarks' />
    void LogFormat (LogLevel logLevel, Enum messageEnum, Exception exceptionObject, params object[] args);

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the specified <paramref name="logLevel"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="logLevel"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormatWithEnum/remarks' />
    void LogFormat (LogLevel logLevel, Enum messageEnum, params object[] args);


    /// <overloads>Log a message object with the <see cref="LogLevel.Debug"/> level.</overloads>
    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Debug"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="message" or @name="exceptionObject"]' />
    void Debug (int eventID, object message, Exception exceptionObject);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Debug"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="message"]' />
    void Debug (int eventID, object message);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Debug"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="message" or @name="exceptionObject"]' />
    void Debug (object message, Exception exceptionObject);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="message"]' />
    void Debug (object message);

    /// <overloads>Log a formatted string with the <see cref="LogLevel.Debug"/> level.</overloads>
    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Debug"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void DebugFormat (int eventID, Exception exceptionObject, string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Debug"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void DebugFormat (int eventID, string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void DebugFormat (string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Debug"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void DebugFormat (Exception exceptionObject, string format, params object[] args);

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Debug"/> level, including the stack 
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormatWithEnum/remarks' />
    void DebugFormat (Enum messageEnum, Exception exceptionObject, params object[] args);

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormatWithEnum/remarks' />
    void DebugFormat (Enum messageEnum, params object[] args);


    /// <overloads>Log a message object with the <see cref="LogLevel.Info"/> level.</overloads>
    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Info"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="message" or @name="exceptionObject"]' />
    void Info (int eventID, object message, Exception exceptionObject);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Info"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="message"]' />
    void Info (int eventID, object message);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Info"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="message" or @name="exceptionObject"]' />
    void Info (object message, Exception exceptionObject);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="message"]' />
    void Info (object message);

    /// <overloads>Log a formatted string with the <see cref="LogLevel.Info"/> level.</overloads>
    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Info"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void InfoFormat (int eventID, Exception exceptionObject, string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Info"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void InfoFormat (int eventID, string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void InfoFormat (string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Info"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void InfoFormat (Exception exceptionObject, string format, params object[] args);

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Info"/> level, including the stack 
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormatWithEnum/remarks' />
    void InfoFormat (Enum messageEnum, Exception exceptionObject, params object[] args);

    /// <summary>
    /// Log message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Info"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormatWithEnum/remarks' />
    void InfoFormat (Enum messageEnum, params object[] args);


    /// <overloads>Log a message object with the <see cref="LogLevel.Warn"/> level.</overloads>
    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Warn"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="message" or @name="exceptionObject"]' />
    void Warn (int eventID, object message, Exception exceptionObject);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Warn"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="message"]' />
    void Warn (int eventID, object message);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Warn"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="message" or @name="exceptionObject"]' />
    void Warn (object message, Exception exceptionObject);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="message"]' />
    void Warn (object message);

    /// <overloads>Log a formatted string with the <see cref="LogLevel.Warn"/> level.</overloads>
    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Warn"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void WarnFormat (int eventID, Exception exceptionObject, string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Warn"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void WarnFormat (int eventID, string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void WarnFormat (string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Warn"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void WarnFormat (Exception exceptionObject, string format, params object[] args);

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Warn"/> level, including the stack 
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormatWithEnum/remarks' />
    void WarnFormat (Enum messageEnum, Exception exceptionObject, params object[] args);

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Warn"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormatWithEnum/remarks' />
    void WarnFormat (Enum messageEnum, params object[] args);


    /// <overloads>Log a message object with the <see cref="LogLevel.Error"/> level.</overloads>
    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Error"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="message" or @name="exceptionObject"]' />
    void Error (int eventID, object message, Exception exceptionObject);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Error"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="message"]' />
    void Error (int eventID, object message);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Error"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="message" or @name="exceptionObject"]' />
    void Error (object message, Exception exceptionObject);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="message"]' />
    void Error (object message);

    /// <overloads>Log a formatted string with the <see cref="LogLevel.Error"/> level.</overloads>
    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Error"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void ErrorFormat (int eventID, Exception exceptionObject, string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Error"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void ErrorFormat (int eventID, string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void ErrorFormat (string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Error"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void ErrorFormat (Exception exceptionObject, string format, params object[] args);

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Error"/> level, including the stack 
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormatWithEnum/remarks' />
    void ErrorFormat (Enum messageEnum, Exception exceptionObject, params object[] args);

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Error"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormatWithEnum/remarks' />
    void ErrorFormat (Enum messageEnum, params object[] args);


    /// <overloads>Log a message object with the <see cref="LogLevel.Fatal"/> level.</overloads>
    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Fatal"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="message" or @name="exceptionObject"]' />
    void Fatal (int eventID, object message, Exception exceptionObject);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Fatal"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="message"]' />
    void Fatal (int eventID, object message);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Fatal"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="message" or @name="exceptionObject"]' />
    void Fatal (object message, Exception exceptionObject);

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="message"]' />
    void Fatal (object message);

    /// <overloads>Log a formatted string with the <see cref="LogLevel.Fatal"/> level.</overloads>
    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Fatal"/> level and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID" or @name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void FatalFormat (int eventID, Exception exceptionObject, string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Fatal"/> level and <paramref name="eventID"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="eventID"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void FatalFormat (int eventID, string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void FatalFormat (string format, params object[] args);

    /// <summary>
    /// Log a formatted string with the <see cref="LogLevel.Fatal"/> level,
    /// including the stack trace of <paramref name="exceptionObject"/>. 
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    void FatalFormat (Exception exceptionObject, string format, params object[] args);

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Fatal"/> level, including the stack 
    /// trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormatWithEnum/remarks' />
    void FatalFormat (Enum messageEnum, Exception exceptionObject, params object[] args);

    /// <summary>
    /// Log a message and event id derived from the <paramref name="messageEnum"/> with the <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="messageEnum" or @name="args"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormatWithEnum/remarks' />
    void FatalFormat (Enum messageEnum, params object[] args);

    
    /// <summary>
    /// Checks if this logger is enabled for the <see cref="LogLevel.Debug"/> level.
    /// </summary>
    bool IsDebugEnabled { get; }

    /// <summary>
    /// Checks if this logger is enabled for the <see cref="LogLevel.Info"/> level.
    /// </summary>
    bool IsInfoEnabled { get; }

    /// <summary>
    /// Checks if this logger is enabled for the <see cref="LogLevel.Warn"/> level.
    /// </summary>
    bool IsWarnEnabled { get; }

    /// <summary>
    /// Checks if this logger is enabled for the <see cref="LogLevel.Error"/> level.
    /// </summary>
    bool IsErrorEnabled { get; }

    /// <summary>
    /// Checks if this logger is enabled for the <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    bool IsFatalEnabled { get; }
  }
}
