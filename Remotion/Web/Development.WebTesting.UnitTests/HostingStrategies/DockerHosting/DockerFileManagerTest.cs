using System;
using System.IO;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.UnitTests.HostingStrategies.DockerHosting
{
  [TestFixture]
  public class DockerFileManagerTest : FileSystemTestBase
  {
    [Test]
    public void Prepare_WithCustomDockerFileInFileSystem_ShouldReturnTypeCustomDockerFile ()
    {
      var dockerfileManager = new DockerFileManager();
      File.Create (Path.Combine (TemporaryDirectory, "dockerfile")).Dispose();

      var dockerfile = dockerfileManager.Prepare (TemporaryDirectory);

      Assert.That (dockerfile, Is.InstanceOf (typeof (CustomDockerFile)));
    }

    [Test]
    public void Prepare_ShouldReturnTypePreparedDockerFileAndCreatePreparedDockerFile ()
    {
      var dockerfileManager = new DockerFileManager();

      var dockerfile = dockerfileManager.Prepare (TemporaryDirectory);

      Assert.That (dockerfile, Is.InstanceOf (typeof (PreparedDockerFile)));
      Assert.That (File.Exists (Path.Combine (TemporaryDirectory, "dockerfile")));
    }
  }
}
