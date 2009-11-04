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

    public override void SetUp()
    {
      base.SetUp();

      _typeConversionProvider = TypeConversionProvider.Create();
      _stubValueConverterBase = new StubValueConverterBase (_typeConversionProvider);
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
    public void GetNullValueForEnum()
    {
      ClassDefinition customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
      PropertyDefinition enumProperty = customerDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Type"];

      _stubValueConverterBase.GetValue (customerDefinition, enumProperty, null);
    }

    [Test]
    public void GetNullValueForNaDateTime()
    {
      ClassDefinition customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
      PropertyDefinition dateTimeProperty = customerDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.CustomerSince"];

      Assert.IsNull (_stubValueConverterBase.GetValue (customerDefinition, dateTimeProperty, null));
    }

    [Test]
    public void GetObjectIDWithInt32Value()
    {
      ObjectID expectedID = new ObjectID ("Official", 1);
      ObjectID actualID = _stubValueConverterBase.GetObjectID (MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Official"), 1);

      Assert.AreEqual (expectedID, actualID);
    }

    [Test]
    public void GetObjectIDWithStringValue()
    {
      ObjectID expectedID = new ObjectID ("Official", "StringValue");
      ObjectID actualID = _stubValueConverterBase.GetObjectID (MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Official"), "StringValue");

      Assert.AreEqual (expectedID, actualID);
    }

    [Test]
    public void GetObjectIDWithGuidValue()
    {
      Guid value = Guid.NewGuid();
      ObjectID expectedID = new ObjectID ("Order", value);
      ObjectID actualID = _stubValueConverterBase.GetObjectID (MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Order"), value);

      Assert.AreEqual (expectedID, actualID);
    }

    [Test]
    public void GetValue_ForString()
    {
      ClassDefinition classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"],
              "abcdeföäü"),
          Is.EqualTo ("abcdeföäü"));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"],
              string.Empty),
          Is.Empty);

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"],
              "abcdeföäü"),
          Is.EqualTo ("abcdeföäü"));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"],
              null),
          Is.Null);

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"],
              string.Empty),
          Is.Empty);
    }

    [Test]
    public void GetValue_ForEnums_WithObject()
    {
      ClassDefinition classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"],
              ClassWithAllDataTypes.EnumType.Value1),
          Is.EqualTo (ClassWithAllDataTypes.EnumType.Value1));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"],
              null),
          Is.Null);
    }

    [Test]
    public void GetValue_ForEnums_WithString()
    {
      ClassDefinition classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"],
              "Value1"),
          Is.EqualTo (ClassWithAllDataTypes.EnumType.Value1));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"],
              string.Empty),
          Is.Null);

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaEnumProperty"],
              null),
          Is.Null);
    }

    [Test]
    public void GetValue_ForInt32_WithObject()
    {
      ClassDefinition classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"],
              2147483647),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              2147483647),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              null),
          Is.Null);
    }

    [Test]
    public void GetValue_ForInt32_WithString()
    {
      ClassDefinition classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"],
              "2147483647"),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              "2147483647"),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              string.Empty),
          Is.Null);

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              null),
          Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage =
       "Error converting the value for property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty' "
        + "of class 'ClassWithAllDataTypes' from persistence medium:\r\n"
        + "The value -1 is not supported for enumeration 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes+EnumType'.")]
    public void GetInvalidEnumValue ()
    {
      ClassDefinition classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));
      PropertyDefinition enumProperty = classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty"];

      _stubValueConverterBase.GetValue (classWithAllDataTypesDefinition, enumProperty, -1);
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage =
      "Error converting the value for property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property' "
       + "of class 'ClassWithAllDataTypes' from persistence medium:\r\n"
       + "Cannot convert value '1' to type 'System.Int32'.")]
    public void GetInvalidValueType ()
    {
      ClassDefinition classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));
      PropertyDefinition int32Property = classWithAllDataTypesDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.Int32Property"];

      _stubValueConverterBase.GetValue (classWithAllDataTypesDefinition, int32Property, 1L);
    }
  }
}
