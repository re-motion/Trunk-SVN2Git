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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Relations;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class MappingReflectorIntegrationTest
  {
    [Test]
    public void GetClassDefinitions ()
    {
      MappingReflector mappingReflector = new MappingReflector (TestMappingConfiguration.GetTypeDiscoveryService());

      var actualClassDefinitions = new ClassDefinitionCollection (mappingReflector.GetClassDefinitions(), true, true);
      mappingReflector.GetRelationDefinitions (actualClassDefinitions);
      Assert.That (actualClassDefinitions, Is.Not.Null);

      var storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
      foreach (ClassDefinition classDefinition in actualClassDefinitions.GetInheritanceRootClasses())
      {
        DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition.Factory.CreatePersistenceModelLoader (
            storageProviderDefinitionFinder, DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition).
            ApplyPersistenceModelToHierarchy (classDefinition);
      }

      ClassDefinitionChecker classDefinitionChecker = new ClassDefinitionChecker();
      classDefinitionChecker.Check (FakeMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions, false, true);
      classDefinitionChecker.CheckPersistenceModel (FakeMappingConfiguration.Current.ClassDefinitions, actualClassDefinitions);
      Assert.That (actualClassDefinitions.Contains (typeof (TestDomainBase)), Is.False);
    }

    [Test]
    public void ShadowedProperties ()
    {
      var property1 = typeof (Shadower).GetProperty ("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      var property2 = typeof (Base).GetProperty ("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService>();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (Arg<Type>.Is.Anything, Arg<bool>.Is.Anything))
          .Return (new[] { typeof (Base), typeof (Shadower) });

      var mappingReflector = new MappingReflector (typeDiscoveryServiceStub);
      var classDefinition1 = mappingReflector.GetClassDefinitions().Single (cd => cd.ClassType == typeof (Shadower));
      var classDefinition2 = classDefinition1.BaseClass;

      var propertyDefinition1 =
          (ReflectionBasedPropertyDefinition)
          classDefinition1.GetMandatoryPropertyDefinition (property1.DeclaringType.FullName + "." + property1.Name);
      var propertyDefinition2 =
          (ReflectionBasedPropertyDefinition)
          classDefinition2.GetMandatoryPropertyDefinition (property2.DeclaringType.FullName + "." + property2.Name);

      Assert.That (propertyDefinition1.PropertyInfo, Is.EqualTo (property1));
      Assert.That (propertyDefinition2.PropertyInfo, Is.EqualTo (property2));
    }

    [Test]
    public void RelationsAboveInheritanceRoot ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService>();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (Arg<Type>.Is.Anything, Arg<bool>.Is.Anything))
          .Return (
              new[]
              {
                  typeof (UnidirectionalRelationClass), typeof (AboveInheritanceRootClassWithRelation), typeof (DerivedInheritanceRootClass1),
                  typeof (DerivedInheritanceRootClass2)
              });
      MappingReflector reflector = new MappingReflector (typeDiscoveryServiceStub);
      var mappingConfiguration = new MappingConfiguration (
          reflector, new PersistenceModelLoader (new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));

      Assert.That (
          mappingConfiguration.RelationDefinitions[0].ID,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Relations.DerivedInheritanceRootClass1:"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Relations.AboveInheritanceRootClassWithRelation.RelationClass"));
      Assert.That (
          mappingConfiguration.RelationDefinitions[1].ID,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Relations.DerivedInheritanceRootClass2:"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Relations.AboveInheritanceRootClassWithRelation.RelationClass"));
    }

    [Instantiable]
    [DBTable]
    public abstract class Base : DomainObject
    {
      public int Name
      {
        get { return Properties[typeof (Base), "Name"].GetValue<int>(); }
      }
    }

    [Instantiable]
    public abstract class Shadower : Base
    {
      [DBColumn ("NewName")]
      public new int Name
      {
        get { return Properties[typeof (Shadower), "Name"].GetValue<int>(); }
      }
    }
  }
}