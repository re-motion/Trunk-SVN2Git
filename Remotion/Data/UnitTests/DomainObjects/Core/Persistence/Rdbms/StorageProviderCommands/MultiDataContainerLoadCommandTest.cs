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
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class MultiDataContainerLoadCommandTest : SqlProviderBaseTest
  {
    private IDataReader _dataReaderMock;
    private IDbCommand _dbCommandMock1;
    private IDbCommand _dbCommandMock2;
    private IDbCommandBuilder _dbCommandBuilder1Mock;
    private IDbCommandBuilder _dbCommandBuilder2Mock;
    private IDataContainerReader _dataContainerReader1Stub;
    private DataContainer _container;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;
    private MockRepository _repository;
    private IDataContainerReader _dataContainerReader2Stub;

    public override void SetUp ()
    {
      base.SetUp();

      _repository = new MockRepository();

      _container = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      _dataReaderMock = _repository.StrictMock<IDataReader>();

      _commandExecutionContextStub = _repository.Stub<IRdbmsProviderCommandExecutionContext>();

      _dbCommandMock1 = _repository.StrictMock<IDbCommand>();
      _dbCommandMock2 = _repository.StrictMock<IDbCommand>();

      _dbCommandBuilder1Mock = _repository.StrictMock<IDbCommandBuilder>();
      _dbCommandBuilder2Mock = _repository.StrictMock<IDbCommandBuilder>();

      _dataContainerReader1Stub = _repository.Stub<IDataContainerReader>();
      _dataContainerReader2Stub = _repository.Stub<IDataContainerReader>();
    }

    [Test]
    public void Initialization ()
    {
      var command = new MultiDataContainerLoadCommand (
          new[] { Tuple.Create (_dbCommandBuilder1Mock, _dataContainerReader1Stub), Tuple.Create (_dbCommandBuilder2Mock, _dataContainerReader2Stub) },
          true);

      Assert.That (command.AllowNulls, Is.True);
      Assert.That (
          command.DbCommandBuilderTuples,
          Is.EqualTo (
              new[]
              { Tuple.Create (_dbCommandBuilder1Mock, _dataContainerReader1Stub), Tuple.Create (_dbCommandBuilder2Mock, _dataContainerReader2Stub) }));
    }

    [Test]
    public void Execute_OneCommandBuilder_AllowNullsTrue ()
    {
      _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock1, CommandBehavior.SingleResult)).Return (_dataReaderMock);
      _dbCommandBuilder1Mock.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock1);
      _dbCommandMock1.Expect (mock => mock.Dispose());
      _dataReaderMock.Expect (mock => mock.Dispose());
      _repository.ReplayAll();

      var command = new MultiDataContainerLoadCommand (new[] { Tuple.Create (_dbCommandBuilder1Mock, _dataContainerReader1Stub) }, true);

      _dataContainerReader1Stub.Stub (stub => stub.ReadSequence (_dataReaderMock)).Return (new[] { _container });
      var result = command.Execute (_commandExecutionContextStub).ToArray();

      Assert.That (result, Is.EqualTo (new[] { _container }));
      _repository.VerifyAll();
    }

    [Test]
    public void Execute_SeveralCommandBuilders_AllowNullsFalse ()
    {
      _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock1, CommandBehavior.SingleResult)).Return (_dataReaderMock);
      _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock2, CommandBehavior.SingleResult)).Return (_dataReaderMock);
      _dbCommandBuilder1Mock.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock1);
      _dbCommandBuilder2Mock.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock2);
      _dbCommandMock1.Expect (mock => mock.Dispose());
      _dbCommandMock2.Expect (mock => mock.Dispose());
      _dataReaderMock.Expect (mock => mock.Dispose()).Repeat.Twice();
      _repository.ReplayAll();

      _dataContainerReader1Stub.Stub (stub => stub.ReadSequence (_dataReaderMock)).Return (new[] { _container });
      _dataContainerReader2Stub.Stub (stub => stub.ReadSequence (_dataReaderMock)).Return (new[] { _container });

      var command = new MultiDataContainerLoadCommand (
          new[] { Tuple.Create (_dbCommandBuilder1Mock, _dataContainerReader1Stub), Tuple.Create (_dbCommandBuilder2Mock, _dataContainerReader2Stub) },
          false);

      var result = command.Execute (_commandExecutionContextStub).ToArray();

      _repository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _container, _container }));
    }

    [Test]
    public void LoadDataContainersFromCommandBuilder ()
    {
      var enumerableStub = _repository.Stub<IEnumerable<DataContainer>>();
      var enumeratorMock = _repository.StrictMock<IEnumerator<DataContainer>>();

      using (_repository.Ordered())
      {
        _dbCommandBuilder1Mock.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock1);
        _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock1, CommandBehavior.SingleResult)).Return (_dataReaderMock);
        _dataContainerReader1Stub.Stub (stub => stub.ReadSequence (_dataReaderMock)).Return (enumerableStub);
        enumerableStub.Stub (stub => stub.GetEnumerator()).Return (enumeratorMock);
        enumeratorMock.Expect (mock => mock.MoveNext()).Return (false);
        enumeratorMock.Expect (mock => mock.Dispose());
        _dataReaderMock.Expect (mock => mock.Dispose());
        _dbCommandMock1.Expect (mock => mock.Dispose());
      }

      _repository.ReplayAll();

      var command = new MultiDataContainerLoadCommand (
          new[] { Tuple.Create (_dbCommandBuilder1Mock, _dataContainerReader1Stub), Tuple.Create (_dbCommandBuilder2Mock, _dataContainerReader2Stub) },
          false);

      var result =
          (IEnumerable<DataContainer>)
          PrivateInvoke.InvokeNonPublicMethod (
              command,
              "LoadDataContainersFromCommandBuilder",
              Tuple.Create (_dbCommandBuilder1Mock, _dataContainerReader1Stub),
              _commandExecutionContextStub);
      result.ToArray();

      _repository.VerifyAll();
    }
  }
}