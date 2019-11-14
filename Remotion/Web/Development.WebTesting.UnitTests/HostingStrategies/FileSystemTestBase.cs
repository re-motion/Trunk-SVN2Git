using System;
using System.IO;
using NUnit.Framework;

namespace Remotion.Web.Development.WebTesting.UnitTests.HostingStrategies
{
  public class FileSystemTestBase
  {
    
    protected string TemporaryDirectory;

    [SetUp]
    public void SetUp ()
    {
      TemporaryDirectory = Path.Combine (Path.GetTempPath(), Path.GetRandomFileName());
      Directory.CreateDirectory (TemporaryDirectory);
    }

    [TearDown]
    public void TearDown ()
    {
      Directory.Delete (TemporaryDirectory, true);
    }
  }
}
