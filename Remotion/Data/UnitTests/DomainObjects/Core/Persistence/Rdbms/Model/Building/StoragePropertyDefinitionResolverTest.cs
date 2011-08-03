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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class StoragePropertyDefinitionResolverTest
  {
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
      _resolver = new StoragePropertyDefinitionResolver();
      _testModel = new RdbmsPersistenceModelLoaderTestHelper ();
      
      _fakeStorageProperyDefinition1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Test1");
      _fakeStorageProperyDefinition2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test2");
      _fakeStorageProperyDefinition3 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test3");
      _fakeStorageProperyDefinition4 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test4");
      _fakeStorageProperyDefinition5 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test5");
      _fakeStorageProperyDefinition6 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test6");
      _fakeStorageProperyDefinition7 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test7");
      
      _testModel.BaseBasePropertyDefinition.SetStorageProperty (_fakeStorageProperyDefinition1);
      _testModel.BasePropertyDefinition.SetStorageProperty (_fakeStorageProperyDefinition2);
      _testModel.TablePropertyDefinition1.SetStorageProperty (_fakeStorageProperyDefinition3);
      _testModel.TablePropertyDefinition2.SetStorageProperty (_fakeStorageProperyDefinition4);
      _testModel.DerivedPropertyDefinition1.SetStorageProperty (_fakeStorageProperyDefinition5);
      _testModel.DerivedPropertyDefinition2.SetStorageProperty (_fakeStorageProperyDefinition6);
      _testModel.DerivedDerivedPropertyDefinition.SetStorageProperty (_fakeStorageProperyDefinition7);
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
                  _fakeStorageProperyDefinition1, _fakeStorageProperyDefinition2, _fakeStorageProperyDefinition3, _fakeStorageProperyDefinition4, _fakeStorageProperyDefinition5,
                  _fakeStorageProperyDefinition6, _fakeStorageProperyDefinition7
              }));
    }

    [Test]
    public void GetStoragePropertiesForHierarchy_AlsoGetsPropertiesFromBaseClasses_SortedFromBaseToDerived ()
    {
      var columns = _resolver.GetStoragePropertiesForHierarchy (_testModel.DerivedClassDefinition1).ToArray ();

      Assert.That (columns, Is.EqualTo (new[] { _fakeStorageProperyDefinition1, _fakeStorageProperyDefinition2,  _fakeStorageProperyDefinition4, _fakeStorageProperyDefinition5 }));
    }

    [Test]
    public void GetStoragePropertiesForHierarchy_NonPersistentPropertiesAreFiltered ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var nonPersistentProperty = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition, "NonPersistentProperty", "NonPersistentProperty", StorageClass.None);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { nonPersistentProperty }, true));
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);

      var columns = _resolver.GetStoragePropertiesForHierarchy (classDefinition).ToArray ();

      Assert.That (columns, Is.EqualTo (new IRdbmsStoragePropertyDefinition[0]));
    }

    [Test]
    public void GetStoragePropertiesForHierarchy_PropertiesWithSamePropertyInfoAreFiltered ()
    {
      var classDefinition =
          ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (
              typeof (ClassHavingStorageSpecificIdentifierAttribute), null);
      var propertyInfo = typeof (ClassHavingStorageSpecificIdentifierAttribute).GetProperty ("StorageSpecificName");
      var propertyDefinition1 = PropertyDefinitionFactory.Create (
          classDefinition,
          "Test1", 
          false,
          true,
          null,
          StorageClass.Persistent,
          propertyInfo,
          null);
      var propertyDefinition2 = PropertyDefinitionFactory.Create (
          classDefinition,
          "Test2", 
          false,
          true,
          null,
          StorageClass.Persistent,
          propertyInfo,
          null);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition1, propertyDefinition2 }, true));
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      propertyDefinition1.SetStorageProperty (_fakeStorageProperyDefinition1);
      propertyDefinition2.SetStorageProperty (_fakeStorageProperyDefinition2);

      var columns = _resolver.GetStoragePropertiesForHierarchy (classDefinition).ToArray ();

      Assert.That (columns.Length, Is.EqualTo (1)); //instead of 2
    }

    [Test]
    public void GetStorageProperty ()
    {
      var result = _resolver.GetStorageProperty (_testModel.BaseBasePropertyDefinition);

      Assert.That (result, Is.SameAs (_fakeStorageProperyDefinition1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
      "Storage property definition has not been set.\r\n"
      + "Declaring type: 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassHavingStorageSpecificIdentifierAttribute'\r\n"
      + "Property: 'StorageSpecificName'")]
    public void GetStorageProperty_StoragePropertyDefinitionHasNotBeenSet ()
    {
      var classDefinition =
          ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (
              typeof (ClassHavingStorageSpecificIdentifierAttribute), null);
      var propertyInfo = typeof (ClassHavingStorageSpecificIdentifierAttribute).GetProperty ("StorageSpecificName");
      var propertyDefinition1 = PropertyDefinitionFactory.Create (
          classDefinition,
          "Test1", 
          false,
          true,
          null,
          StorageClass.Persistent,
          propertyInfo,
          null);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition1 }, true));
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);

      _resolver.GetStorageProperty (propertyDefinition1);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Cannot have non-RDBMS storage properties in an RDBMS mapping.\r\n"
        + "Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.RdbmsPersistenceModelLoaderTestDomain.BaseBaseClass'\r\n"
        + "Property: 'BaseBaseProperty'")]
    public void GetStorageProperty_StoragePropertyDefinitionIsNoRdbmsDefinition ()
    {
      var fakeResult = new FakeStoragePropertyDefinition ("Invalid");
      _testModel.BaseBasePropertyDefinition.SetStorageProperty (fakeResult);

      _resolver.GetStorageProperty (_testModel.BaseBasePropertyDefinition);
    }
  }
}