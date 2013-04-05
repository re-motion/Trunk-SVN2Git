// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class StoragePropertyDefinitionResolverTest
  {
    private IRdbmsPersistenceModelProvider _persistenceModelProviderStub;

    private StoragePropertyDefinitionResolver _resolver;

    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition1;
    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition2;
    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition3;
    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition4;
    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition5;
    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition6;
    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition7;
    private RdbmsPersistenceModelLoaderTestHelper _testModel;


    [SetUp]
    public void SetUp ()
    {
      _persistenceModelProviderStub = MockRepository.GenerateStub<IRdbmsPersistenceModelProvider>();
      _resolver = new StoragePropertyDefinitionResolver (_persistenceModelProviderStub);
      _testModel = new RdbmsPersistenceModelLoaderTestHelper ();
      
      _fakeStorageProperyDefinition1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Test1");
      _fakeStorageProperyDefinition2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test2");
      _fakeStorageProperyDefinition3 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test3");
      _fakeStorageProperyDefinition4 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test4");
      _fakeStorageProperyDefinition5 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test5");
      _fakeStorageProperyDefinition6 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test6");
      _fakeStorageProperyDefinition7 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test7");

      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.BaseBasePropertyDefinition))
          .Return (_fakeStorageProperyDefinition1);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.BaseBasePropertyDefinition))
          .Return (_fakeStorageProperyDefinition1);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.BasePropertyDefinition))
          .Return (_fakeStorageProperyDefinition2);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.TablePropertyDefinition1))
          .Return (_fakeStorageProperyDefinition3);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.TablePropertyDefinition2))
          .Return (_fakeStorageProperyDefinition4);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.DerivedPropertyDefinition1))
          .Return (_fakeStorageProperyDefinition5);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.DerivedPropertyDefinition2))
          .Return (_fakeStorageProperyDefinition6);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.DerivedDerivedPropertyDefinition))
          .Return (_fakeStorageProperyDefinition7);
    }

    [Test]
    public void GetStoragePropertiesForHierarchy_GetsPropertiesFromDerivedClasses_SortedFromBaseToDerived ()
    {
      var columns = _resolver.GetStoragePropertiesForHierarchy (_testModel.BaseBaseClassDefinition).ToArray();

      Assert.That (
          columns,
          Is.EqualTo (
              new[]
              {
                  _fakeStorageProperyDefinition1, 
                  _fakeStorageProperyDefinition2, 
                  _fakeStorageProperyDefinition3, 
                  _fakeStorageProperyDefinition4, 
                  _fakeStorageProperyDefinition5,
                  _fakeStorageProperyDefinition6, 
                  _fakeStorageProperyDefinition7
              }));
    }

    [Test]
    public void GetStoragePropertiesForHierarchy_AlsoGetsPropertiesFromBaseClasses_SortedFromBaseToDerived ()
    {
      var properties = _resolver.GetStoragePropertiesForHierarchy (_testModel.DerivedClassDefinition1).ToArray ();

      Assert.That (properties, Is.EqualTo (
          new[]
          {
              _fakeStorageProperyDefinition1,
              _fakeStorageProperyDefinition2, _fakeStorageProperyDefinition4, _fakeStorageProperyDefinition5
          }));
    }

    [Test]
    public void GetStoragePropertiesForHierarchy_NonPersistentPropertiesAreFiltered ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Order), baseClass: null);
      var nonPersistentProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (classDefinition, "NonPersistentProperty", StorageClass.None);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { nonPersistentProperty }, true));
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);

      var properties = _resolver.GetStoragePropertiesForHierarchy (classDefinition).ToArray ();

      Assert.That (properties, Is.Empty);
    }

    [Test]
    public void GetStoragePropertiesForHierarchy_MultipleStoragePropertiesForSamePropertyInfoAreUnified ()
    {
      var classDefinition =
          ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (ClassHavingStorageSpecificIdentifierAttribute), baseClass: null);
      var propertyDefinition1 = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (classDefinition, "P1");
      var propertyDefinition2 = PropertyDefinitionObjectMother.CreateForPropertyInformation (classDefinition, "P2", propertyDefinition1.PropertyInfo);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition1, propertyDefinition2 }, true));
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);

      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (propertyDefinition1))
          .Return (_fakeStorageProperyDefinition1);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (propertyDefinition2))
          .Return (_fakeStorageProperyDefinition2);

      var properties = _resolver.GetStoragePropertiesForHierarchy (classDefinition).ToArray ();

      Assert.That (properties, Is.EqualTo (new[] { _fakeStorageProperyDefinition1 })); // instead of 2 properties
    }
  }
}