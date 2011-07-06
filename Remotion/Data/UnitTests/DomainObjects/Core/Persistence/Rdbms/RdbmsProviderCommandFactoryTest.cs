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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
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
    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStub;
    private IDataContainerReader _dataContainerReaderStub;
    private IObjectIDReader _objectIDReaderStub;

    private RdbmsProviderCommandFactory _factory;

    private TableDefinition _entityDefinition;
    private IDbCommandBuilder _dbCommandBuilder1Stub;
    private IDbCommandBuilder _dbCommandBuilder2Stub;
    private ObjectID _objectID;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private ObjectID _objectID1;
    private ObjectID _objectID2;
    private ObjectID _objectID3;
    private ObjectID _foreignKeyValue;
    private IDColumnDefinition _foreignKeyColumnDefinition;
    private UnionViewDefinition _unionViewDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _dbCommandBuilderFactoryStub = MockRepository.GenerateStub<IDbCommandBuilderFactory>();
      _dataContainerReaderStub = MockRepository.GenerateStub<IDataContainerReader>();
      _objectIDReaderStub = MockRepository.GenerateStub<IObjectIDReader>();

      _factory = new RdbmsProviderCommandFactory (
          _dbCommandBuilderFactoryStub, _dataContainerReaderStub, _objectIDReaderStub, new RdbmsPersistenceModelProvider());

      _entityDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));
      _dbCommandBuilder1Stub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dbCommandBuilder2Stub = MockRepository.GenerateStub<IDbCommandBuilder> ();
      _objectID = CreateObjectID (_entityDefinition);

      _tableDefinition1 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table1"));
      _tableDefinition2 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table2"));
      _unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "ViewName"),
          _tableDefinition1);

      _foreignKeyValue = CreateObjectID (_tableDefinition1);
      _foreignKeyColumnDefinition = new IDColumnDefinition (ColumnDefinitionObjectMother.ObjectIDColumn, ColumnDefinitionObjectMother.ClassIDColumn);

      _objectID1 = CreateObjectID (_tableDefinition1);
      _objectID2 = CreateObjectID (_tableDefinition1);
      _objectID3 = CreateObjectID (_tableDefinition2);
    }
    
    [Test]
    public void CreateForSingleIDLookup_TableDefinition ()
    {
      var objectID = CreateObjectID (_tableDefinition1);
      _dbCommandBuilderFactoryStub
          .Stub (stub => stub.CreateForSingleIDLookupFromTable (_tableDefinition1, AllSelectedColumnsSpecification.Instance, objectID))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForSingleIDLookup (objectID);

      Assert.That (result, Is.TypeOf (typeof (SingleDataContainerLoadCommand)));
      Assert.That (((SingleDataContainerLoadCommand) result).DbCommandBuilder, Is.SameAs (_dbCommandBuilder1Stub));
      Assert.That (((SingleDataContainerLoadCommand) result).DataContainerReader, Is.SameAs (_dataContainerReaderStub));
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

      var objectID = CreateObjectID (filterViewDefinition);

      _dbCommandBuilderFactoryStub
          .Stub (stub => stub.CreateForSingleIDLookupFromTable (_tableDefinition1, AllSelectedColumnsSpecification.Instance, objectID))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForSingleIDLookup (objectID);

      Assert.That (result, Is.TypeOf (typeof (SingleDataContainerLoadCommand)));
      Assert.That (((SingleDataContainerLoadCommand) result).DbCommandBuilder, Is.SameAs (_dbCommandBuilder1Stub));
      Assert.That (((SingleDataContainerLoadCommand) result).DataContainerReader, Is.SameAs (_dataContainerReaderStub));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The ClassDefinition must not have a NullEntityDefinition.")]
    public void CreateForSingleIDLookup_NullEntityDefinition ()
    {
      var nullEntityDefintion = new NullEntityDefinition (TestDomainStorageProviderDefinition);

      var objectID = CreateObjectID (nullEntityDefintion);

      _dbCommandBuilderFactoryStub
          .Stub (stub => stub.CreateForSingleIDLookupFromTable (_tableDefinition1, AllSelectedColumnsSpecification.Instance, objectID))
          .Return (_dbCommandBuilder1Stub);

      _factory.CreateForSingleIDLookup (objectID);
    }
    
    [Test]
    public void CreateForMultiIDLookup_SingleIDLookup_TableDefinition ()
    {
      _dbCommandBuilderFactoryStub
          .Stub (
              stub =>
              stub.CreateForSingleIDLookupFromTable (
                  _tableDefinition1, AllSelectedColumnsSpecification.Instance, _objectID1))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForMultiIDLookup (new[] { _objectID1 });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (
          ((MultiDataContainerLoadCommand) ((MultiDataContainerSortCommand) result).Command).DbCommandBuilders,
          Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
    }

    [Test]
    public void CreateForMultiIDLookup_MultiIDLookup_TableDefinition ()
    {
      _dbCommandBuilderFactoryStub
          .Stub (
              stub => stub.CreateForMultiIDLookupFromTable (
                  _tableDefinition1,
                  AllSelectedColumnsSpecification.Instance,
                  new[] { _objectID1, _objectID2 }))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForMultiIDLookup (new[] { _objectID1, _objectID2 });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (
          ((MultiDataContainerLoadCommand) ((MultiDataContainerSortCommand) result).Command).DbCommandBuilders,
          Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
    }

    [Test]
    public void CreateForMultiIDLookup_MultipleTableDefinitions ()
    {
      _dbCommandBuilderFactoryStub.Stub (
          stub =>
          stub.CreateForSingleIDLookupFromTable (
              _tableDefinition2, AllSelectedColumnsSpecification.Instance, _objectID3)).Return (
                  _dbCommandBuilder2Stub);
      _dbCommandBuilderFactoryStub.Stub (
          stub =>
          stub.CreateForMultiIDLookupFromTable (
              ((TableDefinition) _objectID1.ClassDefinition.StorageEntityDefinition),
              AllSelectedColumnsSpecification.Instance,
              new[] { _objectID1, _objectID2 })).Return (
                  _dbCommandBuilder1Stub);

      var result = _factory.CreateForMultiIDLookup (new[] { _objectID1, _objectID2, _objectID3 });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (
          ((MultiDataContainerLoadCommand) ((MultiDataContainerSortCommand) result).Command).DbCommandBuilders,
          Is.EqualTo (new[] { _dbCommandBuilder1Stub, _dbCommandBuilder2Stub }));
    }

    [Test]
    public void CreateForMultiIDLookup_FilterViewDefinition ()
    {
      var filterViewDefinition = FilterViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "FileView"),
          _tableDefinition1);

      var objectID = CreateObjectID (filterViewDefinition);

      _dbCommandBuilderFactoryStub.Stub (
          stub =>
          stub.CreateForSingleIDLookupFromTable (_tableDefinition1, AllSelectedColumnsSpecification.Instance, objectID)).
          Return (
              _dbCommandBuilder1Stub);

      var result = _factory.CreateForMultiIDLookup (new[] { objectID });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (
          ((MultiDataContainerLoadCommand) ((MultiDataContainerSortCommand) result).Command).DbCommandBuilders,
          Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
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

    [Ignore ("TODO RM-4090")]
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
      var idPropertyDefinition = CreateForeignLeyEndPointDefinition (classDefinition);
      var relationEndPointDefinition = new RelationEndPointDefinition (idPropertyDefinition, false);

      _dbCommandBuilderFactoryStub
          .Stub (
              stub => stub.CreateForRelationLookupFromTable (
                  (TableDefinition) classDefinition.StorageEntityDefinition,
                  AllSelectedColumnsSpecification.Instance,
                  _foreignKeyColumnDefinition,
                  _foreignKeyValue,
                  EmptyOrderedColumnsSpecification.Instance))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
      Assert.That (((MultiDataContainerLoadCommand) result).AllowNulls, Is.False);
      Assert.That (((MultiDataContainerLoadCommand) result).DataContainerReader, Is.SameAs (_dataContainerReaderStub));
    }

    [Test]
    public void CreateForRelationLookup_TableDefinition_WithSortExpression ()
    {
      var classDefinition = CreateClassDefinition (_tableDefinition1);
      var idPropertyDefinition = CreateForeignLeyEndPointDefinition (classDefinition);
      var relationEndPointDefinition = new RelationEndPointDefinition (idPropertyDefinition, false);

      var sortedPropertySpecification1 = CreateSortedPropertySpecification (
          classDefinition, typeof (Order).GetProperty ("OrderNumber"), ColumnDefinitionObjectMother.ObjectIDColumn, SortOrder.Descending);
      var sortedPropertySpecification2 = CreateSortedPropertySpecification (
          classDefinition, typeof (Order).GetProperty ("OrderNumber"), ColumnDefinitionObjectMother.ObjectIDColumn, SortOrder.Ascending);

      var expectedOrderedColumns = new[]
                                   {
                                       Tuple.Create (ColumnDefinitionObjectMother.ObjectIDColumn, SortOrder.Descending),
                                       Tuple.Create (ColumnDefinitionObjectMother.ObjectIDColumn, SortOrder.Ascending)
                                   };
      _dbCommandBuilderFactoryStub
          .Stub (
              stub => stub.CreateForRelationLookupFromTable (
                  Arg.Is ((TableDefinition) classDefinition.StorageEntityDefinition),
                  Arg.Is (AllSelectedColumnsSpecification.Instance),
                  Arg.Is (_foreignKeyColumnDefinition),
                  Arg.Is (_foreignKeyValue),
                  Arg<OrderedColumnsSpecification>.Matches (o => o.Columns.SequenceEqual (expectedOrderedColumns))))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForRelationLookup (
          relationEndPointDefinition,
          _foreignKeyValue,
          new SortExpressionDefinition (new[] { sortedPropertySpecification1, sortedPropertySpecification2 }));

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
      Assert.That (((MultiDataContainerLoadCommand) result).AllowNulls, Is.False);
      Assert.That (((MultiDataContainerLoadCommand) result).DataContainerReader, Is.SameAs (_dataContainerReaderStub));
    }

    [Test]
    public void CreateForRelationLookup_UnionViewDefinition_NoSortExpression ()
    {
      var classDefinition = CreateClassDefinition (_unionViewDefinition);
      var propertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("OrderTicket"),
          _foreignKeyColumnDefinition);
      var relationEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);

      var expectedSelectedColumns = new[] { _unionViewDefinition.ObjectIDColumn, _unionViewDefinition.ClassIDColumn };
      _dbCommandBuilderFactoryStub
          .Stub (
              stub => stub.CreateForRelationLookupFromUnionView (
                  Arg.Is (_unionViewDefinition),
                  Arg<SelectedColumnsSpecification>.Matches (spec => spec.SelectedColumns.SequenceEqual (expectedSelectedColumns)),
                  Arg.Is (_foreignKeyColumnDefinition),
                  Arg.Is (_foreignKeyValue),
                  Arg.Is (EmptyOrderedColumnsSpecification.Instance)))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      Assert.That (result, Is.TypeOf (typeof (IndirectDataContainerLoadCommand)));
      var command = (IndirectDataContainerLoadCommand) result;
      Assert.That (command.ObjectIDLoadCommand, Is.TypeOf (typeof (MultiObjectIDLoadCommand)));
      Assert.That (((MultiObjectIDLoadCommand) (command.ObjectIDLoadCommand)).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
      Assert.That (((MultiObjectIDLoadCommand) command.ObjectIDLoadCommand).ObjectIDReader, Is.SameAs (_objectIDReaderStub));
    }

    [Test]
    public void CreateForRelationLookup_UnionViewDefinition_WithSortExpression ()
    {
      var classDefinition = CreateClassDefinition (_unionViewDefinition);
      var objectIDColumn = new SimpleColumnDefinition ("OrderTicketID", typeof (Guid), "uniqueidentifier", true, false);
      var classIDColumn = ColumnDefinitionObjectMother.ClassIDColumn;
      var idColumnDefinition = new IDColumnDefinition (objectIDColumn, classIDColumn);
      var idPropertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("OrderTicket"),
          idColumnDefinition);
      var sortPropertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("OrderNumber"),
          objectIDColumn);
      var relationEndPointDefinition = new RelationEndPointDefinition (idPropertyDefinition, false);
      var sortedPropertySpecification1 = new SortedPropertySpecification (sortPropertyDefinition, SortOrder.Descending);
      var sortedPropertySpecification2 = new SortedPropertySpecification (sortPropertyDefinition, SortOrder.Ascending);

      var expectedSelectedColumns = new[] { _unionViewDefinition.ObjectIDColumn, _unionViewDefinition.ClassIDColumn };
      var expectedOrderedColumns = new[] { Tuple.Create (objectIDColumn, SortOrder.Descending), Tuple.Create (objectIDColumn, SortOrder.Ascending) };
      _dbCommandBuilderFactoryStub
          .Stub (
              stub => stub.CreateForRelationLookupFromUnionView (
                  Arg.Is ((UnionViewDefinition) classDefinition.StorageEntityDefinition),
                  Arg<SelectedColumnsSpecification>.Matches (cs => cs.SelectedColumns.SequenceEqual (expectedSelectedColumns)),
                  Arg.Is (idColumnDefinition),
                  Arg.Is (_foreignKeyValue),
                  Arg<OrderedColumnsSpecification>.Matches (o => o.Columns.SequenceEqual (expectedOrderedColumns)))
          ).Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateForRelationLookup (
          relationEndPointDefinition,
          _foreignKeyValue,
          new SortExpressionDefinition (new[] { sortedPropertySpecification1, sortedPropertySpecification2 }));

      Assert.That (result, Is.TypeOf (typeof (IndirectDataContainerLoadCommand)));
      var command = (IndirectDataContainerLoadCommand) result;
      Assert.That (command.ObjectIDLoadCommand, Is.TypeOf (typeof (MultiObjectIDLoadCommand)));
      Assert.That (((MultiObjectIDLoadCommand) command.ObjectIDLoadCommand).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
      Assert.That (((MultiObjectIDLoadCommand) command.ObjectIDLoadCommand).ObjectIDReader, Is.SameAs (_objectIDReaderStub));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The ClassDefinition must not have a NullEntityDefinition.")]
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

      _dbCommandBuilderFactoryStub
          .Stub (stub => stub.CreateForSingleIDLookupFromTable (_tableDefinition1, AllSelectedColumnsSpecification.Instance, objectID))
          .Return (_dbCommandBuilder1Stub);

      _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);
    }

    [Test]
    public void CreateForQuery ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery>();
      var commandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder>();

      _dbCommandBuilderFactoryStub.Stub (stub => stub.CreateForQuery (queryStub)).Return (commandBuilderStub);

      var result = _factory.CreateForDataContainerQuery (queryStub);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { commandBuilderStub }));
      Assert.That (((MultiDataContainerLoadCommand) result).DataContainerReader, Is.SameAs (_dataContainerReaderStub));
      Assert.That (((MultiDataContainerLoadCommand) result).AllowNulls, Is.True);
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

    private PropertyDefinition CreateForeignLeyEndPointDefinition (ClassDefinition classDefinition)
    {
      return PropertyDefinitionFactory.Create (
          classDefinition,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("OrderTicket"),
          new IDColumnDefinition (ColumnDefinitionObjectMother.ObjectIDColumn, ColumnDefinitionObjectMother.ClassIDColumn));
    }

    private SortedPropertySpecification CreateSortedPropertySpecification (
    ClassDefinition classDefinition, PropertyInfo propertyInfo, SimpleColumnDefinition simpleColumnDefinition, SortOrder sortOrder)
    {
      var sortedPropertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition, StorageClass.Persistent, propertyInfo, simpleColumnDefinition);
      return new SortedPropertySpecification (sortedPropertyDefinition, sortOrder);
    }
  }
}