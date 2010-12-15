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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Rhino.Mocks;
using ClassNotInMapping = Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.RelationReflector.RelatedPropertyTypeIsNotInMapping.ClassNotInMapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ColumnDefinitionFactoryTest : StandardMappingTest
  {
    private StorageTypeCalculator _storageTypeCalculatorStub;
    private ColumnDefinitionFactory _columnDefinitionFactory;
    private ReflectionBasedClassDefinition _classWithAllDataTypesDefinition;
    private ReflectionBasedClassDefinition _fileSystemItemClassDefinition;
    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;
    private ReflectionBasedClassDefinition _classAboveDbTableAttribute;
    private ReflectionBasedClassDefinition _classWithDbTableAttribute;
    private ReflectionBasedClassDefinition _classBelowDbTableAttribute;
    private ReflectionBasedClassDefinition _classBelowBelowDbTableAttribute;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
      _storageTypeCalculatorStub = MockRepository.GenerateStub<StorageTypeCalculator> ();
      _storageTypeCalculatorStub.Stub (stub => stub.SqlDataTypeClassID).Return ("varchar(100)");
      _columnDefinitionFactory = new ColumnDefinitionFactory (_storageTypeCalculatorStub);
      _classWithAllDataTypesDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithAllDataTypes));
      _classWithAllDataTypesDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _fileSystemItemClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (FileSystemItem));
      _fileSystemItemClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _classAboveDbTableAttribute = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassNotInMapping));
      _classAboveDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classWithDbTableAttribute = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Company), _classAboveDbTableAttribute);
      _classWithDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classBelowDbTableAttribute = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Partner), _classWithDbTableAttribute);
      _classBelowDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _classBelowBelowDbTableAttribute = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Distributor), _classBelowDbTableAttribute);
      _classBelowBelowDbTableAttribute.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
    }

    [Test]
    public void CreateStoragePropertyDefinition_GetNameFromAttribute ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));
      StubStorageTypeCalculator (propertyDefinition);

      var result = _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.Name, Is.EqualTo ("Boolean"));
    }

    [Test]
    public void CreateStoragePropertyDefinition_UsePropertyName ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Distributor));
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition, StorageClass.Persistent, typeof (Distributor).GetProperty ("NumberOfShops"));
      StubStorageTypeCalculator (propertyDefinition);

      var result = _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.Name, Is.EqualTo ("NumberOfShops"));
    }

    [Test]
    public void CreateStoragePropertyDefinition_UsePropertyName_RelationProperty ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (_fileSystemItemClassDefinition, "ParentFolder", typeof (ObjectID));
      StubStorageTypeCalculator (propertyDefinition);
      
      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.Name, Is.EqualTo ("ParentFolderID"));
      Assert.That (result.PropertyType, Is.SameAs (typeof (ObjectID)));
    }

    [Test]
    public void CreateStoragePropertyDefinition_PropertyType ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));
      StubStorageTypeCalculator (propertyDefinition);

      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.PropertyType, Is.SameAs (typeof (bool)));
    }

    [Test]
    public void CreateStoragePropertyDefinition_StorageType ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));
      StubStorageTypeCalculator (propertyDefinition);

      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.StorageType, Is.EqualTo ("storage type"));
    }

    [Test]
    public void CreateStoragePropertyDefinition_IsNullable ()
    {
      var nullablePropertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, typeof (ClassWithAllDataTypes), "StringProperty", "StringProperty", typeof (string), true);
      var nonNullablePropertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, typeof (ClassWithAllDataTypes), "StringProperty", "StringProperty", typeof (string), false);
      StubStorageTypeCalculator (nullablePropertyDefinition);
      StubStorageTypeCalculator (nonNullablePropertyDefinition);

      var nullableResult = (SimpleColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (nullablePropertyDefinition, _storageProviderDefinitionFinder);
      var nonNullableResult = (SimpleColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (nonNullablePropertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (nullableResult.IsNullable, Is.True);
      Assert.That (nonNullableResult.IsNullable, Is.False);
    }

    [Test]
    public void CreateStoragePropertyDefinition_NotSupportedType ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return (null);

      var result = _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result, Is.TypeOf(typeof(UnsupportedStorageTypeColumnDefinition)));
      Assert.That (((UnsupportedStorageTypeColumnDefinition) result).Name, Is.EqualTo ("Boolean"));

    }

    [Test]
    public void CreateStoragePropertyDefinition_ClassAboveDbTableAttribute_And_PropertyDefinitionIsNotNullabe ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classAboveDbTableAttribute, StorageClass.Persistent, typeof (ClassNotInMapping).GetProperty ("RelationProperty"), false);
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("test");
      
      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.IsNullable, Is.False);
    }

    [Test]
    public void CreateStoragePropertyDefinition_ClassAboveDbTableAttribute_And_PropertyDefinitionIsNullabe ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classAboveDbTableAttribute, StorageClass.Persistent, typeof (ClassNotInMapping).GetProperty ("RelationProperty"), true);
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("test");

      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.IsNullable, Is.True);
    }

    [Test]
    public void CreateStoragePropertyDefinition_ClassWithDbTableAttribute_And_PropertyDefinitionIsNotNullabe ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithDbTableAttribute, StorageClass.Persistent, typeof (Company).GetProperty ("Name"), false);
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("test");

      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.IsNullable, Is.False);
    }

    [Test]
    public void CreateStoragePropertyDefinition_ClassWithDbTableAttribute_And_PropertyDefinitionIsNullabe ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithDbTableAttribute, StorageClass.Persistent, typeof (Company).GetProperty ("Name"), true);
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("test");

      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.IsNullable, Is.True);
    }

    [Test]
    public void CreateStoragePropertyDefinition_ClassBelowDbTableAttribute_And_PropertyDefinitionIsNotNullabe ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classBelowDbTableAttribute, StorageClass.Persistent, typeof (Partner).GetProperty ("ContactPerson"), false);
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("test");

      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.IsNullable, Is.True);
    }

    [Test]
    public void CreateStoragePropertyDefinition_ClassBelowDbTableAttribute_And_PropertyDefinitionIsNullabe ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classBelowDbTableAttribute, StorageClass.Persistent, typeof (Partner).GetProperty ("ContactPerson"), true);
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("test");

      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.IsNullable, Is.True);
    }

    [Test]
    public void CreateStoragePropertyDefinition_ClassBelowBelowDbTableAttribute_And_PropertyDefinitionIsNotNullabe ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classBelowBelowDbTableAttribute, StorageClass.Persistent, typeof (Distributor).GetProperty ("ClassWithoutRelatedClassIDColumn"), false);
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("test");

      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.IsNullable, Is.True);
    }

    [Test]
    public void CreateStoragePropertyDefinition_ClassBelowBelowDbTableAttribute_And_PropertyDefinitionIsNullabe ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classBelowBelowDbTableAttribute, StorageClass.Persistent, typeof (Distributor).GetProperty ("ClassWithoutRelatedClassIDColumn"), true);
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("test");

      var result = (SimpleColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.IsNullable, Is.True);
    }

    [Test]
    public void CreaeStoragePropertyDefinition_NonRelationProperty_GetsNoClassIdColumn ()
    {
      var classDefinition = Configuration.ClassDefinitions[typeof (Order)];
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof(Order).FullName+".OrderNumber"];

      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("test");

      var result = _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result, Is.TypeOf(typeof(SimpleColumnDefinition)));
    }

    [Test]
    public void CreaeStoragePropertyDefinition_RelationPropertyToAClassDefinitionThatISNotPartOfTheMapping_GetsNotClassIDColumn ()
    {
      var classDefinition = Configuration.ClassDefinitions[typeof (ClassWithManySideRelationProperties)];
      
      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (ClassWithManySideRelationProperties).FullName + ".BidirectionalOneToOne"];

      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("test");

      var result = _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result, Is.TypeOf (typeof (SimpleColumnDefinition)));
    }

    [Test]
    public void CreaeStoragePropertyDefinition_RelationPropertyToAClassDefinitionWithBaseClassAndNoDerivedClasses_GetsClassIDColumn ()
    {
      var classDefinition = Configuration.ClassDefinitions[typeof (ClassWithoutRelatedClassIDColumn)];

      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (ClassWithoutRelatedClassIDColumn).FullName + ".Distributor"];

      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("test");

      var result = _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result, Is.TypeOf (typeof (ObjectIDWithClassIDColumnDefinition)));
      Assert.That (((ObjectIDWithClassIDColumnDefinition) result).ObjectIDColumn.Name, Is.EqualTo ("DistributorID"));
      Assert.That (((ObjectIDWithClassIDColumnDefinition) result).ObjectIDColumn.IsNullable, Is.True);
      Assert.That (((ObjectIDWithClassIDColumnDefinition) result).ObjectIDColumn.PropertyType, Is.SameAs(typeof(ObjectID)));
      Assert.That (((ObjectIDWithClassIDColumnDefinition) result).ObjectIDColumn.StorageType, Is.EqualTo("test"));
      Assert.That (((ObjectIDWithClassIDColumnDefinition) result).ClassIDColumn.Name, Is.EqualTo("ClassID"));
      Assert.That (((ObjectIDWithClassIDColumnDefinition) result).ClassIDColumn.IsNullable, Is.False);
      Assert.That (((ObjectIDWithClassIDColumnDefinition) result).ClassIDColumn.PropertyType, Is.SameAs(typeof (string)));
      Assert.That (((ObjectIDWithClassIDColumnDefinition) result).ClassIDColumn.StorageType, Is.EqualTo ("varchar(100)"));
    }

    [Test]
    public void CreaeStoragePropertyDefinition_RelationPropertyToAClassDefinitionWithoutBaseClassAndWithDerivedClasses_GetsClassIDColumn ()
    {
      var classDefinition = Configuration.ClassDefinitions[typeof (Ceo)];

      var propertyDefinition = classDefinition.MyPropertyDefinitions[typeof (Ceo).FullName + ".Company"];

      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("test");

      var result = _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result, Is.TypeOf (typeof (ObjectIDWithClassIDColumnDefinition)));
    }

    private void StubStorageTypeCalculator (PropertyDefinition propertyDefinition)
    {
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("storage type");
    }

    
  }
}