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
using System.Reflection;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class GetCustomAttributeFromMemberInfoTest
  {
    private PropertyInfo _basePropertyWithSingleAttribute;
    private PropertyInfo _derivedPropertyWithSingleAttribute;
    private PropertyInfo _derivedPropertyWithMultipleAttribute;
    private PropertyInfo _derivedProtectedProperty;

    [SetUp]
    public void SetUp ()
    {
      _basePropertyWithSingleAttribute = typeof (SampleClass).GetProperty ("PropertyWithSingleAttribute");
      _derivedPropertyWithSingleAttribute = typeof (DerivedSampleClass).GetProperty ("PropertyWithSingleAttribute");
      _derivedPropertyWithMultipleAttribute = typeof (DerivedSampleClass).GetProperty ("PropertyWithMultipleAttribute");
      _derivedProtectedProperty = typeof (DerivedSampleClass).GetProperty ("ProtectedPropertyWithAttribute",
          BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [Test]
    public void Test_FromBaseWithAttribute ()
    {
      InheritedAttribute attribute =
          (InheritedAttribute) AttributeUtility.GetCustomAttribute (_basePropertyWithSingleAttribute, typeof (InheritedAttribute), true);
      Assert.IsNotNull (attribute);
    }

    [Test]
    public void TestGeneric_FromBaseWithAttribute ()
    {
      InheritedAttribute attribute = AttributeUtility.GetCustomAttribute<InheritedAttribute> (_basePropertyWithSingleAttribute, true);
      Assert.IsNotNull (attribute);
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "Multiple custom attributes of the same type found.")]
    public void Test_FromOverrideWithAttribute_ExpectAmbigousMatch ()
    {
      AttributeUtility.GetCustomAttribute (_derivedPropertyWithMultipleAttribute, typeof (MultipleAttribute), true);
    }

    [Test]
    public void Test_FromBaseWithInterface ()
    {
      ICustomAttribute attribute = 
          (ICustomAttribute) AttributeUtility.GetCustomAttribute (_basePropertyWithSingleAttribute, typeof (ICustomAttribute), true);
      Assert.IsNotNull (attribute);
    }

    [Test]
    public void TestGeneric_FromBaseWithInterface ()
    {
      ICustomAttribute attribute = AttributeUtility.GetCustomAttribute<ICustomAttribute> (_basePropertyWithSingleAttribute, true);
      Assert.IsNotNull (attribute);
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "Multiple custom attributes of the same type found.")]
    public void Test_FromOverrideWithInterface_ExpectAmbigousMatch ()
    {
      AttributeUtility.GetCustomAttribute (_derivedPropertyWithMultipleAttribute, typeof (ICustomAttribute), true);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "The attribute type must be assignable to System.Attribute or an interface.\r\nParameter name: T")]
    public void TestGeneric_FromBaseWithInvalidType ()
    {
      AttributeUtility.GetCustomAttribute<object> (_basePropertyWithSingleAttribute, true);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = 
        "The attribute type must be assignable to System.Attribute or an interface.\r\nParameter name: attributeType")]
    public void Test_FromBaseWithInvalidType ()
    {
      AttributeUtility.GetCustomAttribute (_basePropertyWithSingleAttribute, typeof (object), true);
    }

    [Test]
    public void Test_FromOverrideWithAttribute ()
    {
      Assert.IsNotNull (AttributeUtility.GetCustomAttribute (_derivedPropertyWithSingleAttribute, typeof (InheritedAttribute), true));
    }

    [Test]
    [Ignore ("Not supported at the moment")]
    public void Test_FromProtectedOverrideWithAttribute ()
    {
      Assert.IsNotNull (AttributeUtility.GetCustomAttribute (_derivedProtectedProperty, typeof (InheritedAttribute), true));
    }

    [Test]
    public void Test_FromOverrideWithInterface ()
    {
      Assert.IsNotNull (AttributeUtility.GetCustomAttribute (_derivedPropertyWithSingleAttribute, typeof (ICustomAttribute), true));
    }

    [Test]
    public void Test_FromOverrideWithAttributeAndWithoutInherited ()
    {
      Assert.IsNull (AttributeUtility.GetCustomAttribute (_derivedPropertyWithSingleAttribute, typeof (InheritedAttribute), false));
    }

    [Test]
    public void Test_FromOverrideWithInterfaceAndWithoutInherited ()
    {
      Assert.IsNull (AttributeUtility.GetCustomAttribute (_derivedPropertyWithSingleAttribute, typeof (ICustomAttribute), false));
    }
  }
}
