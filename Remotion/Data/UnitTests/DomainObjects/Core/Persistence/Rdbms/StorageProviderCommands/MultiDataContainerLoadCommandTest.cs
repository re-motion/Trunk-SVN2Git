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
using System.Linq;

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
    private IDataContainerReader _dataContainerReaderStub;
    private DataContainer _container;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp();

      _container = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      _dataReaderMock = MockRepository.GenerateStrictMock<IDataReader>();

      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext> ();
      
      _dbCommandMock1 = MockRepository.GenerateStrictMock<IDbCommand>();
      _dbCommandMock2 = MockRepository.GenerateStrictMock<IDbCommand> ();
      
      _dbCommandBuilder1Mock = MockRepository.GenerateStrictMock<IDbCommandBuilder>();
      _dbCommandBuilder2Mock = MockRepository.GenerateStrictMock<IDbCommandBuilder>();
      
      _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock1, CommandBehavior.SingleResult)).Return (_dataReaderMock);
      _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock2, CommandBehavior.SingleResult)).Return (_dataReaderMock);
      
      _dataContainerReaderStub = MockRepository.GenerateStub<IDataContainerReader>();
    }

    [Test]
    public void Initialization ()
    {
      var command = new MultiDataContainerLoadCommand (
          new[] { _dbCommandBuilder1Mock, _dbCommandBuilder2Mock }, true, _dataContainerReaderStub);

      Assert.That (command.AllowNulls, Is.True);
      Assert.That (command.DataContainerReader, Is.SameAs (_dataContainerReaderStub));
      Assert.That (command.DbCommandBuilders, Is.EqualTo(new[]{_dbCommandBuilder1Mock, _dbCommandBuilder2Mock}));
    }

    [Test]
    public void Execute_OneCommandBuilder_AllowNullsTrue ()
    {
      _dbCommandBuilder1Mock.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock1);
      _dbCommandBuilder1Mock.Replay();
      
      _dbCommandMock1.Expect (mock => mock.Dispose());
      _dbCommandMock1.Replay();

      _dataReaderMock.Expect (mock => mock.Dispose ());
      _dataReaderMock.Replay ();

      var command = new MultiDataContainerLoadCommand (
          new[] { _dbCommandBuilder1Mock }, true, _dataContainerReaderStub);

      _dataContainerReaderStub.Stub (stub => stub.ReadSequence (_dataReaderMock, true)).Return (new[] { _container });
      var result = command.Execute (_commandExecutionContextStub).ToArray ();

      _dbCommandBuilder1Mock.VerifyAllExpectations ();
      _dbCommandBuilder2Mock.VerifyAllExpectations ();
      _dbCommandMock1.VerifyAllExpectations ();
      _dbCommandMock2.VerifyAllExpectations ();
      _dataReaderMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (new[] { _container }));
    }
    
    [Test]
    public void Execute_SeveralCommandBuilders_AllowNullsFalse ()
    {
      _dbCommandBuilder1Mock.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock1);
      _dbCommandBuilder1Mock.Replay ();
      _dbCommandBuilder2Mock.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock2);
      _dbCommandBuilder2Mock.Replay ();

      _dbCommandMock1.Expect (mock => mock.Dispose ());
      _dbCommandMock1.Replay ();
      _dbCommandMock2.Expect (mock => mock.Dispose ());
      _dbCommandMock2.Replay ();

      _dataReaderMock.Expect (mock => mock.Dispose ()).Repeat.Twice();
      _dataReaderMock.Replay ();

      _dataContainerReaderStub.Stub (stub => stub.ReadSequence (_dataReaderMock, false)).Return (new[] { _container });

      var command = new MultiDataContainerLoadCommand (
          new[] { _dbCommandBuilder1Mock, _dbCommandBuilder2Mock }, false, _dataContainerReaderStub);

      var result = command.Execute(_commandExecutionContextStub).ToArray();

      _dbCommandBuilder1Mock.VerifyAllExpectations ();
      _dbCommandBuilder2Mock.VerifyAllExpectations ();
      _dbCommandMock1.VerifyAllExpectations ();
      _dbCommandMock2.VerifyAllExpectations ();
      _dataReaderMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (new[] { _container, _container }));
    }
  }
}