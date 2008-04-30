using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class Common: BaseTest
  {
    [Test]
    public void GetMetadata_ForSingleProperty()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes> ("BooleanProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.IsInstanceOfType (typeof (ReflectionBasedPropertyDefinition), actual);
      Assert.IsNotNull (actual);
      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty", actual.PropertyName);
      Assert.AreEqual ("Boolean", actual.StorageSpecificName);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreSame (typeof (bool), actual.PropertyType);
    }
  }
}