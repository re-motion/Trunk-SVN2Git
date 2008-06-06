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
using Remotion.Logging.BridgeInterfaces;

namespace Remotion.Logging.BridgeImplementations
{
  public class LogManagerImplementation : ILogManagerImplementation
  {
    // TODO: Get from config section
    private static readonly ILogManager s_current = new Log4NetLogManager ();

    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="name">The name of the logger to retrieve.</param>
    /// <returns>A logger for the <paramref name="name"/> specified.</returns>
    public ILog GetLogger (string name)
    {
      return s_current.GetLogger (name);
    }

    /// <summary>
    /// Gets or creates a logger.
    /// </summary>
    /// <param name="type">The full name of <paramref name="type"/> will be used as the name of the logger to retrieve.</param>
    /// <returns>A logger for the fully qualified name of the <paramref name="type"/> specified.</returns>
    public ILog GetLogger (Type type)
    {
      return s_current.GetLogger (type);
    }


    /// <summary>
    /// Initializes the current logging framework.
    /// </summary>
    public void Initialize ()
    {
      //TODO: Test once current ILogManager can be set through configuration.
      s_current.Initialize ();
    }

    /// <summary>
    /// Initializes the current logging framework to log to the console.
    /// </summary>
    public void InitializeConsole ()
    {
      s_current.InitializeConsole ();
    }
  }
}
