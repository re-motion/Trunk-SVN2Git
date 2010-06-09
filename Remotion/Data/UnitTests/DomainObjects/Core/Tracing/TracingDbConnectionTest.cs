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
using Remotion.Data.DomainObjects.Tracing;
using Rhino.Mocks;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Tracing
{
  [TestFixture]
  public class TracingDbConnectionTest
  {
    private MockRepository _mockRepository;
    private TracingDbConnection _connection;
    private IDbConnection _innerConnectionMock;
    private IPersistenceListener _listenerMock;
    
    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _innerConnectionMock = _mockRepository.StrictMock<IDbConnection>();
      _listenerMock = _mockRepository.StrictMock<IPersistenceListener> ();
      
      _connection = new TracingDbConnection (_innerConnectionMock, _listenerMock);
    }

    [Test]
    public void Dispose_ConnectionOpen ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerConnectionMock.Expect (mock => mock.Dispose());
        _listenerMock.Expect (mock => mock.ConnectionClosed (_connection.ConnectionID));
      }
      _mockRepository.ReplayAll();

      _connection.Dispose();
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Close ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerConnectionMock.Expect (mock => mock.Close ());
        _listenerMock.Expect (mock => mock.ConnectionClosed (_connection.ConnectionID));
      }
      _mockRepository.ReplayAll ();

      _connection.Close ();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void BeginTransaction ()
    {
      var isolationLevel = IsolationLevel.Chaos;
      var dbTransactionMock = _mockRepository.StrictMock<IDbTransaction> ();
      dbTransactionMock.Expect (mock => mock.IsolationLevel).Return (isolationLevel);

      _innerConnectionMock.Expect (mock => mock.BeginTransaction()).Return(dbTransactionMock);
      _listenerMock.Expect (mock => mock.TransactionBegan (_connection.ConnectionID, isolationLevel));
      
      _mockRepository.ReplayAll();
      
      var tracingDbTransaction =_connection.BeginTransaction();

      _mockRepository.VerifyAll();
      Assert.That (tracingDbTransaction.WrappedInstance, Is.EqualTo (dbTransactionMock));
      Assert.That (tracingDbTransaction.PersistenceListener, Is.EqualTo (_listenerMock));
      Assert.That (tracingDbTransaction.ConnectionID, Is.EqualTo (_connection.ConnectionID));
    }

    [Test]
    public void BeginTransaction_WithIsolationLevel ()
    {
      var isolationLevel = IsolationLevel.Chaos;

      var dbTransactionMock = _mockRepository.StrictMock<IDbTransaction> ();
      
      _innerConnectionMock.Expect (mock => mock.BeginTransaction (isolationLevel)).Return (dbTransactionMock);
      _listenerMock.Expect (mock => mock.TransactionBegan (_connection.ConnectionID, isolationLevel));

      _mockRepository.ReplayAll ();

      var tracingDbTransaction = _connection.BeginTransaction (isolationLevel);

      _mockRepository.VerifyAll ();
      Assert.That (tracingDbTransaction.WrappedInstance, Is.EqualTo (dbTransactionMock));
      Assert.That (tracingDbTransaction.PersistenceListener, Is.EqualTo (_listenerMock));
      Assert.That (tracingDbTransaction.ConnectionID, Is.EqualTo (_connection.ConnectionID));
    }

    [Test]
    public void CreateCommand ()
    {
      var commandMock = _mockRepository.StrictMock<IDbCommand>();
      _innerConnectionMock.Expect (mock => mock.CreateCommand()).Return (commandMock);

      _mockRepository.ReplayAll();

      var tracingDbCommand = _connection.CreateCommand();

      _mockRepository.VerifyAll();

      Assert.That (tracingDbCommand.WrappedInstance, Is.EqualTo (commandMock));
      Assert.That (tracingDbCommand.PersistenceListener, Is.EqualTo (_listenerMock));
      Assert.That (tracingDbCommand.ConnectionID, Is.EqualTo (_connection.ConnectionID));
    }
  }
}