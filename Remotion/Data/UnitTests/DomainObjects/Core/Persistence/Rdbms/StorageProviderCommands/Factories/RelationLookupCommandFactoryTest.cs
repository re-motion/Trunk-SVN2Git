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
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class RelationLookupCommandFactoryTest : StandardMappingTest
  {
    private RdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;

    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStrictMock;
    private IObjectReaderFactory _objectReaderFactoryStrictMock;
    private IDbCommandBuilder _dbCommandBuilderStub;
    private IObjectReader<DataContainer> _dataContainerReaderStub;
    private IObjectReader<ObjectID> _objectIDReaderStub;
    private IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _fakeStorageProviderCommandFactory;

    private RelationLookupCommandFactory _factory;
    
    private TableDefinition _tableDefinition;
    private UnionViewDefinition _unionViewDefinition;
    private ObjectID _foreignKeyValue;
    private IRdbmsStoragePropertyDefinition _foreignKeyStoragePropertyDefinitionStrictMock;

    private ColumnValue[] _fakeComparedColumns;

    public override void SetUp ()
    {
      base.SetUp();

      _rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider();

      _dbCommandBuilderFactoryStrictMock = MockRepository.GenerateStrictMock<IDbCommandBuilderFactory>();
      _objectReaderFactoryStrictMock = MockRepository.GenerateStrictMock<IObjectReaderFactory>();
      _dbCommandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder> ();
      _dataContainerReaderStub = MockRepository.GenerateStub<IObjectReader<DataContainer>> ();
      _objectIDReaderStub = MockRepository.GenerateStub<IObjectReader<ObjectID>> ();
      _fakeStorageProviderCommandFactory = MockRepository.GenerateStub<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>>();

      _factory = new RelationLookupCommandFactory (
                _fakeStorageProviderCommandFactory,
                _dbCommandBuilderFactoryStrictMock,
                _rdbmsPersistenceModelProvider,
                _objectReaderFactoryStrictMock);

      _tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table1"));
      _unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "ViewName"),
          _tableDefinition);

      _foreignKeyValue = CreateObjectID (_tableDefinition);
      _foreignKeyStoragePropertyDefinitionStrictMock = MockRepository.GenerateStrictMock<IRdbmsStoragePropertyDefinition>();

      _fakeComparedColumns = new[] { new ColumnValue (ColumnDefinitionObjectMother.IDColumn, _foreignKeyValue.Value) };
    }

    [Test]
    public void CreateForRelationLookup_TableDefinition_NoSortExpression ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);
      var oppositeTable = (TableDefinition) relationEndPointDefinition.ClassDefinition.StorageEntityDefinition;

      _foreignKeyStoragePropertyDefinitionStrictMock.Expect (mock => mock.SplitValueForComparison (_foreignKeyValue)).Return (_fakeComparedColumns);
      _foreignKeyStoragePropertyDefinitionStrictMock.Replay ();

      var expectedSelectedColumns = _tableDefinition.GetAllColumns ();
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForRelationLookupFromTable (
                  Arg.Is ((TableDefinition) classDefinition.StorageEntityDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg.Is (_fakeComparedColumns),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (new OrderedColumn[0])))
          .Return (_dbCommandBuilderStub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IEntityDefinition) oppositeTable),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_dataContainerReaderStub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      _foreignKeyStoragePropertyDefinitionStrictMock.VerifyAllExpectations ();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations ();

      Assert.That (result, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));
      var dbCommandBuilderTuples = ((MultiObjectLoadCommand<DataContainer>) result).DbCommandBuildersAndReaders;
      Assert.That (dbCommandBuilderTuples.Length, Is.EqualTo (1));
      Assert.That (dbCommandBuilderTuples[0].Item1, Is.SameAs (_dbCommandBuilderStub));
      Assert.That (dbCommandBuilderTuples[0].Item2, Is.SameAs (_dataContainerReaderStub));
    }

    [Test]
    public void CreateForRelationLookup_TableDefinition_WithSortExpression ()
    {
      var classDefinition = CreateClassDefinition (_tableDefinition);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);

      var spec1 = CreateSortedPropertySpecification (
          classDefinition,
          SortOrder.Descending,
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn);
      var spec2 = CreateSortedPropertySpecification (classDefinition, SortOrder.Ascending, ColumnDefinitionObjectMother.TimestampColumn);

      _foreignKeyStoragePropertyDefinitionStrictMock.Expect (mock => mock.SplitValueForComparison (_foreignKeyValue)).Return (_fakeComparedColumns);
      _foreignKeyStoragePropertyDefinitionStrictMock.Replay ();

      var expectedSelectedColumns = _tableDefinition.GetAllColumns();
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
                  Arg.Is (_fakeComparedColumns),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (expectedOrderedColumns)))
          .Return (_dbCommandBuilderStub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IEntityDefinition) _tableDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_dataContainerReaderStub);
      _objectReaderFactoryStrictMock.Replay();

      _factory.CreateForRelationLookup (
          relationEndPointDefinition,
          _foreignKeyValue,
          new SortExpressionDefinition (new[] { spec1, spec2 }));

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      _foreignKeyStoragePropertyDefinitionStrictMock.VerifyAllExpectations();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateForRelationLookup_UnionViewDefinition_NoSortExpression ()
    {
      var classDefinition = CreateClassDefinition (_unionViewDefinition);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);

      _foreignKeyStoragePropertyDefinitionStrictMock.Expect (mock => mock.SplitValueForComparison (_foreignKeyValue)).Return (_fakeComparedColumns);
      _foreignKeyStoragePropertyDefinitionStrictMock.Replay ();

      var expectedSelectedColumns = new[] { _unionViewDefinition.IDColumn, _unionViewDefinition.ClassIDColumn };
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForRelationLookupFromUnionView (
                  Arg.Is (_unionViewDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg.Is (_fakeComparedColumns),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (new OrderedColumn[0])))
          .Return (_dbCommandBuilderStub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateObjectIDReader (
                  Arg.Is (_unionViewDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_objectIDReaderStub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      _objectReaderFactoryStrictMock.VerifyAllExpectations ();
      _foreignKeyStoragePropertyDefinitionStrictMock.VerifyAllExpectations ();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations ();

      var innerCommand =
          CheckDelegateBasedCommandAndReturnInnerCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IEnumerable<DataContainer>> (result);
      Assert.That (innerCommand, Is.TypeOf (typeof (IndirectDataContainerLoadCommand)));
      var indirectLoadCommand = (IndirectDataContainerLoadCommand) innerCommand;
      Assert.That (indirectLoadCommand.StorageProviderCommandFactory, Is.SameAs (_fakeStorageProviderCommandFactory));
      Assert.That (indirectLoadCommand.ObjectIDLoadCommand, Is.TypeOf (typeof (MultiObjectIDLoadCommand)));
      Assert.That (((MultiObjectIDLoadCommand) (indirectLoadCommand.ObjectIDLoadCommand)).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilderStub }));
      Assert.That (((MultiObjectIDLoadCommand) indirectLoadCommand.ObjectIDLoadCommand).ObjectIDReader, Is.SameAs (_objectIDReaderStub));
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

      _foreignKeyStoragePropertyDefinitionStrictMock.Expect (mock => mock.SplitValueForComparison (_foreignKeyValue)).Return (_fakeComparedColumns);
      _foreignKeyStoragePropertyDefinitionStrictMock.Replay ();

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
                  Arg.Is (_fakeComparedColumns),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (expectedOrderedColumns)))
          .Return (_dbCommandBuilderStub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateObjectIDReader (
                  Arg.Is (_unionViewDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_objectIDReaderStub);
      _objectReaderFactoryStrictMock.Replay();

      _factory.CreateForRelationLookup (
          relationEndPointDefinition,
          _foreignKeyValue,
          new SortExpressionDefinition (new[] { spec1, spec2 }));

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      _foreignKeyStoragePropertyDefinitionStrictMock.VerifyAllExpectations ();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations ();
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
          _foreignKeyStoragePropertyDefinitionStrictMock);
      var relationEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      Assert.That (result, Is.TypeOf (typeof (FixedValueStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>)));
      var fixedValueCommand = (FixedValueStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>) result;
      Assert.That (fixedValueCommand.Value, Is.EqualTo (Enumerable.Empty<DataContainer>()));
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
          _foreignKeyStoragePropertyDefinitionStrictMock);
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