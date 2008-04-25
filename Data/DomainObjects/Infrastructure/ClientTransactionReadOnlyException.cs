using System;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Thrown when a client transaction's state is tried to be modified and the ClientTransaction's internal state is set to read-only,
  /// usually because there is an active nested transaction.
  /// </summary>
  public class ClientTransactionReadOnlyException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientTransactionReadOnlyException"/> class, specifying an exception message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ClientTransactionReadOnlyException (string message)
        : base (message)
    {
    }
  }
}