// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Diagnostics;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// <see cref="Process"/> utility methods.
  /// </summary>
  public static class ProcessUtils
  {
    /// <summary>
    /// Kills all processes with the given <paramref name="processName"/>. All exceptions are swallowed => this method is a best-effort approach.
    /// </summary>
    /// <param name="processName">The process name without the file extension.</param>
    public static void KillAllProcessesWithName ([NotNull] string processName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("processName", processName);

      foreach (var process in Process.GetProcessesByName (processName))
      {
        try
        {
          process.Kill();
        }
            // ReSharper disable once EmptyGeneralCatchClause
        catch
        {
          // Ignore, process is already closing or we do not have the required privileges anyway.
        }
      }
    }
  }
}