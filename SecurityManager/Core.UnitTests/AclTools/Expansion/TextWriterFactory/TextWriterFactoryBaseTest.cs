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
      //var mocks = new MockRepository();
      //var textWriterFactoryBaseMock = mocks.PartialMock<TextWriterFactoryBase>();
      //textWriterFactoryBaseMock.AppendExtension()

      const string name = "huizilipochtli";
      const string extension = "ext";
      Assert.That(TextWriterFactoryBase.AppendExtension (name, extension),Is.EqualTo(name + "." + extension));
      Assert.That (TextWriterFactoryBase.AppendExtension (name, null), Is.EqualTo (name));
      Assert.That (TextWriterFactoryBase.AppendExtension (name, ""), Is.EqualTo (name));
    }
  }
}