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
using Remotion.UnitTests.Utilities.AttributeUtilityTests.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class GetCustomAttributesFromMemberInfoTest
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
    public void TestGeneric_FromBaseWithAttribute ()
    {
      InheritedAttribute[] attributes = AttributeUtility.GetCustomAttributes<InheritedAttribute> (_basePropertyWithSingleAttribute, true);

      Assert.AreEqual (1, attributes.Length);
      Assert.IsNotNull (attributes[0]);
    }

    [Test]
    public void Test_FromBaseWithAttribute ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (_basePropertyWithSingleAttribute, typeof (InheritedAttribute), true);

      Assert.AreEqual (1, attributes.Length);
      Assert.IsNotNull (attributes[0]);
      Assert.IsInstanceOfType (typeof (InheritedAttribute), attributes[0]);
    }

    [Test]
    public void TestGeneric_FromBaseWithInterface ()
    {
      ICustomAttribute[] attributes = AttributeUtility.GetCustomAttributes<ICustomAttribute> (_basePropertyWithSingleAttribute, true);

      Assert.AreEqual (1, attributes.Length);
      Assert.IsNotNull (attributes[0]);
    }

    [Test]
    public void Test_FromBaseWithInterface ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (_basePropertyWithSingleAttribute, typeof (ICustomAttribute), true);

      Assert.AreEqual (1, attributes.Length);
      Assert.IsNotNull (attributes[0]);
      Assert.IsInstanceOfType (typeof (ICustomAttribute), attributes[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "The attribute type must be assignable to System.Attribute or an interface.\r\nParameter name: T")]
    public void TestGeneric_FromBaseWithInvalidType ()
    {
      AttributeUtility.GetCustomAttributes<object> (_basePropertyWithSingleAttribute, true);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "The attribute type must be assignable to System.Attribute or an interface.\r\nParameter name: attributeType")]
    public void Test_FromBaseWithInvalidType ()
    {
      AttributeUtility.GetCustomAttributes (_basePropertyWithSingleAttribute, typeof (object), true);
    }

    [Test]
    public void Test_FromOverrideWithAttribute ()
    {
      InheritedAttribute[] attributes = AttributeUtility.GetCustomAttributes<InheritedAttribute> (_derivedPropertyWithSingleAttribute, true);

      Assert.AreEqual (1, attributes.Length);
      Assert.IsNotNull (attributes[0]);
    }

    [Test]
    [Ignore ("Not supported at the moment")]
    public void Test_FromProtectedOverrideWithAttribute ()
    {
      InheritedAttribute[] attributes = AttributeUtility.GetCustomAttributes<InheritedAttribute> (_derivedProtectedProperty, true);

      Assert.AreEqual (1, attributes.Length);
      Assert.IsNotNull (attributes[0]);
    }

    [Test]
    public void Test_FromOverrideWithInterface ()
    {
      ICustomAttribute[] attributes = AttributeUtility.GetCustomAttributes<ICustomAttribute> (_derivedPropertyWithSingleAttribute, true);

      Assert.AreEqual (1, attributes.Length);
      Assert.IsNotNull (attributes[0]);
    }

    [Test]
    public void Test_FromOverrideWithAttributeAndMultiple ()
    {
      MultipleAttribute[] attributes = AttributeUtility.GetCustomAttributes<MultipleAttribute> (_derivedPropertyWithMultipleAttribute, true);

      Assert.AreEqual (2, attributes.Length);
      Assert.IsNotNull (attributes[0]);
      Assert.IsNotNull (attributes[1]);
    }

    [Test]
    public void Test_FromOverrideWithInterfaceAndMultiple ()
    {
      ICustomAttribute[] attributes = AttributeUtility.GetCustomAttributes<ICustomAttribute> (_derivedPropertyWithMultipleAttribute, true);

      Assert.AreEqual (2, attributes.Length);
      Assert.IsNotNull (attributes[0]);
      Assert.IsNotNull (attributes[1]);
    }

    [Test]
    public void Test_FromOverrideWithAttributeAndWithoutInherited ()
    {
      InheritedAttribute[] attributes = AttributeUtility.GetCustomAttributes<InheritedAttribute> (_derivedPropertyWithSingleAttribute, false);

      Assert.IsEmpty (attributes);
    }

    [Test]
    public void Test_FromOverrideWithInterfaceAndWithoutInherited ()
    {
      ICustomAttribute[] attributes = AttributeUtility.GetCustomAttributes<ICustomAttribute> (_derivedPropertyWithSingleAttribute, false);

      Assert.IsEmpty (attributes);
    }
  }
}
