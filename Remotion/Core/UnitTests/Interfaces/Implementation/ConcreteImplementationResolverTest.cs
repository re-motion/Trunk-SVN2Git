// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.BridgeImplementations;
using Remotion.Development.UnitTesting;
using Remotion.Implementation;

namespace Remotion.UnitTests.Interfaces.Implementation
{
  [TestFixture]
  public class ConcreteImplementationResolverTest
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
      Assert.AreEqual ("Name, Version = 2.4.6.8", ConcreteImplementationResolver.GetTypeName ("Name, Version = <version>"));
    }

    [Test]
    public void GetTypeName_WithKeyToken ()
    {
      FrameworkVersion.Value = new Version (2, 4, 6, 8);
      const string typeName = "Name, Version = <version>, PublicKeyToken = <publicKeyToken>";
      Assert.AreEqual ("Name, Version = 2.4.6.8, PublicKeyToken = fee00910d6e5f53b", ConcreteImplementationResolver.GetTypeName (typeName));
    }

    [Test]
    public void ResolveType ()
    {
      FrameworkVersion.Value = typeof (ConcreteImplementationAttributeTest).Assembly.GetName ().Version;
      const string typeName = "Remotion.UnitTests.Interfaces.Implementation.ConcreteImplementationAttributeTest, Remotion.UnitTests, Version = <version>";
      Assert.AreSame (typeof (ConcreteImplementationAttributeTest), ConcreteImplementationResolver.ResolveType (typeName));
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException), ExpectedMessage = "Could not load type 'Badabing' from assembly 'Remotion.Interfaces, "
       + "Version=.*, Culture=neutral, PublicKeyToken=.*'.", MatchType = MessageMatch.Regex)]
    public void ResolveType_WithInvalidTypeName ()
    {
      FrameworkVersion.Value = typeof (ConcreteImplementationAttributeTest).Assembly.GetName ().Version;
      ConcreteImplementationResolver.ResolveType ("Badabing");
    }

    [Test]
    public void Instantiate ()
    {
      FrameworkVersion.Value = typeof (ConcreteImplementationAttributeTest).Assembly.GetName ().Version;
      const string typeName = "Remotion.UnitTests.Interfaces.Implementation.ConcreteImplementationAttributeTest, Remotion.UnitTests, Version = <version>";
      object instance = ConcreteImplementationResolver.InstantiateType (typeName);
      Assert.IsNotNull (instance);
      Assert.IsInstanceOfType (typeof (ConcreteImplementationAttributeTest), instance);
    }

    [Test]
    public void Instantiate_WithPublicKeyToken ()
    {
      FrameworkVersion.Value = typeof (AdapterRegistryImplementation).Assembly.GetName ().Version;
      const string typeName = "Remotion.BridgeImplementations.AdapterRegistryImplementation, Remotion, Version = <version>, PublicKeyToken = <publicKeyToken>";
      object instance = ConcreteImplementationResolver.InstantiateType (typeName);
      Assert.IsNotNull (instance);
      Assert.IsInstanceOfType (typeof (AdapterRegistryImplementation), instance);
    }

    public class ClassWithoutDefaultConstructor
    {
      public ClassWithoutDefaultConstructor (int i)
      {
        Dev.Null = i;
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "No parameterless constructor defined for this object.")]
    public void Instantiate_WithoutDefaultConstructor ()
    {
      FrameworkVersion.Value = typeof (ConcreteImplementationAttributeTest).Assembly.GetName ().Version;
      const string typeName = "Remotion.UnitTests.Interfaces.Implementation.ConcreteImplementationResolverTest+ClassWithoutDefaultConstructor, "
                              + "Remotion.UnitTests, Version = <version>";
      ConcreteImplementationResolver.InstantiateType (typeName);
    }
  }
}