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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class InfrastructureStoragePropertyDefinitionProviderTest : StandardMappingTest
  {
    private IStorageTypeInformationProvider _storageTypeInformationProviderStub;
    private IStorageNameProvider _storageNameProviderStub;

    private InfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;
    private IEntityDefinition _entityDefinitionStub;
    private ColumnDefinition _idColumn;
    private ColumnDefinition _classIdColumn;
    private ColumnDefinition _timestampColumn;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _storageTypeInformationProviderStub = MockRepository.GenerateStub<IStorageTypeInformationProvider> ();
      _storageTypeInformationProviderStub.Stub (stub => stub.GetStorageTypeForClassID()).Return (
          new StorageTypeInformation ("varchar(100)", DbType.String, typeof (string), new StringConverter()));
      _storageTypeInformationProviderStub.Stub (stub => stub.GetStorageTypeForObjectID()).Return (
          new StorageTypeInformation ("guid", DbType.Guid, typeof (Guid), new GuidConverter()));
      _storageTypeInformationProviderStub.Stub (stub => stub.GetStorageTypeForTimestamp()).Return (
          new StorageTypeInformation ("rowversion", DbType.DateTime, typeof (DateTime), new DateTimeConverter()));

      _storageNameProviderStub = MockRepository.GenerateStub<IStorageNameProvider>();
      _storageNameProviderStub.Stub (stub => stub.IDColumnName).Return ("ID");
      _storageNameProviderStub.Stub (stub => stub.ClassIDColumnName).Return ("ClassID");
      _storageNameProviderStub.Stub (stub => stub.TimestampColumnName).Return ("Timestamp");

      _entityDefinitionStub = MockRepository.GenerateStub<IEntityDefinition>();
      _idColumn = ColumnDefinitionObjectMother.IDColumn;
      _classIdColumn = ColumnDefinitionObjectMother.ClassIDColumn;
      _timestampColumn = ColumnDefinitionObjectMother.TimestampColumn;
      _entityDefinitionStub.Stub (stub => stub.IDColumn).Return (_idColumn);
      _entityDefinitionStub.Stub (stub => stub.ClassIDColumn).Return (_classIdColumn);
      _entityDefinitionStub.Stub (stub => stub.TimestampColumn).Return (_timestampColumn);

      _infrastructureStoragePropertyDefinitionProvider = 
          new InfrastructureStoragePropertyDefinitionProvider (_storageTypeInformationProviderStub, _storageNameProviderStub);
    }

    [Test]
    public void GetObjectIDStoragePropertyDefinition ()
    {
      var result = _infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition (_entityDefinitionStub);

      Assert.That (result, Is.TypeOf (typeof (ObjectIDStoragePropertyDefinition)));
      Assert.That (result.ValueProperty, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));
      Assert.That (((SimpleStoragePropertyDefinition) result.ValueProperty).ColumnDefinition, Is.SameAs (_idColumn));
      Assert.That (((SimpleStoragePropertyDefinition) result.ClassIDProperty).ColumnDefinition, Is.SameAs (_classIdColumn));
    }

    [Test]
    public void GetTimestampStoragePropertyDefinition ()
    {
      var result = _infrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition (_entityDefinitionStub);

      Assert.That (result.ColumnDefinition, Is.SameAs (_timestampColumn));
    }
    
    [Test]
    public void GetIDColumnDefinition ()
    {
      var result = _infrastructureStoragePropertyDefinitionProvider.GetIDColumnDefinition();

      Assert.That (result.Name, Is.EqualTo ("ID"));
      Assert.That (result.IsNullable, Is.False);
      Assert.That (result.IsPartOfPrimaryKey, Is.True);
      Assert.That (result.PropertyType, Is.SameAs (typeof (ObjectID)));
      Assert.That (result.StorageTypeInfo.StorageTypeName, Is.EqualTo ("guid"));
    }

    [Test]
    public void GetIDColumnDefinition_CachedInstance ()
    {
      var result = _infrastructureStoragePropertyDefinitionProvider.GetIDColumnDefinition ();
      var result2 = _infrastructureStoragePropertyDefinitionProvider.GetIDColumnDefinition ();
      Assert.That (result2, Is.SameAs (result));
    }

    [Test]
    public void GetClassIDColumnDefinition ()
    {
      var result = _infrastructureStoragePropertyDefinitionProvider.GetClassIDColumnDefinition();

      Assert.That (result.Name, Is.EqualTo ("ClassID"));
      Assert.That (result.IsNullable, Is.False);
      Assert.That (result.PropertyType, Is.SameAs (typeof (string)));
      Assert.That (result.StorageTypeInfo.StorageTypeName, Is.EqualTo ("varchar(100)"));
      Assert.That (result.IsPartOfPrimaryKey, Is.False);
    }

    [Test]
    public void GetClassIDColumnDefinition_CachedInstance ()
    {
      var result = _infrastructureStoragePropertyDefinitionProvider.GetClassIDColumnDefinition ();
      var result2 = _infrastructureStoragePropertyDefinitionProvider.GetClassIDColumnDefinition ();
      Assert.That (result2, Is.SameAs (result));
    }

    [Test]
    public void GetTimestampColumnDefinition ()
    {
      var result = _infrastructureStoragePropertyDefinitionProvider.GetTimestampColumnDefinition();

      Assert.That (result.Name, Is.EqualTo ("Timestamp"));
      Assert.That (result.IsNullable, Is.False);
      Assert.That (result.PropertyType, Is.SameAs (typeof (object)));
      Assert.That (result.StorageTypeInfo.StorageTypeName, Is.EqualTo ("rowversion"));
      Assert.That (result.IsPartOfPrimaryKey, Is.False);
    }

    [Test]
    public void GetTimestampColumnDefinition_CachedInstance ()
    {
      var result = _infrastructureStoragePropertyDefinitionProvider.GetTimestampColumnDefinition ();
      var result2 = _infrastructureStoragePropertyDefinitionProvider.GetTimestampColumnDefinition ();
      Assert.That (result2, Is.SameAs (result));
    }
  }
}