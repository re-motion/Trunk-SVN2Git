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
using System.Reflection;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using Remotion.Utilities;

namespace Remotion.Logging
{
  /// <summary>
  /// Implementation of <see cref="ILogManager"/> for <b>log4net</b>.
  /// </summary>
  public class Log4NetLogManager : ILogManager
  {
    /// <summary>
    /// Creates a new instance of the <see cref="Log4NetLog"/> type.
    /// </summary>
    /// <param name="name">The name of the logger to retrieve. Must not be <see langword="null"/> or empty.</param>
    /// <returns>A <see cref="Log4NetLog"/> for the <paramref name="name"/> specified.</returns>
    public ILog GetLogger (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);

      return new Log4NetLog (LoggerManager.GetLogger (Assembly.GetCallingAssembly (), name));
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Log4NetLog"/> type.
    /// </summary>
    /// <param name="type">The full name of <paramref name="type"/> will be used as the name of the logger to retrieve. Must not be <see langword="null"/>.</param>
    /// <returns>A <see cref="Log4NetLog"/> for the fully qualified name of the <paramref name="type"/> specified.</returns>
    public ILog GetLogger (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return new Log4NetLog (LoggerManager.GetLogger (Assembly.GetCallingAssembly (), type));
    }

    /// <summary>
    /// Initializes <b>log4net</b> by invoking <see cref="XmlConfigurator.Configure()"/>.
    /// </summary>
    public void Initialize ()
    {
      //TODO: Check if there is a sensible way for testing log4net startup.
      XmlConfigurator.Configure ();
    }

    public void InitializeConsole ()
    {
      ConsoleAppender appender = new ConsoleAppender ();
      appender.Layout = new PatternLayout ("%-5level: %message%newline");
      BasicConfigurator.Configure (appender);
    }
  }
}
