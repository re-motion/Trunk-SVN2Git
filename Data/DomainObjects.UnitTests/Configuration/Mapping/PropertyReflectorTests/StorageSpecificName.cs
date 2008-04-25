using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class StorageSpecificName: BaseTest
  {
    [Test]
    public void GetMetadata_WithNoAttribute()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassHavingStorageSpecificIdentifierAttribute> ("NoAttribute");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassHavingStorageSpecificIdentifierAttribute.NoAttribute",
          actual.PropertyName);
      Assert.AreEqual ("NoAttribute", actual.StorageSpecificName);
    }

    [Test]
    public void GetMetadata_WithStorageSpecificName()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassHavingStorageSpecificIdentifierAttribute> ("StorageSpecificName");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassHavingStorageSpecificIdentifierAttribute.StorageSpecificName",
          actual.PropertyName);
      Assert.AreEqual ("CustomName", actual.StorageSpecificName);
    }
  }
}