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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ColumnDefinitionFactoryTest
  {
    private StorageTypeCalculator _storageTypeCalculatorStub;
    private ColumnDefinitionFactory _columnDefinitionFactory;
    private ReflectionBasedClassDefinition _classWithAllDataTypesDefinition;
    private ReflectionBasedClassDefinition _fileSystemItemClassDefinition;
    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;
    private StorageProviderDefinition _storageProviderDefinition;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
      _storageProviderDefinition = DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition;
      _storageTypeCalculatorStub = MockRepository.GenerateStub<StorageTypeCalculator> ();
      _columnDefinitionFactory = new ColumnDefinitionFactory (_storageTypeCalculatorStub);
      _classWithAllDataTypesDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithAllDataTypes));
      _fileSystemItemClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (FileSystemItem));
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
      
      var result = (ColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.Name, Is.EqualTo ("ParentFolderID"));
      Assert.That (result.PropertyType, Is.SameAs (typeof (ObjectID)));
    }

    [Test]
    public void CreateStoragePropertyDefinition_PropertyType ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));
      StubStorageTypeCalculator (propertyDefinition);

      var result = (ColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.PropertyType, Is.SameAs (typeof (bool)));
    }

    [Test]
    public void CreateStoragePropertyDefinition_StorageType ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));
      StubStorageTypeCalculator (propertyDefinition);

      var result = (ColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

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

      var nullableResult = (ColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (nullablePropertyDefinition, _storageProviderDefinitionFinder);
      var nonNullableResult = (ColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (nonNullablePropertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (nullableResult.IsNullable, Is.True);
      Assert.That (nonNullableResult.IsNullable, Is.False);
    }

    [Test]
    public void CreateStoragePropertyDefinition_NotSupportedType ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          _classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return (null);

      var result = (ColumnDefinition) _columnDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);

      Assert.That (result.StorageType, Is.EqualTo ("not supported"));
    }

    private void StubStorageTypeCalculator (PropertyDefinition propertyDefinition)
    {
      _storageTypeCalculatorStub.Stub (stub => stub.GetStorageType (propertyDefinition, _storageProviderDefinitionFinder)).Return ("storage type");
    }
  }
}