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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ReflectionBasedStorageNameProviderTest
  {
    private ReflectionBasedStorageNameProvider _provider;
    private ReflectionBasedClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _provider = new ReflectionBasedStorageNameProvider();
      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Company), null);
    }

    [Test]
    public void IDColumnName ()
    {
      Assert.That (_provider.IDColumnName, Is.EqualTo ("ID"));
    }

    [Test]
    public void ClassIDColumnName ()
    {
      Assert.That (_provider.ClassIDColumnName, Is.EqualTo ("ClassID"));
    }

    [Test]
    public void TimestampColumnName ()
    {
      Assert.That (_provider.TimestampColumnName, Is.EqualTo ("Timestamp"));
    }

    [Test]
    public void GetTableName_ClassHasDBTableAttributeWithoutName_ReturnsClassIDName ()
    {
      var result = _provider.GetTableName (_classDefinition);

      Assert.That (result, Is.EqualTo ("Company"));
    }

    [Test]
    public void GetTableName_ClassHasDBTableAttributeWithtName_ReturnsAttributeName ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (ClassHavingStorageSpecificIdentifierAttribute), null);

      var result = _provider.GetTableName (classDefinition);

      Assert.That (result, Is.EqualTo ("ClassHavingStorageSpecificIdentifierAttributeTable"));
    }

    [Test]
    public void GetTableName_ClassHasNoDBTableAttribute_ReturnsNull ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Folder), null);

      var result = _provider.GetTableName (classDefinition);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetViewName ()
    {
      var result = _provider.GetViewName (_classDefinition);

      Assert.That (result, Is.EqualTo ("CompanyView"));
    }

    [Test]
    public void GetColumnName_PropertyWithIStorageSpecificIdentifierAttribute_ReturnsNameFromAttribute ()
    {
      var classWithAllDataTypesDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (ClassWithAllDataTypes), null);
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          classWithAllDataTypesDefinition, StorageClass.Persistent, typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty"));

      var result = _provider.GetColumnName (propertyDefinition);

      Assert.That (result, Is.EqualTo ("Boolean"));
    }

    [Test]
    public void GetColumnName_PropertyWithoutIStorageSpecificIdentifierAttribute_ReturnsPropertyName ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Distributor), null);
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition, StorageClass.Persistent, typeof (Distributor).GetProperty ("NumberOfShops"));

      var result = _provider.GetColumnName (propertyDefinition);

      Assert.That (result, Is.EqualTo ("NumberOfShops"));
    }

    [Test]
    public void GetColumnName_RelationProperty_ReturnsRelationIDName ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (FileSystemItem), null);
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, "ParentFolder", typeof (ObjectID));

      var result = _provider.GetColumnName (propertyDefinition);

      Assert.That (result, Is.EqualTo ("ParentFolderID"));
    }

    [Test]
    public void GetRelationClassIDColumnName_PropertyDefinitionParameter ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (FileSystemItem), null);
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, "ParentFolder", typeof (ObjectID));

      var result = _provider.GetRelationClassIDColumnName (propertyDefinition);

      Assert.That (result, Is.EqualTo ("ParentFolderIDClassID"));
    }

    [Test]
    public void GetRelationClassIDColumnName_ColumnNameParameter ()
    {
      var result = _provider.GetRelationClassIDColumnName ("ColumnName");

      Assert.That (result, Is.EqualTo ("ColumnNameClassID"));
    }

    [Test]
    public void GetPrimaryKeyName ()
    {
      var result = _provider.GetPrimaryKeyConstraintName(_classDefinition);

      Assert.That (result, Is.EqualTo ("PK_Company"));
    }

    [Test]
    public void GetForeignKeyConstraintName ()
    {
      var storagePropertyDefinitionStub = MockRepository.GenerateStub<IStoragePropertyDefinition>();
      storagePropertyDefinitionStub.Stub (stub => stub.Name).Return ("FakeStorageName");

      var result = _provider.GetForeignKeyConstraintName (_classDefinition, storagePropertyDefinitionStub);

      Assert.That (result, Is.EqualTo ("FK_Company_FakeStorageName"));
    }
  }
}