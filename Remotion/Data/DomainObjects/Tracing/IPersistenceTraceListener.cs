using System;
using System.Collections.Generic;
using System.Data;

namespace Remotion.Data.DomainObjects.Tracing
{
  public interface IPersistenceTraceListener
  {
    void TraceConnectionOpened (Guid clientTransactionID, Guid connectionID);
    void TraceConnectionClosed (Guid clientTransactionID, Guid connectionID);
    void TraceTransactionBegan (Guid clientTransactionID, Guid connectionID, IsolationLevel isolationLevel);
    void TraceTransactionCommitted (Guid clientTransactionID, Guid connectionID);
    void TraceTransactionRolledback (Guid clientTransactionID, Guid connectionID);
    void TraceTransactionDisposed (Guid clientTransactionID, Guid connectionID);
    void TraceQueryExecuting (Guid clientTransactionID, Guid connectionID, Guid queryID, string commandText, IDictionary<string, object> parameters);
    void TraceQueryExecuted (Guid clientTransactionID, Guid connectionID, Guid queryID, TimeSpan durationOfQueryExecution);
    void TraceQueryCompleted (Guid clientTransactionID, Guid connectionID, Guid queryID, TimeSpan durationOfDataRead, int rowCount);
    void TraceQueryError (Guid clientTransactionID, Guid connectionID, Guid queryID, Exception e);
  }
}