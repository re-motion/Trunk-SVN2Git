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
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class MultiDataContainerSaveCommandTest : StandardMappingTest
  {
    private ObjectID _objectID1;
    private ObjectID _objectID2;
    private IDbCommandBuilder _dbCommandBuilder1Mock;
    private IDbCommandBuilder _dbCommandBuilder2Mock;
    private IDbCommand _dbCommand1Mock;
    private IDbCommand _dbCommand2Mock;
    private IRdbmsProviderCommandExecutionContext _rdbmsExecutionContextStub;
    private Tuple<ObjectID, IDbCommandBuilder> _tuple1;
    private Tuple<ObjectID, IDbCommandBuilder> _tuple2;

    public override void SetUp ()
    {
      base.SetUp ();

      _objectID1 = DomainObjectIDs.Order1;
      _objectID2 = DomainObjectIDs.Order2;

      _dbCommandBuilder1Mock = MockRepository.GenerateStrictMock<IDbCommandBuilder>();
      _dbCommandBuilder2Mock = MockRepository.GenerateStrictMock<IDbCommandBuilder> ();

      _dbCommand1Mock = MockRepository.GenerateStrictMock<IDbCommand> ();
      _dbCommand2Mock = MockRepository.GenerateStrictMock<IDbCommand> ();

      _tuple1 = Tuple.Create (_objectID1, _dbCommandBuilder1Mock);
      _tuple2 = Tuple.Create (_objectID2, _dbCommandBuilder2Mock);

      _rdbmsExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();
    }

    [Test]
    public void Execute_NullCommand ()
    {
      var command = new MultiDataContainerSaveCommand (new[] { _tuple1 });

      _dbCommandBuilder1Mock.Expect (mock => mock.Create (_rdbmsExecutionContextStub)).Return (null);
      _dbCommandBuilder1Mock.Replay();
      _dbCommand1Mock.Replay();
      
      command.Execute (_rdbmsExecutionContextStub);

      _dbCommandBuilder1Mock.VerifyAllExpectations();
      _dbCommand1Mock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException), ExpectedMessage = 
      "Concurrency violation encountered. Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has already been changed by someone else.")]
    public void Execute_NoAffectedRecords ()
    {
      var command = new MultiDataContainerSaveCommand (new[] { _tuple1 });

      _dbCommandBuilder1Mock.Expect (mock => mock.Create (_rdbmsExecutionContextStub)).Return (_dbCommand1Mock);
      _dbCommandBuilder1Mock.Replay ();
      _dbCommand1Mock.Expect (mock => mock.ExecuteNonQuery()).Return (0);
      _dbCommand1Mock.Replay ();

      command.Execute (_rdbmsExecutionContextStub);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = 
      "Error while saving object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.")]
    public void Execute_ExceptionOccurs ()
    {
      var command = new MultiDataContainerSaveCommand (new[] { _tuple1 });

      _dbCommandBuilder1Mock.Expect (mock => mock.Create (_rdbmsExecutionContextStub)).Return (_dbCommand1Mock);
      _dbCommandBuilder1Mock.Replay ();
      _dbCommand1Mock.Expect (mock => mock.ExecuteNonQuery()).WhenCalled (mi => { throw new InvalidOperationException (""); });
      _dbCommand1Mock.Replay ();

      command.Execute (_rdbmsExecutionContextStub);
    }

    [Test]
    public void Execute_OneTuple ()
    {
      var command = new MultiDataContainerSaveCommand (new[] { _tuple1 });

      _dbCommandBuilder1Mock.Expect (mock => mock.Create (_rdbmsExecutionContextStub)).Return (_dbCommand1Mock);
      _dbCommandBuilder1Mock.Replay ();
      _dbCommand1Mock.Expect (mock => mock.ExecuteNonQuery()).Return (1);
      _dbCommand1Mock.Replay ();

      command.Execute (_rdbmsExecutionContextStub);

      _dbCommandBuilder1Mock.Replay();
      _dbCommand1Mock.Replay();
    }

    [Test]
    public void Execute_SeveralTuples ()
    {
      var command = new MultiDataContainerSaveCommand (new[] { _tuple1, _tuple2 });

      _dbCommandBuilder1Mock.Expect (mock => mock.Create (_rdbmsExecutionContextStub)).Return (_dbCommand1Mock);
      _dbCommandBuilder2Mock.Expect (mock => mock.Create (_rdbmsExecutionContextStub)).Return (_dbCommand2Mock);
      _dbCommandBuilder1Mock.Replay ();
      _dbCommand1Mock.Expect (mock => mock.ExecuteNonQuery ()).Return (1);
      _dbCommand2Mock.Expect (mock => mock.ExecuteNonQuery ()).Return (1);
      _dbCommand1Mock.Replay ();

      command.Execute (_rdbmsExecutionContextStub);

      _dbCommandBuilder1Mock.Replay ();
      _dbCommandBuilder2Mock.Replay ();
      _dbCommand1Mock.Replay ();
      _dbCommand2Mock.Replay ();
    }
  }
}