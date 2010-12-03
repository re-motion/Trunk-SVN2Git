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
using System.ComponentModel.Design;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Relations;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class MappingReflectorTest : MappingReflectionTestBase
  {


    [Test]
    public void GetResolveTypes ()
    {
      IMappingLoader mappingReflector = new MappingReflector (TestMappingConfiguration.GetTypeDiscoveryService());
      Assert.IsTrue (mappingReflector.ResolveTypes);
    }

    [Test]
    public void GetRelationDefinitions ()
    {
      MappingReflector mappingReflector = new MappingReflector (TestMappingConfiguration.GetTypeDiscoveryService());

      var actualClassDefinitions = new ClassDefinitionCollection (mappingReflector.GetClassDefinitions(), true, true);
      var actualRelationDefinitions = new RelationDefinitionCollection (mappingReflector.GetRelationDefinitions (actualClassDefinitions), true);

      RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check (FakeMappingConfiguration.Current.RelationDefinitions, actualRelationDefinitions, true);
    }

    [Test]
    public void Get_WithDuplicateAssembly ()
    {
      Assembly assembly = GetType().Assembly;
      MappingReflector expectedMappingReflector = new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (assembly));
      var expectedClassDefinitions = new ClassDefinitionCollection (expectedMappingReflector.GetClassDefinitions(), true, true);
      var expectedRelationDefinitions = new RelationDefinitionCollection (
          expectedMappingReflector.GetRelationDefinitions (expectedClassDefinitions), true);

      MappingReflector mappingReflector = new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (assembly, assembly));
      var actualClassDefinitions = new ClassDefinitionCollection (mappingReflector.GetClassDefinitions(), true, true);

      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker (false);
      classDefinitionChecker.Check (expectedClassDefinitions, actualClassDefinitions, false, false);

      var actualRelationDefinitions = new RelationDefinitionCollection (mappingReflector.GetRelationDefinitions (actualClassDefinitions), true);
      RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check (expectedRelationDefinitions, actualRelationDefinitions, false);
    }

    // TODO Review 3548: Add tests showing that GetClassDefinitions calculates empty and non-empty derived classes
  }
}