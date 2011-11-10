// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.IO;
using Microsoft.Win32;
using Remotion.FunctionalProgramming;

namespace Remotion.Development.UnitTesting.PEVerifyPathSources
{
  public class WindowsSdk6PEVerifyPathSource : PotentialPEVerifyPathSourceBase
  {
    public const string WindowsSdkRegistryKey = @"SOFTWARE\Microsoft\Microsoft SDKs\Windows";
    public const string WindowsSdkRegistryVersionValue = "CurrentVersion";
    public const string WindowsSdkRegistryInstallationFolderValue = "InstallationFolder";

    public override string GetLookupDiagnostics (PEVerifyVersion version)
    {
      if (version != PEVerifyVersion.DotNet2)
        return "Windows SDK 6: n/a";
      else
      {
        return string.Format (
            "Windows SDK 6: Registry: HKEY_LOCAL_MACHINE\\{0}\\[CurrentVersion]\\{1}\\bin\\PEVerify.exe",
            WindowsSdkRegistryKey,
            WindowsSdkRegistryInstallationFolderValue);
      }
    }

    protected override string GetPotentialPEVerifyPath (PEVerifyVersion version)
    {
      if (version != PEVerifyVersion.DotNet2)
        return null;

      return Maybe
          .ForValue (Registry.LocalMachine.OpenSubKey (WindowsSdkRegistryKey, false))
          .Select (key => key.GetValue (WindowsSdkRegistryVersionValue) as string)
          .Select (windowsSdkVersion => Registry.LocalMachine.OpenSubKey (WindowsSdkRegistryKey + "\\" + windowsSdkVersion, false))
          .Select (key => key.GetValue (WindowsSdkRegistryInstallationFolderValue) as string)
          .Select (path => Path.Combine (path, "bin"))
          .Select (path => Path.Combine (path, "PEVerify.exe"))
          .ValueOrDefault();
    }
  }
}