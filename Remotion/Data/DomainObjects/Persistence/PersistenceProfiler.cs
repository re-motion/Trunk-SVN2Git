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
using System.Diagnostics;
using HibernatingRhinos.Profiler.Appender.LinqToSql;

namespace Remotion.Data.DomainObjects.Persistence
{
  public class PersistenceProfiler: IPersistenceProfiler
  {
    public static readonly IPersistenceProfiler Null = new NullPersistenceProfiler();
    private static LinqToSqlAppender s_linqToSqlAppender;

    public static LinqToSqlAppender LinqToSqlAppender
    {
      get
      {
        if (s_linqToSqlAppender == null)
        {
          HibernatingRhinos.Profiler.Appender.LinqToSql.LinqToSqlProfiler.Initialize ();
          s_linqToSqlAppender = LinqToSqlProfiler.GetAppender ("re-store UberProf PoC - RestoreProfiler");
        }
        return s_linqToSqlAppender;
      }
    }


    private readonly Guid _clientTransactionID;
    private Stopwatch _stopwatch;

    public PersistenceProfiler (Guid clientTransactionID)
    {
      _clientTransactionID = clientTransactionID;
    }

    public void ConnectionOpened (Guid connectionID)
    {
      LinqToSqlAppender.ConnectionStarted (_clientTransactionID);
    }

    public void ConnectionClosed (Guid connectionID)
    {
      LinqToSqlAppender.ConnectionDisposed (_clientTransactionID);
    }

    public void TransactionBegan (Guid connectionID, IsolationLevel isolationLevel)
    {
      LinqToSqlAppender.TransactionBegan (connectionID, isolationLevel);
    }

    public void TransactionCommitted (Guid connectionID)
    {
      LinqToSqlAppender.TransactionCommit (connectionID);
    }

    public void TransactionRolledback (Guid connectionID)
    {
      LinqToSqlAppender.TransactionRolledBack (connectionID);
    }

    public void TransactionDisposed (Guid connectionID)
    {
      LinqToSqlAppender.TransactionDisposed (connectionID);
    }

    public void QueryExecuting (Guid connectionID, Guid queryID, string commandText)
    {
      LinqToSqlAppender.StatementExecuted (_clientTransactionID, queryID, commandText);
      _stopwatch = new Stopwatch();
      _stopwatch.Start ();
    }

    public void QueryExecuted (Guid connectionID, Guid queryID)
    {
      LinqToSqlAppender.CommandDurationAndRowCount (_clientTransactionID, _stopwatch.ElapsedMilliseconds, null);
    }

    public void QueryCompleted (Guid connectionID, Guid queryID, int rowCount)
    {
      _stopwatch.Stop();
      LinqToSqlAppender.StatementRowCount (connectionID, queryID, rowCount);
      _stopwatch = null;
    }

    public void QueryError (Guid connectionID, Guid queryID, Exception e)
    {
      _stopwatch.Stop ();
      _stopwatch = null;
      LinqToSqlAppender.StatementError (connectionID, e);
    }
  }

  public class NullPersistenceProfiler : IPersistenceProfiler
  {
    public NullPersistenceProfiler ()
    {
    }

    public void ConnectionOpened (Guid connectionID)
    {
    }

    public void ConnectionClosed (Guid connectionID)
    {
    }

    public void TransactionBegan (Guid connectionID, IsolationLevel isolationLevel)
    {
    }

    public void TransactionCommitted (Guid connectionID)
    {
    }

    public void TransactionRolledback (Guid connectionID)
    {
    }

    public void TransactionDisposed (Guid connectionID)
    {
    }

    public void QueryExecuting (Guid connectionID, Guid queryID, string commandText)
    {
    }

    public void QueryExecuted (Guid connectionID, Guid queryID)
    {
    }

    public void QueryCompleted (Guid connectionID, Guid queryID, int rowCount)
    {
    }

    public void QueryError (Guid connectionID, Guid queryID, Exception e)
    {
    }
  }

  public interface IPersistenceProfiler
  {
    void ConnectionOpened (Guid connectionID);
    void ConnectionClosed (Guid connectionID);
    void TransactionBegan (Guid connectionID, IsolationLevel isolationLevel);
    void TransactionCommitted (Guid connectionID);
    void TransactionRolledback (Guid connectionID);
    void TransactionDisposed (Guid connectionID);
    void QueryExecuting (Guid connectionID, Guid queryID, string commandText);
    void QueryExecuted (Guid connectionID, Guid queryID);
    void QueryCompleted (Guid connectionID, Guid queryID, int rowCount);
    void QueryError (Guid connectionID, Guid queryID, Exception e);
  }
}