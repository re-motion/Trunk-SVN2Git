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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.TextWriterFactory
{
  [TestFixture]
  public class TextWriterFactoryBaseTest
  {
    [Test]
    public void AppendExtensionTest ()
    {
      const string name = "huizilipochtli";
      const string extension = "ext";
      Assert.That(TextWriterFactoryBase.AppendExtension (name, extension),Is.EqualTo(name + "." + extension));
      Assert.That (TextWriterFactoryBase.AppendExtension (name, null), Is.EqualTo (name));
      Assert.That (TextWriterFactoryBase.AppendExtension (name, ""), Is.EqualTo (name));
    }
    

    [Test]
    public void GetRelativePathTest ()
    {
      var mocks = new MockRepository ();
      var textWriterFactoryBaseMock = mocks.PartialMock<TextWriterFactoryBase> ();
      textWriterFactoryBaseMock.Expect (x => x.TextWriterExists ("yang")).Return (true);
      textWriterFactoryBaseMock.Replay();
      textWriterFactoryBaseMock.Extension = "dat";
      var result = textWriterFactoryBaseMock.GetRelativePath ("yin", "yang");
      Assert.That (result, Is.EqualTo (@".\yang.dat"));
      textWriterFactoryBaseMock.VerifyAllExpectations ();
    }


    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = @"No TextWriter with name ""yang"" registered => no relative path exists.")]
    public void GetRelativePathNoEntryWithNameExistsTest ()
    {
      var mocks = new MockRepository ();
      var textWriterFactoryBaseMock = mocks.PartialMock<TextWriterFactoryBase> ();

      textWriterFactoryBaseMock.Expect (x => x.TextWriterExists ("yang")).Return (false);
      textWriterFactoryBaseMock.Replay ();
      textWriterFactoryBaseMock.GetRelativePath ("yin", "yang");
      textWriterFactoryBaseMock.VerifyAllExpectations();
    }   

  }
}
