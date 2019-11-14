using System;
using System.IO;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.UnitTests.HostingStrategies.DockerHosting
{
  [TestFixture]
  public class PreparedDockerFileTest : FileSystemTestBase
  {
    [Test]
    public void GetDockerFileFullPath_ShouldReturnDockerFileFullPath ()
    {
      var testPath = "TestPath";
      var customDockerFile = new PreparedDockerFile (testPath);

      Assert.That (customDockerFile.GetDockerFileFullPath(), Is.EqualTo (testPath));
    }

    [Test]
    public void Dispose_ShouldDeleteDockerFile ()
    {
      var dockerfileFullPath = Path.Combine (TemporaryDirectory, "dockerfile");

      File.Create (dockerfileFullPath).Dispose();

      var customDockerFile = new PreparedDockerFile (dockerfileFullPath);
      customDockerFile.Dispose();

      Assert.That (File.Exists (dockerfileFullPath), Is.False);
    }
  }
}
