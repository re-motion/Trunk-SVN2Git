using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting
{
  public static class PEVerifier
  {
    public const string DefaultPEVerifyPath = @"C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\PEVerify.exe";
    private static string s_peVerifyPath = DefaultPEVerifyPath;

    public static string PEVerifyPath
    {
      get { return s_peVerifyPath; }
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