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
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class RelatedDataContainerLookupCommandFactoryTest : StandardMappingTest
  {
    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStub;
    private IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _storageProviderCommandFactory;
    private RelatedDataContainerLookupCommandFactory _factory;
    private IDataContainerReader _dataContainerReaderStub;
    private IObjectIDReader _objectIDReaderStub;
    private IDbCommandBuilder _dbCommandBuilderStub;
    private TableDefinition _tableDefinition;
    private UnionViewDefinition _unionViewDefinition;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp();

      _dbCommandBuilderFactoryStub = MockRepository.GenerateStub<IDbCommandBuilderFactory>();
      _storageProviderCommandFactory = MockRepository.GenerateStub<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>>();

      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();

      _dataContainerReaderStub = MockRepository.GenerateStub<IDataContainerReader>();
      _objectIDReaderStub = MockRepository.GenerateStub<IObjectIDReader>();

      _dbCommandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder>();

      _tableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "Table"),
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn);
      _unionViewDefinition = new UnionViewDefinition (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "ViewName"),
          new[] { _tableDefinition },
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      _factory = new RelatedDataContainerLookupCommandFactory (
          _dbCommandBuilderFactoryStub, _storageProviderCommandFactory, _dataContainerReaderStub, _objectIDReaderStub);
    }

    // TODO Review 4074: Refactor the same way as below
    [Test]
    public void CreateCommand_TableDefinition_NoSortExpression ()
    {
      var foreignKeyValue = new ObjectID ("OrderTicket", Guid.NewGuid());
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      var foreignKeyColumn =
          new IDColumnDefinition (
              new SimpleColumnDefinition ("OrderTicketID", typeof (Guid), "uniqueidentifier", true, false),
              new SimpleColumnDefinition ("OrderTicketID", typeof (Guid), "uniqueidentifier", true, false));
      var propertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("OrderTicket"),
          foreignKeyColumn);
      var relationEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);

      _dbCommandBuilderFactoryStub
          .Stub (
              stub => stub.CreateForRelationLookupFromTable (
                  (TableDefinition) classDefinition.StorageEntityDefinition,
                  AllSelectedColumnsSpecification.Instance,
                  foreignKeyColumn,
                  foreignKeyValue,
                  EmptyOrderedColumnsSpecification.Instance))
          .Return (_dbCommandBuilderStub);

      var result = _factory.CreateCommand (
          relationEndPointDefinition,
          foreignKeyValue,
          null,
          _commandExecutionContextStub);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilderStub }));
      Assert.That (((MultiDataContainerLoadCommand) result).AllowNulls, Is.False);
      Assert.That (((MultiDataContainerLoadCommand) result).DataContainerReader, Is.SameAs (_dataContainerReaderStub));
    }

    [Test]
    public void CreateCommand_TableDefinition_WithSortExpression ()
    {
      // TODO Review 4074: Move to Setup, use CreateObjectID from SingleDataContainerLookupCommandFactoryTest
      var foreignKeyValue = new ObjectID ("OrderTicket", Guid.NewGuid());

      // TODO Review 4074: Create _tableDefinition in SetUp, add a CreateClassDefinition (IEntityDefinition) method that creates a class definition and sets the entity (extract from CreateObjectID)
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);

      // TODO Review 4074: Create idColumnDefinition in Setup, rename to foreignKeyColumnDefinition
      var objectIDColumn = new SimpleColumnDefinition ("OrderTicketID", typeof (Guid), "uniqueidentifier", true, false);
      var classIDColumn = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false);
      var idColumnDefinition = new IDColumnDefinition (objectIDColumn, classIDColumn);

      // TODO Review 4074: Move to CreateForeignKeyEndPointDefinition method
      var idPropertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("OrderTicket"),
          idColumnDefinition);
      var relationEndPointDefinition = new RelationEndPointDefinition (idPropertyDefinition, false);

      // TODO Review 4074: Extract to CreateSortedPropertyDefinition (SimpleColumnDefinition, SortOrder)
      var sortedPropertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition, StorageClass.Persistent, typeof (Order).GetProperty ("OrderNumber"), objectIDColumn);
      var sortedPropertySpecification1 = new SortedPropertySpecification (sortedPropertyDefinition, SortOrder.Descending);

      var sortedPropertySpecification2 = new SortedPropertySpecification (sortedPropertyDefinition, SortOrder.Ascending);

      // TODO Review 4074: also refactor other tests to use expectedOrderedColumns
      var expectedOrderedColumns = new[] { Tuple.Create (objectIDColumn, SortOrder.Descending), Tuple.Create (objectIDColumn, SortOrder.Ascending) };
      _dbCommandBuilderFactoryStub
          .Stub (
              stub => stub.CreateForRelationLookupFromTable (
                  Arg.Is ((TableDefinition) classDefinition.StorageEntityDefinition),
                  Arg.Is (AllSelectedColumnsSpecification.Instance),
                  Arg.Is (idColumnDefinition),
                  Arg.Is (foreignKeyValue),
                  Arg<OrderedColumnsSpecification>.Matches (o => o.Columns.SequenceEqual (expectedOrderedColumns))))
          .Return (_dbCommandBuilderStub);

      var result = _factory.CreateCommand (
          relationEndPointDefinition,
          foreignKeyValue,
          new SortExpressionDefinition (new[] { sortedPropertySpecification1, sortedPropertySpecification2 }),
          _commandExecutionContextStub);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilderStub }));
      Assert.That (((MultiDataContainerLoadCommand) result).AllowNulls, Is.False);
      Assert.That (((MultiDataContainerLoadCommand) result).DataContainerReader, Is.SameAs (_dataContainerReaderStub));
    }

    [Test]
    public void CreateCommand_UnionViewDefinition_NoSortExpression ()
    {
      var foreignKeyValue = new ObjectID ("OrderTicket", Guid.NewGuid());
      // TODO Review 4074: Manually create UnionViewDefinition, using UnionViewObjectMother, then use CreateClassDefinition
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      PrivateInvoke.SetNonPublicField (classDefinition, "_storageEntityDefinition", _unionViewDefinition);
      var unionViewDefinition = (UnionViewDefinition) classDefinition.StorageEntityDefinition;

      var simpleColumnDefinition =
          new IDColumnDefinition (
              new SimpleColumnDefinition ("OrderTicketID", typeof (Guid), "uniqueidentifier", true, false),
              new SimpleColumnDefinition ("OrderTicketID", typeof (Guid), "uniqueidentifier", true, false));
      var propertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("OrderTicket"),
          simpleColumnDefinition);
      var relationEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);

      var expectedSelectedColumns = new[] { unionViewDefinition.ObjectIDColumn, unionViewDefinition.ClassIDColumn };
      _dbCommandBuilderFactoryStub
          .Stub (
              stub => stub.CreateForRelationLookupFromUnionView (
                  Arg.Is (unionViewDefinition),
                  Arg<SelectedColumnsSpecification>.Matches (spec => spec.SelectedColumns.SequenceEqual (expectedSelectedColumns)),
                  Arg.Is (simpleColumnDefinition),
                  Arg.Is (foreignKeyValue),
                  Arg.Is (EmptyOrderedColumnsSpecification.Instance)))
          .Return (_dbCommandBuilderStub);

      var result = _factory.CreateCommand (
          relationEndPointDefinition,
          foreignKeyValue,
          null,
          _commandExecutionContextStub);

      // TODO Review 4074: Extract variable
      Assert.That (result, Is.TypeOf (typeof (IndirectDataContainerLoadCommand)));
      Assert.That (((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand, Is.TypeOf (typeof (MultiObjectIDLoadCommand)));
      Assert.That (
          ((MultiObjectIDLoadCommand) ((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand).DbCommandBuilders,
          Is.EqualTo (new[] { _dbCommandBuilderStub }));
      Assert.That (
          ((MultiObjectIDLoadCommand) ((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand).ObjectIDReader,
          Is.SameAs (_objectIDReaderStub));
      Assert.That (((IndirectDataContainerLoadCommand) result).StorageProviderCommandFactory, Is.SameAs (_storageProviderCommandFactory));
    }

    // TODO Review 4074: Refactor the same way as above
    [Test]
    public void CreateCommand_UnionViewDefinition_WithSortExpression ()
    {
      var foreignKeyValue = new ObjectID ("OrderTicket", Guid.NewGuid());
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      PrivateInvoke.SetNonPublicField (classDefinition, "_storageEntityDefinition", _unionViewDefinition);
      var objectIDColumn = new SimpleColumnDefinition ("OrderTicketID", typeof (Guid), "uniqueidentifier", true, false);
      var idColumnDefinition =
          new IDColumnDefinition (
              objectIDColumn,
              new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false));
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

      _dbCommandBuilderFactoryStub
          .Stub (
              stub => stub.CreateForRelationLookupFromUnionView (
                  Arg.Is ((UnionViewDefinition) classDefinition.StorageEntityDefinition),
                  // TODO Review 4074: use expectedSelectedColumns
                  Arg<SelectedColumnsSpecification>.Is.Anything,
                  Arg.Is (idColumnDefinition),
                  Arg.Is (foreignKeyValue),
                  Arg<OrderedColumnsSpecification>.Matches (o => o.Columns.ToList().Count == 2)))
          .WhenCalled (
              mi =>
              {
                var orderSpecificationColumns = ((OrderedColumnsSpecification) mi.Arguments[4]).Columns.ToList();
                Assert.That (orderSpecificationColumns[0].Item1, Is.SameAs (objectIDColumn));
                Assert.That (orderSpecificationColumns[0].Item2, Is.EqualTo (SortOrder.Descending));
                Assert.That (orderSpecificationColumns[1].Item1, Is.SameAs (objectIDColumn));
                Assert.That (orderSpecificationColumns[1].Item2, Is.EqualTo (SortOrder.Ascending));
              }).Return (_dbCommandBuilderStub);

      var result = _factory.CreateCommand (
          relationEndPointDefinition,
          foreignKeyValue,
          new SortExpressionDefinition (new[] { sortedPropertySpecification1, sortedPropertySpecification2 }),
          _commandExecutionContextStub);

      Assert.That (result, Is.TypeOf (typeof (IndirectDataContainerLoadCommand)));
      Assert.That (((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand, Is.TypeOf (typeof (MultiObjectIDLoadCommand)));
      Assert.That (
          ((MultiObjectIDLoadCommand) ((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand).DbCommandBuilders,
          Is.EqualTo (new[] { _dbCommandBuilderStub }));
      Assert.That (
          ((MultiObjectIDLoadCommand) ((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand).ObjectIDReader,
          Is.SameAs (_objectIDReaderStub));
      Assert.That (((IndirectDataContainerLoadCommand) result).StorageProviderCommandFactory, Is.SameAs (_storageProviderCommandFactory));
    }
  }
}