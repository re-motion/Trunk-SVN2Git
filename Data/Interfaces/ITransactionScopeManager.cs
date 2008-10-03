/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.Data
{
  /// <summary>
  /// The <see cref="ITransactionScopeManager{TTransaction,TScope}"/> interface defines methods required to manage transaction scopes within a
  /// user interface application such as a web application using the Execution Engine flow control infrastructure.
  /// </summary>
  /// <typeparam name="TTransaction"></typeparam>
  /// <typeparam name="TScope"></typeparam>
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

    /// <summary>
    /// Creates a new root transaction instance. This instance is not yet managed by a scope.
    /// </summary>
    /// <returns>A new root transaction.</returns>
    TTransaction CreateRootTransaction ();

    /// <summary>
    /// Tries to enlist the given object in the given transaction, returning false if the object is not of an enlistable type.
    /// </summary>
    /// <param name="transaction">The transaction to enlist the object in.</param>
    /// <param name="objectToBeEnlisted">The object to be enlisted.</param>
    /// <returns>True if the object could be enlisted, or false if it was not of an enlistable type. (Otherwise, an exception should be thrown.)</returns>
    bool TryEnlistObject (TTransaction transaction, object objectToBeEnlisted);

    /// <summary>
    /// Enlists the objects from <paramref name="sourceTransaction"/> in <paramref name="destinationTransaction"/>, copying collection event 
    /// handlers if necessary.
    /// </summary>
    /// <param name="sourceTransaction">The transaction whose enlisted objects should be copied.</param>
    /// <param name="destinationTransaction">The transaction to enlist the objects in.</param>
    /// <param name="copyCollectionEventHandlers">If true, any event handlers registered with collection properties of the objects being enlisted will
    /// be copied to the new transactions. Events are only copied for objects that are newly enlisted by this method.</param>
    void EnlistSameObjects (TTransaction sourceTransaction, TTransaction destinationTransaction, bool copyCollectionEventHandlers);

    /// <summary>
    /// Copies all event handlers registered with <paramref name="sourceTransaction"/> to <paramref name="destinationTransaction"/> in.
    /// </summary>
    /// <param name="sourceTransaction">The transaction whose event handlers should be copied.</param>
    /// <param name="destinationTransaction">The transaction to register the event handlers with.</param>
    void CopyTransactionEventHandlers (TTransaction sourceTransaction, TTransaction destinationTransaction);

    /// <summary>
    /// Ensures the given object has been loaded into the given transaction. The object must be enlisted in the transaction first.
    /// </summary>
    /// <param name="transaction">The transaction to load the object in.</param>
    /// <param name="objectEnlistedInTransaction">The object to be loaded.</param>
    /// <returns></returns>
    void EnsureEnlistedObjectIsLoaded (TTransaction transaction, object objectEnlistedInTransaction);
  }
}
