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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Factories;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class MappingConfigurationTest : TableInheritanceMappingTest
  {
    [Test]
    [ExpectedException (typeof (MappingException))]
    public void Validate ()
    {
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false);

      MappingConfiguration mappingConfiguration = 
          new MappingConfiguration (new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (TestDomainFactory.ConfigurationMappingTestDomainEmpty)));
      mappingConfiguration.ClassDefinitions.Add (personClass);
      mappingConfiguration.Validate ();
    }

    [Test]
    public void SetCurrentValidates ()
    {
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false);

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
