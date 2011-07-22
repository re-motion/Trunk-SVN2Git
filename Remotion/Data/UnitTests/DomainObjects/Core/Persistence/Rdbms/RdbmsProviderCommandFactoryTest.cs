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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
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
    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStrictMock;
    private RdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
    private IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProviderStub;

    private RdbmsProviderCommandFactory _factory;

    private IDbCommandBuilder _dbCommandBuilder1Stub;
    private IDbCommandBuilder _dbCommandBuilder2Stub;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private ObjectID _objectID1;
    private ObjectID _objectID2;
    private ObjectID _objectID3;
    private ObjectID _foreignKeyValue;
    private ObjectIDStoragePropertyDefinition _foreignKeyColumnDefinition;
    private UnionViewDefinition _unionViewDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _dbCommandBuilderFactoryStrictMock = MockRepository.GenerateStrictMock<IDbCommandBuilderFactory>();
      _rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider();
      _infrastructureStoragePropertyDefinitionProviderStub = MockRepository.GenerateStub<IInfrastructureStoragePropertyDefinitionProvider>();

      _factory = new RdbmsProviderCommandFactory (
          _dbCommandBuilderFactoryStrictMock,
          _rdbmsPersistenceModelProvider,
          _infrastructureStoragePropertyDefinitionProviderStub);

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
    }

    [Test]
    public void CreateForSingleIDLookup_TableDefinition ()
    {
      var objectID = CreateObjectID (_tableDefinition1);
      var expectedSelectedColumns = _tableDefinition1.GetAllColumns().ToArray();

      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForSingleIDLookupFromTable (
                  Arg.Is (_tableDefinition1),
                  Arg<SelectedColumnsSpecification>.Matches (c => c.SelectedColumns.SequenceEqual (expectedSelectedColumns)),
                  Arg.Is (objectID)))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForSingleIDLookup (objectID);

      var innerCommand = CheckDelegateBasedCommandAndReturnInnerCommand<DataContainer, ObjectLookupResult<DataContainer>> (result);
      Assert.That (innerCommand, Is.TypeOf (typeof (SingleObjectLoadCommand<DataContainer>)));
      var loadCommand = ((SingleObjectLoadCommand<DataContainer>) innerCommand);
      Assert.That (loadCommand.DbCommandBuilder, Is.SameAs (_dbCommandBuilder1Stub));
      Assert.That (loadCommand.DataContainerReader, Is.TypeOf (typeof (DataContainerReader)));

      CheckDataContainerReaderForKnownProjection (loadCommand.DataContainerReader, expectedSelectedColumns);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An ObjectID's EntityDefinition cannot be a UnionViewDefinition.")]
    public void CreateForSingleIDLookup_UnionViewDefinition ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "ViewName"),
          _tableDefinition1);

      var objectID = CreateObjectID (unionViewDefinition);

      _factory.CreateForSingleIDLookup (objectID);
    }

    [Test]
    public void CreateForSingleIDLookup_FilterViewDefinition ()
    {
      var filterViewDefinition = FilterViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "ViewName"),
          _tableDefinition1);
      var expectedSelectedColumns = _tableDefinition1.GetAllColumns().ToArray();

      var objectID = CreateObjectID (filterViewDefinition);

      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub =>
              stub.CreateForSingleIDLookupFromTable (
                  Arg.Is (_tableDefinition1),
                  Arg<SelectedColumnsSpecification>.Matches (c => c.SelectedColumns.SequenceEqual (expectedSelectedColumns)),
                  Arg.Is (objectID)))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForSingleIDLookup (objectID);

      var innerCommand = CheckDelegateBasedCommandAndReturnInnerCommand<DataContainer, ObjectLookupResult<DataContainer>> (result);
      Assert.That (innerCommand, Is.TypeOf (typeof (SingleObjectLoadCommand<DataContainer>)));
      var loadCommand = ((SingleObjectLoadCommand<DataContainer>) innerCommand);
      Assert.That (loadCommand.DbCommandBuilder, Is.SameAs (_dbCommandBuilder1Stub));
      Assert.That (loadCommand.DataContainerReader, Is.TypeOf (typeof (DataContainerReader)));

      CheckDataContainerReaderForKnownProjection (loadCommand.DataContainerReader, expectedSelectedColumns);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An ObjectID's EntityDefinition cannot be a NullEntityDefinition.")]
    public void CreateForSingleIDLookup_NullEntityDefinition ()
    {
      var nullEntityDefintion = new NullEntityDefinition (TestDomainStorageProviderDefinition);

      var objectID = CreateObjectID (nullEntityDefintion);

      _dbCommandBuilderFactoryStrictMock
          .Stub (stub => stub.CreateForSingleIDLookupFromTable (_tableDefinition1, AllSelectedColumnsSpecification.Instance, objectID))
          .Return (_dbCommandBuilder1Stub);

      _factory.CreateForSingleIDLookup (objectID);
    }

    [Test]
    public void CreateForMultiIDLookup_TableDefinition_SingleIDLookup ()
    {
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForSingleIDLookupFromTable (
                  Arg.Is (_tableDefinition1),
                  Arg<SelectedColumnsSpecification>.Matches (c => c.SelectedColumns.SequenceEqual (_tableDefinition1.GetAllColumns())),
                  Arg.Is (_objectID1)))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForMultiIDLookup (new[] { _objectID1 });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));

      var dbCommandBuilderTuples =
          ((MultiObjectLoadCommand<DataContainer>) ((MultiDataContainerSortCommand) result).Command).DbCommandBuildersAndReaders;
      Assert.That (dbCommandBuilderTuples.Length, Is.EqualTo (1));
      CheckDataContainerReaderForKnownProjection (dbCommandBuilderTuples[0].Item2, _tableDefinition1.GetAllColumns().ToArray());
    }

    [Test]
    public void CreateForMultiIDLookup_TableDefinition_MultipleIDLookup_AndMultipleTables ()
    {
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForSingleIDLookupFromTable (
                  Arg.Is (_tableDefinition2),
                  Arg<SelectedColumnsSpecification>.Matches (c => c.SelectedColumns.SequenceEqual (_tableDefinition2.GetAllColumns())),
                  Arg.Is (_objectID3)))
          .Return (_dbCommandBuilder1Stub);
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForMultiIDLookupFromTable (
                  Arg.Is (((TableDefinition) _objectID1.ClassDefinition.StorageEntityDefinition)),
                  Arg<SelectedColumnsSpecification>.Matches (c => c.SelectedColumns.SequenceEqual (_tableDefinition2.GetAllColumns())),
                  Arg.Is (new[] { _objectID1, _objectID2 })))
          .Return (_dbCommandBuilder2Stub);

      var result = _factory.CreateForMultiIDLookup (new[] { _objectID1, _objectID2, _objectID3 });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));

      var dbCommandBuilderTuples =
          ((MultiObjectLoadCommand<DataContainer>) ((MultiDataContainerSortCommand) result).Command).DbCommandBuildersAndReaders;
      Assert.That (dbCommandBuilderTuples.Length, Is.EqualTo (2));

      // Convert to Dictionary because the order of tuples is not defined
      var dbCommandBuilderDictionary = dbCommandBuilderTuples.ToDictionary (tuple => tuple.Item1, tuple => tuple.Item2);

      Assert.That (dbCommandBuilderDictionary.ContainsKey (_dbCommandBuilder1Stub), Is.True);
      var readerForCommandBuilder1 = dbCommandBuilderDictionary[_dbCommandBuilder1Stub];
      CheckDataContainerReaderForKnownProjection (readerForCommandBuilder1, _tableDefinition1.GetAllColumns().ToArray());

      Assert.That (dbCommandBuilderDictionary.ContainsKey (_dbCommandBuilder2Stub), Is.True);
      var readerForCommandBuilder2 = dbCommandBuilderDictionary[_dbCommandBuilder2Stub];
      CheckDataContainerReaderForKnownProjection (readerForCommandBuilder2, _tableDefinition2.GetAllColumns().ToArray());
    }

    [Test]
    public void CreateForMultiIDLookup_FilterViewDefinition ()
    {
      var filterViewDefinition = FilterViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "FileView"),
          _tableDefinition1);

      var objectID = CreateObjectID (filterViewDefinition);

      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForSingleIDLookupFromTable (
                  Arg.Is (_tableDefinition1),
                  Arg<SelectedColumnsSpecification>.Matches (c => c.SelectedColumns.SequenceEqual (_tableDefinition1.GetAllColumns())),
                  Arg.Is (objectID)))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForMultiIDLookup (new[] { objectID });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));
      var dbCommandBuilderTuples =
          ((MultiObjectLoadCommand<DataContainer>) ((MultiDataContainerSortCommand) result).Command).DbCommandBuildersAndReaders;
      Assert.That (dbCommandBuilderTuples.Length, Is.EqualTo (1));
      CheckDataContainerReaderForKnownProjection (dbCommandBuilderTuples[0].Item2, _tableDefinition1.GetAllColumns().ToArray());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An ObjectID's EntityDefinition cannot be a UnionViewDefinition.")]
    public void CreateForMultiIDLookup_UnionViewDefinition ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "Table"));
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"), tableDefinition);

      var objectID = CreateObjectID (unionViewDefinition);

      _factory.CreateForMultiIDLookup (new[] { objectID });
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An ObjectID's EntityDefinition cannot be a NullEntityDefinition.")]
    public void CreateForMultiIDLookup_NullEntityDefinition ()
    {
      var nullEntityDefintion = new NullEntityDefinition (TestDomainStorageProviderDefinition);

      var objectID = CreateObjectID (nullEntityDefintion);

      _factory.CreateForMultiIDLookup (new[] { objectID });
    }

    [Test]
    public void CreateForRelationLookup_TableDefinition_NoSortExpression ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);

      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForRelationLookupFromTable (
                  Arg.Is ((TableDefinition) classDefinition.StorageEntityDefinition),
                  Arg<SelectedColumnsSpecification>.Matches (c => c.SelectedColumns.SequenceEqual (_tableDefinition1.GetAllColumns())),
                  Arg.Is (_foreignKeyColumnDefinition),
                  Arg.Is (_foreignKeyValue),
                  Arg.Is (EmptyOrderedColumnsSpecification.Instance)))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      Assert.That (result, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));
      var dbCommandBuilderTuples = ((MultiObjectLoadCommand<DataContainer>) result).DbCommandBuildersAndReaders;
      Assert.That (dbCommandBuilderTuples.Length, Is.EqualTo (1));
      CheckDataContainerReaderForKnownProjection (dbCommandBuilderTuples[0].Item2, _tableDefinition1.GetAllColumns().ToArray());
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

      var expectedOrderedColumns = new[]
                                   {
                                       Tuple.Create (ColumnDefinitionObjectMother.IDColumn, SortOrder.Descending),
                                       Tuple.Create (ColumnDefinitionObjectMother.ClassIDColumn, SortOrder.Descending),
                                       Tuple.Create (ColumnDefinitionObjectMother.TimestampColumn, SortOrder.Ascending)
                                   };

      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForRelationLookupFromTable (
                  Arg.Is ((TableDefinition) classDefinition.StorageEntityDefinition),
                  Arg<SelectedColumnsSpecification>.Matches (c => c.SelectedColumns.SequenceEqual (_tableDefinition1.GetAllColumns())),
                  Arg.Is (_foreignKeyColumnDefinition),
                  Arg.Is (_foreignKeyValue),
                  Arg<OrderedColumnsSpecification>.Matches (o => o.Columns.SequenceEqual (expectedOrderedColumns))))
          .Return (_dbCommandBuilder1Stub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _factory.CreateForRelationLookup (
          relationEndPointDefinition,
          _foreignKeyValue,
          new SortExpressionDefinition (new[] { spec1, spec2 }));

      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations();
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
                  Arg<SelectedColumnsSpecification>.Matches (spec => spec.SelectedColumns.SequenceEqual (expectedSelectedColumns)),
                  Arg.Is (_foreignKeyColumnDefinition),
                  Arg.Is (_foreignKeyValue),
                  Arg.Is (EmptyOrderedColumnsSpecification.Instance)))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      var innerCommand =
          CheckDelegateBasedCommandAndReturnInnerCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IEnumerable<DataContainer>> (result);
      Assert.That (innerCommand, Is.TypeOf (typeof (IndirectDataContainerLoadCommand)));
      var command = (IndirectDataContainerLoadCommand) innerCommand;
      Assert.That (command.ObjectIDLoadCommand, Is.TypeOf (typeof (MultiObjectIDLoadCommand)));
      Assert.That (((MultiObjectIDLoadCommand) (command.ObjectIDLoadCommand)).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
      Assert.That (((MultiObjectIDLoadCommand) command.ObjectIDLoadCommand).ObjectIDReader, Is.TypeOf (typeof (ObjectIDReader)));
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

      var expectedOrderedColumns = new[]
                                   {
                                       Tuple.Create (ColumnDefinitionObjectMother.IDColumn, SortOrder.Descending),
                                       Tuple.Create (ColumnDefinitionObjectMother.ClassIDColumn, SortOrder.Descending),
                                       Tuple.Create (ColumnDefinitionObjectMother.TimestampColumn, SortOrder.Ascending)
                                   };

      var expectedSelectedColumns = new[] { _unionViewDefinition.IDColumn, _unionViewDefinition.ClassIDColumn };

      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForRelationLookupFromUnionView (
                  Arg.Is ((UnionViewDefinition) classDefinition.StorageEntityDefinition),
                  Arg<SelectedColumnsSpecification>.Matches (cs => cs.SelectedColumns.SequenceEqual (expectedSelectedColumns)),
                  Arg.Is (_foreignKeyColumnDefinition),
                  Arg.Is (_foreignKeyValue),
                  Arg<OrderedColumnsSpecification>.Matches (o => o.Columns.SequenceEqual (expectedOrderedColumns)))
          ).Return (_dbCommandBuilder1Stub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _factory.CreateForRelationLookup (
          relationEndPointDefinition,
          _foreignKeyValue,
          new SortExpressionDefinition (new[] { spec1, spec2 }));

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
          .Stub (stub => stub.CreateForSingleIDLookupFromTable (_tableDefinition1, AllSelectedColumnsSpecification.Instance, objectID))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      Assert.That (result, Is.TypeOf (typeof (FixedValueStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>)));
    }

    [Test]
    public void CreateForDataContainerQuery ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery>();

      var commandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dbCommandBuilderFactoryStrictMock.Stub (stub => stub.CreateForQuery (queryStub)).Return (commandBuilderStub);

      var objectIDColumnDefinition = ColumnDefinitionObjectMother.IDColumn;
      var classIDColumnDefinition = ColumnDefinitionObjectMother.ClassIDColumn;
      var timestampColumnDefinition = ColumnDefinitionObjectMother.TimestampColumn;

      _infrastructureStoragePropertyDefinitionProviderStub.Stub (stub => stub.GetIDColumnDefinition()).Return (objectIDColumnDefinition);
      _infrastructureStoragePropertyDefinitionProviderStub.Stub (stub => stub.GetClassIDColumnDefinition()).Return (classIDColumnDefinition);
      _infrastructureStoragePropertyDefinitionProviderStub.Stub (stub => stub.GetTimestampColumnDefinition()).Return (timestampColumnDefinition);

      var result = _factory.CreateForDataContainerQuery (queryStub);

      Assert.That (result, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));
      var command = ((MultiObjectLoadCommand<DataContainer>) result);
      Assert.That (command.DbCommandBuildersAndReaders.Length, Is.EqualTo (1));
      Assert.That (command.DbCommandBuildersAndReaders[0].Item1, Is.SameAs (commandBuilderStub));
      Assert.That (command.DbCommandBuildersAndReaders[0].Item2, Is.TypeOf<DataContainerReader>());

      var dataContainerReader = ((DataContainerReader) command.DbCommandBuildersAndReaders[0].Item2);
      Assert.That (dataContainerReader.IDProperty, Is.TypeOf<ObjectIDStoragePropertyDefinition>());
      Assert.That (
          ((ObjectIDStoragePropertyDefinition) dataContainerReader.IDProperty).ValueProperty,
          Is.TypeOf<SimpleStoragePropertyDefinition>().With.Property ("ColumnDefinition").SameAs (objectIDColumnDefinition));
      Assert.That (
          ((ObjectIDStoragePropertyDefinition) dataContainerReader.IDProperty).ClassIDProperty,
          Is.TypeOf<SimpleStoragePropertyDefinition>().With.Property ("ColumnDefinition").SameAs (classIDColumnDefinition));
      Assert.That (
          dataContainerReader.TimestampProperty,
          Is.TypeOf<SimpleStoragePropertyDefinition>().With.Property ("ColumnDefinition").SameAs (timestampColumnDefinition));
    }

    [Test]
    public void CreateForSave ()
    {
      var dataContainerNew1 = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var dataContainerNew2 = DataContainer.CreateNew (DomainObjectIDs.Order2);
      var dataContainerUnchanged = DataContainer.CreateForExisting (DomainObjectIDs.Order3, null, pd => pd.DefaultValue);
      var dataContainerChanged1 = DataContainer.CreateForExisting (DomainObjectIDs.Order4, null, pd => pd.DefaultValue);
      dataContainerChanged1.MarkAsChanged();
      var dataContainerChanged2 = DataContainer.CreateForExisting (DomainObjectIDs.OrderItem1, null, pd => pd.DefaultValue);
      dataContainerChanged2.MarkAsChanged();
      var dataContainerDeleted1 = DataContainer.CreateForExisting (DomainObjectIDs.OrderItem2, null, pd => pd.DefaultValue);
      dataContainerDeleted1.Delete();
      var dataContainerDeleted2 = DataContainer.CreateForExisting (DomainObjectIDs.OrderItem3, null, pd => pd.DefaultValue);
      dataContainerDeleted2.Delete();

      var insertDbCommandBuilderNew1 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var insertDbCommandBuilderNew2 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var updateDbCommandBuilderNew1 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var updateDbCommandBuilderNew2 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var updateDbCommandBuilderChanged1 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var updateDbCommandBuilderChanged2 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var updateDbCommandBuilderDeleted1 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var updateDbCommandBuilderDeleted2 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var deleteDbCommandBuilderDeleted1 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var deleteDbCommandBuilderDeleted2 = MockRepository.GenerateStub<IDbCommandBuilder>();

      _dbCommandBuilderFactoryStrictMock.Stub (stub => stub.CreateForInsert (dataContainerNew1)).Return (insertDbCommandBuilderNew1);
      _dbCommandBuilderFactoryStrictMock.Stub (stub => stub.CreateForInsert (dataContainerNew2)).Return (insertDbCommandBuilderNew2);
      _dbCommandBuilderFactoryStrictMock.Stub (stub => stub.CreateForUpdate (dataContainerNew1)).Return (updateDbCommandBuilderNew1);
      _dbCommandBuilderFactoryStrictMock.Stub (stub => stub.CreateForUpdate (dataContainerNew2)).Return (updateDbCommandBuilderNew2);
      _dbCommandBuilderFactoryStrictMock.Stub (stub => stub.CreateForUpdate (dataContainerChanged1)).Return (updateDbCommandBuilderChanged1);
      _dbCommandBuilderFactoryStrictMock.Stub (stub => stub.CreateForUpdate (dataContainerChanged2)).Return (updateDbCommandBuilderChanged2);
      _dbCommandBuilderFactoryStrictMock.Stub (stub => stub.CreateForUpdate (dataContainerDeleted1)).Return (updateDbCommandBuilderDeleted1);
      _dbCommandBuilderFactoryStrictMock.Stub (stub => stub.CreateForUpdate (dataContainerDeleted2)).Return (updateDbCommandBuilderDeleted2);
      _dbCommandBuilderFactoryStrictMock.Stub (stub => stub.CreateForDelete (dataContainerDeleted1)).Return (deleteDbCommandBuilderDeleted1);
      _dbCommandBuilderFactoryStrictMock.Stub (stub => stub.CreateForDelete (dataContainerDeleted2)).Return (deleteDbCommandBuilderDeleted2);

      var result = _factory.CreateForSave (
          new[]
          {
              dataContainerNew1,
              dataContainerChanged1,
              dataContainerDeleted1,
              dataContainerDeleted2,
              dataContainerUnchanged,
              dataContainerChanged2,
              dataContainerNew2
          });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSaveCommand)));
      var tuples = ((MultiDataContainerSaveCommand) result).Tuples.ToList();

      Assert.That (tuples.Count, Is.EqualTo (10));
      Assert.That (tuples[0].Item1, Is.EqualTo (dataContainerNew1.ID));
      Assert.That (tuples[0].Item2, Is.SameAs (insertDbCommandBuilderNew1));
      Assert.That (tuples[1].Item1, Is.EqualTo (dataContainerNew2.ID));
      Assert.That (tuples[1].Item2, Is.SameAs (insertDbCommandBuilderNew2));
      Assert.That (tuples[2].Item1, Is.EqualTo (dataContainerNew1.ID));
      Assert.That (tuples[2].Item2, Is.SameAs (updateDbCommandBuilderNew1));
      Assert.That (tuples[3].Item1, Is.EqualTo (dataContainerNew2.ID));
      Assert.That (tuples[3].Item2, Is.SameAs (updateDbCommandBuilderNew2));
      Assert.That (tuples[4].Item1, Is.EqualTo (dataContainerChanged1.ID));
      Assert.That (tuples[4].Item2, Is.SameAs (updateDbCommandBuilderChanged1));
      Assert.That (tuples[5].Item1, Is.EqualTo (dataContainerChanged2.ID));
      Assert.That (tuples[5].Item2, Is.SameAs (updateDbCommandBuilderChanged2));
      Assert.That (tuples[6].Item1, Is.EqualTo (dataContainerDeleted1.ID));
      Assert.That (tuples[6].Item2, Is.SameAs (updateDbCommandBuilderDeleted1));
      Assert.That (tuples[7].Item1, Is.EqualTo (dataContainerDeleted2.ID));
      Assert.That (tuples[7].Item2, Is.SameAs (updateDbCommandBuilderDeleted2));
      Assert.That (tuples[8].Item1, Is.EqualTo (dataContainerDeleted1.ID));
      Assert.That (tuples[8].Item2, Is.SameAs (deleteDbCommandBuilderDeleted1));
      Assert.That (tuples[9].Item1, Is.EqualTo (dataContainerDeleted2.ID));
      Assert.That (tuples[9].Item2, Is.SameAs (deleteDbCommandBuilderDeleted2));
    }

    private ObjectID CreateObjectID (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetStorageEntity (entityDefinition);

      return new ObjectID (classDefinition, Guid.NewGuid());
    }

    private ClassDefinition CreateClassDefinition (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetStorageEntity (entityDefinition);
      return classDefinition;
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


    private SortedPropertySpecification CreateSortedPropertySpecification (
        ClassDefinition classDefinition, PropertyInfo propertyInfo, IStoragePropertyDefinition columnDefinition, SortOrder sortOrder)
    {
      var sortedPropertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition, StorageClass.Persistent, propertyInfo, columnDefinition);
      return new SortedPropertySpecification (sortedPropertyDefinition, sortOrder);
    }

    private IStorageProviderCommand<TIn, IRdbmsProviderCommandExecutionContext> CheckDelegateBasedCommandAndReturnInnerCommand<TIn, TResult> (
        IStorageProviderCommand<TResult, IRdbmsProviderCommandExecutionContext> result)
    {
      Assert.That (
          result,
          Is.TypeOf (typeof (DelegateBasedStorageProviderCommand<TIn, TResult, IRdbmsProviderCommandExecutionContext>)));
      return ((DelegateBasedStorageProviderCommand<TIn, TResult, IRdbmsProviderCommandExecutionContext>) result).Command;
    }

    private void CheckInfrastructureProperties (
        DataContainerReader dataContainerReader,
        ColumnDefinition expectedIDColumn,
        ColumnDefinition expectedClassIDColumn,
        ColumnDefinition expectedTimestampColumn)
    {
      Assert.That (
          ((SimpleStoragePropertyDefinition) ((ObjectIDStoragePropertyDefinition) dataContainerReader.IDProperty).ValueProperty).ColumnDefinition,
          Is.SameAs (expectedIDColumn));
      Assert.That (
          ((SimpleStoragePropertyDefinition) ((ObjectIDStoragePropertyDefinition) dataContainerReader.IDProperty).ClassIDProperty).ColumnDefinition,
          Is.SameAs (expectedClassIDColumn));
      Assert.That (
          ((SimpleStoragePropertyDefinition) dataContainerReader.TimestampProperty).ColumnDefinition, Is.SameAs (expectedTimestampColumn));
    }

    private void CheckDictionaryBasedColumnOrdinalProvider (IColumnOrdinalProvider ordinalProvider, ColumnDefinition[] expectedSelectedColumns)
    {
      Assert.That (ordinalProvider, Is.TypeOf (typeof (DictionaryBasedColumnOrdinalProvider)));
      var ordinals = ((DictionaryBasedColumnOrdinalProvider) ordinalProvider).Ordinals;

      Assert.That (ordinals.Count, Is.EqualTo (expectedSelectedColumns.Length));

      for (int i = 0; i < expectedSelectedColumns.Length; ++i)
        Assert.That (ordinals[expectedSelectedColumns[i]], Is.EqualTo (i));
    }

    private void CheckDataContainerReaderForKnownProjection (IObjectReader<DataContainer> reader, ColumnDefinition[] expectedSelectedColumns)
    {
      var dataContainerReader = (DataContainerReader) reader;
      CheckInfrastructureProperties (
          dataContainerReader,
          _tableDefinition1.IDColumn,
          _tableDefinition1.ClassIDColumn,
          _tableDefinition1.TimestampColumn);
      Assert.That (dataContainerReader.PersistenceModelProvider, Is.SameAs (_rdbmsPersistenceModelProvider));

      CheckDictionaryBasedColumnOrdinalProvider (dataContainerReader.OrdinalProvider, expectedSelectedColumns);
    }
  }
}