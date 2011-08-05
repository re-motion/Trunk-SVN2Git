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
using System.ComponentModel;
using System.Data;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class DataStoragePropertyDefinitionFactoryTest : StandardMappingTest
  {
    private PropertyInfo _propertyInfoStub;

    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;
    private IStorageTypeInformationProvider _storageTypeInformationProviderStub;
    private IStorageNameProvider _storageNameProviderStub;

    private ClassDefinition _classWithAllDataTypesDefinition;
    private ClassDefinition _fileSystemItemClassDefinition;
    private ClassDefinition _classAboveDbTableAttribute;
    private ClassDefinition _classWithDbTableAttribute;
    private ClassDefinition _classBelowDbTableAttribute;
    private ClassDefinition _classBelowBelowDbTableAttribute;

    private DataStoragePropertyDefinitionFactory _DataStoragePropertyDefinitionFactory;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _propertyInfoStub = typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty");
      
      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
      _storageTypeInformationProviderStub = MockRepository.GenerateStub<IStorageTypeInformationProvider> ();
      _storageTypeInformationProviderStub.Stub (stub => stub.GetStorageTypeForClassID()).Return (
          new StorageTypeInformation ("varchar(100)", DbType.String, typeof (string), new StringConverter()));
      _storageTypeInformationProviderStub.Stub (stub => stub.GetStorageTypeForObjectID()).Return (
          new StorageTypeInformation ("guid", DbType.Guid, typeof (Guid), new GuidConverter()));
      _storageTypeInformationProviderStub.Stub (stub => stub.GetStorageTypeForSerializedObjectID()).Return (
          new StorageTypeInformation ("varchar (255)", DbType.String, typeof (string), new StringConverter()));
      _storageNameProviderStub = MockRepository.GenerateStub<IStorageNameProvider>();
      _storageNameProviderStub.Stub (stub => stub.IDColumnName).Return ("ID");
      _storageNameProviderStub.Stub (stub => stub.ClassIDColumnName).Return ("ClassID");
      _storageNameProviderStub.Stub (stub => stub.TimestampColumnName).Return ("Timestamp");

      _classWithAllDataTypesDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (ClassWithAllDataTypes));
      _classWithAllDataTypesDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _fileSystemItemClassDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (FileSystemItem));
      _fileSystemItemClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classAboveDbTableAttribute = ClassDefinitionFactory.CreateClassDefinition (typeof (ClassNotInMapping));
      _classAboveDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classWithDbTableAttribute = ClassDefinitionFactory.CreateClassDefinition (typeof (Company), _classAboveDbTableAttribute);
      _classWithDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classBelowDbTableAttribute = ClassDefinitionFactory.CreateClassDefinition (typeof (Partner), _classWithDbTableAttribute);
      _classBelowDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classBelowBelowDbTableAttribute = ClassDefinitionFactory.CreateClassDefinition (
          typeof (Distributor), _classBelowDbTableAttribute);
      _classBelowBelowDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());

      _DataStoragePropertyDefinitionFactory = new DataStoragePropertyDefinitionFactory (
          _storageTypeInformationProviderStub,
          _storageNameProviderStub,
          _storageProviderDefinitionFinder);
    }

    [Test]
    public void CreateStoragePropertyDefinition_RelationProperty ()
    {
      var propertyDefinition = PropertyDefinitionFactory.Create (_fileSystemItemClassDefinition, "ParentFolder");
      StubStorageCalculators (propertyDefinition);

      var result = (SimpleStoragePropertyDefinition) _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result.Name, Is.EqualTo ("FakeColumnName"));
      Assert.That (result.ColumnDefinition.PropertyType, Is.SameAs (typeof (ObjectID)));
    }

    [Test]
    public void CreateStoragePropertyDefinition_PropertyType ()
    {
      var propertyDefinition = PropertyDefinitionFactory.Create (_classWithAllDataTypesDefinition, StorageClass.Persistent, _propertyInfoStub);
      StubStorageCalculators (propertyDefinition);

      var result = (SimpleStoragePropertyDefinition) _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result.Name, Is.EqualTo ("FakeColumnName"));
      Assert.That (result.ColumnDefinition.PropertyType, Is.SameAs (typeof (bool)));
    }

    [Test]
    public void CreateStoragePropertyDefinition_StorageType ()
    {
      var propertyDefinition = PropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, _propertyInfoStub);
      StubStorageCalculators (propertyDefinition);

      var result = (SimpleStoragePropertyDefinition) _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result.Name, Is.EqualTo ("FakeColumnName"));
      Assert.That (result.ColumnDefinition.StorageTypeInfo.StorageTypeName, Is.EqualTo ("storage type"));
    }

    [Test]
    public void CreateStoragePropertyDefinition_IsNullable ()
    {
      var nullablePropertyDefinition = PropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, typeof (ClassWithAllDataTypes), "StringProperty", "StringProperty", true);
      var nonNullablePropertyDefinition = PropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, typeof (ClassWithAllDataTypes), "StringProperty", "StringProperty", false);
      StubStorageCalculators (nullablePropertyDefinition);
      StubStorageCalculators (nonNullablePropertyDefinition);

      var nullableResult =
          (SimpleStoragePropertyDefinition) _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (nullablePropertyDefinition);
      var nonNullableResult =
          (SimpleStoragePropertyDefinition) _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (nonNullablePropertyDefinition);

      Assert.That (nullableResult.ColumnDefinition.IsNullable, Is.True);
      Assert.That (nonNullableResult.ColumnDefinition.IsNullable, Is.False);
    }

    [Test]
    public void CreateStoragePropertyDefinition_NotSupportedType ()
    {
      var propertyDefinition = PropertyDefinitionFactory.Create (_classWithAllDataTypesDefinition, StorageClass.Persistent, _propertyInfoStub);
      _storageTypeInformationProviderStub.Stub (stub => stub.GetStorageType (propertyDefinition)).Return (null);

      var result = _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (UnsupportedStoragePropertyDefinition)));
    }

    [Test]
    public void CreateStoragePropertyDefinition_RespectsNullability_ForClassAboveDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionFactory.Create (
          _classAboveDbTableAttribute, StorageClass.Persistent, _propertyInfoStub, false);
      var propertyDefinitionNullable = PropertyDefinitionFactory.Create (
          _classAboveDbTableAttribute, StorageClass.Persistent, _propertyInfoStub, true);

      StubStorageCalculators (propertyDefinitionNotNullable);
      StubStorageCalculators (propertyDefinitionNullable);

      Assert.That (_classAboveDbTableAttribute.BaseClass, Is.Null);

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      Assert.That (resultNotNullable.ColumnDefinition.IsNullable, Is.False);
      Assert.That (resultNullable.ColumnDefinition.IsNullable, Is.True);
    }

    [Test]
    public void CreateStoragePropertyDefinition_RespectsNullability_ForClassWithDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionFactory.Create (
          _classWithDbTableAttribute, StorageClass.Persistent, _propertyInfoStub, false);
      var propertyDefinitionNullable = PropertyDefinitionFactory.Create (
          _classWithDbTableAttribute, StorageClass.Persistent, _propertyInfoStub, true);

      StubStorageCalculators (propertyDefinitionNotNullable);
      StubStorageCalculators (propertyDefinitionNullable);

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      Assert.That (resultNotNullable.ColumnDefinition.IsNullable, Is.False);
      Assert.That (resultNullable.ColumnDefinition.IsNullable, Is.True);
    }

    [Test]
    public void CreateStoragePropertyDefinition_OverridesNullability_ForClassBelowDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionFactory.Create (
          _classBelowDbTableAttribute, StorageClass.Persistent, _propertyInfoStub, false);
      var propertyDefinitionNullable = PropertyDefinitionFactory.Create (
          _classBelowDbTableAttribute, StorageClass.Persistent, _propertyInfoStub, true);

      StubStorageCalculators (propertyDefinitionNotNullable);
      StubStorageCalculators (propertyDefinitionNullable);
      _storageNameProviderStub.Stub (stub => stub.GetTableName (_classWithDbTableAttribute)).Return (_classWithDbTableAttribute.ID);

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      Assert.That (resultNotNullable.ColumnDefinition.IsNullable, Is.True);
      Assert.That (resultNullable.ColumnDefinition.IsNullable, Is.True);
    }

    [Test]
    public void CreateStoragePropertyDefinition_OverridesNullability_ForClassBelowBelowDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionFactory.Create (
          _classBelowBelowDbTableAttribute,
          StorageClass.Persistent,
          _propertyInfoStub,
          false);
      var propertyDefinitionNullable = PropertyDefinitionFactory.Create (
          _classBelowBelowDbTableAttribute,
          StorageClass.Persistent,
          _propertyInfoStub,
          true);

      StubStorageCalculators (propertyDefinitionNotNullable);
      StubStorageCalculators (propertyDefinitionNullable);
      _storageNameProviderStub.Stub (stub => stub.GetTableName (_classWithDbTableAttribute)).Return (_classWithDbTableAttribute.ID);

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      Assert.That (resultNotNullable.ColumnDefinition.IsNullable, Is.True);
      Assert.That (resultNullable.ColumnDefinition.IsNullable, Is.True);
    }

    [Test]
    public void CreaeStoragePropertyDefinition_NonRelationProperty_GetsNoClassIdColumn ()
    {
      var classDefinition = Configuration.GetTypeDefinition (typeof (Order));
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (Order).FullName + ".OrderNumber"];
      StubStorageCalculators (propertyDefinition);

      var result = _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));
    }

    [Test]
    public void CreaeStoragePropertyDefinition_RelationPropertyToAClassDefinitionThatISNotPartOfTheMapping_GetsNotClassIDColumn ()
    {
      var classDefinition = Configuration.GetTypeDefinition (typeof (ClassWithManySideRelationProperties));
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (ClassWithManySideRelationProperties).FullName + ".BidirectionalOneToOne"];
      StubStorageCalculators (propertyDefinition);

      var result = _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (ObjectIDWithoutClassIDStoragePropertyDefinition)));
    }

    [Test]
    public void CreaeStoragePropertyDefinition_RelationPropertyToAClassDefinitionWithBaseClassAndNoDerivedClasses_GetsClassIDColumn ()
    {
      var classDefinition = Configuration.GetTypeDefinition (typeof (ClassWithoutRelatedClassIDColumn));
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (ClassWithoutRelatedClassIDColumn).FullName + ".Distributor"];
      StubStorageCalculators (propertyDefinition);

      var result = _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (ObjectIDStoragePropertyDefinition)));
      var valueProperty = ((ObjectIDStoragePropertyDefinition) result).ValueProperty;
      var classIDValueProperty = ((ObjectIDStoragePropertyDefinition) result).ClassIDProperty;

      Assert.That (valueProperty, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));
      Assert.That (classIDValueProperty, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));

      var valueIDColumn = ((SimpleStoragePropertyDefinition) valueProperty).ColumnDefinition;
      var classIDColumn = ((SimpleStoragePropertyDefinition) classIDValueProperty).ColumnDefinition;

      Assert.That (valueIDColumn.Name, Is.EqualTo ("FakeColumnNameID"));
      Assert.That (valueIDColumn.IsNullable, Is.True);
      Assert.That (valueIDColumn.PropertyType, Is.SameAs (typeof (ObjectID)));
      Assert.That (valueIDColumn.StorageTypeInfo.StorageTypeName, Is.EqualTo ("guid"));
      Assert.That (valueIDColumn.IsPartOfPrimaryKey, Is.False);
      Assert.That (classIDColumn.Name, Is.EqualTo ("FakeRelationClassID"));
      Assert.That (classIDColumn.IsNullable, Is.True);
      Assert.That (classIDColumn.PropertyType, Is.SameAs (typeof (string)));
      Assert.That (classIDColumn.StorageTypeInfo.StorageTypeName, Is.EqualTo ("varchar(100)"));
      Assert.That (classIDColumn.IsPartOfPrimaryKey, Is.False);
    }

    [Test]
    public void CreaeStoragePropertyDefinition_RelationPropertyToAClassDefinitionWithoutBaseClassAndWithDerivedClasses_GetsClassIDColumn ()
    {
      var classDefinition = Configuration.GetTypeDefinition (typeof (Ceo));
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (Ceo).FullName + ".Company"];
      StubStorageCalculators (propertyDefinition);

      var result = _DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (ObjectIDStoragePropertyDefinition)));
    }

    [Test]
    public void CreaeStoragePropertyDefinition_RelationPropertyToAClassDefinitionWithDifferentStorageProvider ()
    {
      var classDefinition = Configuration.GetTypeDefinition (typeof (Ceo));
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (Ceo).FullName + ".Company"];
      StubStorageCalculators (propertyDefinition);

      var storageProviderDefinitionFinder = MockRepository.GenerateStub<IStorageProviderDefinitionFinder>();
      storageProviderDefinitionFinder
          .Stub (stub => stub.GetStorageProviderDefinition (Arg<Type>.Is.Anything, Arg<string>.Is.Anything))
          .Return (TestDomainStorageProviderDefinition).Repeat.Once();
      storageProviderDefinitionFinder
          .Stub (stub => stub.GetStorageProviderDefinition (Arg<Type>.Is.Anything, Arg<string>.Is.Anything))
          .Return (UnitTestStorageProviderDefinition).Repeat.Once();
      var DataStoragePropertyDefinitionFactory = new DataStoragePropertyDefinitionFactory (
          _storageTypeInformationProviderStub, _storageNameProviderStub, storageProviderDefinitionFinder);

      var result = DataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (SerializedObjectIDStoragePropertyDefinition)));
    }

    private void StubStorageCalculators (PropertyDefinition propertyDefinition)
    {
      _storageTypeInformationProviderStub.Stub (stub => stub.GetStorageType (propertyDefinition)).Return (
          new StorageTypeInformation ("storage type", DbType.String, typeof(string), new StringConverter()));
      _storageNameProviderStub.Stub (stub => stub.GetColumnName (propertyDefinition)).Return ("FakeColumnName");
      _storageNameProviderStub.Stub (stub => stub.GetRelationColumnName (propertyDefinition)).Return ("FakeColumnNameID");
      _storageNameProviderStub.Stub (stub => stub.GetRelationClassIDColumnName (propertyDefinition)).Return ("FakeRelationClassID");
    }
  }
}