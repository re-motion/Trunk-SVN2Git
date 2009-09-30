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

namespace Remotion.Logging
{
  /// <summary>
  /// Provides extension methods used for logging.
  /// </summary>
  public static class LogExtensions
  {
    /// <summary>
    /// Logs the given value and returns it to the caller. This is typically used to log a value returned by a method directly in the return 
    /// statement.
    /// </summary>
    /// <typeparam name="T">The (inferred) type of the value to be logged.</typeparam>
    /// <param name="value">The value to be logged.</param>
    /// <param name="log">The <see cref="ILog"/> to log the value with.</param>
    /// <param name="logLevel">The <see cref="LogLevel"/> to log the value at. If the <paramref name="log"/> does not support this level, the 
    /// <paramref name="messageCreator"/> is not called.</param>
    /// <param name="messageCreator">A function object building the message to be logged.</param>
    /// <returns>The <paramref name="value"/> passed in to the method.</returns>
    public static T LogAndReturn<T> (this T value, ILog log, LogLevel logLevel, Func<T, string> messageCreator)
    {
      if (log.IsEnabled (logLevel))
      {
        log.Log (logLevel, messageCreator (value));
      }
      return value;
    }
  }
}