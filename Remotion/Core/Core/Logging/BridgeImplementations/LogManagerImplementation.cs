// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Logging.BridgeInterfaces;

namespace Remotion.Logging.BridgeImplementations
{
  public class LogManagerImplementation : ILogManagerImplementation
  {
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
      //TODO: Test.
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
