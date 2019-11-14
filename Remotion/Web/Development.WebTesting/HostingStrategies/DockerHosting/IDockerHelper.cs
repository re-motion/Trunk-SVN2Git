using System;
using System.Collections.Generic;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  public interface IDockerHelper
  {
    void Pull (string dockerImageName);
    void Build (string tag, IReadOnlyDictionary<string, string> buildArgs, IDockerFile dockerFile);
    void Run (bool detached, bool removeContainer, IReadOnlyDictionary<int, int> publishedPorts, string containerName, string hostName, string imageName);
    void Stop (string containerName);
    void RemoveImage (string imageName);
  }
}
