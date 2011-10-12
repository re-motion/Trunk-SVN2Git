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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class RdbmsProviderCommandFactoryTest : StandardMappingTest
  {
    private RdbmsProviderCommandFactory _factory;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private ObjectID _objectID1;
    private ObjectID _objectID2;
    private ObjectID _objectID3;
    private ObjectID _foreignKeyValue;
    private ObjectIDStoragePropertyDefinition _foreignKeyColumnDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      var rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider();
      var storageTypeInformationProvider = new SqlStorageTypeInformationProvider ();
      var storageNameProvider = new ReflectionBasedStorageNameProvider();

      var infrastructureStoragePropertyDefinitionProvider = 
          new InfrastructureStoragePropertyDefinitionProvider (storageTypeInformationProvider, storageNameProvider);
      var storageProviderDefinitionFinder = new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage);
      var dataStoragePropertyDefinitionFactory = new DataStoragePropertyDefinitionFactory (
          TestDomainStorageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider,
          storageProviderDefinitionFinder);
      _factory = new RdbmsProviderCommandFactory (TestDomainStorageProviderDefinition,
                                                  new SqlDbCommandBuilderFactory (SqlDialect.Instance),
                                                  rdbmsPersistenceModelProvider,
                                                  new ObjectReaderFactory (
                                                      rdbmsPersistenceModelProvider, infrastructureStoragePropertyDefinitionProvider),
                                                  new TableDefinitionFinder (rdbmsPersistenceModelProvider),
                                                  dataStoragePropertyDefinitionFactory);

      _tableDefinition1 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table1"));
      _tableDefinition2 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table2"));

      _foreignKeyValue = CreateObjectID (_tableDefinition1);
      _foreignKeyColumnDefinition = new ObjectIDStoragePropertyDefinition (
          SimpleStoragePropertyDefinitionObjectMother.IDProperty, SimpleStoragePropertyDefinitionObjectMother.ClassIDProperty);

      _objectID1 = CreateObjectID (_tableDefinition1);
      _objectID2 = CreateObjectID (_tableDefinition1);
      _objectID3 = CreateObjectID (_tableDefinition2);
    }

    [Test]
    public void CreateForSingleIDLookup ()
    {
      var result = _factory.CreateForSingleIDLookup (_objectID1);

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    public void CreateForSortedMultiIDLookup ()
    {
      var result = _factory.CreateForSortedMultiIDLookup (new[] { _objectID1 });

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    public void CreateForRelationLookup ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    public void CreateForDataContainerQuery ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery>();
      queryStub.Stub (stub => stub.Statement).Return ("Statement");
      queryStub.Stub (stub => stub.Parameters).Return (new QueryParameterCollection());

      var result = _factory.CreateForDataContainerQuery (queryStub);

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    public void CreateForScalarQuery ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery> ();
      queryStub.Stub (stub => stub.Statement).Return ("Statement");
      queryStub.Stub (stub => stub.Parameters).Return (new QueryParameterCollection ());

      var result = _factory.CreateForScalarQuery (queryStub);

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    public void CreateForMultiTimestampLookup ()
    {
      var result = _factory.CreateForMultiTimestampLookup (new[] { _objectID1, _objectID2, _objectID3 });

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    public void CreateForSave ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Computer1);
      SetPropertyValue (dataContainer, typeof (Computer), "SerialNumber", "123456");
      var result = _factory.CreateForSave (new[] { dataContainer });

      Assert.That (result, Is.Not.Null);
    }

    private ObjectID CreateObjectID (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetStorageEntity (entityDefinition);

      return new ObjectID (classDefinition, Guid.NewGuid());
    }

    private RelationEndPointDefinition CreateForeignKeyEndPointDefinition (ClassDefinition classDefinition)
    {
      var idPropertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("OrderTicket"),
          _foreignKeyColumnDefinition);

      return new RelationEndPointDefinition (idPropertyDefinition, false);
    }
  }
}