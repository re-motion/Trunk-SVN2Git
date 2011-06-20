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
  public class RelatedDataContainerLookupCommandFactoryTest : SqlProviderBaseTest
  {
    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStub;
    private IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _storageProviderCommandFactory;
    private RelatedDataContainerLookupCommandFactory _factory;
    private IDataContainerReader _dataContainerReaderStub;
    private IObjectIDFactory _objectIDFactoryStub;
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
      _objectIDFactoryStub = MockRepository.GenerateStub<IObjectIDFactory>();

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

      _factory = new RelatedDataContainerLookupCommandFactory (_dbCommandBuilderFactoryStub, _storageProviderCommandFactory);
    }

    [Test]
    public void CreateCommand_TableDefinition_NoSortExpression ()
    {
      var foreignKeyValue = new ObjectID ("OrderTicket", Guid.NewGuid());
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
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

      _dbCommandBuilderFactoryStub
          .Stub (
              stub => stub.CreateForRelationLookupFromTable (
                  (TableDefinition) classDefinition.StorageEntityDefinition,
                  AllSelectedColumnsSpecification.Instance,
                  simpleColumnDefinition,
                  foreignKeyValue,
                  EmptyOrderedColumnsSpecification.Instance))
          .Return (_dbCommandBuilderStub);

      var result = _factory.CreateCommand (
          relationEndPointDefinition,
          foreignKeyValue,
          null,
          _commandExecutionContextStub,
          _dataContainerReaderStub,
          _objectIDFactoryStub);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilderStub }));
      Assert.That (((MultiDataContainerLoadCommand) result).AllowNulls, Is.False);
      Assert.That (((MultiDataContainerLoadCommand) result).DataContainerReader, Is.SameAs (_dataContainerReaderStub));
      Assert.That (((MultiDataContainerLoadCommand) result).CommandExecutionContext, Is.SameAs (_commandExecutionContextStub));
    }

    [Test]
    public void CreateCommand_TableDefinition_WithSortExpression ()
    {
      var foreignKeyValue = new ObjectID ("OrderTicket", Guid.NewGuid());
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      var objectIDColumn = new SimpleColumnDefinition ("OrderTicketID", typeof (Guid), "uniqueidentifier", true, false);
      var classIDColumn = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false);
      var idColumnDefinition = new IDColumnDefinition (objectIDColumn, classIDColumn);
      var idPropertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("OrderTicket"),
          idColumnDefinition);
      var sortPropertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition, StorageClass.Persistent, typeof (Order).GetProperty ("OrderNumber"), objectIDColumn);
      var relationEndPointDefinition = new RelationEndPointDefinition (idPropertyDefinition, false);
      var sortedPropertySpecification1 = new SortedPropertySpecification (sortPropertyDefinition, SortOrder.Descending);
      var sortedPropertySpecification2 = new SortedPropertySpecification (sortPropertyDefinition, SortOrder.Ascending);

      _dbCommandBuilderFactoryStub
          .Stub (
              stub => stub.CreateForRelationLookupFromTable (
                  Arg.Is ((TableDefinition) classDefinition.StorageEntityDefinition),
                  Arg.Is (AllSelectedColumnsSpecification.Instance),
                  Arg.Is(idColumnDefinition),
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
              })
          .Return (_dbCommandBuilderStub);

      var result = _factory.CreateCommand (
          relationEndPointDefinition,
          foreignKeyValue,
          new SortExpressionDefinition (new[] { sortedPropertySpecification1, sortedPropertySpecification2 }),
          _commandExecutionContextStub,
          _dataContainerReaderStub,
          _objectIDFactoryStub);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilderStub }));
      Assert.That (((MultiDataContainerLoadCommand) result).AllowNulls, Is.False);
      Assert.That (((MultiDataContainerLoadCommand) result).DataContainerReader, Is.SameAs (_dataContainerReaderStub));
      Assert.That (((MultiDataContainerLoadCommand) result).CommandExecutionContext, Is.SameAs (_commandExecutionContextStub));
    }

    [Test]
    public void CreateCommand_UnionViewDefinition_NoSortExpression ()
    {
      var foreignKeyValue = new ObjectID ("OrderTicket", Guid.NewGuid());
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      PrivateInvoke.SetNonPublicField (classDefinition, "_storageEntityDefinition", _unionViewDefinition);
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

      _dbCommandBuilderFactoryStub
          .Stub (
              stub => stub.CreateForRelationLookupFromUnionView (
                  Arg.Is ((UnionViewDefinition) classDefinition.StorageEntityDefinition),
                  Arg<SelectedColumnsSpecification>.Is.Anything,
                  Arg.Is (simpleColumnDefinition),
                  Arg.Is (foreignKeyValue),
                  Arg.Is (EmptyOrderedColumnsSpecification.Instance)))
          .Return (_dbCommandBuilderStub);

      var result = _factory.CreateCommand (
          relationEndPointDefinition,
          foreignKeyValue,
          null,
          _commandExecutionContextStub,
          _dataContainerReaderStub,
          _objectIDFactoryStub);

      Assert.That (result, Is.TypeOf (typeof (IndirectDataContainerLoadCommand)));
      Assert.That (((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand, Is.TypeOf (typeof (MultiObjectIDLoadCommand)));
      Assert.That (
          ((MultiObjectIDLoadCommand) ((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand).DbCommandBuilders,
          Is.EqualTo (new[] { _dbCommandBuilderStub }));
      Assert.That (
          ((MultiObjectIDLoadCommand) ((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand).CommandExecutionContext,
          Is.SameAs (_commandExecutionContextStub));
      Assert.That (
          ((MultiObjectIDLoadCommand) ((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand).ObjectIDFactory,
          Is.SameAs (_objectIDFactoryStub));
      Assert.That (((IndirectDataContainerLoadCommand) result).StorageProviderCommandFactory, Is.SameAs (_storageProviderCommandFactory));
    }

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
          _commandExecutionContextStub,
          _dataContainerReaderStub,
          _objectIDFactoryStub);

      Assert.That (result, Is.TypeOf (typeof (IndirectDataContainerLoadCommand)));
      Assert.That (((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand, Is.TypeOf (typeof (MultiObjectIDLoadCommand)));
      Assert.That (
          ((MultiObjectIDLoadCommand) ((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand).DbCommandBuilders,
          Is.EqualTo (new[] { _dbCommandBuilderStub }));
      Assert.That (
          ((MultiObjectIDLoadCommand) ((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand).CommandExecutionContext,
          Is.SameAs (_commandExecutionContextStub));
      Assert.That (
          ((MultiObjectIDLoadCommand) ((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand).CommandExecutionContext,
          Is.SameAs (_commandExecutionContextStub));
      Assert.That (
          ((MultiObjectIDLoadCommand) ((IndirectDataContainerLoadCommand) result).ObjectIDLoadCommand).ObjectIDFactory,
          Is.SameAs (_objectIDFactoryStub));
      Assert.That (((IndirectDataContainerLoadCommand) result).StorageProviderCommandFactory, Is.SameAs (_storageProviderCommandFactory));
    }
  }
}