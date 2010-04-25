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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Tracing
{
  /// <summary>
  /// Provides a wrapper for implementations of <see cref="IDbTransaction"/>. The lifetime of the transaction is traced using the
  /// <see cref="IPersistenceListener"/> passed during the instantiation.
  /// </summary>
  public class TracingDbTransaction : IDbTransaction
  {
    private readonly IDbTransaction _transaction;
    private readonly IPersistenceListener _persistenceListener;
    private readonly Guid _connectionID;
    private readonly Guid _transactionID;
    private bool _isTransactionDisposed;

    public TracingDbTransaction (IDbTransaction transaction, IPersistenceListener persistenceListener, Guid connectionID)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("persistenceListener", persistenceListener);
      _transaction = transaction;
      _persistenceListener = persistenceListener;
      _connectionID = connectionID;
      _transactionID = Guid.NewGuid();
    }

    public IDbTransaction WrappedInstance
    {
      get { return _transaction; }
    }

    public Guid ConnectionID
    {
      get { return _connectionID; }
    }

    public Guid TransactionID
    {
      get { return _transactionID; }
    }

    public IPersistenceListener PersistenceListener
    {
      get { return _persistenceListener; }
    }

    public void Dispose ()
    {
      _transaction.Dispose();

      if (!_isTransactionDisposed)
      {
        PersistenceListener.TransactionDisposed (_connectionID);
        _isTransactionDisposed = true;
      }
    }

    public void Commit ()
    {
      _transaction.Commit();
      if (!_isTransactionDisposed)
        PersistenceListener.TransactionCommitted (_connectionID);
    }

    public void Rollback ()
    {
      _transaction.Rollback();
      if (!_isTransactionDisposed)
        PersistenceListener.TransactionRolledBack (_connectionID);
    }

    IDbConnection IDbTransaction.Connection
    {
      get { return _transaction.Connection; }
    }

    public IsolationLevel IsolationLevel
    {
      get { return _transaction.IsolationLevel; }
    }
  }
}