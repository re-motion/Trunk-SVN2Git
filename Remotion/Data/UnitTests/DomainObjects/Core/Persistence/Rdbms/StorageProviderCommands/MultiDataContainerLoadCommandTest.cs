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
    private IDataReader _dataReaderStub;
    private IDbCommand _dbCommandStub;
    private IDbCommandBuilder _dbCommandBuilder1Stub;
    private IDbCommandBuilder _dbCommandBuilder2Stub;
    private IDataContainerReader _dataContainerReaderStub;
    private DataContainer _container;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp();

      _container = Computer.GetObject (DomainObjectIDs.Computer1).InternalDataContainer;
      
      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();

      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext> ();
      
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      // TODO Review 4070: Use two commands (also in MultiObjectIDLoadCommandTest)
      _dbCommandBuilder1Stub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dbCommandBuilder1Stub.Stub (stub => stub.Create (_commandExecutionContextStub)).Return (_dbCommandStub);
      _dbCommandBuilder2Stub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dbCommandBuilder2Stub.Stub (stub => stub.Create (_commandExecutionContextStub)).Return (_dbCommandStub);
      
      _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandStub, CommandBehavior.SingleResult)).Return (_dataReaderStub);

      _dataContainerReaderStub = MockRepository.GenerateStub<IDataContainerReader>();
    }

    [Test]
    public void Initialization ()
    {
      var command = new MultiDataContainerLoadCommand (
          new[] { _dbCommandBuilder1Stub, _dbCommandBuilder2Stub }, true, _commandExecutionContextStub, _dataContainerReaderStub);

      Assert.That (command.AllowNulls, Is.True);
      Assert.That (command.DataContainerReader, Is.SameAs (_dataContainerReaderStub));
      Assert.That (command.DbCommandBuilders, Is.EqualTo(new[]{_dbCommandBuilder1Stub, _dbCommandBuilder2Stub}));
      Assert.That (command.CommandExecutionContext, Is.SameAs (_commandExecutionContextStub));
    }

    [Test]
    public void Execute_OneCommandBuilder_AllowNullsTrue ()
    {
      var command = new MultiDataContainerLoadCommand (
          new[] { _dbCommandBuilder1Stub }, true, _commandExecutionContextStub, _dataContainerReaderStub);

      _dataContainerReaderStub.Stub (stub => stub.ReadSequence (_dataReaderStub, true)).Return (new[] { _container });
      var result = command.Execute (_commandExecutionContextStub).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { _container }));
    }


    [Test]
    public void Execute_SeveralCommandBuilders_AllowNullsFalse ()
    {
      _dataContainerReaderStub.Stub (stub => stub.ReadSequence (_dataReaderStub, false)).Return (new[] { _container });

      var command = new MultiDataContainerLoadCommand (
          new[] { _dbCommandBuilder1Stub, _dbCommandBuilder2Stub }, false, _commandExecutionContextStub, _dataContainerReaderStub);

      var result = command.Execute(_commandExecutionContextStub).ToArray();

      Assert.That (result, Is.EqualTo (new[] { _container, _container }));
    }
  }
}