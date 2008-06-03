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
  /// The <see cref="ILogManager"/> interface declares the methods available for retrieving a logger that implements
  /// <see cref="ILog"/> and initializing the respective logging framework.
  /// </summary>
  public interface ILogManager
  {
    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="name">The name of the logger to retrieve.</param>
    /// <returns>A logger for the <paramref name="name"/> specified.</returns>
    ILog GetLogger (string name);

    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="type">The full name of <paramref name="type"/> will be used as the name of the logger to retrieve.</param>
    /// <returns>A logger for the fully qualified name of the <paramref name="type"/> specified.</returns>
    ILog GetLogger (Type type);

    /// <summary>
    /// Initializes the logging framework abstracted through the <see cref="ILogManager"/> interface.
    /// </summary>
    void Initialize ();

    /// <summary>
    /// Initializes the logging framework to log to the console.
    /// </summary>
    void InitializeConsole ();
  }
}
