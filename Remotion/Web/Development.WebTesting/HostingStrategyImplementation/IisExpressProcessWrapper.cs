using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategyImplementation
{
  /// <summary>
  /// Wraps an IIS Express instance hosting the web application under test.
  /// </summary>
  public class IisExpressProcessWrapper : IDisposable
  {
    private readonly Process _iisProcess;

    /// <summary>
    /// Initializes the wrapper, does not yet run the IIS Express process.
    /// </summary>
    /// <param name="webApplicationPath">Absolute file path to the web application under test.</param>
    /// <param name="webApplicationPort">Port to be used when hosting the web application.</param>
    public IisExpressProcessWrapper ([NotNull] string webApplicationPath, int webApplicationPort)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("webApplicationPath", webApplicationPath);

      var startInfo = new ProcessStartInfo
                      {
                          WindowStyle = ProcessWindowStyle.Minimized,
                          ErrorDialog = true,
                          LoadUserProfile = true,
                          CreateNoWindow = false,
                          UseShellExecute = false
                      };

      var programFilesPath = GetProgramFilesPath (startInfo);
      var iisExpressExecutablePath = Path.Combine (programFilesPath, "IIS Express", "iisexpress.exe");

      startInfo.FileName = iisExpressExecutablePath;
      startInfo.Arguments = string.Format ("/path:\"{0}\" /port:\"{1}\"", webApplicationPath, webApplicationPort);

      _iisProcess = new Process { StartInfo = startInfo };
    }

    /// <summary>
    /// Starts the IIS Express process and thereby hosts the web application.
    /// </summary>
    public void Run ()
    {
      _iisProcess.Start();
      _iisProcess.WaitForExit();
    }

    /// <summary>
    /// Stops the IIS Express process and thereby "unhosts" the web application.
    /// </summary>
    public void Dispose ()
    {
      if (_iisProcess != null && !_iisProcess.HasExited)
      {
        _iisProcess.CloseMainWindow();
        _iisProcess.Dispose();
      }
    }

    private static string GetProgramFilesPath (ProcessStartInfo startInfo)
    {
      return string.IsNullOrEmpty (startInfo.EnvironmentVariables["ProgramFiles"])
          ? startInfo.EnvironmentVariables["ProgramFiles(x86)"]
          : startInfo.EnvironmentVariables["ProgramFiles"];
    }
  }
}