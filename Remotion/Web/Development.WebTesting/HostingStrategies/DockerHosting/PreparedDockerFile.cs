using System;
using System.IO;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  public class PreparedDockerFile : IDockerFile
  {
    private readonly string _dockerFileLocationFullPath;

    public PreparedDockerFile ([NotNull] string dockerFileLocationFullPath)
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
      File.Delete (_dockerFileLocationFullPath);
    }
  }
}
