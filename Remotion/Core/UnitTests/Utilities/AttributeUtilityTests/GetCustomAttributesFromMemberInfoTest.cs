// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Reflection;
using NUnit.Framework;
using Remotion.UnitTests.Utilities.AttributeUtilityTests.TestDomain;
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

      Assert.That (attributes.Length, Is.EqualTo (1));
      Assert.That (attributes[0], Is.Not.Null);
    }

    [Test]
    public void Test_FromBaseWithAttribute ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (_basePropertyWithSingleAttribute, typeof (InheritedAttribute), true);

      Assert.That (attributes.Length, Is.EqualTo (1));
      Assert.That (attributes[0], Is.Not.Null);
      Assert.IsInstanceOf (typeof (InheritedAttribute), attributes[0]);
    }

    [Test]
    public void TestGeneric_FromBaseWithInterface ()
    {
      ICustomAttribute[] attributes = AttributeUtility.GetCustomAttributes<ICustomAttribute> (_basePropertyWithSingleAttribute, true);

      Assert.That (attributes.Length, Is.EqualTo (1));
      Assert.That (attributes[0], Is.Not.Null);
    }

    [Test]
    public void Test_FromBaseWithInterface ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (_basePropertyWithSingleAttribute, typeof (ICustomAttribute), true);

      Assert.That (attributes.Length, Is.EqualTo (1));
      Assert.That (attributes[0], Is.Not.Null);
      Assert.IsInstanceOf (typeof (ICustomAttribute), attributes[0]);
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

      Assert.That (attributes.Length, Is.EqualTo (1));
      Assert.That (attributes[0], Is.Not.Null);
    }

    [Test]
    [Ignore ("Not supported at the moment by Attribute.GetCustomAttribute - should we leave this or add a workaround?")]
    public void Test_FromProtectedOverrideWithAttribute ()
    {
      InheritedAttribute[] attributes = AttributeUtility.GetCustomAttributes<InheritedAttribute> (_derivedProtectedProperty, true);

      Assert.That (attributes.Length, Is.EqualTo (1));
      Assert.That (attributes[0], Is.Not.Null);
    }

    [Test]
    public void Test_FromOverrideWithInterface ()
    {
      ICustomAttribute[] attributes = AttributeUtility.GetCustomAttributes<ICustomAttribute> (_derivedPropertyWithSingleAttribute, true);

      Assert.That (attributes.Length, Is.EqualTo (1));
      Assert.That (attributes[0], Is.Not.Null);
    }

    [Test]
    public void Test_FromOverrideWithAttributeAndMultiple ()
    {
      MultipleAttribute[] attributes = AttributeUtility.GetCustomAttributes<MultipleAttribute> (_derivedPropertyWithMultipleAttribute, true);

      Assert.That (attributes.Length, Is.EqualTo (2));
      Assert.That (attributes[0], Is.Not.Null);
      Assert.That (attributes[1], Is.Not.Null);
    }

    [Test]
    public void Test_FromOverrideWithInterfaceAndMultiple ()
    {
      ICustomAttribute[] attributes = AttributeUtility.GetCustomAttributes<ICustomAttribute> (_derivedPropertyWithMultipleAttribute, true);

      Assert.That (attributes.Length, Is.EqualTo (2));
      Assert.That (attributes[0], Is.Not.Null);
      Assert.That (attributes[1], Is.Not.Null);
    }

    [Test]
    public void Test_FromOverrideWithAttributeAndWithoutInherited ()
    {
      InheritedAttribute[] attributes = AttributeUtility.GetCustomAttributes<InheritedAttribute> (_derivedPropertyWithSingleAttribute, false);

      Assert.That (attributes, Is.Empty);
    }

    [Test]
    public void Test_FromOverrideWithInterfaceAndWithoutInherited ()
    {
      ICustomAttribute[] attributes = AttributeUtility.GetCustomAttributes<ICustomAttribute> (_derivedPropertyWithSingleAttribute, false);

      Assert.That (attributes, Is.Empty);
    }

    [Test]
    public void Test_ReturnSpecificArrayType ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (_basePropertyWithSingleAttribute, typeof (BaseInheritedAttribute), false);
      Assert.That (attributes, Is.InstanceOf (typeof (BaseInheritedAttribute[])));
    }

    [Test]
    public void Test_ReturnsNewInstance ()
    {
      var attribute1 = AttributeUtility.GetCustomAttributes (_basePropertyWithSingleAttribute, typeof (InheritedAttribute), false).Single();
      var attribute2 = AttributeUtility.GetCustomAttributes (_basePropertyWithSingleAttribute, typeof (InheritedAttribute), false).Single();

      Assert.That (attribute1, Is.Not.SameAs (attribute2));
    }
  }
}
