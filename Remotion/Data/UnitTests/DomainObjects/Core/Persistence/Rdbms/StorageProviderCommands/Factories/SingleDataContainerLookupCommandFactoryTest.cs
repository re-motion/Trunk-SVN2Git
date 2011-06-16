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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class SingleDataContainerLookupCommandFactoryTest : SqlProviderBaseTest
  {
    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStub;
    private SingleDataContainerLookupCommandFactory _factory;
    private IDbCommandFactory _dbCommandFactoryStub;
    private IDbCommandExecutor _dbCommandExecutorStub;
    private IDbCommandBuilder _dbCommandBuilderStub;
    private IDataContainerReader _dataContainerReaderStub;

    public override void SetUp ()
    {
      base.SetUp();

      _dbCommandBuilderFactoryStub = MockRepository.GenerateStub<IDbCommandBuilderFactory>();
      _dbCommandFactoryStub = MockRepository.GenerateStub<IDbCommandFactory>();
      _dbCommandExecutorStub = MockRepository.GenerateStub<IDbCommandExecutor>();
      _dbCommandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dataContainerReaderStub = MockRepository.GenerateStub<IDataContainerReader>();

      _factory = new SingleDataContainerLookupCommandFactory (_dbCommandBuilderFactoryStub);
    }

    [Test]
    public void CreateCommand_TableDefinition ()
    {
      var objectID = DomainObjectIDs.Order1;

      _dbCommandBuilderFactoryStub
          .Stub (
              stub =>
              stub.CreateForSingleIDLookupFromTable (
                  ((TableDefinition) objectID.ClassDefinition.StorageEntityDefinition), AllSelectedColumnsSpecification.Instance, objectID))
          .Return (_dbCommandBuilderStub);

      var result = _factory.CreateCommand (objectID, _dbCommandFactoryStub, _dbCommandExecutorStub, _dataContainerReaderStub);

      Assert.That (result, Is.TypeOf (typeof (SingleDataContainerLoadCommand)));
      Assert.That (((SingleDataContainerLoadCommand) result).DbCommandBuilder, Is.SameAs (_dbCommandBuilderStub));
      Assert.That (((SingleDataContainerLoadCommand) result).DbCommandExecutor, Is.SameAs (_dbCommandExecutorStub));
      Assert.That (((SingleDataContainerLoadCommand) result).DbCommandFactory, Is.SameAs (_dbCommandFactoryStub));
      Assert.That (((SingleDataContainerLoadCommand) result).DataContainerReader, Is.SameAs (_dataContainerReaderStub));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An ObjectID's EntityDefinition cannot be a UnionViewDefinition.")]
    public void CreateCommand_UnionViewDefinition ()
    {
      var objectIDColumn = new SimpleColumnDefinition ("ID", typeof (Guid), "uniqueidentifier", false, true);
      var classIDColumn = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", true, false);
      var timestampColumn = new SimpleColumnDefinition ("Timestamp", typeof (DateTime), "datetime", true, false);

      var tableDefinition = new TableDefinition (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "Table"),
          null,
          objectIDColumn,
          classIDColumn,
          timestampColumn,
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      var unionViewDefinition = new UnionViewDefinition (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "ViewName"),
          new[] { tableDefinition },
          objectIDColumn,
          classIDColumn,
          timestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order), TestDomainStorageProviderDefinition);
      PrivateInvoke.SetNonPublicField (classDefinition, "_storageEntityDefinition", unionViewDefinition);

      var objectID = new ObjectID (classDefinition, Guid.NewGuid ());

      _factory.CreateCommand (objectID, _dbCommandFactoryStub, _dbCommandExecutorStub, _dataContainerReaderStub);
    }

    [Test]
    public void CreateCommand_FilterViewDefinition ()
    {
      var objectID = new ObjectID ("File", Guid.NewGuid());

      _dbCommandBuilderFactoryStub
          .Stub (
              stub =>
              stub.CreateForSingleIDLookupFromTable (
                  ((TableDefinition) objectID.ClassDefinition.BaseClass.StorageEntityDefinition), AllSelectedColumnsSpecification.Instance, objectID))
          .Return (_dbCommandBuilderStub);

      var result = _factory.CreateCommand (objectID, _dbCommandFactoryStub, _dbCommandExecutorStub, _dataContainerReaderStub);

      Assert.That (result, Is.TypeOf (typeof (SingleDataContainerLoadCommand)));
      Assert.That (((SingleDataContainerLoadCommand) result).DbCommandBuilder, Is.SameAs (_dbCommandBuilderStub));
      Assert.That (((SingleDataContainerLoadCommand) result).DbCommandExecutor, Is.SameAs (_dbCommandExecutorStub));
      Assert.That (((SingleDataContainerLoadCommand) result).DbCommandFactory, Is.SameAs (_dbCommandFactoryStub));
      Assert.That (((SingleDataContainerLoadCommand) result).DataContainerReader, Is.SameAs (_dataContainerReaderStub));
    }
  }
}