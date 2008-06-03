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
using Remotion.Implementation;
using Remotion.Development.UnitTesting;

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
    public void GetPartialTypeName ()
    {
      FrameworkVersion.Value = new Version (2, 4, 6, 8);
      ConcreteImplementationAttribute attribute = new ConcreteImplementationAttribute ("Name, Version = <version>");
      Assert.AreEqual ("Name, Version = 2.4.6.8", attribute.GetPartialTypeName());
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
