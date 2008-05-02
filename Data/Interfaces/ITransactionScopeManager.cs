namespace Remotion.Data
{
  public interface ITransactionScopeManager<TTransaction, TScope>
    where TTransaction : class, ITransaction
    where TScope : class, ITransactionScope<TTransaction>
  {
    /// <summary>
    /// Gets the active transaction scope, or <see langword="null"/> if no active scope exists.
    /// </summary>
    /// <value>The active transaction scope.</value>
    TScope ActiveScope { get; }

    /// <summary>
    /// Enters a new scope for the given transaction, making it the active transaction while the scope exists.
    /// </summary>
    /// <param name="transaction">The transaction to be made active.</param>
    /// <returns>The scope keeping the transaction active.</returns>
    /// <remarks>The scope must not discard the transaction when it is left.</remarks>
    TScope EnterScope (TTransaction transaction);
  }
}