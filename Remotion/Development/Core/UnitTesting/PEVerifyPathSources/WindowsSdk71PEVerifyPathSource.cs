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
using System.IO;
using Microsoft.Win32;
using Remotion.FunctionalProgramming;

namespace Remotion.Development.UnitTesting.PEVerifyPathSources
{
  public class WindowsSdk71PEVerifyPathSource : PotentialPEVerifyPathSourceBase
  {
    public const string WindowsSdkRegistryKey35 = @"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDKNetFx35Tools";
    public const string WindowsSdkRegistryKey40 = @"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools";
    public const string WindowsSdkRegistryInstallationFolderValue = "InstallationFolder";

    public override string GetLookupDiagnostics (PEVerifyVersion version)
    {
      switch (version)
      {
        case PEVerifyVersion.DotNet2:
          return string.Format (
              "Windows SDK 7.1: Registry: HKEY_LOCAL_MACHINE\\{0}\\{1}\\PEVerify.exe",
              WindowsSdkRegistryKey35,
              WindowsSdkRegistryInstallationFolderValue);

        case PEVerifyVersion.DotNet4:
          return string.Format (
              "Windows SDK 7.1: Registry: HKEY_LOCAL_MACHINE\\{0}\\{1}\\PEVerify.exe",
              WindowsSdkRegistryKey40,
              WindowsSdkRegistryInstallationFolderValue);

        default:
          return "Windows SDK 7.1: n/a";
      }
    }

    protected override string GetPotentialPEVerifyPath (PEVerifyVersion version)
    {
      switch (version)
      {
        case PEVerifyVersion.DotNet2:
          return Maybe
              .ForValue (Registry.LocalMachine.OpenSubKey (WindowsSdkRegistryKey35, false))
              .Select (key => key.GetValue (WindowsSdkRegistryInstallationFolderValue) as string)
              .Select (path => Path.Combine (path, "PEVerify.exe"))
              .ValueOrDefault ();

        case PEVerifyVersion.DotNet4:
          return Maybe
              .ForValue (Registry.LocalMachine.OpenSubKey (WindowsSdkRegistryKey40, false))
              .Select (key => key.GetValue (WindowsSdkRegistryInstallationFolderValue) as string)
              .Select (path => Path.Combine (path, "PEVerify.exe"))
              .ValueOrDefault ();

        default:
          return null;
      }
    }

  }
}