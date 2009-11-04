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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting
{
  public static class PEVerifier
  {
    public const string c_sdkRegistryKey = @"SOFTWARE\Microsoft\.NETFramework";
    public const string c_sdkRegistryValue = "sdkInstallRootv2.0";

    public const string c_windowsSdkRegistryKey = @"SOFTWARE\Microsoft\Microsoft SDKs\Windows";
    public const string c_windowsSdkRegistryVersionValue = "CurrentVersion";
    public const string c_windowsSdkRegistryInstallationFolderValue = "InstallationFolder";

    private static string s_peVerifyPath = null;

    public static string DefaultPEVerifyPath
    {
      get { return Path.Combine (GetFrameworkSDKPath(), @"bin\PEVerify.exe"); }
    }

    public static string GetFrameworkSDKPath ()
    {
      string path;
      try
      {
        path = GetSDKPathFromRegistry();
        if (path == null)
          path = GetWindowsSDKPathFromRegistry();
      }
      catch (Exception ex)
      {
        throw CreateSdkNotFoundException (ex, ex.Message);
      }

      if (path == null)
        throw CreateSdkNotFoundException (null, "Registry key not found.");
      else
        return path;
    }

    private static string GetSDKPathFromRegistry ()
    {
      return (string) Registry.LocalMachine.OpenSubKey (c_sdkRegistryKey, false).GetValue (c_sdkRegistryValue);
    }

    private static string GetWindowsSDKPathFromRegistry ()
    {
      string windowsSDKVersion = (string) Registry.LocalMachine.OpenSubKey (c_windowsSdkRegistryKey, false).GetValue (c_windowsSdkRegistryVersionValue);
      string installationFolder = (string) Registry.LocalMachine.OpenSubKey (c_windowsSdkRegistryKey + "\\" + windowsSDKVersion, false).GetValue (c_windowsSdkRegistryInstallationFolderValue);
      return installationFolder;
    }

    private static Exception CreateSdkNotFoundException (Exception inner, string reason)
    {
      string message = string.Format ("Cannot retrieve framework SDK location from {0}\\{1} or {2}\\[{3}]\\{4}: {5}", 
        c_sdkRegistryKey, c_sdkRegistryValue, c_windowsSdkRegistryKey, c_windowsSdkRegistryVersionValue, c_windowsSdkRegistryInstallationFolderValue, reason);
      return new InvalidOperationException (message, inner);
    }

    public static string PEVerifyPath
    {
      get 
      { 
        if (s_peVerifyPath == null)
          s_peVerifyPath = DefaultPEVerifyPath;

        return s_peVerifyPath; 
      }
      set
      {
        ArgumentUtility.CheckNotNullOrEmpty ("value", value);
        s_peVerifyPath = value;
      }
    }

    public static void VerifyPEFile (string modulePath)
    {
      ArgumentUtility.CheckNotNull ("modulePath", modulePath);

      Process process = new Process();

      string verifierPath = PEVerifyPath;
      if (!File.Exists (verifierPath))
        throw new PEVerifyException ("PEVerify could not be found at path '" + verifierPath + "'.");
      else
      {
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = verifierPath;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
        process.StartInfo.Arguments = modulePath;
        process.Start();
        
        bool finished = process.WaitForExit (10000);
        if (!finished)
          process.Kill();

        string output = process.StandardOutput.ReadToEnd();
        Console.WriteLine ("PEVerify: " + process.ExitCode);

        if (!finished)
          throw new PEVerifyException ("PEVerify needed more than ten seconds to complete. Output was: " + output);

        if (process.ExitCode != 0)
        {
          Console.WriteLine (output);
          throw new PEVerifyException (process.ExitCode, output);
        }

      }
    }

    public static void VerifyPEFile (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      VerifyPEFile (assembly.ManifestModule.FullyQualifiedName);
    }
  }
}
