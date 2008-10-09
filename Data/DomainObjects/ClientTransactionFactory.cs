using System;

namespace Remotion.Data.DomainObjects
{
  /// <summary>The <see cref="ClientTransactionFactory"/> is the default implementation of the <see cref="ITransactionFactory"/> interface.</summary>
  public class ClientTransactionFactory : ITransactionFactory
  {
    /// <summary>
    /// Creates a new root transaction instance. This instance is not yet managed by a scope.
    /// </summary>
    /// <returns>A new root transaction.</returns>
    public ITransaction CreateRootTransaction ()
    {
      return ClientTransaction.CreateRootTransaction ().ToITransation();
    }

  }
}