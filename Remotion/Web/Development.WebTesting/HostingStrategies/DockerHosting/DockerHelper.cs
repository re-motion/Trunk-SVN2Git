using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  /// <summary>
  /// The <see cref="DockerHelper"/> provides functionality to control Docker. It uses the command line to run the Docker commands.
  /// </summary>
  public class DockerHelper : IDockerHelper
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (DockerHelper));
    private readonly string _dockerExeFullPath;
    private readonly TimeSpan _waitForExitTimeout = TimeSpan.FromMilliseconds (100);
    private readonly TimeSpan _commandTimeout;
    private string _lastCommandLineOutputLine;

    public DockerHelper (TimeSpan commandTimeout)
    {
      _commandTimeout = commandTimeout;
      _dockerExeFullPath = GetDockerExeFullPath();
    }

    public void Pull ([NotNull] string dockerImageName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("dockerImageName", dockerImageName);

      var dockerPullCommand = $"pull {dockerImageName}";

      RunDockerCommand (dockerPullCommand);
    }

    public void Build ([NotNull] string tag, [NotNull] IReadOnlyDictionary<string, string> buildArgs, [NotNull] IDockerFile dockerFile)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("tag", tag);
      ArgumentUtility.CheckNotNull ("buildArgs", buildArgs);
      ArgumentUtility.CheckNotNull ("dockerFile", dockerFile);

      var buildContextPath = Path.GetDirectoryName (dockerFile.GetDockerFileFullPath());

      var buildDockerImageCommand =
          "build"
          + $" -t {tag}"
          + string.Join ("", buildArgs.Select (x => $" --build-arg {x.Key}={x.Value}"))
          + $" \"{buildContextPath}\"";

      RunDockerCommand (buildDockerImageCommand, buildContextPath);
    }

    public void Run (
        bool detached,
        bool removeContainer,
        [NotNull] IReadOnlyDictionary<int, int> publishedPorts,
        [NotNull] string containerName,
        [NotNull] string hostName,
        [NotNull] string imageName)
    {
      ArgumentUtility.CheckNotNull ("publishedPorts", publishedPorts);
      ArgumentUtility.CheckNotNullOrEmpty ("containerName", containerName);
      ArgumentUtility.CheckNotNullOrEmpty ("hostName", hostName);
      ArgumentUtility.CheckNotNullOrEmpty ("imageName", imageName);
      
      var iisHostWebSiteInDockerCommand =
          "run"
          + (detached ? " -d" : "")
          + (removeContainer ? " --rm" : "")
          + string.Join ("", publishedPorts.Select (x => $" -p {x.Key}:{x.Value}"))
          + $" --name {containerName}"
          + $" --hostname {hostName}"
          + $" {imageName}";

      RunDockerCommand (iisHostWebSiteInDockerCommand);

      var verifyContainerIsStartedCommand =
          "inspect"
          + " -f '{{.State.Running}}'"
          + $" {containerName}";

      while (_lastCommandLineOutputLine != "'true'")
        RunDockerCommand (verifyContainerIsStartedCommand);
    }

    public void Stop ([NotNull] string containerName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("containerName", containerName);

      var stopDockerContainerCommand = "stop" + $" {containerName}";
      RunDockerCommand (stopDockerContainerCommand);
    }

    public void RemoveImage ([NotNull] string imageName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("imageName", imageName);

      var removeDockerImageCommand = "image rm" + $" {imageName}";
      RunDockerCommand (removeDockerImageCommand);
    }

    private string GetDockerExeFullPath()
    {
      //Note: We explicitly do not use Environment.ProgramFiles, as it chooses ProgramFiles (x86) when we run our Unit Tests in a 32 Bit Context
      // As the Build Server is 64 bit, Docker cannot be found there under this circumstances.
      const string programFiles = "C:\\Program Files";

      var listOfKnownDockerLocations = new List<string>
                                       {
                                           Path.Combine (programFiles, "Docker", "docker.exe"),
                                           Path.Combine (programFiles, "Docker", "Docker", "Resources", "bin", "docker.exe")
                                       };

      var foundDockers = listOfKnownDockerLocations.Where (File.Exists).ToList();

      if (!foundDockers.Any())
        throw new FileNotFoundException (
            "Could not find Docker installed on this system. Checked paths: " +
            Environment.NewLine +
            string.Join (Environment.NewLine, listOfKnownDockerLocations.ToArray()));

      return foundDockers.First();
    }

    private void RunDockerCommand (string dockerCommand, string workingDirectory = null)
    {
      var startInfo = new ProcessStartInfo
                      {
                          WindowStyle = ProcessWindowStyle.Hidden,
                          ErrorDialog = true,
                          LoadUserProfile = true,
                          CreateNoWindow = false,
                          UseShellExecute = false,
                          RedirectStandardOutput = true,
                          RedirectStandardError = true
                      };
      
      if (!String.IsNullOrEmpty (workingDirectory))
        startInfo.WorkingDirectory = workingDirectory;
      
      startInfo.FileName = _dockerExeFullPath;
      startInfo.Arguments = dockerCommand;

      var dockerProcess = new Process { StartInfo = startInfo };
      dockerProcess.Start();

      dockerProcess.OutputDataReceived += (sender, outputLine) =>
      {
        if (outputLine.Data != null)
        {
          s_log.Info (outputLine.Data);
          _lastCommandLineOutputLine = outputLine.Data;
        }
      };

      dockerProcess.BeginOutputReadLine();

      WaitForExit (dockerProcess, dockerCommand);

      dockerProcess.Dispose();
    }

    private void WaitForExit (Process dockerProcess, string dockerCommand)
    {
      var stopwatch = Stopwatch.StartNew();

      while (!dockerProcess.HasExited)
      {
        dockerProcess.WaitForExit (_waitForExitTimeout.Milliseconds);

        if (stopwatch.ElapsedMilliseconds > _commandTimeout.TotalMilliseconds)
          throw new InvalidOperationException ($"Docker command '{dockerCommand}' ran longer than the configured timeout of '{_waitForExitTimeout}'. Abort.");
      }

      if (dockerProcess.ExitCode != 0)
      {
        var error = dockerProcess.StandardError.ReadToEnd(); 
        throw new InvalidOperationException ($"Docker command '{dockerCommand}' failed: {error}");
      }
    }
  }
}