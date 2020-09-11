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
using System.IO;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure
{
  /// <summary>
  /// Provides useful helper methods for the download infrastructure.
  /// </summary>
  internal static class FileDownloadUtilities
  {
    /// <summary>
    /// Used to determine if a file is currently being locked by another process.
    /// </summary>
    public static bool IsFileLocked ([NotNull] string filePath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("filePath", filePath);

      try
      {
        using (var stream = File.Open (filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
          stream.Close();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine (ex);
        return true;
      }

      return false;
    }
  }
}