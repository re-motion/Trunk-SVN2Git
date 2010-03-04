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

namespace Remotion.Data.DomainObjects.Tracing
{
  /// <summary>
  /// The <see cref="IPersistenceTracer"/> defines the API for tracing database access.
  /// </summary>
  public interface IPersistenceTracer : INullObject
  {
    /// <summary>
    /// Invoking this method signals that a database connection has been established.
    /// </summary>
    /// <param name="connectionID">An ID unique to all operations traced for this database connection.</param>
    void TraceConnectionOpened (Guid connectionID);

    /// <summary>
    /// Invoking this method signals that a database connection has been closed.
    /// </summary>
    /// <param name="connectionID">An ID unique to all operations traced for this database connection.</param>
    void TraceConnectionClosed (Guid connectionID);

    /// <summary>
    /// Invoking this method signals the begin of a database transaction.
    /// </summary>
    /// <param name="connectionID">An ID unique to all operations traced for this database connection.</param>
    /// <param name="isolationLevel">The <see cref="IsolationLevel"/> used for this transaction</param>
    void TraceTransactionBegan (Guid connectionID, IsolationLevel isolationLevel);

    /// <summary>
    /// Invoking this method signals that a database transaction was committed.
    /// </summary>
    /// <param name="connectionID">An ID unique to all operations traced for this database connection.</param>
    void TraceTransactionCommitted (Guid connectionID);

    /// <summary>
    /// Invoking this method signals that a database transaction was rolled-back.
    /// </summary>
    /// <param name="connectionID">An ID unique to all operations traced for this database connection.</param>
    void TraceTransactionRolledback (Guid connectionID);

    /// <summary>
    /// Invoking this method signals that a database transaction was disposed.
    /// </summary>
    /// <param name="connectionID">An ID unique to all operations traced for this database connection.</param>
    void TraceTransactionDisposed (Guid connectionID);

    /// <summary>
    /// Invoking this method signals the begin of a database query.
    /// </summary>
    /// <param name="connectionID">An ID unique to all operations traced for this database connection.</param>
    /// <param name="queryID">An ID unique to this specific database query.</param>
    /// <param name="commandText">The command-text of the query.</param>
    /// <param name="parameters">The parameters of the query.</param>
    void TraceQueryExecuting (Guid connectionID, Guid queryID, string commandText, IDictionary<string, object> parameters);

    /// <summary>
    /// Invoking this method signals the return of the executing database query.
    /// </summary>
    /// <param name="connectionID">An ID unique to all operations traced for this database connection.</param>
    /// <param name="queryID">An ID unique to this specific database query.</param>
    /// <param name="durationOfQueryExecution">The time taken for exeucting the query against the database.</param>
    void TraceQueryExecuted (Guid connectionID, Guid queryID, TimeSpan durationOfQueryExecution);

    /// <summary>
    /// Invoking this method signals that the application has finished processing the query result.
    /// </summary>
    /// <param name="connectionID">An ID unique to all operations traced for this database connection.</param>
    /// <param name="queryID">An ID unique to this specific database query.</param>
    /// <param name="durationOfDataRead">The time taken for processing the query result.</param>
    /// <param name="rowCount">The number of rows processed.</param>
    void TraceQueryCompleted (Guid connectionID, Guid queryID, TimeSpan durationOfDataRead, int rowCount);

    /// <summary>
    /// Invoking this method signals that the executing query has generated an error.
    /// </summary>
    /// <param name="connectionID">An ID unique to all operations traced for this database connection.</param>
    /// <param name="queryID">An ID unique to this specific database query.</param>
    /// <param name="e">The exection thrown becasue of the error.</param>
    void TraceQueryError (Guid connectionID, Guid queryID, Exception e);
  }
}