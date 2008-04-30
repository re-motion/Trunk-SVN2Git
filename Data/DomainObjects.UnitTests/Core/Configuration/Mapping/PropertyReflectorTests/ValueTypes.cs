using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class ValueTypes: BaseTest
  {
    [Test]
    public void GetMetadata_WithBasicType()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> ("BooleanProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty", actual.PropertyName);
      Assert.AreSame (typeof (bool), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (false, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithNullableBasicType()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> ("NaBooleanProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty", actual.PropertyName);
      Assert.AreSame (typeof (bool?), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.IsNull (actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithEnumProperty()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> ("EnumProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty", actual.PropertyName);
      Assert.AreSame (typeof (ClassWithAllDataTypes.EnumType), actual.PropertyType);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, actual.DefaultValue);
    }

    [Test]
    public void GetMetadata_WithOptionalRelationProperty()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithGuidKey> ("ClassWithValidRelationsOptional");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional", actual.PropertyName);
      Assert.AreSame (typeof (ObjectID), actual.PropertyType);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual (null, actual.DefaultValue);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The property type System.Object is not supported.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.PropertyReflectorTests.ValueTypes, "
        + "property: ObjectProperty")]
    public void GetMetadata_WithInvalidPropertyType()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ValueTypes> ("ObjectProperty");

      propertyReflector.GetMetadata();
    }

    public object ObjectProperty
    {
      get { return null; }
      set { }
    }
  }
}