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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
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
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
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
  public class RdbmsProviderCommandFactoryTest : SqlProviderBaseTest
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
      _factory = new RdbmsProviderCommandFactory (
          new SqlDbCommandBuilderFactory(SqlDialect.Instance, ValueConverter, TestDomainStorageProviderDefinition),
          rdbmsPersistenceModelProvider,
          new InfrastructureStoragePropertyDefinitionProvider (StorageTypeInformationProvider, StorageNameProvider),
          new ObjectReaderFactory (rdbmsPersistenceModelProvider),
          new TableDefinitionFinder (rdbmsPersistenceModelProvider));

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
      var objectID = CreateObjectID (_tableDefinition1);
      
      var result = _factory.CreateForSingleIDLookup (objectID);

      var innerCommand = CheckDelegateBasedCommandAndReturnInnerCommand<DataContainer, ObjectLookupResult<DataContainer>> (result);
      Assert.That (innerCommand, Is.TypeOf (typeof (SingleObjectLoadCommand<DataContainer>)));
    }

    [Test]
    public void CreateForSortedMultiIDLookup ()
    {
      var result = _factory.CreateForSortedMultiIDLookup (new[] { _objectID1 });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));
    }

    [Test]
    public void CreateForRelationLookup ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);
      
      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      Assert.That (result, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));
    }

    [Test]
    public void CreateForDataContainerQuery ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery>();

      var result = _factory.CreateForDataContainerQuery (queryStub);

      Assert.That (result, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));
    }

    [Test]
    public void CreateForMultiTimestampLookup ()
    {
      var result = _factory.CreateForMultiTimestampLookup (new[] { _objectID1, _objectID2, _objectID3 });

      var innerCommand =
          CheckDelegateBasedCommandAndReturnInnerCommand<IEnumerable<Tuple<ObjectID, object>>, IEnumerable<ObjectLookupResult<object>>> (result);
      Assert.That (innerCommand, Is.TypeOf (typeof (MultiObjectLoadCommand<Tuple<ObjectID, object>>)));
    }

    [Test]
    public void CreateForSave_New ()
    {
      var dataContainerNew1 = DataContainer.CreateNew (DomainObjectIDs.Computer1);
      SetPropertyValue (dataContainerNew1, typeof (Computer), "SerialNumber", "123456");
      var dataContainerNew2 = DataContainer.CreateNew (DomainObjectIDs.Computer2);
      SetPropertyValue (dataContainerNew2, typeof (Computer), "SerialNumber", "654321");

      dataContainerNew2.SetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber", "654321");
      var dataContainerNewWithoutRelations = DataContainer.CreateNew (DomainObjectIDs.Official1);

      var result = _factory.CreateForSave (
          new[]
          {
              dataContainerNew1,
              dataContainerNew2,
              dataContainerNewWithoutRelations
          });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSaveCommand)));
    }

    [Test]
    public void CreateForSave_Changed ()
    {
      var dataContainerChangedSerialNumber = DataContainer.CreateForExisting (DomainObjectIDs.Computer1, null, pd => pd.DefaultValue);
      dataContainerChangedSerialNumber.SetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber", "123456");
      var dataContainerChangedEmployee = DataContainer.CreateForExisting (DomainObjectIDs.Computer2, null, pd => pd.DefaultValue);
      dataContainerChangedEmployee.SetValue (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee", DomainObjectIDs.Employee2);
      var dataContainerChangedMarkedAsChanged = DataContainer.CreateForExisting (DomainObjectIDs.Computer3, null, pd => pd.DefaultValue);
      dataContainerChangedMarkedAsChanged.MarkAsChanged();

      var result =
          _factory.CreateForSave (new[] { dataContainerChangedSerialNumber, dataContainerChangedEmployee, dataContainerChangedMarkedAsChanged });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSaveCommand)));
    }

    [Test]
    public void CreateForSave_Deleted ()
    {
      var dataContainerDeletedWithoutRelations = DataContainer.CreateForExisting (DomainObjectIDs.Official1, null, pd => pd.DefaultValue);
      dataContainerDeletedWithoutRelations.Delete();
      var dataContainerDeletedWithRelations1 = DataContainer.CreateForExisting (DomainObjectIDs.Computer1, null, pd => pd.DefaultValue);
      dataContainerDeletedWithRelations1.Delete();
      var dataContainerDeletedWithRelations2 = DataContainer.CreateForExisting (DomainObjectIDs.Computer2, null, pd => pd.DefaultValue);
      dataContainerDeletedWithRelations2.Delete();

      var result = _factory.CreateForSave (
          new[]
          {
              dataContainerDeletedWithoutRelations,
              dataContainerDeletedWithRelations1,
              dataContainerDeletedWithRelations2
          });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSaveCommand)));
    }

    [Test]
    public void CreateForSave_Unchanged ()
    {
      var dataContainerUnchanged = DataContainer.CreateForExisting (DomainObjectIDs.Order3, null, pd => pd.DefaultValue);

      var result = _factory.CreateForSave (new[] { dataContainerUnchanged });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSaveCommand)));
    }

    private ObjectID CreateObjectID (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetStorageEntity (entityDefinition);

      return new ObjectID (classDefinition, Guid.NewGuid());
    }

    private RelationEndPointDefinition CreateForeignKeyEndPointDefinition (ClassDefinition classDefinition)
    {
      var idPropertyDefinition = CreateForeignKeyPropertyDefinition (classDefinition);
      return new RelationEndPointDefinition (idPropertyDefinition, false);
    }

    private PropertyDefinition CreateForeignKeyPropertyDefinition (ClassDefinition classDefinition)
    {
      return PropertyDefinitionFactory.Create (
          classDefinition,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("OrderTicket"),
          _foreignKeyColumnDefinition);
    }

    
    private IStorageProviderCommand<TIn, IRdbmsProviderCommandExecutionContext> CheckDelegateBasedCommandAndReturnInnerCommand<TIn, TResult> (
        IStorageProviderCommand<TResult, IRdbmsProviderCommandExecutionContext> command)
    {
      Assert.That (
          command,
          Is.TypeOf (typeof (DelegateBasedStorageProviderCommand<TIn, TResult, IRdbmsProviderCommandExecutionContext>)));
      return ((DelegateBasedStorageProviderCommand<TIn, TResult, IRdbmsProviderCommandExecutionContext>) command).Command;
    }
  }
}