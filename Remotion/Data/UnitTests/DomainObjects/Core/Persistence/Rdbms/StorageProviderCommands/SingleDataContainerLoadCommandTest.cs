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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class SingleDataContainerLoadCommandTest : SqlProviderBaseTest
  {
    private IDbCommandBuilder _dbCommandBuilderStub;
    private IDbCommandFactory _dbCommandFactory;
    private IDbCommand _dbCommandStub;
    private IDbCommandExecutor _dbCommandExecutorStub;
    private IDataReader _dataReaderMock;
    private SingleDataContainerLoadCommand _command;
    private IDataContainerReader _dataContainerReaderStub;
    private DataContainer _container;

    public override void SetUp ()
    {
      base.SetUp();

      _container = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;

      _dataReaderMock = MockRepository.GenerateStub<IDataReader>();

      _dbCommandFactory = MockRepository.GenerateStub<IDbCommandFactory>();
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbCommandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dbCommandBuilderStub.Stub (stub => stub.Create (_dbCommandFactory)).Return (_dbCommandStub);

      _dbCommandExecutorStub = MockRepository.GenerateStub<IDbCommandExecutor>();
      _dbCommandExecutorStub.Stub (stub => stub.ExecuteReader (_dbCommandStub, CommandBehavior.SingleRow)).Return(_dataReaderMock);

      _dataContainerReaderStub = MockRepository.GenerateStub<IDataContainerReader>();
      _command = new SingleDataContainerLoadCommand (
          _dbCommandBuilderStub, _dbCommandFactory, _dbCommandExecutorStub, _dataContainerReaderStub);
      _dataContainerReaderStub.Stub (stub => stub.CreateDataContainer (_dataReaderMock)).Return (_container);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.DbCommandBuilder, Is.SameAs (_dbCommandBuilderStub));
      Assert.That (_command.DbCommandExecutor, Is.SameAs (_dbCommandExecutorStub));
      Assert.That (_command.DbCommandFactory, Is.SameAs (_dbCommandFactory));
      Assert.That (_command.DataContainerReader, Is.SameAs(_dataContainerReaderStub));
    }

    [Test]
    public void Execute ()
    {
      var result = _command.Execute();

      Assert.That (result, Is.SameAs (_container));
    }
    
  }
}