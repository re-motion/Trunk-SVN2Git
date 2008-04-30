using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  [TestFixture]
  public class MappingReflectorTest: StandardMappingTest
  {
    [Test]
    public void GetResolveTypes()
    {
      IMappingLoader mappingReflector = new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (GetType().Assembly));
      Assert.IsTrue (mappingReflector.ResolveTypes);
    }

    [Test]
    public void GetClassDefinitions ()
    {
      MappingReflector mappingReflector = new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (GetType().Assembly));

      ClassDefinitionCollection actualClassDefinitions = mappingReflector.GetClassDefinitions();

      Assert.IsNotNull (actualClassDefinitions);
      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker();
      classDefinitionChecker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, false, true);
      Assert.IsFalse (actualClassDefinitions.Contains (typeof (TestDomainBase)));
    }

    [Test]
    public void GetRelationDefinitions()
    {
      MappingReflector mappingReflector = new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (GetType().Assembly));

      ClassDefinitionCollection actualClassDefinitions = mappingReflector.GetClassDefinitions();
      RelationDefinitionCollection actualRelationDefinitions = mappingReflector.GetRelationDefinitions (actualClassDefinitions);

      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker();
      classDefinitionChecker.Check (TestMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, false, true);

      RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check (TestMappingConfiguration.Current.RelationDefinitions, actualRelationDefinitions, true);
    }

    [Test]
    public void Get_WithDuplicateAssembly ()
    {
      Assembly assembly = GetType ().Assembly;
      MappingReflector expectedMappingReflector = new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (assembly));
      ClassDefinitionCollection expectedClassDefinitions = expectedMappingReflector.GetClassDefinitions ();
      RelationDefinitionCollection expectedRelationDefinitions = expectedMappingReflector.GetRelationDefinitions (expectedClassDefinitions);
      
      MappingReflector mappingReflector = new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (assembly, assembly));
      ClassDefinitionCollection actualClassDefinitions = mappingReflector.GetClassDefinitions ();

      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker ();
      classDefinitionChecker.Check (expectedClassDefinitions, actualClassDefinitions, false, false);

      RelationDefinitionCollection actualRelationDefinitions = mappingReflector.GetRelationDefinitions (actualClassDefinitions);
      RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker ();
      relationDefinitionChecker.Check (expectedRelationDefinitions, actualRelationDefinitions, false);
    }
  }
}