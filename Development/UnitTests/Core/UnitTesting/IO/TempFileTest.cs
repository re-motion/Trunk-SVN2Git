/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.IO;

namespace Remotion.Development.UnitTests.Core.UnitTesting.IO
{
  [TestFixture]
  public class TempFileTest
  {
    [Test]
    public void Initialize()
    {
      using (TempFile tempFile = new TempFile())
      {
        Assert.That (tempFile.FileName, Is.Not.Empty);
        Assert.That (File.Exists (tempFile.FileName), Is.True);
      }
    }

    [Test]
    public void Dispose()
    {
      TempFile tempFile = new TempFile();
      string fileName = tempFile.FileName;
      tempFile.Dispose();
      Assert.That (File.Exists (fileName), Is.False);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Object disposed.")]
    public void GetFileNameAfterDispose_Throws()
    {
      TempFile tempFile = new TempFile();
      tempFile.Dispose();
      Dev.Null = tempFile.FileName;
    }

    [Test]
    public void WriteStream()
    {
      using (TempFile tempFile = new TempFile())
      {
        string expectedText = "Some\r\nText";
        byte[] buffer = Encoding.ASCII.GetBytes (expectedText);
        using (Stream stream = new MemoryStream (buffer.Length))
        {
          stream.Write (buffer, 0, buffer.Length);
          stream.Position = 0;
          tempFile.WriteStream (stream);
        }

        string actual = File.ReadAllText (tempFile.FileName);
        Assert.That (actual, Is.EqualTo (expectedText));
      }
    }

    [Test]
    public void WriteStream_WithContentLongerThanBuffer()
    {
      using (TempFile tempFile = new TempFile())
      {
        string expectedText = "0123456789\r\n0123456789\r\n0123456789\r\n0123456789\r\n0123456789\r\n"
                              + "0123456789\r\n0123456789\r\n0123456789\r\n0123456789\r\n0123456789\r\n"
                              + "0123456789\r\n0123456789\r\n0123456789\r\n0123456789\r\n0123456789\r\n";

        byte[] buffer = Encoding.ASCII.GetBytes (expectedText);
        using (Stream stream = new MemoryStream (buffer.Length))
        {
          stream.Write (buffer, 0, buffer.Length);
          stream.Position = 0;
          tempFile.WriteStream (stream);
        }

        string actual = File.ReadAllText (tempFile.FileName);
        Assert.That (actual, Is.EqualTo (expectedText));
      }
    }

    [Test]
    public void WriteAllBytes()
    {
      using (TempFile tempFile = new TempFile())
      {
        string expectedText = "Some\r\nText";
        tempFile.WriteAllBytes (Encoding.ASCII.GetBytes (expectedText));

        string actual = File.ReadAllText (tempFile.FileName);
        Assert.That (actual, Is.EqualTo (expectedText));
      }
    }
  }
}
