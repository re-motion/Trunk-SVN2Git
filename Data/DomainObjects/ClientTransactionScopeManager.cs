using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Implements the <see cref="ITransactionScopeManager{TTransaction,TScope}"/> interface for <see cref="ClientTransaction"/> and
  /// <see cref="ClientTransactionScope"/>.
  /// </summary>
  public class ClientTransactionScopeManager : ITransactionScopeManager<ClientTransaction, ClientTransactionScope>
  {
    /// <summary>
    /// Gets the active transaction scope, or <see langword="null"/> if no active scope exists.
    /// </summary>
    /// <value>The active transaction scope.</value>
    public ClientTransactionScope ActiveScope
    {
      get { return ClientTransactionScope.ActiveScope; }
    }

    /// <summary>
    /// Enters a new scope for the given transaction, making it the active transaction while the scope exists.
    /// </summary>
    /// <param name="transaction">The transaction to be made active.</param>
    /// <returns>
    /// The scope keeping the transaction active.
    /// </returns>
    /// <remarks>The scope does not discard the transaction when it is left.</remarks>
    public ClientTransactionScope EnterScope (ClientTransaction transaction)
    {
      return transaction.EnterNonDiscardingScope ();
    }
  }
}