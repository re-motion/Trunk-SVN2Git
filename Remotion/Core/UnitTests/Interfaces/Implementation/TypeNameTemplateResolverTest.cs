// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Linq;
using NUnit.Framework;
using Remotion.Implementation;
using Remotion.Text;

namespace Remotion.UnitTests.Interfaces.Implementation
{
  [TestFixture]
  public class TypeNameTemplateResolverTest
  {
    [SetUp]
    public void SetUp ()
    {
      FrameworkVersion.Reset ();
    }

    [TearDown]
    public void TearDown ()
    {
      FrameworkVersion.Reset ();
    }

    [Test]
    public void GetTypeName ()
    {
      FrameworkVersion.Value = new Version (2, 4, 6, 8);
      Assert.AreEqual ("Name, Version = 2.4.6.8", TypeNameTemplateResolver.ResolveToTypeName ("Name, Version = <version>"));
    }

    [Test]
    public void GetTypeName_WithKeyToken ()
    {
      FrameworkVersion.Value = new Version (2, 4, 6, 8);
      const string typeName = "Name, Version = <version>, PublicKeyToken = <publicKeyToken>";
      var token = typeof (FrameworkVersion).Assembly.GetName().GetPublicKeyToken();
      var expectedTokenString = SeparatedStringBuilder.Build ("", token.Select (b => b.ToString ("x2")));
      Assert.AreEqual ("Name, Version = 2.4.6.8, PublicKeyToken = " + expectedTokenString, TypeNameTemplateResolver.ResolveToTypeName (typeName));
    }

    [Test]
    public void ResolveType ()
    {
      FrameworkVersion.Value = typeof (ConcreteImplementationAttributeTest).Assembly.GetName ().Version;
      const string typeName = "Remotion.UnitTests.Interfaces.Implementation.ConcreteImplementationAttributeTest, Remotion.UnitTests, Version = <version>";
      Assert.AreSame (typeof (ConcreteImplementationAttributeTest), TypeNameTemplateResolver.ResolveToType (typeName));
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException), ExpectedMessage = "Could not load type 'Badabing' from assembly 'Remotion.Interfaces, "
       + "Version=.*, Culture=neutral, PublicKeyToken=.*'.", MatchType = MessageMatch.Regex)]
    public void ResolveType_WithInvalidTypeName ()
    {
      FrameworkVersion.Value = typeof (ConcreteImplementationAttributeTest).Assembly.GetName ().Version;
      TypeNameTemplateResolver.ResolveToType ("Badabing");
    }
    
  }
}