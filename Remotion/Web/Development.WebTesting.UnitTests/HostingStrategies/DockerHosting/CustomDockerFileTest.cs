using System;
using System.IO;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.UnitTests.HostingStrategies.DockerHosting
{
  [TestFixture]
  public class CustomDockerFileTest : FileSystemTestBase
  {
    [Test]
    public void GetDockerFileFullPath_ShouldReturnDockerFileFullPath ()
    {
      var testPath = "TestPath";
      var customDockerFile = new CustomDockerFile (testPath);

      Assert.That (customDockerFile.GetDockerFileFullPath(), Is.EqualTo (testPath));
    }

    [Test]
    public void Dispose_ShouldNotDeleteDockerFile ()
    {
      var dockerfileFullPath = Path.Combine (TemporaryDirectory, "dockerfile");

      File.Create (dockerfileFullPath).Dispose();

      var customDockerFile = new CustomDockerFile (dockerfileFullPath);
      customDockerFile.Dispose();

      Assert.That (File.Exists (dockerfileFullPath), Is.True);
    }
  }
}