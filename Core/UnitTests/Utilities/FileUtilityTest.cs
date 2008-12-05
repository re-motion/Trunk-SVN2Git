// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.Utilities;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;

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

    [Test]
    public void CopyStream_WriteAllAtOnce ()
    {
      MockRepository mockRepository = new MockRepository();
      Stream inputMock = mockRepository.StrictMock<Stream>();

      Func<byte[], int, int, int> writeAllAtOnce = delegate (byte[] buffer, int offset, int count)
      {
        for (int i = 0; i < 10; ++i)
          buffer[offset + i] = (byte) i;
        return 10;
      };

      using (mockRepository.Ordered ())
      {
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (Mocks_Is.Anything(), Mocks_Is.Equal (0), Mocks_Is.Equal (FileUtility.CopyBufferSize))
            .Do (writeAllAtOnce);
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments ()
            .Constraints (Mocks_Is.Anything (), Mocks_Is.Equal (0), Mocks_Is.Equal (FileUtility.CopyBufferSize))
            .Return (0);
      }

      MemoryStream outputStream = new MemoryStream();
      mockRepository.ReplayAll ();

      FileUtility.CopyStream (inputMock, outputStream);
      Assert.That (outputStream.ToArray(), Is.EqualTo (new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));

      mockRepository.VerifyAll ();
    }

    [Test]
    public void CopyStream_WriteInSteps ()
    {
      MockRepository mockRepository = new MockRepository ();
      Stream inputMock = mockRepository.StrictMock<Stream> ();
      SetupResult.For (inputMock.Length).Return (10L);
      Func<byte[], int, int, int> writeStep1 = delegate (byte[] buffer, int offset, int count)
      {
        for (int i = 0; i < 5; ++i)
          buffer[offset + i] = (byte) i;
        return 5;
      };

      Func<byte[], int, int, int> writeStep2 = delegate (byte[] buffer, int offset, int count)
      {
        for (int i = 0; i < 3; ++i)
          buffer[offset + i] = (byte) i;
        return 3;
      };

      Func<byte[], int, int, int> writeStep3 = delegate (byte[] buffer, int offset, int count)
      {
        for (int i = 0; i < 2; ++i)
          buffer[offset + i] = (byte) i;
        return 2;
      };

      using (mockRepository.Ordered ())
      {
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments ()
            .Constraints (Mocks_Is.Anything (), Mocks_Is.Equal (0), Mocks_Is.Equal (FileUtility.CopyBufferSize))
            .Do (writeStep1);
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments ()
            .Constraints (Mocks_Is.Anything (), Mocks_Is.Equal (0), Mocks_Is.Equal (FileUtility.CopyBufferSize))
            .Do (writeStep2);
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments ()
            .Constraints (Mocks_Is.Anything (), Mocks_Is.Equal (0), Mocks_Is.Equal (FileUtility.CopyBufferSize))
            .Do (writeStep3);
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments ()
            .Constraints (Mocks_Is.Anything (), Mocks_Is.Equal (0), Mocks_Is.Equal (FileUtility.CopyBufferSize))
            .Return (0);
      }

      MemoryStream outputStream = new MemoryStream ();
      mockRepository.ReplayAll ();

      FileUtility.CopyStream (inputMock, outputStream);
      Assert.That (outputStream.ToArray (), Is.EqualTo (new byte[] { 0, 1, 2, 3, 4, 0, 1, 2, 0, 1 }));

      mockRepository.VerifyAll ();
    }



    [Test]
    public void WriteEmbeddedStringResourceToFileTest ()
    {
      FileUtility.WriteEmbeddedStringResourceToFile (GetType (), "TestData.WriteEmbeddedStringResourceToFileTestData.txt", c_testFileName);
      Assert.That (File.Exists (c_testFileName));
      string result = File.ReadAllText (c_testFileName);
      Assert.That (result, Is.EqualTo ("Hat der alte Hexenmeister sich doch einmal fortbegeben und nun sollen seine Geister..."));
    }

  }
}
