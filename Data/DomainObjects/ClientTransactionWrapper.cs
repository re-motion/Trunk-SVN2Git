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
using System.Linq;
using System.Collections;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// <see cref="ClientTransactionWrapper"/> provides a wrapper for ClientTransactions that implements the <see cref="ITransaction"/> interface.
  /// </summary>
  public class ClientTransactionWrapper : ITransaction
  {
    private ClientTransaction _wrappedInstance;

    internal ClientTransactionWrapper (ClientTransaction wrappedInstance)
    {
      ArgumentUtility.CheckNotNull ("wrappedInstance", wrappedInstance);
      _wrappedInstance = wrappedInstance;
    }

    public ClientTransaction WrappedInstance
    {
      get { return _wrappedInstance; }
    }

    /// <summary> Gets the native transaction.</summary>
    /// <typeparam name="TTransaction">The type of the transaction abstracted by this instance.</typeparam>
    public TTransaction To<TTransaction> ()
    {
      ArgumentUtility.CheckTypeIsAssignableFrom ("TTransaction", typeof (TTransaction), typeof (ClientTransaction));
      return (TTransaction) (object) _wrappedInstance;
    }

    /// <summary> Commits the transaction. </summary>
    public void Commit ()
    {
      _wrappedInstance.Commit();
    }

    /// <summary> Rolls the transaction back. </summary>
    public void Rollback ()
    {
      _wrappedInstance.Rollback();
    }

    /// <summary> 
    ///   Gets a flag that describes whether the transaction supports creating child transactions by invoking
    ///   <see cref="ITransaction.CreateChild"/>.
    /// </summary>
    public bool CanCreateChild
    {
      get { return true; }
    }

    /// <summary> Creates a new child transaction for the current transaction. </summary>
    /// <returns> 
    ///   A new instance of the of a type implementing <see cref="ITransaction"/> that has the creating transaction
    ///   as a parent.
    /// </returns>
    /// <exception cref="NotSupportedException"> 
    ///   Thrown if the method is invoked while <see cref="ITransaction.CanCreateChild"/> is <see langword="false"/>.
    /// </exception>
    public ITransaction CreateChild ()
    {
      return new ClientTransactionWrapper (_wrappedInstance.CreateSubTransaction());
    }

    /// <summary> Allows the transaction to implement clean up logic. </summary>
    /// <remarks> This method is called when the transaction is no longer needed. </remarks>
    public void Release ()
    {
      _wrappedInstance.Discard();
    }

    /// <summary> Gets the transaction's parent transaction. </summary>
    /// <value> 
    ///   An instance of the of a type implementing <see cref="ITransaction"/> or <see langword="null"/> if the
    ///   transaction is a root transaction.
    /// </value>
    public ITransaction Parent
    {
      get { return new ClientTransactionWrapper (_wrappedInstance.ParentTransaction); }
    }

    /// <summary>Gets a flag describing whether the transaction is a child transaction.</summary>
    /// <value> <see langword="true"/> if the transaction is a child transaction. </value>
    public bool IsChild
    {
      get { return _wrappedInstance.ParentTransaction != null; }
    }

    /// <summary>Gets a flag describing whether the transaction has been changed since the last commit or rollback.</summary>
    /// <value> <see langword="true"/> if the transaction has uncommitted changes. </value>
    public bool HasUncommittedChanges
    {
      get { return _wrappedInstance.HasChanged(); }
    }

    /// <summary>Gets a flag describing whether the transaction is in a read-only state.</summary>
    /// <value> <see langword="true"/> if the transaction cannot be modified. </value>
    /// <remarks>Implementations that do not support read-only transactions should always return false.</remarks>
    public bool IsReadOnly
    {
      get { return _wrappedInstance.IsReadOnly; }
    }

    /// <summary>
    /// Enters a new scope for the given transaction, making it the active transaction while the scope exists.
    /// </summary>
    /// <returns>The scope keeping the transaction active.</returns>
    /// <remarks>The scope must not discard the transaction when it is left.</remarks>
    public ITransactionScope EnterScope ()
    {
      return _wrappedInstance.EnterNonDiscardingScope();
    }

    /// <summary>Registers the <paramref name="objects"/> with the transaction.</summary>
    /// <param name="objects">The objects to be registered. Must not be <see langword="null" />.</param>
    /// <remarks>If the type of of of the objects is not supported by the transaction, the object must be ignored.</remarks>
    public void RegisterObjects (IEnumerable objects)
    {
      ArgumentUtility.CheckNotNull ("objects", objects);

      var domainObjects = objects.OfType<DomainObject>();
      _wrappedInstance.EnlistDomainObjects (domainObjects);
      _wrappedInstance.GetObjects<DomainObject> (domainObjects.Select (domainObject => domainObject.ID).ToArray());
    }

    public void Reset()
    {
      if (_wrappedInstance.IsReadOnly)
      {
        throw new InvalidOperationException (
            "The transaction cannot be reset as it is read-only. The reason might be an open child transaction.");
      }

      if (_wrappedInstance.HasChanged ())
      {
        throw new InvalidOperationException (
            "The transaction cannot be reset as it is in a dirty state and needs to be committed or rolled back.");
      }

      _wrappedInstance.Discard ();

      ClientTransaction newTransaction = _wrappedInstance.CreateEmptyTransactionOfSameType();
      newTransaction.EnlistSameDomainObjects (_wrappedInstance, true);
      newTransaction.CopyTransactionEventHandlers (_wrappedInstance);

      _wrappedInstance = newTransaction;
    }
  }
}