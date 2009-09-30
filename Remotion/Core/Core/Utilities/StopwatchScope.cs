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
using System.Diagnostics;
using System.IO;
using Remotion.Logging;

namespace Remotion.Utilities
{
  /// <summary>
  /// Provides a simple way of timing a piece of code wrapped into a <c>using</c> block. At the end of the block, an action is performed or a log 
  /// message written.
  /// </summary>
  public struct StopwatchScope : IDisposable
  {
    /// <summary>
    /// Creates a <see cref="StopwatchScope"/> that measures the time, executing the given
    /// <paramref name="action"/> when the scope is disposed.
    /// </summary>
    /// <param name="action">The <see cref="Action{T}"/> to receive the result.</param>
    /// <returns>
    /// A <see cref="StopwatchScope"/> that measures the time.
    /// </returns>
    public static StopwatchScope CreateScope (Action<TimeSpan> action)
    {
      return new StopwatchScope (action);
    }

    /// <summary>
    /// Creates a <see cref="StopwatchScope"/> that measures the time, writing the result to the given 
    /// <paramref name="writer"/> when the scope is disposed.
    /// </summary>
    /// <param name="writer">The writer to receive the result.</param>
    /// <param name="formatString">A string to format the result with. The string must contain a '{0}' placeholder, which is automatically replaced 
    /// by the elapsed time in the default format used by <see cref="TimeSpan.ToString"/>.</param>
    /// <returns>A <see cref="StopwatchScope"/> that measures the time in milliseconds.</returns>
    public static StopwatchScope CreateScope (TextWriter writer, string formatString)
    {
      return new StopwatchScope (ts => writer.WriteLine (formatString, ts.ToString ()));
    }

    /// <summary>
    /// Creates a <see cref="StopwatchScope"/> that measures the time in milliseconds, writing the result to the given <paramref name="writer"/>
    /// when the scope is disposed.
    /// </summary>
    /// <param name="writer">The writer to receive the result.</param>
    /// <param name="formatString">A string to format the result with. The string must contain a '{0}' placeholder, which is automatically replaced 
    /// by the elapsed milliseconds.</param>
    /// <returns>A <see cref="StopwatchScope"/> that measures the time in milliseconds.</returns>
    public static StopwatchScope CreateScopeForMilliseconds (TextWriter writer, string formatString)
    {
      return new StopwatchScope (ts => writer.WriteLine (formatString, ts.TotalMilliseconds));
    }

    /// <summary>
    /// Creates a <see cref="StopwatchScope"/> that measures the time, writing the result to the given
    /// <paramref name="log"/> when the scope is disposed.
    /// </summary>
    /// <param name="log">The <see cref="ILog"/> to receive the result.</param>
    /// <param name="logLevel">The log level to log the result with.</param>
    /// <param name="formatString">A string to format the result with. The string must contain a '{0}' placeholder, which is automatically replaced
    /// by the elapsed time in the default format used by <see cref="TimeSpan.ToString"/>.</param>
    /// <returns>
    /// A <see cref="StopwatchScope"/> that measures the time in milliseconds.
    /// </returns>
    public static StopwatchScope CreateScope (ILog log, LogLevel logLevel, string formatString)
    {
      return new StopwatchScope (ts => log.LogFormat (logLevel, formatString, ts.ToString ()));
    }

    /// <summary>
    /// Creates a <see cref="StopwatchScope"/> that measures the time in milliseconds, writing the result to the given <paramref name="log"/>
    /// when the scope is disposed.
    /// </summary>
    /// <param name="log">The <see cref="ILog"/> to receive the result.</param>
    /// <param name="logLevel">The log level to log the result with.</param>
    /// <param name="formatString">A string to format the result with. The string must contain a '{0}' placeholder, which is automatically replaced 
    /// by the elapsed milliseconds.</param>
    /// <returns>A <see cref="StopwatchScope"/> that measures the time in milliseconds.</returns>
    public static StopwatchScope CreateScopeForMilliseconds (ILog log, LogLevel logLevel, string formatString)
    {
      return new StopwatchScope (ts => log.LogFormat (logLevel, formatString, ts.TotalMilliseconds));
    }

    private readonly Stopwatch _stopwatch;
    private readonly Action<TimeSpan> _action;

    /// <summary>
    /// Initializes a new instance of the <see cref="StopwatchScope"/> structure. When the instance is disposed, <paramref name="action"/> is called
    /// with the time elapsed between initialization and disposal.
    /// </summary>
    /// <param name="action">The action.</param>
    private StopwatchScope (Action<TimeSpan> action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      _action = action;
      _stopwatch = Stopwatch.StartNew ();
    }

    /// <summary>
    /// Stops measuring the time and invokes the action defined to be called at the end of the measuring scope.
    /// </summary>
    public void Dispose ()
    {
      _stopwatch.Stop ();
      _action (Elapsed);
    }

    /// <summary>
    /// Gets the time elapsed since the construction of this <see cref="StopwatchScope"/> until now or the point of time where <see cref="Dispose"/> 
    /// was called, whichever occurs first.
    /// </summary>
    /// <value>The elapsed time.</value>
    public TimeSpan Elapsed
    {
      get { return _stopwatch.Elapsed; }
    }
  }
}