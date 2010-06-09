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

namespace Remotion.Data.UnitTests.DomainObjects.Core.Tracing
{
  [TestFixture]
  public class TracingDbTransactionTest
  {
    private MockRepository _mockRepository;
    private IDbTransaction _innerTransactionMock;
    private IPersistenceListener _listenerMock;
    private Guid _connectionID;
    private TracingDbTransaction _transaction;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
      _innerTransactionMock = _mockRepository.StrictMock<IDbTransaction>();
      _listenerMock = _mockRepository.StrictMock<IPersistenceListener> ();
      _connectionID = Guid.NewGuid ();

      _transaction = new TracingDbTransaction (_innerTransactionMock, _listenerMock, _connectionID);
    }

    [Test]
    public void Dispose ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerTransactionMock.Expect (mock => mock.Dispose());
        _listenerMock.Expect (mock => mock.TransactionDisposed (_connectionID));
      }
      _mockRepository.ReplayAll();

      _transaction.Dispose();
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Dispose_DisposedTransaction ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerTransactionMock.Expect (mock => mock.Dispose ());
        _listenerMock.Expect (mock => mock.TransactionDisposed (_connectionID));
        _innerTransactionMock.Expect (mock => mock.Dispose ());
      }
      _mockRepository.ReplayAll ();

      _transaction.Dispose ();
      _transaction.Dispose ();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Commit ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerTransactionMock.Expect (mock => mock.Commit ());
        _listenerMock.Expect (mock => mock.TransactionCommitted (_connectionID));
      }
      _mockRepository.ReplayAll();

      _transaction.Commit();
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Commit_DisposedTransaction ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerTransactionMock.Expect (mock => mock.Dispose());
        _listenerMock.Expect (mock => mock.TransactionDisposed (_connectionID));
        _innerTransactionMock.Expect (mock => mock.Commit ());
        
      }
      _mockRepository.ReplayAll ();

      _transaction.Dispose();
      _transaction.Commit ();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Rollback ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerTransactionMock.Expect (mock => mock.Rollback ());
        _listenerMock.Expect (mock => mock.TransactionRolledBack (_connectionID));
      }
      _mockRepository.ReplayAll ();

      _transaction.Rollback ();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Rollback_DisposedTransaction ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerTransactionMock.Expect (mock => mock.Dispose ());
        _listenerMock.Expect (mock => mock.TransactionDisposed (_connectionID));
        _innerTransactionMock.Expect (mock => mock.Rollback ());

      }
      _mockRepository.ReplayAll ();

      _transaction.Dispose ();
      _transaction.Rollback ();
      _mockRepository.VerifyAll ();
    }
  }
}