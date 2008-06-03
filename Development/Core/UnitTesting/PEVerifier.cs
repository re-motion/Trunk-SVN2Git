/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
    private static string s_peVerifyPath = null;

    public static string DefaultPEVerifyPath
    {
      get { return Path.Combine (FrameworkSDKPath, @"bin\PEVerify.exe"); }
    }

    public static string FrameworkSDKPath
    {
      get
      {
        try
        {
          return (string) Registry.LocalMachine.OpenSubKey (c_sdkRegistryKey, false).GetValue (c_sdkRegistryValue);
        }
        catch (Exception ex)
        {
          string message = string.Format ("Cannot retrieve framework SDK location from {0}\\{1}: {2}", 
              c_sdkRegistryKey, c_sdkRegistryValue, ex.Message);
          throw new InvalidOperationException (message, ex);
        }
      }
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
          throw new PEVerifyException (process.ExitCode, output);

      }
    }

    public static void VerifyPEFile (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      VerifyPEFile (assembly.ManifestModule.FullyQualifiedName);
    }
  }
}
