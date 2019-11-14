using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  public class CustomDockerFile : IDockerFile
  {
    private readonly string _dockerFileLocationFullPath;

    public CustomDockerFile ([NotNull] string dockerFileLocationFullPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("dockerFileLocationFullPath", dockerFileLocationFullPath);

      _dockerFileLocationFullPath = dockerFileLocationFullPath;
    }

    public string GetDockerFileFullPath ()
    {
      return _dockerFileLocationFullPath;
    }

    public void Dispose ()
    {
      // A custom Dockerfile does not have to be disposed up by our infrastructure
    }
  }
}
