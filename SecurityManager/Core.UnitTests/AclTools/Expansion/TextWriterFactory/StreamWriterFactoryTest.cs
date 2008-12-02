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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.TextWriterFactory
{
  [TestFixture]
  public class StreamWriterFactoryTest
  {
    // TODO AE: Non-TDD code? More fine-grained unit tests needed for StreamWriterFactory and TextWriterFactoryBase.
    [Test]
    public void DirectoryAndExtensionSettingTest ()
    {
      var streamWriterFactory = new StreamWriterFactory ();
      string directory = Path.Combine (Path.GetTempPath (), "StreamWriterFactoryTest_DirectoryTest");
      streamWriterFactory.Directory = directory;
      const string extension = "xyz";
      streamWriterFactory.Extension = extension;
      const string fileName = "someFile";
      using (StreamWriter streamWriter = (StreamWriter) streamWriterFactory.NewTextWriter (fileName))
      {
        var fileStream = (FileStream) streamWriter.BaseStream;
        //To.ConsoleLine.e (fileStream.Name);
        string filePathExpected = Path.Combine (directory, fileName + "." + extension);
        Assert.That (fileStream.Name, Is.EqualTo (filePathExpected));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Directory must not be null. Set using \"Directory\"-property before calling \"NewTextWriter\"")]
    public void NewTextWriterWithNullDirectoryThrowsTest ()
    {
      var streamWriterFactory = new StreamWriterFactory ();
      streamWriterFactory.Directory = null;
      streamWriterFactory.NewTextWriter ("whatever");
    }


  }
}