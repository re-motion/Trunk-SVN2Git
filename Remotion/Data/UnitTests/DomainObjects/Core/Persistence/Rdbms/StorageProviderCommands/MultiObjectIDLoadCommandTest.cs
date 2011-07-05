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
using System.Data;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class MultiObjectIDLoadCommandTest : SqlProviderBaseTest
  {
    private IDbCommandBuilder _dbCommandBuilder1Mock;
    private IDbCommandBuilder _dbCommandBuilder2Mock;
    private MultiObjectIDLoadCommand _command;
    private IDbCommand _dbCommandMock1;
    private IDbCommand _dbCommandMock2;
    private IDataReader _dataReaderMock;
    private IObjectIDReader _objectIDReaderStub;
    private ObjectID[] _fakeResult;
    private ObjectID _objectID1;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;
    private MockRepository _repository;

    public override void SetUp ()
    {
      base.SetUp();

      _repository = new MockRepository();

      _dbCommandBuilder1Mock = _repository.StrictMock<IDbCommandBuilder>();
      _dbCommandBuilder2Mock = _repository.StrictMock<IDbCommandBuilder>();

      _commandExecutionContextStub = _repository.Stub<IRdbmsProviderCommandExecutionContext> ();
      _objectIDReaderStub = _repository.Stub<IObjectIDReader> ();

      _dbCommandMock1 = _repository.StrictMock<IDbCommand>();
      _dbCommandMock2 = _repository.StrictMock<IDbCommand> ();
      _dataReaderMock = _repository.StrictMock<IDataReader>();
      
      _command = new MultiObjectIDLoadCommand (new[] { _dbCommandBuilder1Mock, _dbCommandBuilder2Mock }, _objectIDReaderStub);

      _objectID1 = new ObjectID ("Order", Guid.NewGuid());
      _fakeResult = new[] { _objectID1 };
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilder1Mock, _dbCommandBuilder2Mock }));
      Assert.That (_command.ObjectIDReader, Is.SameAs (_objectIDReaderStub));
    }

    [Test]
    public void Execute_OneDbCommandBuilder ()
    {
      _dbCommandBuilder1Mock.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock1);
      _dbCommandMock1.Expect (mock => mock.Dispose());
      _dataReaderMock.Expect (mock => mock.Dispose());
      _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock1, CommandBehavior.SingleResult)).Return (_dataReaderMock);
      _objectIDReaderStub.Stub (stub => stub.ReadSequence (_dataReaderMock)).Return (_fakeResult);
      _repository.ReplayAll();
      
      var command = new MultiObjectIDLoadCommand (new[] { _dbCommandBuilder1Mock }, _objectIDReaderStub);

      var result = command.Execute(_commandExecutionContextStub).ToArray();

      _repository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _objectID1 }));
    }

    [Test]
    public void Execute_SeveralDbCommandBuilders ()
    {
      _dbCommandBuilder1Mock.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock1);
      _dbCommandBuilder2Mock.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock2);
      _dbCommandMock1.Expect (mock => mock.Dispose ());
      _dbCommandMock2.Expect (mock => mock.Dispose ());
      _dataReaderMock.Expect (mock => mock.Dispose ()).Repeat.Twice();
      _repository.ReplayAll();

      _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock1, CommandBehavior.SingleResult)).Return (_dataReaderMock);
      _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock2, CommandBehavior.SingleResult)).Return (_dataReaderMock);
      _objectIDReaderStub.Stub (stub => stub.ReadSequence (_dataReaderMock)).Return (_fakeResult);

      var command = new MultiObjectIDLoadCommand (
          new[] { _dbCommandBuilder1Mock, _dbCommandBuilder2Mock }, _objectIDReaderStub);

      var result = command.Execute(_commandExecutionContextStub).ToArray();

      _repository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _objectID1, _objectID1 }));
    }

    [Test]
    public void LoadObjectIDsFromCommandBuilder ()
    {
      var enumerableStub = _repository.Stub<IEnumerable<ObjectID>> ();
      var enumeratorMock = _repository.StrictMock<IEnumerator<ObjectID>> ();

      using (_repository.Ordered ())
      {
        _dbCommandBuilder1Mock.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock1);
        _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock1, CommandBehavior.SingleResult)).Return (_dataReaderMock);
        _objectIDReaderStub.Stub (stub => stub.ReadSequence (_dataReaderMock)).Return (enumerableStub);
        enumerableStub.Stub (stub => stub.GetEnumerator ()).Return (enumeratorMock);
        enumeratorMock.Expect (mock => mock.MoveNext ()).Return (false);
        enumeratorMock.Expect (mock => mock.Dispose ());
        _dataReaderMock.Expect (mock => mock.Dispose ());
        _dbCommandMock1.Expect (mock => mock.Dispose ());
      }

      _repository.ReplayAll ();

      var command = new MultiObjectIDLoadCommand (new[] { _dbCommandBuilder1Mock, _dbCommandBuilder2Mock }, _objectIDReaderStub);

      var result =
          (IEnumerable<ObjectID>)
          PrivateInvoke.InvokeNonPublicMethod (command, "LoadObjectIDsFromCommandBuilder", _dbCommandBuilder1Mock, _commandExecutionContextStub);
      result.ToArray ();

      _repository.VerifyAll ();
    }
  }
}