using System;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  public interface IDockerFile : IDisposable
  {
    string GetDockerFileFullPath();
  }
}