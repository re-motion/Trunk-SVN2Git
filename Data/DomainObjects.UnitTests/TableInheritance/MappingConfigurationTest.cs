using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;
using Remotion.Mixins.Context;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  public class MappingConfigurationTest : TableInheritanceMappingTest
  {
    [Test]
    [ExpectedException (typeof (MappingException))]
    public void Validate ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false, new List<Type> ());

      MappingConfiguration mappingConfiguration = 
          new MappingConfiguration (new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (TestDomainFactory.ConfigurationMappingTestDomainEmpty)));
      mappingConfiguration.ClassDefinitions.Add (personClass);
      mappingConfiguration.Validate ();
    }

    [Test]
    public void SetCurrentValidates ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false, new List<Type> ());

      MappingConfiguration mappingConfiguration = 
          new MappingConfiguration (new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (TestDomainFactory.ConfigurationMappingTestDomainEmpty)));
      mappingConfiguration.ClassDefinitions.Add (personClass);

      try
      {
        MappingConfiguration.SetCurrent (mappingConfiguration);
        Assert.Fail ("ArgumentException was expected.");
      }
      catch (ArgumentException ex)
      {
        Assert.AreNotSame (mappingConfiguration, MappingConfiguration.Current);
        Assert.IsInstanceOfType (typeof (MappingException), ex.InnerException);

        string expectedMessage = string.Format (
            "The specified MappingConfiguration is invalid due to the following reason: '{0}'.\r\nParameter name: mappingConfiguration",
            ex.InnerException.Message);
      }
    }

    [Test]
    public void TableInheritanceMapping ()
    {
      MappingConfiguration mappingConfiguration = new MappingConfiguration (new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (GetType().Assembly)));
      ClassDefinition domainBaseClass = mappingConfiguration.ClassDefinitions.GetMandatory (typeof (DomainBase));
      Assert.IsNull (domainBaseClass.MyEntityName);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Type 'Person' must be abstract, because neither class 'TI_Person' nor its base classes specify an entity name.")]
    [Ignore( "TODO: Implement")]
    public void ConstructorValidates ()
    {
      //MappingConfiguration.CreateConfigurationFromFileBasedLoader("TableInheritanceMappingWithNonAbstractClassWithoutEntity.xml");
    }
  }
}
