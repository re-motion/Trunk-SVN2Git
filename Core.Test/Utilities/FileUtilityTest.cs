using System;
using System.IO;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class FileUtilityTest
  {
    // types

    // static members and constants

    private const string c_testFileName = "FileUtilityTest_testfile.txt";

    // member fields

    // construction and disposing

    public FileUtilityTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      File.WriteAllText (c_testFileName, "File content");
    }

    [TearDown]
    public void TearDown ()
    {
      if (File.Exists (c_testFileName))
        File.Delete (c_testFileName);
    }

    [Test]
    public void MoveAndWaitForCompletionWithSameFileName ()
    {
      Assert.IsTrue (File.Exists (c_testFileName));

      FileUtility.MoveAndWaitForCompletion (c_testFileName, c_testFileName);

      Assert.IsTrue (File.Exists (c_testFileName));
    }

    [Test]
    public void MoveAndWaitForCompletionWithSameFile ()
    {
      Assert.IsTrue (File.Exists (c_testFileName));
      Assert.IsTrue (File.Exists (Path.GetFullPath (c_testFileName)));

      FileUtility.MoveAndWaitForCompletion (c_testFileName, Path.GetFullPath (c_testFileName));

      Assert.IsTrue (File.Exists (c_testFileName));
      Assert.IsTrue (File.Exists (Path.GetFullPath (c_testFileName)));
    }
  }
}
