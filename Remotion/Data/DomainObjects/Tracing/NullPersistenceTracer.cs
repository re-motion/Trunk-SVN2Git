using System;
using System.Collections.Generic;
using System.Data;

namespace Remotion.Data.DomainObjects.Tracing
{
  /// <summary>
  /// <see cref="INullObject"/> implementation of <see cref="IPersistenceTracer"/>.
  /// Use <see cref="PersistenceTracer.Null"/> to get an instance of the <see cref="NullPersistenceTracer"/>.
  /// </summary>
  public sealed class NullPersistenceTracer : IPersistenceTracer
  {
    internal NullPersistenceTracer ()
    {      
    }

    public bool IsNull
    {
      get { return true; }
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

    public void TraceQueryExecuting (Guid connectionID, Guid queryID, string commandText, IDictionary<string, object> parameters)
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
}