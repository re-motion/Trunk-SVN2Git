using System;
using System.IO;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  public class DockerFileManager : IDockerFileManager
  {
    private const string dockerfileName = "dockerfile";

    public IDockerFile Prepare (string absoluteWebApplicationPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("absoluteWebApplicationPath", absoluteWebApplicationPath);

      var dockerfileLocationFullPath = Path.Combine (absoluteWebApplicationPath, dockerfileName);

      if (File.Exists (dockerfileLocationFullPath))
        return new CustomDockerFile (dockerfileLocationFullPath);

      SaveDockerfileFromManifestResource (dockerfileLocationFullPath);

      return new PreparedDockerFile (dockerfileLocationFullPath);
    }

    private void SaveDockerfileFromManifestResource (string dockerfileLocationFullPath)
    {
      var aspNetDockerContainerWrapperType = typeof (AspNetDockerContainerWrapper);
      var assembly = aspNetDockerContainerWrapperType.Assembly;
      var resourceName = $"{aspNetDockerContainerWrapperType.Namespace}.{dockerfileName}";

      SaveManifestToFile (assembly, resourceName, dockerfileLocationFullPath);
    }

    private void SaveManifestToFile (Assembly assembly, string resourceName, string destinationPath)
    {
      using (var manifestResourceStream = assembly.GetManifestResourceStream (resourceName))
      using (var fileStream = File.Create (destinationPath))
      {
        Assertion.IsNotNull (manifestResourceStream, "'{0}' not found as embedded resource in '{1}'.", resourceName, assembly);
        
        manifestResourceStream.CopyTo (fileStream);
      }
    }
  }
}
