// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
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
