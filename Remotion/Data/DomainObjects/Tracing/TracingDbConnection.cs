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
  /// Provides a wrapper for implementations of <see cref="IDbConnection"/>. The lifetime of the connection is traced using the
  /// <see cref="IPersistenceTracer"/> passed during the instantiation.
  /// </summary>
  public class TracingDbConnection : IDbConnection
  {
    #region IDbConnection implementation

    public void ChangeDatabase (string databaseName)
    {
      _connection.ChangeDatabase (databaseName);
    }

    public string ConnectionString
    {
      get { return _connection.ConnectionString; }
      set { _connection.ConnectionString = value; }
    }

    public int ConnectionTimeout
    {
      get { return _connection.ConnectionTimeout; }
    }

    public string Database
    {
      get { return _connection.Database; }
    }

    public ConnectionState State
    {
      get { return _connection.State; }
    }

    IDbTransaction IDbConnection.BeginTransaction ()
    {
      return BeginTransaction();
    }

    IDbTransaction IDbConnection.BeginTransaction (IsolationLevel isolationLevel)
    {
      return BeginTransaction (isolationLevel);
    }

    IDbCommand IDbConnection.CreateCommand ()
    {
      return CreateCommand();
    }

    #endregion

    private readonly IDbConnection _connection;
    private readonly IPersistenceTracer _persistenceTracer;
    private readonly Guid _connectionID;
    private bool _isConnectionClosed;

    public TracingDbConnection (IDbConnection connection, IPersistenceTracer persistenceTracer)
    {
      ArgumentUtility.CheckNotNull ("connection", connection);
      ArgumentUtility.CheckNotNull ("persistenceTracer", persistenceTracer);

      _connection = connection;
      _persistenceTracer = persistenceTracer;
      _connectionID = Guid.NewGuid();
    }

    public IDbConnection WrappedInstance
    {
      get { return _connection; }
    }

    public Guid ConnectionID
    {
      get { return _connectionID; }
    }

    public IPersistenceTracer PersistenceTracer
    {
      get { return _persistenceTracer; }
    }

    public void Open ()
    {
      _connection.Open();
      PersistenceTracer.TraceConnectionOpened (_connectionID);
    }

    public void Close ()
    {
      _connection.Close();

      TraceConnectionClosed();
    }

    public void Dispose ()
    {
      _connection.Dispose();

      TraceConnectionClosed();
    }

    public TracingDbTransaction BeginTransaction ()
    {
      var transaction = _connection.BeginTransaction();
      PersistenceTracer.TraceTransactionBegan (_connectionID, transaction.IsolationLevel);
      return CreateTracingTransaction (transaction);
    }

    public TracingDbTransaction BeginTransaction (IsolationLevel isolationLevel)
    {
      var transaction = _connection.BeginTransaction (isolationLevel);
      PersistenceTracer.TraceTransactionBegan (_connectionID, isolationLevel);
      return CreateTracingTransaction (transaction);
    }

    public TracingDbCommand CreateCommand ()
    {
      return CreateTracingCommand (_connection.CreateCommand());
    }

    private TracingDbTransaction CreateTracingTransaction (IDbTransaction transaction)
    {
      return new TracingDbTransaction (transaction, _persistenceTracer, _connectionID);
    }

    private TracingDbCommand CreateTracingCommand (IDbCommand command)
    {
      return new TracingDbCommand (command, _persistenceTracer, _connectionID);
    }

    private void TraceConnectionClosed ()
    {
      if (!_isConnectionClosed)
      {
        PersistenceTracer.TraceConnectionClosed (_connectionID);
        _isConnectionClosed = true;
      }
    }
  }
}