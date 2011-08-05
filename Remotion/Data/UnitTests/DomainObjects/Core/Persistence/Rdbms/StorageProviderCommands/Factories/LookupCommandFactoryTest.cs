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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class LookupCommandFactoryTest : StandardMappingTest
  {
    private RdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
    private TableDefinitionFinder _tableDefinitionFinder;

    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStrictMock;
    private IObjectReaderFactory _objectReaderFactoryStrictMock;
    private IDbCommandBuilder _dbCommandBuilder1Stub;
    private IDbCommandBuilder _dbCommandBuilder2Stub;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private UnionViewDefinition _unionViewDefinition;
    private ObjectID _foreignKeyValue;
    private ObjectIDStoragePropertyDefinition _foreignKeyColumnDefinition;
    private ObjectID _objectID1;
    private ObjectID _objectID2;
    private ObjectID _objectID3;
    private IObjectReader<Tuple<ObjectID, object>> _timestampReader1Stub;
    private IObjectReader<Tuple<ObjectID, object>> _timestampReader2Stub;
    private IObjectReader<DataContainer> _dataContainerReader1Stub;
    private IObjectReader<DataContainer> _dataContainerReader2Stub;
    private IObjectReader<ObjectID> _objectIDReader1Stub;
    private LookupCommandFactory _factory;

    public override void SetUp ()
    {
      base.SetUp();

      _rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider();
      _tableDefinitionFinder = new TableDefinitionFinder (_rdbmsPersistenceModelProvider);

      _dbCommandBuilderFactoryStrictMock = MockRepository.GenerateStrictMock<IDbCommandBuilderFactory>();

      _objectReaderFactoryStrictMock = MockRepository.GenerateStrictMock<IObjectReaderFactory>();

      _factory = new LookupCommandFactory (
          MockRepository.GenerateStub<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>>(),
          _dbCommandBuilderFactoryStrictMock,
          _rdbmsPersistenceModelProvider,
          _objectReaderFactoryStrictMock,
          _tableDefinitionFinder,
          new SqlStorageTypeInformationProvider(),
          TestDomainStorageProviderDefinition);

      _dbCommandBuilder1Stub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dbCommandBuilder2Stub = MockRepository.GenerateStub<IDbCommandBuilder>();

      _tableDefinition1 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table1"));
      _tableDefinition2 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table2"));
      _unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "ViewName"),
          _tableDefinition1);

      _foreignKeyValue = CreateObjectID (_tableDefinition1);
      _foreignKeyColumnDefinition = new ObjectIDStoragePropertyDefinition (
          SimpleStoragePropertyDefinitionObjectMother.IDProperty, SimpleStoragePropertyDefinitionObjectMother.ClassIDProperty);

      _objectID1 = CreateObjectID (_tableDefinition1);
      _objectID2 = CreateObjectID (_tableDefinition1);
      _objectID3 = CreateObjectID (_tableDefinition2);

      _timestampReader1Stub = MockRepository.GenerateStub<IObjectReader<Tuple<ObjectID, object>>>();
      _timestampReader2Stub = MockRepository.GenerateStub<IObjectReader<Tuple<ObjectID, object>>>();

      _dataContainerReader1Stub = MockRepository.GenerateStub<IObjectReader<DataContainer>>();
      _dataContainerReader2Stub = MockRepository.GenerateStub<IObjectReader<DataContainer>>();

      _objectIDReader1Stub = MockRepository.GenerateStub<IObjectReader<ObjectID>>();
    }

    [Test]
    public void CreateForSingleIDLookup ()
    {
      var expectedSelectedColumns = _tableDefinition1.GetAllColumns().ToArray();

      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForSingleIDLookupFromTable (
                  Arg.Is (_tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg.Is (_objectID1)))
          .Return (_dbCommandBuilder1Stub);

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IEntityDefinition) _tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_dataContainerReader1Stub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForSingleIDLookup (_objectID1);

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      var innerCommand = CheckDelegateBasedCommandAndReturnInnerCommand<DataContainer, ObjectLookupResult<DataContainer>> (result);
      Assert.That (innerCommand, Is.TypeOf (typeof (SingleObjectLoadCommand<DataContainer>)));
      var loadCommand = ((SingleObjectLoadCommand<DataContainer>) innerCommand);
      Assert.That (loadCommand.DbCommandBuilder, Is.SameAs (_dbCommandBuilder1Stub));
      Assert.That (loadCommand.ObjectReader, Is.SameAs (_dataContainerReader1Stub));
    }

    [Test]
    public void CreateForSortedMultiIDLookup_SingleIDLookup ()
    {
      var expectedSelectedColumns = _tableDefinition1.GetAllColumns();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForSingleIDLookupFromTable (
                  Arg.Is (_tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg.Is (_objectID1)))
          .Return (_dbCommandBuilder1Stub);

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IEntityDefinition) _tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_dataContainerReader1Stub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForSortedMultiIDLookup (new[] { _objectID1 });

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));

      var dbCommandBuilderTuples =
          ((MultiObjectLoadCommand<DataContainer>) ((MultiDataContainerSortCommand) result).Command).DbCommandBuildersAndReaders;
      Assert.That (dbCommandBuilderTuples.Length, Is.EqualTo (1));
      Assert.That (dbCommandBuilderTuples[0].Item1, Is.SameAs (_dbCommandBuilder1Stub));
      Assert.That (dbCommandBuilderTuples[0].Item2, Is.SameAs (_dataContainerReader1Stub));
    }

    [Test]
    public void CreateForSortedMultiIDLookup_TableDefinition_MultipleIDLookup_AndMultipleTables ()
    {
      var expectedSelectedColumns1 = _tableDefinition1.GetAllColumns();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForMultiIDLookupFromTable (
                  Arg.Is (_tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns1),
                  Arg.Is (new[] { _objectID1, _objectID2 })))
          .Return (_dbCommandBuilder1Stub);

      var expectedSelectedColumns2 = _tableDefinition2.GetAllColumns();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForSingleIDLookupFromTable (
                  Arg.Is (_tableDefinition2),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (_tableDefinition2.GetAllColumns()),
                  Arg.Is (_objectID3)))
          .Return (_dbCommandBuilder2Stub);

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IEntityDefinition) _tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns1)))
          .Return (_dataContainerReader1Stub);
      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IEntityDefinition) _tableDefinition2),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns2)))
          .Return (_dataContainerReader2Stub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForSortedMultiIDLookup (new[] { _objectID1, _objectID2, _objectID3 });

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));

      var dbCommandBuilderTuples =
          ((MultiObjectLoadCommand<DataContainer>) ((MultiDataContainerSortCommand) result).Command).DbCommandBuildersAndReaders;
      Assert.That (dbCommandBuilderTuples.Length, Is.EqualTo (2));

      // Convert to Dictionary because the order of tuples is not defined
      var dbCommandBuilderDictionary = dbCommandBuilderTuples.ToDictionary (tuple => tuple.Item1, tuple => tuple.Item2);

      Assert.That (dbCommandBuilderDictionary.ContainsKey (_dbCommandBuilder1Stub), Is.True);
      Assert.That (dbCommandBuilderDictionary[_dbCommandBuilder1Stub], Is.SameAs (_dataContainerReader1Stub));
      Assert.That (dbCommandBuilderDictionary.ContainsKey (_dbCommandBuilder2Stub), Is.True);
      Assert.That (dbCommandBuilderDictionary[_dbCommandBuilder2Stub], Is.SameAs (_dataContainerReader2Stub));
    }

    [Test]
    public void CreateForRelationLookup_TableDefinition_NoSortExpression ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);
      var oppositeTable = (TableDefinition) relationEndPointDefinition.ClassDefinition.StorageEntityDefinition;

      var expectedSelectedColumns = _tableDefinition1.GetAllColumns();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForRelationLookupFromTable (
                  Arg.Is ((TableDefinition) classDefinition.StorageEntityDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg.Is (_foreignKeyColumnDefinition),
                  Arg.Is (_foreignKeyValue),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (new OrderedColumn[0])))
          .Return (_dbCommandBuilder1Stub);

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IEntityDefinition) oppositeTable),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_dataContainerReader1Stub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));
      var dbCommandBuilderTuples = ((MultiObjectLoadCommand<DataContainer>) result).DbCommandBuildersAndReaders;
      Assert.That (dbCommandBuilderTuples.Length, Is.EqualTo (1));
      Assert.That (dbCommandBuilderTuples[0].Item1, Is.SameAs (_dbCommandBuilder1Stub));
      Assert.That (dbCommandBuilderTuples[0].Item2, Is.SameAs (_dataContainerReader1Stub));
    }

    [Test]
    public void CreateForRelationLookup_TableDefinition_WithSortExpression ()
    {
      var classDefinition = CreateClassDefinition (_tableDefinition1);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);

      var spec1 = CreateSortedPropertySpecification (
          classDefinition,
          SortOrder.Descending,
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn);
      var spec2 = CreateSortedPropertySpecification (classDefinition, SortOrder.Ascending, ColumnDefinitionObjectMother.TimestampColumn);


      var expectedSelectedColumns = _tableDefinition1.GetAllColumns();
      var expectedOrderedColumns = new[]
                                   {
                                       new OrderedColumn (ColumnDefinitionObjectMother.IDColumn, SortOrder.Descending),
                                       new OrderedColumn (ColumnDefinitionObjectMother.ClassIDColumn, SortOrder.Descending),
                                       new OrderedColumn (ColumnDefinitionObjectMother.TimestampColumn, SortOrder.Ascending)
                                   };
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForRelationLookupFromTable (
                  Arg.Is ((TableDefinition) classDefinition.StorageEntityDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg.Is (_foreignKeyColumnDefinition),
                  Arg.Is (_foreignKeyValue),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (expectedOrderedColumns)))
          .Return (_dbCommandBuilder1Stub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IEntityDefinition) _tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_dataContainerReader1Stub);
      _objectReaderFactoryStrictMock.Replay();

      _factory.CreateForRelationLookup (
          relationEndPointDefinition,
          _foreignKeyValue,
          new SortExpressionDefinition (new[] { spec1, spec2 }));

      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations();
      _objectReaderFactoryStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateForRelationLookup_UnionViewDefinition_NoSortExpression ()
    {
      var classDefinition = CreateClassDefinition (_unionViewDefinition);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);

      var expectedSelectedColumns = new[] { _unionViewDefinition.IDColumn, _unionViewDefinition.ClassIDColumn };
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForRelationLookupFromUnionView (
                  Arg.Is (_unionViewDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg.Is (_foreignKeyColumnDefinition),
                  Arg.Is (_foreignKeyValue),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (new OrderedColumn[0])))
          .Return (_dbCommandBuilder1Stub);

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateObjectIDReader (
                  Arg.Is (_unionViewDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_objectIDReader1Stub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      var innerCommand =
          CheckDelegateBasedCommandAndReturnInnerCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IEnumerable<DataContainer>> (result);
      Assert.That (innerCommand, Is.TypeOf (typeof (IndirectDataContainerLoadCommand)));
      var command = (IndirectDataContainerLoadCommand) innerCommand;
      Assert.That (command.ObjectIDLoadCommand, Is.TypeOf (typeof (MultiObjectIDLoadCommand)));
      Assert.That (((MultiObjectIDLoadCommand) (command.ObjectIDLoadCommand)).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
      Assert.That (((MultiObjectIDLoadCommand) command.ObjectIDLoadCommand).ObjectIDReader, Is.SameAs (_objectIDReader1Stub));
    }

    [Test]
    public void CreateForRelationLookup_UnionViewDefinition_WithSortExpression ()
    {
      var classDefinition = CreateClassDefinition (_unionViewDefinition);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);

      var spec1 = CreateSortedPropertySpecification (
          classDefinition,
          SortOrder.Descending,
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn);
      var spec2 = CreateSortedPropertySpecification (classDefinition, SortOrder.Ascending, ColumnDefinitionObjectMother.TimestampColumn);

      var expectedSelectedColumns = new[] { _unionViewDefinition.IDColumn, _unionViewDefinition.ClassIDColumn };
      var expectedOrderedColumns = new[]
                                   {
                                       new OrderedColumn (ColumnDefinitionObjectMother.IDColumn, SortOrder.Descending),
                                       new OrderedColumn (ColumnDefinitionObjectMother.ClassIDColumn, SortOrder.Descending),
                                       new OrderedColumn (ColumnDefinitionObjectMother.TimestampColumn, SortOrder.Ascending)
                                   };
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForRelationLookupFromUnionView (
                  Arg.Is (_unionViewDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg.Is (_foreignKeyColumnDefinition),
                  Arg.Is (_foreignKeyValue),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (expectedOrderedColumns)))
          .Return (_dbCommandBuilder1Stub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateObjectIDReader (
                  Arg.Is (_unionViewDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_objectIDReader1Stub);
      _objectReaderFactoryStrictMock.Replay();

      _factory.CreateForRelationLookup (
          relationEndPointDefinition,
          _foreignKeyValue,
          new SortExpressionDefinition (new[] { spec1, spec2 }));

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateForRelationLookup_NullEntityDefinition ()
    {
      var nullEntityDefintion = new NullEntityDefinition (TestDomainStorageProviderDefinition);
      var classDefinition = CreateClassDefinition (nullEntityDefintion);
      var propertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("OrderTicket"),
          _foreignKeyColumnDefinition);
      var relationEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);

      var objectID = CreateObjectID (nullEntityDefintion);

      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub =>
              stub.CreateForSingleIDLookupFromTable (Arg.Is (_tableDefinition1), Arg<IEnumerable<ColumnDefinition>>.Is.Anything, Arg.Is (objectID)))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      Assert.That (result, Is.TypeOf (typeof (FixedValueStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>)));
      var fixedValueCommand = (FixedValueStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>) result;
      Assert.That (fixedValueCommand.Value, Is.EqualTo (Enumerable.Empty<DataContainer>()));
    }

    [Test]
    public void CreateForDataContainerQuery ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery>();
      queryStub.Stub (stub => stub.Statement).Return ("statement");
      queryStub.Stub (stub => stub.Parameters).Return (new QueryParameterCollection());

      var commandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dbCommandBuilderFactoryStrictMock.Stub (
          stub => stub.CreateForQuery (Arg.Is ("statement"), Arg<IEnumerable<QueryParameterWithType>>.List.Equal (new QueryParameterWithType[0]))).
          Return (
              commandBuilderStub);

      _objectReaderFactoryStrictMock.Expect (mock => mock.CreateDataContainerReader()).Return (_dataContainerReader1Stub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForDataContainerQuery (queryStub);

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));
      var command = ((MultiObjectLoadCommand<DataContainer>) result);
      Assert.That (command.DbCommandBuildersAndReaders.Length, Is.EqualTo (1));
      Assert.That (command.DbCommandBuildersAndReaders[0].Item1, Is.SameAs (commandBuilderStub));
      Assert.That (command.DbCommandBuildersAndReaders[0].Item2, Is.SameAs (_dataContainerReader1Stub));
    }

    [Test]
    public void CreateForMultiTimestampLookup ()
    {
      var expectedSelection1 = new[] { _tableDefinition1.IDColumn, _tableDefinition1.ClassIDColumn, _tableDefinition1.TimestampColumn };
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              mock => mock.CreateForMultiIDLookupFromTable (
                  Arg.Is (_tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelection1),
                  Arg<ObjectID[]>.List.Equal (new[] { _objectID1, _objectID2 })))
          .Return (_dbCommandBuilder1Stub);
      var expectedSelection2 = new[] { _tableDefinition2.IDColumn, _tableDefinition2.ClassIDColumn, _tableDefinition2.TimestampColumn };
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              mock => mock.CreateForSingleIDLookupFromTable (
                  Arg.Is (_tableDefinition2),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelection2),
                  Arg.Is (_objectID3)))
          .Return (_dbCommandBuilder2Stub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateTimestampReader (
                  Arg.Is (_tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelection1)))
          .Return (_timestampReader1Stub);
      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateTimestampReader (
                  Arg.Is (_tableDefinition2),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelection2)))
          .Return (_timestampReader2Stub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForMultiTimestampLookup (new[] { _objectID1, _objectID2, _objectID3 });

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations();

      var innerCommand =
          CheckDelegateBasedCommandAndReturnInnerCommand<IEnumerable<Tuple<ObjectID, object>>, IEnumerable<ObjectLookupResult<object>>> (result);
      Assert.That (innerCommand, Is.TypeOf (typeof (MultiObjectLoadCommand<Tuple<ObjectID, object>>)));

      var commandBuildersAndReaders = ((MultiObjectLoadCommand<Tuple<ObjectID, object>>) innerCommand).DbCommandBuildersAndReaders;
      Assert.That (commandBuildersAndReaders.Length, Is.EqualTo (2));
      Assert.That (commandBuildersAndReaders[0].Item1, Is.SameAs (_dbCommandBuilder1Stub));
      Assert.That (commandBuildersAndReaders[0].Item2, Is.SameAs (_timestampReader1Stub));
      Assert.That (commandBuildersAndReaders[1].Item1, Is.SameAs (_dbCommandBuilder2Stub));
      Assert.That (commandBuildersAndReaders[1].Item2, Is.SameAs (_timestampReader2Stub));
    }

    private ObjectID CreateObjectID (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetStorageEntity (entityDefinition);

      return new ObjectID (classDefinition, Guid.NewGuid());
    }

    private IStorageProviderCommand<TIn, IRdbmsProviderCommandExecutionContext> CheckDelegateBasedCommandAndReturnInnerCommand<TIn, TResult> (
        IStorageProviderCommand<TResult, IRdbmsProviderCommandExecutionContext> command)
    {
      Assert.That (
          command,
          Is.TypeOf (typeof (DelegateBasedStorageProviderCommand<TIn, TResult, IRdbmsProviderCommandExecutionContext>)));
      return ((DelegateBasedStorageProviderCommand<TIn, TResult, IRdbmsProviderCommandExecutionContext>) command).Command;
    }

    private SortedPropertySpecification CreateSortedPropertySpecification (
        ClassDefinition classDefinition,
        SortOrder sortOrder,
        ColumnDefinition sortedColumn)
    {
      return CreateSortedPropertySpecification (
          classDefinition,
          typeof (Order).GetProperty ("OrderNumber"),
          new SimpleStoragePropertyDefinition (sortedColumn),
          sortOrder);
    }

    private SortedPropertySpecification CreateSortedPropertySpecification (
        ClassDefinition classDefinition,
        SortOrder sortOrder,
        ColumnDefinition sortedColumn1,
        ColumnDefinition sortedColumn2)
    {
      return CreateSortedPropertySpecification (
          classDefinition,
          typeof (Order).GetProperty ("OrderNumber"),
          new ObjectIDStoragePropertyDefinition (
              new SimpleStoragePropertyDefinition (sortedColumn1), new SimpleStoragePropertyDefinition (sortedColumn2)),
          sortOrder);
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

    private SortedPropertySpecification CreateSortedPropertySpecification (
        ClassDefinition classDefinition, PropertyInfo propertyInfo, IStoragePropertyDefinition columnDefinition, SortOrder sortOrder)
    {
      var sortedPropertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition, StorageClass.Persistent, propertyInfo, columnDefinition);
      return new SortedPropertySpecification (sortedPropertyDefinition, sortOrder);
    }

    private ClassDefinition CreateClassDefinition (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetStorageEntity (entityDefinition);
      return classDefinition;
    }
  }
}