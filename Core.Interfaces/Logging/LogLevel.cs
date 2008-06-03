/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.Logging
{
  /// <summary>
  /// Defines the log levels available when logging through the <see cref="ILog"/> interface.
  /// </summary>
  public enum LogLevel
  {
    /// <summary>
    /// The <see cref="LogLevel.Debug"/> level designates fine-grained informational events that are most useful to debug an application.
    /// </summary>
    Debug = 0,

    /// <summary>
    /// The <see cref="LogLevel.Info"/> level designates informational messages that highlight the progress of the application at coarse-grained level. 
    /// </summary>
    Info,

    /// <summary>
    /// The <see cref="LogLevel.Warn"/> level designates potentially harmful situations.
    /// </summary>
    Warn,

    /// <summary>
    /// The <see cref="LogLevel.Error"/> level designates error events that might still allow the application to continue running. 
    /// </summary>
    Error,

    /// <summary>
    /// The <see cref="LogLevel.Fatal"/> level designates very severe error events that will presumably lead the application to abort.
    /// </summary>
    Fatal
  }
}
