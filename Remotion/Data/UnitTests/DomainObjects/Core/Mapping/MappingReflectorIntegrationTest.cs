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
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class MappingReflectorIntegrationTest
  {
    [Test]
    public void GetClassDefinitions ()
    {
      MappingReflector mappingReflector = MappingReflectorFactory.CreateMappingReflector(TestMappingConfiguration.GetTypeDiscoveryService());

      var actualClassDefinitions = mappingReflector.GetClassDefinitions().ToDictionary (cd => cd.ClassType);
      mappingReflector.GetRelationDefinitions (actualClassDefinitions);
      Assert.That (actualClassDefinitions, Is.Not.Null);

      var inheritanceRootClasses = actualClassDefinitions.Values.Select (cd => cd.GetInheritanceRootClass()).Distinct();

      // Pretend that all classes have the storage provider definition used by FakeMappingConfiguration
      var storageProviderDefinition = FakeMappingConfiguration.Current.StorageProviderDefinition;
      var storageProviderDefinitionFinderStub = MockRepository.GenerateStub<IStorageProviderDefinitionFinder> ();
      storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (Arg<ClassDefinition>.Is.Anything, Arg<string>.Is.Anything))
          .Return (storageProviderDefinition);

      foreach (ClassDefinition classDefinition in inheritanceRootClasses)
      {
        var persistenceModelLoader = storageProviderDefinition.Factory.CreatePersistenceModelLoader (storageProviderDefinition, storageProviderDefinitionFinderStub);
        persistenceModelLoader.ApplyPersistenceModelToHierarchy (classDefinition);
      }

      var classDefinitionChecker = new ClassDefinitionChecker();
      classDefinitionChecker.Check (FakeMappingConfiguration.Current.TypeDefinitions.Values, actualClassDefinitions, false, true);
      classDefinitionChecker.CheckPersistenceModel (FakeMappingConfiguration.Current.TypeDefinitions.Values, actualClassDefinitions);
      Assert.That (actualClassDefinitions.ContainsKey (typeof (TestDomainBase)), Is.False);
    }

    [Test]
    public void ShadowedProperties ()
    {
      var property1 = typeof (Shadower).GetProperty ("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      var property2 = typeof (Base).GetProperty ("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService>();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (Arg<Type>.Is.Anything, Arg<bool>.Is.Anything))
          .Return (new[] { typeof (Base), typeof (Shadower) });

      var mappingReflector = MappingReflectorFactory.CreateMappingReflector(typeDiscoveryServiceStub);
      var classDefinition1 = mappingReflector.GetClassDefinitions().Single (cd => cd.ClassType == typeof (Shadower));
      var classDefinition2 = classDefinition1.BaseClass;

      var propertyDefinition1 =
          classDefinition1.GetMandatoryPropertyDefinition (property1.DeclaringType.FullName + "." + property1.Name);
      var propertyDefinition2 =
          classDefinition2.GetMandatoryPropertyDefinition (property2.DeclaringType.FullName + "." + property2.Name);

      Assert.That (propertyDefinition1.PropertyInfo, Is.EqualTo (PropertyInfoAdapter.Create(property1)));
      Assert.That (propertyDefinition2.PropertyInfo, Is.EqualTo (PropertyInfoAdapter.Create(property2)));
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
      var reflector = MappingReflectorFactory.CreateMappingReflector(typeDiscoveryServiceStub);
      var mappingConfiguration = new MappingConfiguration (
          reflector, new PersistenceModelLoader (new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage)));

      var derivedClass1 = mappingConfiguration.GetTypeDefinition (typeof (DerivedInheritanceRootClass1));
      var derivedClass2 = mappingConfiguration.GetTypeDefinition (typeof (DerivedInheritanceRootClass2));

      var derivedClass1RelationEndPoint =
          derivedClass1.GetRelationEndPointDefinition (typeof (AboveInheritanceRootClassWithRelation).FullName + ".RelationClass");
      var derivedClass2RelationEndPoint =
          derivedClass2.GetRelationEndPointDefinition (typeof (AboveInheritanceRootClassWithRelation).FullName + ".RelationClass");

      Assert.That (derivedClass1RelationEndPoint.RelationDefinition, Is.Not.SameAs (derivedClass2RelationEndPoint.RelationDefinition));
      Assert.That (
          derivedClass1RelationEndPoint.RelationDefinition.ID,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Relations.DerivedInheritanceRootClass1:"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Relations.AboveInheritanceRootClassWithRelation.RelationClass"));
      Assert.That (
          derivedClass2RelationEndPoint.RelationDefinition.ID,
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