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
using System.Collections.Generic;
using System.Text;
using log4net;
using Remotion.Utilities;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using System.Configuration;

namespace Remotion.Data.DomainObjects.Oracle.CodeGenerator
{
  public static class LogUtility
  {
    // types

    // static members and constants

    private static ILog s_logger;

    // construction and disposing

    // static methods and properties

    private static ILog Logger
    {
      get
      {
        lock (typeof (LogUtility))
        {
          if (s_logger == null)
          {
            if (ConfigurationManager.GetSection ("log4net") != null)
            {
              XmlConfigurator.Configure ();
            }
            else
            {
              ConsoleAppender consoleAppender = new ConsoleAppender ();
              consoleAppender.Threshold = Level.Warn;
              consoleAppender.Name = "ConsoleAppender";
              consoleAppender.Layout = new PatternLayout ("%m\r\n\r\n");
              BasicConfigurator.Configure (consoleAppender);
            }
            s_logger = LogManager.GetLogger ("Remotion.Data.DomainObjects.Oracle.CodeGenerator");
          }
        }

        return s_logger;
      }
    }

    public static void LogError (string message, Exception exception)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);
      ArgumentUtility.CheckNotNull ("exception", exception);

      Logger.Error (message, exception);
    }

    public static void LogWarning (string message)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);

      Logger.Warn (message);
    }

    public static void LogWarning (string message, params object[] args)
    {
      LogWarning (string.Format (message, args));
    }

    public static void Shutdown ()
    {
      s_logger = null;
      LogManager.Shutdown ();
    }
  }
}
