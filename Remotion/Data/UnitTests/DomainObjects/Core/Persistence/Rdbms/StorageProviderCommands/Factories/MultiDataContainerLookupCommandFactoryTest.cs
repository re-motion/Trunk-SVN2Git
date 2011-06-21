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
using Remotion.Data.DomainObjects.Persistence.Model;
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
  public class MultiDataContainerLookupCommandFactoryTest : StandardMappingTest
  {
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;

    private ObjectID _objectID1;
    private ObjectID _objectID2;
    private ObjectID _objectID3;

    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStub;
    private MultiDataContainerLookupCommandFactory _factory;
    private IDataContainerReader _dataContainerReaderStub;
    private IDbCommandBuilder _dbCommandBuilder1Stub;
    private IDbCommandBuilder _dbCommandBuilder2Stub;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp();

      _tableDefinition1 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table1"));
      _tableDefinition2 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table1"));

      _objectID1 = CreateObjectID (_tableDefinition1);
      _objectID2 = CreateObjectID (_tableDefinition1);
      _objectID3 = CreateObjectID (_tableDefinition2);

      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext> ();
      _dataContainerReaderStub = MockRepository.GenerateStub<IDataContainerReader> ();

      _dbCommandBuilder1Stub = MockRepository.GenerateStub<IDbCommandBuilder> ();
      _dbCommandBuilder2Stub = MockRepository.GenerateStub<IDbCommandBuilder> ();

      // TODO Review 4071: In the tests, instead of _objectID1/2/3.ClassDefinition.StorageEntityDefinition => _tableDefinition1/2

      _dbCommandBuilderFactoryStub = MockRepository.GenerateStub<IDbCommandBuilderFactory>();
      _factory = new MultiDataContainerLookupCommandFactory (_dbCommandBuilderFactoryStub, _dataContainerReaderStub);
    }

    [Test]
    public void CreateCommand_SingleIDLookup_TableDefinition ()
    {
      _dbCommandBuilderFactoryStub
          .Stub (
              stub =>
              stub.CreateForSingleIDLookupFromTable (
                  ((TableDefinition) _objectID1.ClassDefinition.StorageEntityDefinition), AllSelectedColumnsSpecification.Instance, _objectID1))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateCommand (new[] { _objectID1 }, _commandExecutionContextStub);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
    }

    [Test]
    public void CreateCommand_MultiIDLookup_TableDefinition ()
    {
      // TODO Review 4071: Reformat
      _dbCommandBuilderFactoryStub.Stub (
          stub =>
          stub.CreateForMultiIDLookupFromTable (
              ((TableDefinition) _objectID1.ClassDefinition.StorageEntityDefinition),
              AllSelectedColumnsSpecification.Instance,
              new[] { _objectID1, _objectID2 })).Return (
                  _dbCommandBuilder1Stub);

      var result = _factory.CreateCommand (
          new[] { _objectID1, _objectID2 }, _commandExecutionContextStub);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
    }

    [Test]
    public void CreateCommand_MultipleTableDefinitions ()
    {
      _dbCommandBuilderFactoryStub.Stub (
          stub =>
          stub.CreateForSingleIDLookupFromTable (
              ((TableDefinition) _objectID3.ClassDefinition.StorageEntityDefinition), AllSelectedColumnsSpecification.Instance, _objectID3)).Return (
                  _dbCommandBuilder2Stub);
      _dbCommandBuilderFactoryStub.Stub (
          stub =>
          stub.CreateForMultiIDLookupFromTable (
              ((TableDefinition) _objectID1.ClassDefinition.StorageEntityDefinition),
              AllSelectedColumnsSpecification.Instance,
              new[] { _objectID1, _objectID2 })).Return (
                  _dbCommandBuilder1Stub);

      var result = _factory.CreateCommand (
          new[] { _objectID1, _objectID2, _objectID3 }, _commandExecutionContextStub);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub, _dbCommandBuilder2Stub }));
    }

    [Test]
    public void CreateCommand_FilterViewDefinition ()
    {
      // TODO Review 4071: Add FilterViewDefinitionObjectMother (also use in SingleDataContainerLookupCommandFactoryTest and other tests)
      // TODO Review 4071: Explicitly create FilterViewDefinition, use CreateObjectID
      var objectID = new ObjectID ("File", Guid.NewGuid());

      _dbCommandBuilderFactoryStub.Stub (
          stub =>
          stub.CreateForSingleIDLookupFromTable (
              ((TableDefinition) objectID.ClassDefinition.BaseClass.StorageEntityDefinition), AllSelectedColumnsSpecification.Instance, objectID)).
          Return (
              _dbCommandBuilder1Stub);

      var result = _factory.CreateCommand (new[] { objectID }, _commandExecutionContextStub);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An ObjectID's EntityDefinition cannot be a UnionViewDefinition.")]
    public void CreateCommand_UnionViewDefinition ()
    {
      // TODO Review 4071: Add UnionViewDefinitionObjectMother (also use in SingleDataContainerLookupCommandFactoryTest and other tests)
      // TODO Review 4071: Use shorter overload
      var tableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "Table"),
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn);
      var unionViewDefinition = new UnionViewDefinition (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "ViewName"),
          new[] { tableDefinition },
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      // TODO Review 4071: Use CreateObjectID
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      PrivateInvoke.SetNonPublicField (classDefinition, "_storageEntityDefinition", unionViewDefinition);

      var objectID = new ObjectID (classDefinition, Guid.NewGuid());

      _factory.CreateCommand (new[] { objectID }, _commandExecutionContextStub);
    }

    private ObjectID CreateObjectID (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetStorageEntity (entityDefinition);

      return new ObjectID (classDefinition, Guid.NewGuid());
    }
  }
}