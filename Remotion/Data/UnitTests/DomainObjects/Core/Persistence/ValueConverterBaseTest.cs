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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence
{
  [TestFixture]
  public class ValueConverterBaseTest: StandardMappingTest
  {
    private StubValueConverterBase _stubValueConverterBase;
    private TypeConversionProvider _typeConversionProvider;
    
    private ClassDefinition _customerDefinition;
    private ClassDefinition _classWithAllDataTypesDefinition;

    public override void SetUp()
    {
      base.SetUp();

      _typeConversionProvider = TypeConversionProvider.Create();
      _stubValueConverterBase = new StubValueConverterBase (_typeConversionProvider);

      _customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
      _classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));
    }

    [Test]
    public void Initialize()
    {
      Assert.That (_typeConversionProvider, Is.SameAs (_typeConversionProvider));
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage = 
        "Invalid null value for not-nullable property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Type' encountered. "
        + "Class: 'Customer'.")]
    public void GetValue_NullForEnum()
    {
      PropertyDefinition enumProperty = _customerDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Type"];

      _stubValueConverterBase.GetValue (_customerDefinition, enumProperty, null);
    }

    [Test]
    public void GetValue_NullForNaDateTime()
    {
      PropertyDefinition dateTimeProperty = _customerDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.CustomerSince"];

      Assert.IsNull (_stubValueConverterBase.GetValue (_customerDefinition, dateTimeProperty, null));
    }

    [Test]
    public void GetObjectID_WithInt32Value()
    {
      var expectedID = new ObjectID ("Official", 1);
      ObjectID actualID = _stubValueConverterBase.GetObjectID (MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Official"), 1);

      Assert.AreEqual (expectedID, actualID);
    }

    [Test]
    public void GetObjectID_WithStringValue()
    {
      var expectedID = new ObjectID ("Official", "StringValue");
      ObjectID actualID = _stubValueConverterBase.GetObjectID (MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Official"), "StringValue");

      Assert.AreEqual (expectedID, actualID);
    }

    [Test]
    public void GetObjectID_WithGuidValue()
    {
      Guid value = Guid.NewGuid();
      var expectedID = new ObjectID ("Order", value);
      ObjectID actualID = _stubValueConverterBase.GetObjectID (MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Order"), value);

      Assert.AreEqual (expectedID, actualID);
    }

    [Test]
    public void GetValue_ForString()
    {
      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"],
              "abcdeföäü"),
          Is.EqualTo ("abcdeföäü"));

      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"],
              string.Empty),
          Is.Empty);

      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"],
              "abcdeföäü"),
          Is.EqualTo ("abcdeföäü"));

      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"],
              null),
          Is.Null);

      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"],
              string.Empty),
          Is.Empty);
    }

    [Test]
    public void GetValue_ForEnums_WithObject()
    {
      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"],
              ClassWithAllDataTypes.EnumType.Value1),
          Is.EqualTo (ClassWithAllDataTypes.EnumType.Value1));

      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"],
              null),
          Is.Null);
    }

    [Test]
    public void GetValue_ForEnums_WithString()
    {
      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"],
              "Value1"),
          Is.EqualTo (ClassWithAllDataTypes.EnumType.Value1));

      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"],
              string.Empty),
          Is.Null);

      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"],
              null),
          Is.Null);
    }

    [Test]
    public void GetValue_ForInt32_WithObject()
    {
      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"],
              2147483647),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              2147483647),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              null),
          Is.Null);
    }

    [Test]
    public void GetValue_ForInt32_WithString()
    {
      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"],
              "2147483647"),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              "2147483647"),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              string.Empty),
          Is.Null);

      Assert.That (
          _stubValueConverterBase.GetValue (
              _classWithAllDataTypesDefinition,
              _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              null),
          Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage =
       "Error converting the value for property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty' "
        + "of class 'ClassWithAllDataTypes' from persistence medium:\r\n"
        + "The value -1 is not supported for enumeration 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes+EnumType'.")]
    public void GetValue_InvalidEnumValue ()
    {
      PropertyDefinition enumProperty = _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"];

      _stubValueConverterBase.GetValue (_classWithAllDataTypesDefinition, enumProperty, -1);
    }

    [Test]
    public void GetValue_ForExtensibleEnum ()
    {
      PropertyDefinition propertyDefinition = _classWithAllDataTypesDefinition[typeof (ClassWithAllDataTypes).FullName + ".ExtensibleEnumProperty"];

      Assert.That (_stubValueConverterBase.GetValue (_classWithAllDataTypesDefinition, propertyDefinition, Color.Values.Red ().ID), Is.EqualTo (Color.Values.Red ()));
    }

    [Test]
    public void GetValue_ForExtensibleEnum_Null_Nullable ()
    {
      PropertyDefinition propertyDefinition = _classWithAllDataTypesDefinition[typeof (ClassWithAllDataTypes).FullName + ".ExtensibleEnumWithNullValueProperty"];

      Assert.That (_stubValueConverterBase.GetValue (_classWithAllDataTypesDefinition, propertyDefinition, null), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage =
        "Invalid null value for not-nullable property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty' "
        + "encountered. Class: 'ClassWithAllDataTypes'.")]
    public void GetValue_ForExtensibleEnum_Null_NonNullable ()
    {
      PropertyDefinition propertyDefinition = _classWithAllDataTypesDefinition[typeof (ClassWithAllDataTypes).FullName + ".ExtensibleEnumProperty"];

      _stubValueConverterBase.GetValue (_classWithAllDataTypesDefinition, propertyDefinition, null);
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage =
        "Error converting the value for property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty' of " 
        + "class 'ClassWithAllDataTypes' from persistence medium:\r\nThe extensible enum type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Color' "
        + "does not define a value called '?'.")]
    public void GetValue_ForExtensibleEnum_InvalidValue ()
    {
      PropertyDefinition propertyDefinition = _classWithAllDataTypesDefinition[typeof (ClassWithAllDataTypes).FullName + ".ExtensibleEnumProperty"];

      _stubValueConverterBase.GetValue (_classWithAllDataTypesDefinition, propertyDefinition, "?");
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage =
      "Error converting the value for property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property' "
       + "of class 'ClassWithAllDataTypes' from persistence medium:\r\n"
       + "Cannot convert value '1' to type 'System.Int32'.")]
    public void GetValue_InvalidValueType ()
    {
      PropertyDefinition int32Property = _classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"];

      _stubValueConverterBase.GetValue (_classWithAllDataTypesDefinition, int32Property, 1L);
    }
  }
}
