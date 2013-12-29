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
using JetBrains.Annotations;

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
    void Log (LogLevel logLevel, int? eventID, object message, Exception exceptionObject);

    /// <overloads>Log a formatted string with the specified <paramref name="logLevel"/>.</overloads>
    /// <summary>
    /// Log a formatted string with the specified <paramref name="logLevel"/> and <paramref name="eventID"/>,
    /// including the stack trace of <paramref name="exceptionObject"/>.
    /// </summary>
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/Log/param[@name="logLevel" or @name="eventID" or @name="exceptionObject"]' />
    /// <include file='doc\include\Logging\ILog.xml' path='ILog/LogFormat/param[@name="format" or @name="args"]' />
    [StringFormatMethod ("format")]
    void LogFormat (LogLevel logLevel, int? eventID, Exception exceptionObject, string format, params object[] args);






    
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

    /// <summary>
    /// Checks if this logger is enabled for the given <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="logLevel">The log level to check for.</param>
    /// <returns>
    ///   <see langword="true"/> if the specified log level is enabled; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsEnabled (LogLevel logLevel);
  }
}
