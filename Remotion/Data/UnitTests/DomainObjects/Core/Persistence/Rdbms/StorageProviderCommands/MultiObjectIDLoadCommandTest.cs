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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class MultiObjectIDLoadCommandTest : SqlProviderBaseTest
  {
    private IDbCommandBuilder _dbCommandBuilder1Stub;
    private IDbCommandBuilder _dbCommandBuilder2Stub;
    private IDbCommandFactory _dbCommandFactoryStub;
    private IDbCommandExecutor _dbCommandExecutor;
    private MultiObjectIDLoadCommand _command;
    private IDbCommand _dbCommandStub;
    private IDataReader _dataReaderStub;
    private IObjectIDFactory _objectIDFactoryStub;
    private ObjectID[] _fakeResult;
    private ObjectID _objectID1;

    public override void SetUp ()
    {
      base.SetUp();

      _dbCommandBuilder1Stub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dbCommandBuilder2Stub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dbCommandFactoryStub = MockRepository.GenerateStub<IDbCommandFactory>();
      _dbCommandExecutor = MockRepository.GenerateStub<IDbCommandExecutor>();

      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _objectIDFactoryStub = MockRepository.GenerateStub<IObjectIDFactory>();

      _command = new MultiObjectIDLoadCommand (
          new[] { _dbCommandBuilder1Stub, _dbCommandBuilder2Stub }, _dbCommandFactoryStub, _dbCommandExecutor, _objectIDFactoryStub);

      _objectID1 = new ObjectID ("Order", Guid.NewGuid());
      _fakeResult = new[] { _objectID1 };
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Stub, _dbCommandBuilder2Stub }));
      Assert.That (_command.DbCommandExecutor, Is.SameAs (_dbCommandExecutor));
      Assert.That (_command.DbCommandFactory, Is.SameAs (_dbCommandFactoryStub));
      Assert.That (_command.ObjectIDFactory, Is.SameAs (_objectIDFactoryStub));
    }

    [Test]
    public void Execute_OneDbCommandBuilder ()
    {
      _dbCommandBuilder1Stub.Stub (stub => stub.Create (_dbCommandFactoryStub)).Return (_dbCommandStub);
      _dbCommandExecutor.Stub (stub => stub.ExecuteReader (_dbCommandStub, CommandBehavior.SingleResult)).Return (_dataReaderStub);
      _objectIDFactoryStub.Stub (stub => stub.ReadSequence (_dataReaderStub)).Return (_fakeResult);

      var command = new MultiObjectIDLoadCommand (new[] { _dbCommandBuilder1Stub }, _dbCommandFactoryStub, _dbCommandExecutor, _objectIDFactoryStub);

      var result = command.Execute().ToArray();

      Assert.That (result, Is.EqualTo (new[] { _objectID1 }));
    }

    [Test]
    public void Execute_SeveralDbCommandBuilders ()
    {
      _dbCommandBuilder1Stub.Stub (stub => stub.Create (_dbCommandFactoryStub)).Return (_dbCommandStub);
      _dbCommandBuilder2Stub.Stub (stub => stub.Create (_dbCommandFactoryStub)).Return (_dbCommandStub);
      _dbCommandExecutor.Stub (stub => stub.ExecuteReader (_dbCommandStub, CommandBehavior.SingleResult)).Return (_dataReaderStub);
      _objectIDFactoryStub.Stub (stub => stub.ReadSequence (_dataReaderStub)).Return (_fakeResult);

      var command = new MultiObjectIDLoadCommand (
          new[] { _dbCommandBuilder1Stub, _dbCommandBuilder2Stub }, _dbCommandFactoryStub, _dbCommandExecutor, _objectIDFactoryStub);

      var result = command.Execute().ToArray();

      Assert.That (result, Is.EqualTo (new[] { _objectID1, _objectID1 }));
    }
  }
}