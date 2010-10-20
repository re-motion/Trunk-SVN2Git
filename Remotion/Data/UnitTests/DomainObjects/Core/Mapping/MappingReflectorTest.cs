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
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Relations;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class MappingReflectorTest : MappingReflectionTestBase
  {
    [Test]
    public void ProveThatRelationsAboveTheInheritanceRootAreSupported ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (Arg<Type>.Is.Anything, Arg<bool>.Is.Anything))
          .Return (
              new[]
              {
                  typeof (UnidirectionalRelationClass), typeof (AboveInheritanceRootClassWithRelation), typeof (DerivedInheritanceRootClass1),
                  typeof (DerivedInheritanceRootClass2)
              });
      MappingReflector reflector = new MappingReflector (typeDiscoveryServiceStub);
      var mappingConfiguration = new MappingConfiguration (reflector);
      mappingConfiguration.Validate ();

      Assert.That (mappingConfiguration.RelationDefinitions[0].ID, 
        Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Relations.DerivedInheritanceRootClass1->"
                   +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Relations.AboveInheritanceRootClassWithRelation.RelationClass"));
      Assert.That (mappingConfiguration.RelationDefinitions[1].ID, 
        Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Relations.DerivedInheritanceRootClass2->"
                   +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Relations.AboveInheritanceRootClassWithRelation.RelationClass"));
    }

    [Test]
    public void GetResolveTypes ()
    {
      IMappingLoader mappingReflector = new MappingReflector (TestMappingConfiguration.GetTypeDiscoveryService());
      Assert.IsTrue (mappingReflector.ResolveTypes);
    }

    [Test]
    public void GetClassDefinitions ()
    {
      MappingReflector mappingReflector = new MappingReflector (TestMappingConfiguration.GetTypeDiscoveryService());

      ClassDefinitionCollection actualClassDefinitions = mappingReflector.GetClassDefinitions();

      Assert.IsNotNull (actualClassDefinitions);
      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker();
      classDefinitionChecker.Check (FakeMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, false, true);
      Assert.IsFalse (actualClassDefinitions.Contains (typeof (TestDomainBase)));
    }

    [Test]
    public void GetRelationDefinitions ()
    {
      MappingReflector mappingReflector = new MappingReflector (TestMappingConfiguration.GetTypeDiscoveryService());

      ClassDefinitionCollection actualClassDefinitions = mappingReflector.GetClassDefinitions();
      RelationDefinitionCollection actualRelationDefinitions = mappingReflector.GetRelationDefinitions (actualClassDefinitions);

      RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check (FakeMappingConfiguration.Current.RelationDefinitions, actualRelationDefinitions, true);
    }

    [Test]
    public void Get_WithDuplicateAssembly ()
    {
      Assembly assembly = GetType().Assembly;
      MappingReflector expectedMappingReflector = new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (assembly));
      ClassDefinitionCollection expectedClassDefinitions = expectedMappingReflector.GetClassDefinitions();
      RelationDefinitionCollection expectedRelationDefinitions = expectedMappingReflector.GetRelationDefinitions (expectedClassDefinitions);

      MappingReflector mappingReflector = new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (assembly, assembly));
      ClassDefinitionCollection actualClassDefinitions = mappingReflector.GetClassDefinitions();

      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker();
      classDefinitionChecker.Check (expectedClassDefinitions, actualClassDefinitions, false, false);

      RelationDefinitionCollection actualRelationDefinitions = mappingReflector.GetRelationDefinitions (actualClassDefinitions);
      RelationDefinitionChecker relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check (expectedRelationDefinitions, actualRelationDefinitions, false);
    }
  }
}