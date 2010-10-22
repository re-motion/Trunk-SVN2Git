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
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class MappingConfigurationTest : TableInheritanceMappingTest
  {
    [Test]
    [ExpectedException (typeof (MappingException))]
    [Ignore("TODO 3413: use an MappingRelectorStub (Loader) an return class definition. Remove")]
    public void Validate ()
    {
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false);

      var nullTypeDiscoveryService = new NullTypeDiscoveryService();

      //IMappingLoader stub;
      //stub.Stub (stub => stub.GetClassDefinitions).Returns (new ClassDefinition[]{personClass})
      var mappingConfiguration = new MappingConfiguration (new MappingReflector (nullTypeDiscoveryService));
      mappingConfiguration.ClassDefinitions.Add (personClass);
      //mappingConfiguration.Validate ();
    }

    [Test]
    [Ignore("TODO 3423: Remove?")]
    public void SetCurrentValidates ()
    {
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false);

      var nullTypeDiscoveryService = new NullTypeDiscoveryService ();
      MappingConfiguration mappingConfiguration = new MappingConfiguration (new MappingReflector (nullTypeDiscoveryService));
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
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Neither class 'TI_Person' nor its base classes specify an entity name. Make "
      + "class 'Person' abstract or apply a DBTable attribute to it or one of its base classes.")]
    [Ignore( "TODO: Implement")]
    public void ConstructorValidates ()
    {
      //MappingConfiguration.CreateConfigurationFromFileBasedLoader("TableInheritanceMappingWithNonAbstractClassWithoutEntity.xml");
    }
  }
}
