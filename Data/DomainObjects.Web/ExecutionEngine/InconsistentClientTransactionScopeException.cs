using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Data.DomainObjects.Web.ExecutionEngine
{
  /// <summary>
  /// Thrown when a <see cref="WxeTransaction"/> cannot perform correct <see cref="ClientTransaction"/> handling, because one of its execution
  /// steps left an incorrect <see cref="ClientTransactionScope"/> state.
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
