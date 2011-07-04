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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
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

      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();
      _dataContainerReaderStub = MockRepository.GenerateStub<IDataContainerReader>();

      _dbCommandBuilder1Stub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dbCommandBuilder2Stub = MockRepository.GenerateStub<IDbCommandBuilder>();

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
                  _tableDefinition1, AllSelectedColumnsSpecification.Instance, _objectID1))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateCommand (new[] { _objectID1 });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (
          ((MultiDataContainerLoadCommand) ((MultiDataContainerSortCommand) result).Command).DbCommandBuilders,
          Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
    }

    [Test]
    public void CreateCommand_MultiIDLookup_TableDefinition ()
    {
      _dbCommandBuilderFactoryStub
          .Stub (
              stub => stub.CreateForMultiIDLookupFromTable (
                  _tableDefinition1,
                  AllSelectedColumnsSpecification.Instance,
                  new[] { _objectID1, _objectID2 }))
          .Return (_dbCommandBuilder1Stub);

      var result = _factory.CreateCommand (new[] { _objectID1, _objectID2 });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (
          ((MultiDataContainerLoadCommand) ((MultiDataContainerSortCommand) result).Command).DbCommandBuilders,
          Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
    }

    [Test]
    public void CreateCommand_MultipleTableDefinitions ()
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

      var result = _factory.CreateCommand (new[] { _objectID1, _objectID2, _objectID3 });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (
          ((MultiDataContainerLoadCommand) ((MultiDataContainerSortCommand) result).Command).DbCommandBuilders,
          Is.EqualTo (new[] { _dbCommandBuilder1Stub, _dbCommandBuilder2Stub }));
    }

    [Test]
    public void CreateCommand_FilterViewDefinition ()
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

      var result = _factory.CreateCommand (new[] { objectID });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
      Assert.That (((MultiDataContainerSortCommand) result).Command, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (
          ((MultiDataContainerLoadCommand) ((MultiDataContainerSortCommand) result).Command).DbCommandBuilders,
          Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An ObjectID's EntityDefinition cannot be a UnionViewDefinition.")]
    public void CreateCommand_UnionViewDefinition ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "Table"));
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"), tableDefinition);

      var objectID = CreateObjectID (unionViewDefinition);

      _factory.CreateCommand (new[] { objectID });
    }

    [Ignore ("TODO RM-4090")]
    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An ObjectID's EntityDefinition cannot be a NullEntityDefinition.")]
    public void CreateCommand_NullEntityDefinition ()
    {
      var nullEntityDefintion = new NullEntityDefinition (TestDomainStorageProviderDefinition);

      var objectID = CreateObjectID (nullEntityDefintion);

      _factory.CreateCommand (new[] { objectID });
    }

    private ObjectID CreateObjectID (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetStorageEntity (entityDefinition);

      return new ObjectID (classDefinition, Guid.NewGuid());
    }
  }
}