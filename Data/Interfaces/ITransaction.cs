using System;

namespace Remotion.Data
{
  /// <summary> The <see cref="ITransaction"/> interface provides functionality needed when working with a transaction.</summary>
  public interface ITransaction
  {
    /// <summary> Commits the transaction. </summary>
    void Commit ();

    /// <summary> Rolls the transaction back. </summary>
    void Rollback ();

    /// <summary> 
    ///   Gets a flag that describes whether the transaction supports creating child transactions by invoking
    ///   <see cref="CreateChild"/>.
    /// </summary>
    bool CanCreateChild { get; }

    /// <summary> Creates a new child transaction for the current transaction. </summary>
    /// <returns> 
    ///   A new instance of the of a type implementing <see cref="ITransaction"/> that has the creating transaction
    ///   as a parent.
    /// </returns>
    /// <exception cref="NotSupportedException"> 
    ///   Thrown if the method is invoked while <see cref="CanCreateChild"/> is <see langword="false"/>.
    /// </exception>
    ITransaction CreateChild ();

    /// <summary> Allows the transaction to implement clean up logic. </summary>
    /// <remarks> This method is called when the transaction is no longer needed. </remarks>
    void Release ();

    /// <summary> Gets the transaction's parent transaction. </summary>
    /// <value> 
    ///   An instance of the of a type implementing <see cref="ITransaction"/> or <see langword="null"/> if the
    ///   transaction is a root transaction.
    /// </value>
    ITransaction Parent { get; }

    /// <summary>Gets a flag describing whether the transaction is a child transaction.</summary>
    /// <value> <see langword="true"/> if the transaction is a child transaction. </value>
    bool IsChild { get; }

    /// <summary>Gets a flag describing whether the transaction has been changed since the last commit or rollback.</summary>
    /// <value> <see langword="true"/> if the transaction has uncommitted changes. </value>
    bool HasUncommittedChanges { get; }

    /// <summary>Gets a flag describing whether the transaction is in a read-only state.</summary>
    /// <value> <see langword="true"/> if the transaction cannot be modified. </value>
    /// <remarks>Implementations that do not support read-only transactions should always return false.</remarks>
    bool IsReadOnly { get; }
  }
}