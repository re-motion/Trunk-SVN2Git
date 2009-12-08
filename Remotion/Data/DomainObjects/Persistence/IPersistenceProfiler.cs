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

namespace Remotion.Data.DomainObjects.Persistence
{
  public class NullPersistenceProfiler : IPersistenceProfiler
  {
    public NullPersistenceProfiler ()
    {
    }

    public void TraceConnectionOpened (Guid connectionID)
    {
    }

    public void TraceConnectionClosed (Guid connectionID)
    {
    }

    public void TraceTransactionBegan (Guid connectionID, IsolationLevel isolationLevel)
    {
    }

    public void TraceTransactionCommitted (Guid connectionID)
    {
    }

    public void TraceTransactionRolledback (Guid connectionID)
    {
    }

    public void TraceTransactionDisposed (Guid connectionID)
    {
    }

    public void TraceQueryExecuting (Guid connectionID, Guid queryID, string commandText)
    {
    }

    public void TraceQueryExecuted (Guid connectionID, Guid queryID, TimeSpan durationOfQueryExecution)
    {
    }

    public void TraceQueryCompleted (Guid connectionID, Guid queryID, TimeSpan durationOfDataRead, int rowCount)
    {
    }

    public void TraceQueryError (Guid connectionID, Guid queryID, Exception e)
    {
    }
  }

  public interface IPersistenceProfiler
  {
    void TraceConnectionOpened (Guid connectionID);
    void TraceConnectionClosed (Guid connectionID);
    void TraceTransactionBegan (Guid connectionID, IsolationLevel isolationLevel);
    void TraceTransactionCommitted (Guid connectionID);
    void TraceTransactionRolledback (Guid connectionID);
    void TraceTransactionDisposed (Guid connectionID);
    void TraceQueryExecuting (Guid connectionID, Guid queryID, string commandText);
    void TraceQueryExecuted (Guid connectionID, Guid queryID, TimeSpan durationOfQueryExecution);
    void TraceQueryCompleted (Guid connectionID, Guid queryID, TimeSpan durationOfDataRead, int rowCount);
    void TraceQueryError (Guid connectionID, Guid queryID, Exception e);
  }
}