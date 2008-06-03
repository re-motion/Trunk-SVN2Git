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
using Remotion.Implementation;
using Remotion.Logging.BridgeInterfaces;

namespace Remotion.Logging
{
  /// <summary>
  /// Use this class to create a logger implementing <see cref="ILog"/> from the current <see cref="ILogManager"/>.
  /// </summary>
  /// <remarks>
  /// Currently only <b>log4net</b> is supported as logging infrastructure.
  /// </remarks>
  public static class LogManager
  {
    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="name">The name of the logger to retrieve.</param>
    /// <returns>A logger for the <paramref name="name"/> specified.</returns>
    public static ILog GetLogger (string name)
    {
      return VersionDependentImplementationBridge<ILogManagerImplementation>.Implementation.GetLogger (name);
    }

    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="type">The full name of <paramref name="type"/> will be used as the name of the logger to retrieve.</param>
    /// <returns>A logger for the fully qualified name of the <paramref name="type"/> specified.</returns>
    public static ILog GetLogger (Type type)
    {
      return VersionDependentImplementationBridge<ILogManagerImplementation>.Implementation.GetLogger (type);
    }


    /// <summary>
    /// Initializes the current logging framework.
    /// </summary>
    public static void Initialize ()
    {
      VersionDependentImplementationBridge<ILogManagerImplementation>.Implementation.Initialize ();
    }

    /// <summary>
    /// Initializes the current logging framework to log to the console.
    /// </summary>
    public static void InitializeConsole ()
    {
      VersionDependentImplementationBridge<ILogManagerImplementation>.Implementation.InitializeConsole ();
    }
  }
}
