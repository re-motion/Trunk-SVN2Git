// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class DataStoragePropertyDefinitionFactoryTest : StandardMappingTest
  {
    private StorageTypeInformation _fakeStorageTypeInformation1;
    private StorageTypeInformation _fakeStorageTypeInformation2;

    private StorageGroupBasedStorageProviderDefinitionFinder _storageProviderDefinitionFinder;
    private IStorageNameProvider _storageNameProviderStub;
    private IStorageTypeInformationProvider _storageTypeInformationProviderStrictMock;

    private ClassDefinition _classWithAllDataTypesDefinition;
    private ClassDefinition _fileSystemItemClassDefinition;
    private ClassDefinition _classAboveDbTableAttribute;
    private ClassDefinition _classWithDbTableAttribute;
    private ClassDefinition _classBelowDbTableAttribute;
    private ClassDefinition _classBelowBelowDbTableAttribute;

    private DataStoragePropertyDefinitionFactory _dataStoragePropertyDefinitionFactory;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _fakeStorageTypeInformation1 = StorageTypeInformationObjectMother.CreateStorageTypeInformation ();
      _fakeStorageTypeInformation2 = StorageTypeInformationObjectMother.CreateStorageTypeInformation ();

      _storageProviderDefinitionFinder = new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
      _storageTypeInformationProviderStrictMock = MockRepository.GenerateStrictMock<IStorageTypeInformationProvider> ();

      _storageNameProviderStub = MockRepository.GenerateStub<IStorageNameProvider>();

      _classWithAllDataTypesDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (ClassWithAllDataTypes));
      _classWithAllDataTypesDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _fileSystemItemClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (FileSystemItem));
      _fileSystemItemClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classAboveDbTableAttribute = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (ClassNotInMapping));
      _classAboveDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classWithDbTableAttribute = ClassDefinitionObjectMother.CreateClassDefinition (typeof (Company), _classAboveDbTableAttribute);
      _classWithDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classBelowDbTableAttribute = ClassDefinitionObjectMother.CreateClassDefinition (typeof (Partner), _classWithDbTableAttribute);
      _classBelowDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classBelowBelowDbTableAttribute = ClassDefinitionObjectMother.CreateClassDefinition (
          typeof (Distributor), _classBelowDbTableAttribute);
      _classBelowBelowDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());

      _dataStoragePropertyDefinitionFactory = new DataStoragePropertyDefinitionFactory (
          TestDomainStorageProviderDefinition,
          _storageTypeInformationProviderStrictMock,
          _storageNameProviderStub,
          _storageProviderDefinitionFinder);
    }

    [Test]
    public void CreateStoragePropertyDefinition_NotSupportedType ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_classWithAllDataTypesDefinition);
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageType (Arg.Is (propertyDefinition), Arg<bool>.Is.Anything))
          .Throw (new NotSupportedException ("Msg."));
      _storageTypeInformationProviderStrictMock.Replay();

      var result = _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);
      _storageTypeInformationProviderStrictMock.VerifyAllExpectations ();

      Assert.That (result, Is
          .TypeOf (typeof (UnsupportedStoragePropertyDefinition))
          .With.Property ("Message").EqualTo ("Msg.")
          .And.Property ("PropertyType").EqualTo (propertyDefinition.PropertyType));
    }

    [Test]
    public void CreateStoragePropertyDefinition_ValueProperty ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_classWithAllDataTypesDefinition);
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageType (propertyDefinition, false))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderStrictMock.Replay();

      _storageNameProviderStub
        .Stub (stub => stub.GetColumnName (propertyDefinition))
        .Return ("FakeColumnName");

      var result = (SimpleStoragePropertyDefinition) _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);
      _storageTypeInformationProviderStrictMock.VerifyAllExpectations ();

      Assert.That (result.PropertyType, Is.SameAs (typeof (string)));
      Assert.That (result.ColumnDefinition.Name, Is.EqualTo ("FakeColumnName"));
      Assert.That (result.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
    }

    [Test]
    public void CreateStoragePropertyDefinition_ValueProperty_RespectsNullability_ForClassAboveDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_classWithAllDataTypesDefinition, false);
      var propertyDefinitionNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_classWithAllDataTypesDefinition, true);
      Assert.That (_classAboveDbTableAttribute.BaseClass, Is.Null);

      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNotNullable, false))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNullable, false))
          .Return (_fakeStorageTypeInformation2);
      _storageTypeInformationProviderStrictMock.Replay();

      _storageNameProviderStub
        .Stub (stub => stub.GetColumnName (propertyDefinitionNotNullable))
        .Return ("FakeColumnName");
      _storageNameProviderStub
        .Stub (stub => stub.GetColumnName (propertyDefinitionNullable))
        .Return ("FakeColumnName");

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      _storageTypeInformationProviderStrictMock.VerifyAllExpectations ();

      Assert.That (resultNotNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (resultNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation2));
    }

    [Test]
    public void CreateStoragePropertyDefinition_ValueProperty_RespectsNullability_ForClassWithDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(_classWithDbTableAttribute, false);
      var propertyDefinitionNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_classWithDbTableAttribute, true);

      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNotNullable, false))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNullable, false))
          .Return (_fakeStorageTypeInformation2);
      _storageTypeInformationProviderStrictMock.Replay ();

      _storageNameProviderStub
        .Stub (stub => stub.GetColumnName (propertyDefinitionNotNullable))
        .Return ("FakeColumnName");
      _storageNameProviderStub
        .Stub (stub => stub.GetColumnName (propertyDefinitionNullable))
        .Return ("FakeColumnName");

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      _storageTypeInformationProviderStrictMock.Replay();

      Assert.That (resultNotNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (resultNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation2));
    }

    [Test]
    public void CreateStoragePropertyDefinition_ValueProperty_OverridesNullability_ForClassBelowDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_classBelowDbTableAttribute, false);
      var propertyDefinitionNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_classBelowDbTableAttribute, true);

      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNotNullable, true))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNullable, true))
          .Return (_fakeStorageTypeInformation2);
      _storageTypeInformationProviderStrictMock.Replay();

      _storageNameProviderStub
          .Stub (stub => stub.GetTableName (_classWithDbTableAttribute))
          .Return (new EntityNameDefinition (null, _classWithDbTableAttribute.ID));
      _storageNameProviderStub
        .Stub (stub => stub.GetColumnName (propertyDefinitionNotNullable))
        .Return ("FakeColumnName");
      _storageNameProviderStub
        .Stub (stub => stub.GetColumnName (propertyDefinitionNullable))
        .Return ("FakeColumnName");

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      _storageTypeInformationProviderStrictMock.VerifyAllExpectations();

      Assert.That (resultNotNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (resultNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation2));
    }

    [Test]
    public void CreateStoragePropertyDefinition_ValueProperty_OverridesNullability_ForClassBelowBelowDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_classBelowBelowDbTableAttribute, false);
      var propertyDefinitionNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_classBelowBelowDbTableAttribute, true);

      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNotNullable, true))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNullable, true))
          .Return (_fakeStorageTypeInformation2);
      _storageTypeInformationProviderStrictMock.Replay ();     

      _storageNameProviderStub
          .Stub (stub => stub.GetTableName (_classWithDbTableAttribute))
          .Return (new EntityNameDefinition (null, _classWithDbTableAttribute.ID));
      _storageNameProviderStub
        .Stub (stub => stub.GetColumnName (propertyDefinitionNotNullable))
        .Return ("FakeColumnName");
      _storageNameProviderStub
        .Stub (stub => stub.GetColumnName (propertyDefinitionNullable))
        .Return ("FakeColumnName");

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      _storageTypeInformationProviderStrictMock.VerifyAllExpectations();

      Assert.That (resultNotNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (resultNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation2));
    }

    [Test]
    public void CreateStoragePropertyDefinition_RelationProperty_ToAClassDefinitionWithoutHierarchy ()
    {
      var relationEndPointDefinition =
          (RelationEndPointDefinition) GetEndPointDefinition (typeof (ClassWithManySideRelationProperties), "BidirectionalOneToOne");
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageTypeForID (true))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderStrictMock.Replay ();

      _storageNameProviderStub
          .Stub (stub => stub.GetRelationColumnName (relationEndPointDefinition))
          .Return ("FakeRelationColumnName");
      _storageNameProviderStub
          .Stub (stub => stub.GetRelationClassIDColumnName (relationEndPointDefinition))
          .Return ("FakeRelationClassIDColumnName");

      var result = _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (relationEndPointDefinition.PropertyDefinition);

      _storageTypeInformationProviderStrictMock.VerifyAllExpectations();

      Assert.That (result, Is.TypeOf (typeof (ObjectIDWithoutClassIDStoragePropertyDefinition)));

      var objectIDWithoutClassIDStorageProperty = ((ObjectIDWithoutClassIDStoragePropertyDefinition) result);
      Assert.That (objectIDWithoutClassIDStorageProperty.ValueProperty, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));
      
      var valueStoragePropertyDefinition = ((SimpleStoragePropertyDefinition) objectIDWithoutClassIDStorageProperty.ValueProperty);
      Assert.That (valueStoragePropertyDefinition.PropertyType, Is.SameAs (typeof (object)));
      Assert.That (valueStoragePropertyDefinition.ColumnDefinition.Name, Is.EqualTo ("FakeRelationColumnName"));
      Assert.That (valueStoragePropertyDefinition.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (valueStoragePropertyDefinition.ColumnDefinition.IsPartOfPrimaryKey, Is.False);

      Assert.That (
          objectIDWithoutClassIDStorageProperty.ClassDefinition,
          Is.SameAs (Configuration.GetTypeDefinition (typeof (ClassWithOneSideRelationProperties))));
    }

    [Test]
    public void CreateStoragePropertyDefinition_RelationProperty_ToAClassWithInheritanceHierarchy ()
    {
      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Ceo), "Company");

      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageTypeForID (true))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageTypeForClassID (true))
          .Return (_fakeStorageTypeInformation2);
      _storageTypeInformationProviderStrictMock.Replay ();

      _storageNameProviderStub
          .Stub (stub => stub.GetRelationColumnName (relationEndPointDefinition))
          .Return ("FakeRelationColumnName");
      _storageNameProviderStub
          .Stub (stub => stub.GetRelationClassIDColumnName (relationEndPointDefinition))
          .Return ("FakeRelationClassIDColumnName");

      var result = _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (relationEndPointDefinition.PropertyDefinition);

      _storageTypeInformationProviderStrictMock.VerifyAllExpectations();

      Assert.That (result, Is.TypeOf (typeof (ObjectIDStoragePropertyDefinition)));
      var valueProperty = ((ObjectIDStoragePropertyDefinition) result).ValueProperty;
      var classIDValueProperty = ((ObjectIDStoragePropertyDefinition) result).ClassIDProperty;

      Assert.That (valueProperty, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));
      Assert.That (valueProperty.PropertyType, Is.SameAs (typeof (object)));

      Assert.That (classIDValueProperty, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));
      Assert.That (classIDValueProperty.PropertyType, Is.SameAs (typeof (string)));

      var valueIDColumn = ((SimpleStoragePropertyDefinition) valueProperty).ColumnDefinition;
      var classIDColumn = ((SimpleStoragePropertyDefinition) classIDValueProperty).ColumnDefinition;

      Assert.That (valueIDColumn.Name, Is.EqualTo ("FakeRelationColumnName"));
      Assert.That (valueIDColumn.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (valueIDColumn.IsPartOfPrimaryKey, Is.False);

      Assert.That (classIDColumn.Name, Is.EqualTo ("FakeRelationClassIDColumnName"));
      Assert.That (classIDColumn.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation2));
      Assert.That (classIDColumn.IsPartOfPrimaryKey, Is.False);
    }

    [Test]
    public void CreateStoragePropertyDefinition_RelationProperty_ToAClassDefinitionWithDifferentStorageProvider ()
    {
      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Order), "Official");

      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageTypeForSerializedObjectID (true))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderStrictMock.Replay ();

      _storageNameProviderStub
          .Stub (stub => stub.GetRelationColumnName (relationEndPointDefinition))
          .Return ("FakeRelationColumnName");
      _storageNameProviderStub
          .Stub (stub => stub.GetRelationClassIDColumnName (relationEndPointDefinition))
          .Return ("FakeRelationClassIDColumnName");

      var result = _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (relationEndPointDefinition.PropertyDefinition);

      _storageTypeInformationProviderStrictMock.VerifyAllExpectations();

      Assert.That (result, Is.TypeOf (typeof (SerializedObjectIDStoragePropertyDefinition)));
      Assert.That (((SerializedObjectIDStoragePropertyDefinition) result).SerializedIDProperty, Is.TypeOf<SimpleStoragePropertyDefinition>());
      var serializedIDProperty = ((SimpleStoragePropertyDefinition) ((SerializedObjectIDStoragePropertyDefinition) result).SerializedIDProperty);

      Assert.That (serializedIDProperty.PropertyType, Is.SameAs (typeof (ObjectID)));
      Assert.That (serializedIDProperty.ColumnDefinition.Name, Is.EqualTo ("FakeRelationColumnName"));
      Assert.That (serializedIDProperty.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (serializedIDProperty.ColumnDefinition.IsPartOfPrimaryKey, Is.False);
    }

    [Test]
    public void CreateStoragePropertyDefinition_ForValue_NotSupportedType ()
    {
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageType ("Test"))
          .Throw (new NotSupportedException ("Msg."));
      _storageTypeInformationProviderStrictMock.Replay ();

      var result = _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition ("Test");
      _storageTypeInformationProviderStrictMock.VerifyAllExpectations ();

      Assert.That (result, Is
          .TypeOf (typeof (UnsupportedStoragePropertyDefinition))
          .With.Property ("Message").EqualTo ("Msg.")
          .And.Property ("PropertyType").EqualTo (typeof (string)));
    }

    [Test]
    public void CreateStoragePropertyDefinition_ForValue_Null ()
    {
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageType ((object) null))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderStrictMock.Replay ();

      var result = _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition ((object) null);
      _storageTypeInformationProviderStrictMock.VerifyAllExpectations ();

      Assert.That (result, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));
      Assert.That (result.PropertyType, Is.SameAs (typeof (object)));
      var column = StoragePropertyDefinitionTestHelper.GetSingleColumn (result);
      Assert.That (column.Name, Is.EqualTo ("Value"));
      Assert.That (column.IsPartOfPrimaryKey, Is.False);
      Assert.That (column.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
    }

    [Test]
    public void CreateStoragePropertyDefinition_ForValue_SimpleValue ()
    {
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageType ("Test"))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderStrictMock.Replay ();

      var result = _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition ("Test");
      _storageTypeInformationProviderStrictMock.VerifyAllExpectations ();

      Assert.That (result, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));
      Assert.That (result.PropertyType, Is.SameAs (typeof (string)));
      var column = StoragePropertyDefinitionTestHelper.GetSingleColumn (result);
      Assert.That (column.Name, Is.EqualTo ("Value"));
      Assert.That (column.IsPartOfPrimaryKey, Is.False);
      Assert.That (column.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
    }

    [Test]
    public void CreateStoragePropertyDefinition_ForValue_ObjectID_ToAClassDefinitionWithoutHierarchy ()
    {
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageTypeForID (true))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderStrictMock.Replay ();

      var result = _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (DomainObjectIDs.ClassWithAllDataTypes1);

      _storageTypeInformationProviderStrictMock.VerifyAllExpectations ();

      Assert.That (result, Is.TypeOf (typeof (ObjectIDWithoutClassIDStoragePropertyDefinition)));

      var objectIDWithoutClassIDStorageProperty = ((ObjectIDWithoutClassIDStoragePropertyDefinition) result);
      Assert.That (objectIDWithoutClassIDStorageProperty.ValueProperty, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));

      var valueStoragePropertyDefinition = ((SimpleStoragePropertyDefinition) objectIDWithoutClassIDStorageProperty.ValueProperty);
      Assert.That (valueStoragePropertyDefinition.PropertyType, Is.SameAs (typeof (object)));
      Assert.That (valueStoragePropertyDefinition.ColumnDefinition.Name, Is.EqualTo ("Value"));
      Assert.That (valueStoragePropertyDefinition.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (valueStoragePropertyDefinition.ColumnDefinition.IsPartOfPrimaryKey, Is.False);

      Assert.That (
          objectIDWithoutClassIDStorageProperty.ClassDefinition,
          Is.SameAs (Configuration.GetTypeDefinition (typeof (ClassWithAllDataTypes))));
    }

    [Test]
    public void CreateStoragePropertyDefinition_ForValue_ObjectID_ToAClassWithInheritanceHierarchy()
    {
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageTypeForID (true))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageTypeForClassID (true))
          .Return (_fakeStorageTypeInformation2);
      _storageTypeInformationProviderStrictMock.Replay ();

      var result = _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (DomainObjectIDs.Company1);

      _storageTypeInformationProviderStrictMock.VerifyAllExpectations ();

      Assert.That (result, Is.TypeOf (typeof (ObjectIDStoragePropertyDefinition)));
      var valueProperty = ((ObjectIDStoragePropertyDefinition) result).ValueProperty;
      var classIDValueProperty = ((ObjectIDStoragePropertyDefinition) result).ClassIDProperty;

      Assert.That (valueProperty, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));
      Assert.That (valueProperty.PropertyType, Is.SameAs (typeof (object)));

      Assert.That (classIDValueProperty, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));
      Assert.That (classIDValueProperty.PropertyType, Is.SameAs (typeof (string)));

      var valueIDColumn = ((SimpleStoragePropertyDefinition) valueProperty).ColumnDefinition;
      var classIDColumn = ((SimpleStoragePropertyDefinition) classIDValueProperty).ColumnDefinition;

      Assert.That (valueIDColumn.Name, Is.EqualTo ("Value"));
      Assert.That (valueIDColumn.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (valueIDColumn.IsPartOfPrimaryKey, Is.False);

      Assert.That (classIDColumn.Name, Is.EqualTo ("ValueClassID"));
      Assert.That (classIDColumn.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation2));
      Assert.That (classIDColumn.IsPartOfPrimaryKey, Is.False);
    }

    [Test]
    public void CreateStoragePropertyDefinition_ForValue_ObjectID_ClassDefinitionWithDifferentStorageProvider ()
    {
      _storageTypeInformationProviderStrictMock
          .Expect (mock => mock.GetStorageTypeForSerializedObjectID (true))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderStrictMock.Replay ();

      var result = _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (DomainObjectIDs.Official1);

      _storageTypeInformationProviderStrictMock.VerifyAllExpectations ();

      Assert.That (result, Is.TypeOf (typeof (SerializedObjectIDStoragePropertyDefinition)));
      Assert.That (((SerializedObjectIDStoragePropertyDefinition) result).SerializedIDProperty, Is.TypeOf<SimpleStoragePropertyDefinition> ());
      var serializedIDProperty = ((SimpleStoragePropertyDefinition) ((SerializedObjectIDStoragePropertyDefinition) result).SerializedIDProperty);

      Assert.That (serializedIDProperty.PropertyType, Is.SameAs (typeof (ObjectID)));
      Assert.That (serializedIDProperty.ColumnDefinition.Name, Is.EqualTo ("Value"));
      Assert.That (serializedIDProperty.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (serializedIDProperty.ColumnDefinition.IsPartOfPrimaryKey, Is.False);
    }

  }
}