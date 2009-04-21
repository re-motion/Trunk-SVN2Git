// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
