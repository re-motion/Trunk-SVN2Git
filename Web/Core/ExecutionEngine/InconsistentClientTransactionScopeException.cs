using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Thrown when an execution step of a <see cref="WxeScopedTransaction{TTransaction,TScope,TScopeManager}"/> leaves an incorrect transaction scope.
  /// </summary>
  public class InconsistentClientTransactionScopeException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="InconsistentClientTransactionScopeException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public InconsistentClientTransactionScopeException (string message)
      : base (message)
    {
    }
  }
}
