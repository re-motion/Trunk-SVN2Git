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
using System.Collections;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// <see cref="ClientTransactionWrapper"/> provides a wrapper for ClientTransactions that implements the <see cref="ITransaction"/> interface.
  /// </summary>
  public class ClientTransactionWrapper : ITransaction
  {
    private readonly ClientTransaction _wrappedInstance;

    public ClientTransactionWrapper (ClientTransaction wrappedInstance)
    {
      ArgumentUtility.CheckNotNull ("wrappedInstance", wrappedInstance);
      _wrappedInstance = wrappedInstance;
    }

    public ClientTransaction WrappedInstance
    {
      get { return _wrappedInstance; }
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
      return _wrappedInstance.CreateSubTransaction();
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
      get { return _wrappedInstance.ParentTransaction; }
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
      //all newly registered objects should be loaded
      throw new NotImplementedException();
    }

    public void Reset()
    {
      throw new NotImplementedException ();
      //        _transactionManager.ReleaseTransaction ();
      //Assertion.IsFalse (oldTransaction.HasUncommittedChanges, "WxeTransaction should have thrown if the transaction had been changed");

      //ScopeManager.EnlistSameObjects (oldTransaction, MyTransaction, copyEventHandlers);
      //  ScopeManager.CopyTransactionEventHandlers (oldTransaction, MyTransaction);
      // if called from Reset: collection event handlers should be copied, transaction event handlers should be copied
    }
  }
}