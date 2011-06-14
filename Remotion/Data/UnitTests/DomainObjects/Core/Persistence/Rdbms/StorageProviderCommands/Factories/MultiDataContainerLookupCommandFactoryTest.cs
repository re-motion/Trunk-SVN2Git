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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class MultiDataContainerLookupCommandFactoryTest : SqlProviderBaseTest
  {
    private ObjectID _objectID1;
    private ObjectID _objectID2;
    private ObjectID _objectID3;
    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStub;
    private MultiDataContainerLookupCommandFactory _factory;
    private IDbCommandFactory _dbCommandFactoryStub;
    private IDbCommandExecutor _dbCommandExecutorStub;
    private IDataContainerFactory _dataContainerFactoryStub;
    private IDbCommandBuilder _dbCommandBuilder1Stub;
    private IDbCommandBuilder _dbCommandBuilder2Stub;

    public override void SetUp ()
    {
      base.SetUp();

      _objectID1 = new ObjectID ("Order", Guid.NewGuid());
      _objectID2 = new ObjectID ("Order", Guid.NewGuid());
      _objectID3 = new ObjectID ("OrderItem", Guid.NewGuid());

      _dbCommandBuilderFactoryStub = MockRepository.GenerateStub<IDbCommandBuilderFactory>();
      _factory = new MultiDataContainerLookupCommandFactory (_dbCommandBuilderFactoryStub);

      _dbCommandFactoryStub = MockRepository.GenerateStub<IDbCommandFactory>();
      _dbCommandExecutorStub = MockRepository.GenerateStub<IDbCommandExecutor>();
      _dataContainerFactoryStub = MockRepository.GenerateStub<IDataContainerFactory>();

      _dbCommandBuilder1Stub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dbCommandBuilder2Stub = MockRepository.GenerateStub<IDbCommandBuilder>();
    }

    [Test]
    public void CreateCommand_SingleIDLookup ()
    {
      _dbCommandBuilderFactoryStub.Stub (
          stub =>
          stub.CreateForSingleIDLookupFromTable (
              ((TableDefinition) _objectID1.ClassDefinition.StorageEntityDefinition), AllSelectedColumnsSpecification.Instance, _objectID1)).Return (
                  _dbCommandBuilder1Stub);

      var result = _factory.CreateCommand (new[] { _objectID1 }, _dbCommandFactoryStub, _dbCommandExecutorStub, _dataContainerFactoryStub);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
    }

    [Test]
    public void CreateCommand_MultiIDLookup ()
    {
      _dbCommandBuilderFactoryStub.Stub (
          stub =>
          stub.CreateForMultiIDLookupFromTable (
              ((TableDefinition) _objectID1.ClassDefinition.StorageEntityDefinition),
              AllSelectedColumnsSpecification.Instance,
              new[] { _objectID1, _objectID2 })).Return (
                  _dbCommandBuilder1Stub);

      var result = _factory.CreateCommand (
          new[] { _objectID1, _objectID2 }, _dbCommandFactoryStub, _dbCommandExecutorStub, _dataContainerFactoryStub);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub }));
    }

    [Test]
    public void CreateCommand_SingleAndMultiIDLookup ()
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
          new[] { _objectID1, _objectID2, _objectID3 }, _dbCommandFactoryStub, _dbCommandExecutorStub, _dataContainerFactoryStub);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub, _dbCommandBuilder2Stub }));
    }
  }
}