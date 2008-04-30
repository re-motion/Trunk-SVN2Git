using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence
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
        "Invalid null value for not-nullable property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Type' encountered. "
        + "Class: 'Customer'.")]
    public void GetNullValueForEnum()
    {
      ClassDefinition customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
      PropertyDefinition enumProperty = customerDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Type"];

      _stubValueConverterBase.GetValue (customerDefinition, enumProperty, null);
    }

    [Test]
    public void GetNullValueForNaDateTime()
    {
      ClassDefinition customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
      PropertyDefinition dateTimeProperty = customerDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.CustomerSince"];

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
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"],
              "abcdeföäü"),
          Is.EqualTo ("abcdeföäü"));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"],
              string.Empty),
          Is.Empty);

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"],
              "abcdeföäü"),
          Is.EqualTo ("abcdeföäü"));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"],
              null),
          Is.Null);

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"],
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
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"],
              ClassWithAllDataTypes.EnumType.Value1),
          Is.EqualTo (ClassWithAllDataTypes.EnumType.Value1));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaEnumProperty"],
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
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"],
              "Value1"),
          Is.EqualTo (ClassWithAllDataTypes.EnumType.Value1));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaEnumProperty"],
              string.Empty),
          Is.Null);

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaEnumProperty"],
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
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"],
              2147483647),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              2147483647),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
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
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"],
              "2147483647"),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              "2147483647"),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              string.Empty),
          Is.Null);

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              null),
          Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage =
       "Error converting the value for property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty' "
        + "of class 'ClassWithAllDataTypes' from persistence medium:\r\n"
        + "The value -1 is not supported for enumeration 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes+EnumType'.")]
    public void GetInvalidEnumValue ()
    {
      ClassDefinition classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));
      PropertyDefinition enumProperty = classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"];

      _stubValueConverterBase.GetValue (classWithAllDataTypesDefinition, enumProperty, -1);
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage =
      "Error converting the value for property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property' "
       + "of class 'ClassWithAllDataTypes' from persistence medium:\r\n"
       + "Cannot convert value '1' to type 'System.Int32'.")]
    public void GetInvalidValueType ()
    {
      ClassDefinition classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));
      PropertyDefinition int32Property = classWithAllDataTypesDefinition["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"];

      _stubValueConverterBase.GetValue (classWithAllDataTypesDefinition, int32Property, 1L);
    }
  }
}