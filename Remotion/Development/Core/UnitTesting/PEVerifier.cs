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
using System.Reflection;
using Remotion.Development.UnitTesting.PEVerifyPathSources;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting
{
  public class PEVerifier
  {
    public static PEVerifier CreateDefault ()
    {
      return
          new PEVerifier (
              new CompoundPEVerifyPathSource (
                  new DotNetSdk20PEVerifyPathSource(),
                  new WindowsSdk6PEVerifyPathSource(),
                  new WindowsSdk70aPEVerifyPathSource(),
                  new WindowsSdk71PEVerifyPathSource(),
                  new WindowsSdk80aPEVerifyPathSource()));
    }

    private readonly IPEVerifyPathSource _pathSource;

    public PEVerifier (IPEVerifyPathSource pathSource)
    {
      _pathSource = pathSource;
    }

    public string GetVerifierPath (PEVerifyVersion version)
    {
      string verifierPath = _pathSource.GetPEVerifyPath (version);
      if (verifierPath == null)
      {
        var message = string.Format (
            "PEVerify for version '{0}' could not be found. Locations searched:\r\n{1}",
            version,
            _pathSource.GetLookupDiagnostics (version));
        throw new PEVerifyException (message);
      }
      return verifierPath;
    }

    public PEVerifyVersion GetDefaultVerifierVersion ()
    {
      return Environment.Version.Major == 4 ? PEVerifyVersion.DotNet4 : PEVerifyVersion.DotNet2;
    }


    public void VerifyPEFile (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      VerifyPEFile (assembly.ManifestModule.FullyQualifiedName);
    }

    public void VerifyPEFile (Assembly assembly, PEVerifyVersion version)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      VerifyPEFile (assembly.ManifestModule.FullyQualifiedName, version);
    }

    public void VerifyPEFile (string modulePath)
    {
      ArgumentUtility.CheckNotNull ("modulePath", modulePath);

      var version = GetDefaultVerifierVersion();
      VerifyPEFile (modulePath, version);
    }

    public void VerifyPEFile (string modulePath, PEVerifyVersion version)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("modulePath", modulePath);

      var process = StartPEVerifyProcess (modulePath, version);

      string output = process.StandardOutput.ReadToEnd();
      process.WaitForExit();

      if (process.ExitCode != 0)
      {
        throw new PEVerifyException (process.ExitCode, output);
      }
    }

    private Process StartPEVerifyProcess (string modulePath, PEVerifyVersion version)
    {
      string verifierPath = GetVerifierPath(version);

      var process = new Process();
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.FileName = verifierPath;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
      process.StartInfo.Arguments = string.Format ("/verbose \"{0}\"", modulePath);
      process.Start();
      return process;
    }
  }
}