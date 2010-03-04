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
using Microsoft.Practices.ServiceLocation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Tracing
{
  /// <summary>
  /// The default implementation of the <see cref="IPersistenceListener"/>.
  /// </summary>
  public class PersistenceTracer : IPersistenceListener
  {
    public static readonly IPersistenceListener Null = new NullPersistenceListener();

    private readonly List<IPersistenceTraceListener> _listeners = new List<IPersistenceTraceListener>();
    private readonly Guid _clientTransactionID;

    public PersistenceTracer (Guid clientTransactionID)
    {
      _clientTransactionID = clientTransactionID;

      IServiceLocator serviceLocator = SafeServiceLocator.Current;
      _listeners.AddRange (serviceLocator.GetAllInstances<IPersistenceTraceListener>());
    }

    public void ConnectionOpened (Guid connectionID)
    {
      foreach (var listener in _listeners)
        listener.TraceConnectionOpened (_clientTransactionID, connectionID);
    }

    public void ConnectionClosed (Guid connectionID)
    {
      foreach (var listener in _listeners)
        listener.TraceConnectionClosed (_clientTransactionID, connectionID);
    }

    public void TransactionBegan (Guid connectionID, IsolationLevel isolationLevel)
    {
      foreach (var listener in _listeners)
        listener.TraceTransactionBegan (_clientTransactionID, connectionID, isolationLevel);
    }

    public void TransactionCommitted (Guid connectionID)
    {
      foreach (var listener in _listeners)
        listener.TraceTransactionCommitted (_clientTransactionID, connectionID);
    }

    public void TransactionRolledback (Guid connectionID)
    {
      foreach (var listener in _listeners)
        listener.TraceTransactionRolledback (_clientTransactionID, connectionID);
    }

    public void TransactionDisposed (Guid connectionID)
    {
      foreach (var listener in _listeners)
        listener.TraceTransactionDisposed (_clientTransactionID, connectionID);
    }

    public void QueryExecuting (Guid connectionID, Guid queryID, string commandText, IDictionary<string, object> parameters)
    {
      ArgumentUtility.CheckNotNull ("commandText", commandText);
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      foreach (var listener in _listeners)
        listener.TraceQueryExecuting (_clientTransactionID, connectionID, queryID, commandText, parameters);
    }

    public void QueryExecuted (Guid connectionID, Guid queryID, TimeSpan durationOfQueryExecution)
    {
      foreach (var listener in _listeners)
        listener.TraceQueryExecuted (_clientTransactionID, connectionID, queryID, durationOfQueryExecution);
    }

    public void QueryCompleted (Guid connectionID, Guid queryID, TimeSpan durationOfDataRead, int rowCount)
    {
      foreach (var listener in _listeners)
        listener.TraceQueryCompleted (_clientTransactionID, connectionID, queryID, durationOfDataRead, rowCount);
    }

    public void QueryError (Guid connectionID, Guid queryID, Exception e)
    {
      foreach (var listener in _listeners)
        listener.TraceQueryError (_clientTransactionID, connectionID, queryID, e);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}