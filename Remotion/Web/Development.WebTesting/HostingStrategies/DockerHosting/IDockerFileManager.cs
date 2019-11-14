using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  public interface IDockerFileManager
  {
    IDockerFile Prepare ([NotNull] string absoluteWebApplicationPath);
  }
}
