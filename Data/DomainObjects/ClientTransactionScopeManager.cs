/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Implements the <see cref="ITransactionScopeManager{TTransaction,TScope}"/> interface for <see cref="ClientTransaction"/> and
  /// <see cref="ClientTransactionScope"/>.
  /// </summary>
  public class ClientTransactionScopeManager: ITransactionScopeManager<ClientTransaction, ClientTransactionScope>
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
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      return transaction.EnterNonDiscardingScope ();
    }

    /// <summary>
    /// Creates a new root transaction instance. This instance is not yet managed by a scope.
    /// </summary>
    /// <returns>A new root transaction.</returns>
    public ClientTransaction CreateRootTransaction ()
    {
      return ClientTransaction.CreateRootTransaction ();
    }

    /// <summary>
    /// Tries to enlist the given object in the given transaction, returning false if the object is not of type <see cref="DomainObject"/>.
    /// </summary>
    /// <param name="transaction">The transaction to enlist the object in.</param>
    /// <param name="objectToBeEnlisted">The object to be enlisted.</param>
    /// <returns>
    /// True if the object could be enlisted, or false if it was not of type <see cref="DomainObject"/>. (Otherwise, an exception is thrown.)
    /// </returns>
    public bool TryEnlistObject (ClientTransaction transaction, object objectToBeEnlisted)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("objectToBeEnlisted", objectToBeEnlisted);
      DomainObject domainObject = objectToBeEnlisted as DomainObject;
      if (domainObject != null)
      {
        transaction.EnlistDomainObject (domainObject);
        return true;
      }
      else 
        return false;
    }

    /// <summary>
    /// Enlists the objects from <paramref name="sourceTransaction"/> in <paramref name="destinationTransaction"/>, copying collection event
    /// handlers if necessary.
    /// </summary>
    /// <param name="sourceTransaction">The transaction whose enlisted objects should be copied.</param>
    /// <param name="destinationTransaction">The transaction to enlist the objects in.</param>
    /// <param name="copyCollectionEventHandlers">If true, any event handlers registered with collection properties of the objects being enlisted will
    /// be copied to the new transactions. Events are only copied for objects that are newly enlisted by this method.</param>
    public void EnlistSameObjects (ClientTransaction sourceTransaction, ClientTransaction destinationTransaction, bool copyCollectionEventHandlers)
    {
      ArgumentUtility.CheckNotNull ("sourceTransaction", sourceTransaction);
      ArgumentUtility.CheckNotNull ("destinationTransaction", destinationTransaction);
      destinationTransaction.EnlistSameDomainObjects (sourceTransaction, copyCollectionEventHandlers);
    }

    /// <summary>
    /// Copies all event handlers registered with <paramref name="sourceTransaction"/> to <paramref name="destinationTransaction"/> in.
    /// </summary>
    /// <param name="sourceTransaction">The transaction whose event handlers should be copied.</param>
    /// <param name="destinationTransaction">The transaction to register the event handlers with.</param>
    public void CopyTransactionEventHandlers (ClientTransaction sourceTransaction, ClientTransaction destinationTransaction)
    {
      ArgumentUtility.CheckNotNull ("sourceTransaction", sourceTransaction);
      ArgumentUtility.CheckNotNull ("destinationTransaction", destinationTransaction);
      destinationTransaction.CopyTransactionEventHandlers (sourceTransaction);
    }

    /// <summary>
    /// Ensures the given object has been loaded into the given transaction. The object must be enlisted in the transaction first.
    /// </summary>
    /// <param name="transaction">The transaction to load the object in.</param>
    /// <param name="objectEnlistedInTransaction">The object to be loaded.</param>
    public void EnsureEnlistedObjectIsLoaded (ClientTransaction transaction, object objectEnlistedInTransaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("objectEnlistedInTransaction", objectEnlistedInTransaction);

      DomainObject domainObject = ArgumentUtility.CheckType<DomainObject> ("objectEnlistedInTransaction", objectEnlistedInTransaction);
      if (!transaction.IsEnlisted (domainObject))
        throw new ArgumentException ("The given object is not enlisted in the given transaction.", "objectEnlistedInTransaction");

      DomainObject enlistedObject = transaction.GetObject (domainObject.ID, false);
      Assertion.IsTrue (domainObject == enlistedObject);
    }
  }
}
