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
using System.IO;
using Microsoft.Win32;
using Remotion.FunctionalProgramming;

namespace Remotion.Development.UnitTesting.PEVerifyPathSources
{
  public class DotNetSdk20PEVerifyPathSource : PotentialPEVerifyPathSourceBase
  {
    public const string SdkRegistryKey = @"SOFTWARE\Microsoft\.NETFramework";
    public const string SdkRegistryValue = "sdkInstallRootv2.0";

    public override string GetLookupDiagnostics (PEVerifyVersion version)
    {
      if (version != PEVerifyVersion.DotNet2)
        return ".NET SDK 2.0: n/a";
      else
        return string.Format (".NET SDK 2.0: Registry: HKEY_LOCAL_MACHINE\\{0}\\{1}\\bin\\PEVerify.exe", SdkRegistryKey, SdkRegistryValue);
    }

    protected override string GetPotentialPEVerifyPath (PEVerifyVersion version)
    {
      if (version != PEVerifyVersion.DotNet2)
        return null;

      return Maybe
          .ForValue (Registry.LocalMachine.OpenSubKey (SdkRegistryKey, false))
          .Select (key => key.GetValue (SdkRegistryValue) as string)
          .Select (path => Path.Combine (path, "bin"))
          .Select (path => Path.Combine (path, "PEVerify.exe"))
          .ValueOrDefault();
    }
  }
}