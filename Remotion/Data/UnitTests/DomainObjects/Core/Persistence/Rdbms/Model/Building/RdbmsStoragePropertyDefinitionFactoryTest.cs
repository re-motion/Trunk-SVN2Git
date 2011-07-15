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
using ClassNotInMapping =
    Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.RelationReflector.RelatedPropertyTypeIsNotInMapping.ClassNotInMapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class RdbmsStoragePropertyDefinitionFactoryTest : StandardMappingTest
  {
    private StorageTypeCalculator _storageTypeCalculatorStub;
    private RdbmsStoragePropertyDefinitionFactory _rdbmsStoragePropertyDefinitionFactory;
    private ClassDefinition _classWithAllDataTypesDefinition;
    private ClassDefinition _fileSystemItemClassDefinition;
    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;
    private ClassDefinition _classAboveDbTableAttribute;
    private ClassDefinition _classWithDbTableAttribute;
    private ClassDefinition _classBelowDbTableAttribute;
    private ClassDefinition _classBelowBelowDbTableAttribute;
    private IStorageNameProvider _storageNameProviderStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
      _storageTypeCalculatorStub = MockRepository.GenerateStub<StorageTypeCalculator> (_storageProviderDefinitionFinder);
      _storageTypeCalculatorStub.Stub (stub => stub.ClassIDStorageType).Return (
          new StorageTypeInformation ("varchar(100)", DbType.String, typeof (string), new StringConverter()));
      _storageTypeCalculatorStub.Stub (stub => stub.ObjectIDStorageType).Return (
          new StorageTypeInformation ("guid", DbType.Guid, typeof (Guid), new GuidConverter()));
      _storageTypeCalculatorStub.Stub (stub => stub.TimestampStorageType).Return (
          new StorageTypeInformation ("rowversion", DbType.DateTime, typeof (DateTime), new DateTimeConverter()));
      _storageTypeCalculatorStub.Stub (stub => stub.SerializedObjectIDStorageType).Return (
          new StorageTypeInformation ("varchar (255)", DbType.String, typeof (string), new StringConverter()));
      _storageNameProviderStub = MockRepository.GenerateStub<IStorageNameProvider>();
      _storageNameProviderStub.Stub (stub => stub.IDColumnName).Return ("ID");
      _storageNameProviderStub.Stub (stub => stub.ClassIDColumnName).Return ("ClassID");
      _storageNameProviderStub.Stub (stub => stub.TimestampColumnName).Return ("Timestamp");
      _rdbmsStoragePropertyDefinitionFactory = new RdbmsStoragePropertyDefinitionFactory (
          _storageTypeCalculatorStub, _storageNameProviderStub, _storageProviderDefinitionFinder);
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
    }

    [Test]
    public void CreateColumnDefinition_RelationProperty ()
    {
      var propertyDefinition = PropertyDefinitionFactory.Create (_fileSystemItemClassDefinition, "ParentFolder");
      StubStorageCalculators (propertyDefinition);

      var result = (SimpleStoragePropertyDefinition) _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result.Name, Is.EqualTo ("FakeColumnName"));
      Assert.That (result.ColumnDefinition.PropertyType, Is.SameAs (typeof (ObjectID)));
    }

    [Test]
    public void CreateColumnDefinition_PropertyType ()
    {
      var propertyDefinition = PropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));
      StubStorageCalculators (propertyDefinition);

      var result = (SimpleStoragePropertyDefinition) _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result.Name, Is.EqualTo ("FakeColumnName"));
      Assert.That (result.ColumnDefinition.PropertyType, Is.SameAs (typeof (bool)));
    }

    [Test]
    public void CreateColumnDefinition_StorageType ()
    {
      var propertyDefinition = PropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));
      StubStorageCalculators (propertyDefinition);

      var result = (SimpleStoragePropertyDefinition) _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result.Name, Is.EqualTo ("FakeColumnName"));
      Assert.That (result.ColumnDefinition.StorageTypeInfo.StorageType, Is.EqualTo ("storage type"));
    }

    [Test]
    public void CreateColumnDefinition_IsNullable ()
    {
      var nullablePropertyDefinition = PropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, typeof (ClassWithAllDataTypes), "StringProperty", "StringProperty", true);
      var nonNullablePropertyDefinition = PropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, typeof (ClassWithAllDataTypes), "StringProperty", "StringProperty", false);
      StubStorageCalculators (nullablePropertyDefinition);
      StubStorageCalculators (nonNullablePropertyDefinition);

      var nullableResult =
          (SimpleStoragePropertyDefinition) _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (nullablePropertyDefinition);
      var nonNullableResult =
          (SimpleStoragePropertyDefinition) _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (nonNullablePropertyDefinition);

      Assert.That (nullableResult.ColumnDefinition.IsNullable, Is.True);
      Assert.That (nonNullableResult.ColumnDefinition.IsNullable, Is.False);
    }

    [Test]
    public void CreateColumnDefinition_NotSupportedType ()
    {
      var propertyDefinition = PropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition)).Return (null);

      var result = _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (UnsupportedStoragePropertyDefinition)));
    }

    [Test]
    public void CreateColumnDefinition_ClassAboveDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionFactory.Create (
          _classAboveDbTableAttribute, StorageClass.Persistent, typeof (ClassNotInMapping).GetProperty ("RelationProperty"), false);
      var propertyDefinitionNullable = PropertyDefinitionFactory.Create (
          _classAboveDbTableAttribute, StorageClass.Persistent, typeof (ClassNotInMapping).GetProperty ("RelationProperty"), true);

      StubStorageCalculators (propertyDefinitionNotNullable);
      StubStorageCalculators (propertyDefinitionNullable);

      Assert.That (_classAboveDbTableAttribute.BaseClass, Is.Null);

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      Assert.That (resultNotNullable.ColumnDefinition.IsNullable, Is.False);
      Assert.That (resultNullable.ColumnDefinition.IsNullable, Is.True);
    }

    [Test]
    public void CreateColumnDefinition_ClassWithDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionFactory.Create (
          _classWithDbTableAttribute, StorageClass.Persistent, typeof (Company).GetProperty ("Name"), false);
      var propertyDefinitionNullable = PropertyDefinitionFactory.Create (
          _classWithDbTableAttribute, StorageClass.Persistent, typeof (Company).GetProperty ("Name"), true);

      StubStorageCalculators (propertyDefinitionNotNullable);
      StubStorageCalculators (propertyDefinitionNullable);

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      Assert.That (resultNotNullable.ColumnDefinition.IsNullable, Is.False);
      Assert.That (resultNullable.ColumnDefinition.IsNullable, Is.True);
    }

    [Test]
    public void CreateColumnDefinition_ClassBelowDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionFactory.Create (
          _classBelowDbTableAttribute, StorageClass.Persistent, typeof (Partner).GetProperty ("ContactPerson"), false);
      var propertyDefinitionNullable = PropertyDefinitionFactory.Create (
          _classBelowDbTableAttribute, StorageClass.Persistent, typeof (Partner).GetProperty ("ContactPerson"), true);

      StubStorageCalculators (propertyDefinitionNotNullable);
      StubStorageCalculators (propertyDefinitionNullable);
      _storageNameProviderStub.Stub (stub => stub.GetTableName (_classWithDbTableAttribute)).Return (_classWithDbTableAttribute.ID);

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      Assert.That (resultNotNullable.ColumnDefinition.IsNullable, Is.True);
      Assert.That (resultNullable.ColumnDefinition.IsNullable, Is.True);
    }

    [Test]
    public void CreateColumnDefinition_ClassBelowBelowDbTableAttribute_And_PropertyDefinitionIsNotNullabe ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionFactory.Create (
          _classBelowBelowDbTableAttribute,
          StorageClass.Persistent,
          typeof (Distributor).GetProperty ("ClassWithoutRelatedClassIDColumn", BindingFlags.NonPublic | BindingFlags.Instance),
          false);
      var propertyDefinitionNullable = PropertyDefinitionFactory.Create (
          _classBelowBelowDbTableAttribute,
          StorageClass.Persistent,
          typeof (Distributor).GetProperty ("ClassWithoutRelatedClassIDColumn", BindingFlags.NonPublic | BindingFlags.Instance),
          true);

      StubStorageCalculators (propertyDefinitionNotNullable);
      StubStorageCalculators (propertyDefinitionNullable);
      _storageNameProviderStub.Stub (stub => stub.GetTableName (_classWithDbTableAttribute)).Return (_classWithDbTableAttribute.ID);

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      Assert.That (resultNotNullable.ColumnDefinition.IsNullable, Is.True);
      Assert.That (resultNullable.ColumnDefinition.IsNullable, Is.True);
    }

    [Test]
    public void CreaeStoragePropertyDefinition_NonRelationProperty_GetsNoClassIdColumn ()
    {
      var classDefinition = Configuration.GetTypeDefinition (typeof (Order));
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (Order).FullName + ".OrderNumber"];
      StubStorageCalculators (propertyDefinition);

      var result = _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));
    }

    [Test]
    public void CreaeStoragePropertyDefinition_RelationPropertyToAClassDefinitionThatISNotPartOfTheMapping_GetsNotClassIDColumn ()
    {
      var classDefinition = Configuration.GetTypeDefinition (typeof (ClassWithManySideRelationProperties));
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (ClassWithManySideRelationProperties).FullName + ".BidirectionalOneToOne"];
      StubStorageCalculators (propertyDefinition);

      var result = _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (ObjectIDWithoutClassIDStoragePropertyDefinition)));
    }

    [Test]
    public void CreaeStoragePropertyDefinition_RelationPropertyToAClassDefinitionWithBaseClassAndNoDerivedClasses_GetsClassIDColumn ()
    {
      var classDefinition = Configuration.GetTypeDefinition (typeof (ClassWithoutRelatedClassIDColumn));
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (ClassWithoutRelatedClassIDColumn).FullName + ".Distributor"];
      StubStorageCalculators (propertyDefinition);

      var result = _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

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
      Assert.That (valueIDColumn.StorageTypeInfo.StorageType, Is.EqualTo ("guid"));
      Assert.That (valueIDColumn.IsPartOfPrimaryKey, Is.False);
      Assert.That (classIDColumn.Name, Is.EqualTo ("FakeRelationClassID"));
      Assert.That (classIDColumn.IsNullable, Is.True);
      Assert.That (classIDColumn.PropertyType, Is.SameAs (typeof (string)));
      Assert.That (classIDColumn.StorageTypeInfo.StorageType, Is.EqualTo ("varchar(100)"));
      Assert.That (classIDColumn.IsPartOfPrimaryKey, Is.False);
    }

    [Test]
    public void CreaeStoragePropertyDefinition_RelationPropertyToAClassDefinitionWithoutBaseClassAndWithDerivedClasses_GetsClassIDColumn ()
    {
      var classDefinition = Configuration.GetTypeDefinition (typeof (Ceo));
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (Ceo).FullName + ".Company"];
      StubStorageCalculators (propertyDefinition);

      var result = _rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

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
      var rdbmsStoragePropertyDefinitionFactory = new RdbmsStoragePropertyDefinitionFactory (
          _storageTypeCalculatorStub, _storageNameProviderStub, storageProviderDefinitionFinder);

      var result = rdbmsStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);

      Assert.That (result, Is.TypeOf (typeof (SerializedObjectIDStoragePropertyDefinition)));
    }

    [Test]
    public void CreateObjectIDColumnDefinition ()
    {
      var result = _rdbmsStoragePropertyDefinitionFactory.CreateObjectIDColumnDefinition();

      Assert.That (result.Name, Is.EqualTo ("ID"));
      Assert.That (result.IsNullable, Is.False);
      Assert.That (result.IsPartOfPrimaryKey, Is.True);
      Assert.That (result.PropertyType, Is.SameAs (typeof (ObjectID)));
      Assert.That (result.StorageTypeInfo.StorageType, Is.EqualTo ("guid"));
    }

    [Test]
    public void CreateClassIDColumnDefinition ()
    {
      var result = _rdbmsStoragePropertyDefinitionFactory.CreateClassIDColumnDefinition();

      Assert.That (result.Name, Is.EqualTo ("ClassID"));
      Assert.That (result.IsNullable, Is.False);
      Assert.That (result.PropertyType, Is.SameAs (typeof (string)));
      Assert.That (result.StorageTypeInfo.StorageType, Is.EqualTo ("varchar(100)"));
      Assert.That (result.IsPartOfPrimaryKey, Is.False);
    }

    [Test]
    public void CreateTimestampColumnDefinition ()
    {
      var result = _rdbmsStoragePropertyDefinitionFactory.CreateTimestampColumnDefinition();

      Assert.That (result.Name, Is.EqualTo ("Timestamp"));
      Assert.That (result.IsNullable, Is.False);
      Assert.That (result.PropertyType, Is.SameAs (typeof (object)));
      Assert.That (result.StorageTypeInfo.StorageType, Is.EqualTo ("rowversion"));
      Assert.That (result.IsPartOfPrimaryKey, Is.False);
    }

    private void StubStorageCalculators (PropertyDefinition propertyDefinition)
    {
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition)).Return (
          new StorageTypeInformation ("storage type", DbType.String, typeof(string), new StringConverter()));
      _storageNameProviderStub.Stub (stub => stub.GetColumnName (propertyDefinition)).Return ("FakeColumnName");
      _storageNameProviderStub.Stub (stub => stub.GetRelationColumnName (propertyDefinition)).Return ("FakeColumnNameID");
      _storageNameProviderStub.Stub (stub => stub.GetRelationClassIDColumnName (propertyDefinition)).Return ("FakeRelationClassID");
    }
  }
}