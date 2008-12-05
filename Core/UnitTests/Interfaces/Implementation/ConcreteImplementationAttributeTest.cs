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
using Remotion.BridgeImplementations;
using Remotion.Development.UnitTesting;
using Remotion.Implementation;

namespace Remotion.UnitTests.Interfaces.Implementation
{
  [TestFixture]
  public class ConcreteImplementationAttributeTest
  {
    [SetUp]
    public void SetUp ()
    {
      FrameworkVersion.Reset();
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
      ConcreteImplementationAttribute attribute = new ConcreteImplementationAttribute ("Name, Version = <version>");
      Assert.AreEqual ("Name, Version = 2.4.6.8", attribute.GetTypeName());
    }

    [Test]
    public void GetTypeName_WithKeyToken ()
    {
      FrameworkVersion.Value = new Version (2, 4, 6, 8);
      ConcreteImplementationAttribute attribute = new ConcreteImplementationAttribute ("Name, Version = <version>, PublicKeyToken = <publicKeyToken>");
      Assert.AreEqual ("Name, Version = 2.4.6.8, PublicKeyToken = fee00910d6e5f53b", attribute.GetTypeName ());
    }

    [Test]
    public void ResolveType ()
    {
      FrameworkVersion.Value = typeof (ConcreteImplementationAttributeTest).Assembly.GetName().Version;
      ConcreteImplementationAttribute attribute = 
          new ConcreteImplementationAttribute ("Remotion.UnitTests.Interfaces.Implementation.ConcreteImplementationAttributeTest, "
          + "Remotion.UnitTests, Version = <version>");
      Assert.AreSame (typeof (ConcreteImplementationAttributeTest), attribute.ResolveType());
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException), ExpectedMessage = "Could not load type 'Badabing' from assembly 'Remotion.Interfaces, "
       + "Version=.*, Culture=neutral, PublicKeyToken=.*'.", MatchType = MessageMatch.Regex)]
    public void ResolveType_WithInvalidTypeName ()
    {
      FrameworkVersion.Value = typeof (ConcreteImplementationAttributeTest).Assembly.GetName ().Version;
      ConcreteImplementationAttribute attribute =
          new ConcreteImplementationAttribute ("Badabing");
      attribute.ResolveType ();
    }

    [Test]
    public void Instantiate ()
    {
      FrameworkVersion.Value = typeof (ConcreteImplementationAttributeTest).Assembly.GetName ().Version;
      ConcreteImplementationAttribute attribute =
          new ConcreteImplementationAttribute ("Remotion.UnitTests.Interfaces.Implementation.ConcreteImplementationAttributeTest, "
          + "Remotion.UnitTests, Version = <version>");
      object instance = attribute.InstantiateType();
      Assert.IsNotNull (instance);
      Assert.IsInstanceOfType(typeof (ConcreteImplementationAttributeTest), instance);
    }

    [Test]
    public void Instantiate_WithPublicKeyToken ()
    {
      FrameworkVersion.Value = typeof (AdapterRegistryImplementation).Assembly.GetName ().Version;
      ConcreteImplementationAttribute attribute =
          new ConcreteImplementationAttribute ("Remotion.BridgeImplementations.AdapterRegistryImplementation, Remotion, Version = <version>, "
          + "PublicKeyToken = <publicKeyToken>");
      object instance = attribute.InstantiateType ();
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
      ConcreteImplementationAttribute attribute = new ConcreteImplementationAttribute (
          "Remotion.UnitTests.Interfaces.Implementation.ConcreteImplementationAttributeTest+ClassWithoutDefaultConstructor, "
          + "Remotion.UnitTests, Version = <version>");
      attribute.InstantiateType ();
    }
  }
}
