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
using System.Text;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UberProfIntegration
{
  /// <summary>
  /// Implements <see cref="IPersistenceListener"/> for <b><a href="http://l2sprof.com/">Linq to Sql Profiler</a></b>. (Tested for build 661)
  /// <seealso cref="LinqToSqlAppender"/>
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class LinqToSqlPersistenceListener : IPersistenceListener
  {
    private readonly LinqToSqlAppender _appender;
    private readonly Guid _clientTransactionID;

    public LinqToSqlPersistenceListener (Guid clientTransactionID)
    {
      _clientTransactionID = clientTransactionID;
      _appender = LinqToSqlAppender.Instance;
    }

    public void ConnectionOpened (Guid connectionID)
    {
      _appender.ConnectionStarted (_clientTransactionID);
    }

    public void ConnectionClosed (Guid connectionID)
    {
      _appender.ConnectionDisposed (_clientTransactionID);
    }

    public void QueryCompleted (Guid connectionID, Guid queryID, TimeSpan durationOfDataRead, int rowCount)
    {
      _appender.StatementRowCount (_clientTransactionID, queryID, rowCount);
    }

    public void QueryError (Guid connectionID, Guid queryID, Exception e)
    {
      _appender.StatementError (_clientTransactionID, e);
    }

    public void QueryExecuted (Guid connectionID, Guid queryID, TimeSpan durationOfQueryExecution)
    {
      _appender.CommandDurationAndRowCount (_clientTransactionID, durationOfQueryExecution.Milliseconds, null);
    }

    public void QueryExecuting (
        Guid connectionID, Guid queryID, string commandText, IDictionary<string, object> parameters)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("commandText", commandText);
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      _appender.StatementExecuted (_clientTransactionID, queryID, AppendParametersToCommandText (commandText, parameters));
    }

    public void TransactionBegan (Guid connectionID, IsolationLevel isolationLevel)
    {
      _appender.TransactionBegan (_clientTransactionID, isolationLevel);
    }

    public void TransactionCommitted (Guid connectionID)
    {
      _appender.TransactionCommit (_clientTransactionID);
    }

    public void TransactionDisposed (Guid connectionID)
    {
      _appender.TransactionDisposed (_clientTransactionID);
    }

    public void TransactionRolledback (Guid connectionID)
    {
      _appender.TransactionRolledBack (_clientTransactionID);
    }

    private string AppendParametersToCommandText (string commandText, IDictionary<string, object> parameters)
    {
      StringBuilder builder = new StringBuilder (commandText).AppendLine().AppendLine ("-- Parameters:");
      foreach (string str in parameters.Keys)
      {
        string str2 = str;
        if (!str2.StartsWith ("@"))
          str2 = "@" + str2;
        builder.Append ("-- ").Append (str2).Append (" = [-[").Append (parameters[str]).AppendLine ("]-]");
      }
      return builder.ToString();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}