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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class SingleDataContainerLoadCommandTest : SqlProviderBaseTest
  {
    private IDbCommandBuilder _dbCommandBuilderMock;
    private IDbCommand _dbCommandMock;
    private IDataReader _dataReaderMock;
    private SingleDataContainerLoadCommand _command;
    private IDataContainerReader _dataContainerReaderMock;
    private DataContainer _container;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextMock;

    public override void SetUp ()
    {
      base.SetUp();

      _container = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
     
      _dataReaderMock = MockRepository.GenerateStub<IDataReader>();
      _commandExecutionContextMock = MockRepository.GenerateStrictMock<IRdbmsProviderCommandExecutionContext>();
      _dbCommandMock = MockRepository.GenerateStrictMock<IDbCommand>();
      _dbCommandBuilderMock = MockRepository.GenerateStrictMock<IDbCommandBuilder>();
      _dataContainerReaderMock = MockRepository.GenerateStrictMock<IDataContainerReader> ();

      _command = new SingleDataContainerLoadCommand (_dbCommandBuilderMock, _dataContainerReaderMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.DbCommandBuilder, Is.SameAs (_dbCommandBuilderMock));
      Assert.That (_command.DataContainerReader, Is.SameAs(_dataContainerReaderMock));
    }

    [Test]
    public void Execute ()
    {
      _commandExecutionContextMock.Expect (mock => mock.ExecuteReader (_dbCommandMock, CommandBehavior.SingleRow)).Return (_dataReaderMock);
      _commandExecutionContextMock.Replay();

      _dataContainerReaderMock.Expect (mock => mock.Read (_dataReaderMock)).Return (_container);
      _dataContainerReaderMock.Replay();

      _dbCommandBuilderMock.Expect (mock => mock.Create (_commandExecutionContextMock)).Return (_dbCommandMock);
      _dbCommandBuilderMock.Replay();

      _dbCommandMock.Expect (mock => mock.Dispose());
      _dbCommandMock.Replay();
      
      var result = _command.Execute(_commandExecutionContextMock);

      _commandExecutionContextMock.VerifyAllExpectations();
      _dataContainerReaderMock.VerifyAllExpectations();
      _dbCommandBuilderMock.VerifyAllExpectations();
      _dbCommandMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_container));
    }

    [Test]
    public void Execute_NullContainer ()
    {
      _commandExecutionContextMock.Expect (mock => mock.ExecuteReader (_dbCommandMock, CommandBehavior.SingleRow)).Return (_dataReaderMock);
      _commandExecutionContextMock.Replay ();

      _dataContainerReaderMock.Expect (mock => mock.Read (_dataReaderMock)).Return (null);
      _dataContainerReaderMock.Replay ();

      _dbCommandBuilderMock.Expect (mock => mock.Create (_commandExecutionContextMock)).Return (_dbCommandMock);
      _dbCommandBuilderMock.Replay ();

      _dbCommandMock.Expect (mock => mock.Dispose ());
      _dbCommandMock.Replay ();

      var result = _command.Execute (_commandExecutionContextMock);

      _commandExecutionContextMock.VerifyAllExpectations ();
      _dataContainerReaderMock.VerifyAllExpectations ();
      _dbCommandBuilderMock.VerifyAllExpectations ();
      _dbCommandMock.VerifyAllExpectations ();
      Assert.That (result, Is.Null);
    }
    
  }
}