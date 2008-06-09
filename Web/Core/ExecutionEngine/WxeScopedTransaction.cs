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
using System.Collections.Generic;
using Remotion.Data;
using Remotion.Web.ExecutionEngine;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Extends <see cref="WxeTransactionBase{TTransaction}"/> by automatic scope management for the <typeparamref name="TTransaction"/> managed 
  /// by this transaction.
  /// </summary>
  /// <remarks>Derived classes can provide specific implementations of <typeparamref name="TTransaction"/> by overriding
  /// <see cref="WxeTransactionBase{TTransaction}.GetRootTransactionFromFunction"/>. In many cases it will however be more convenient to override 
  /// <see cref="WxeTransactedFunctionBase{TTransaction}.CreateRootTransaction"/>.</remarks>
  [Serializable]
  public class WxeScopedTransaction<TTransaction, TScope, TScopeManager> : WxeTransactionBase<TTransaction>
    where TTransaction : class, ITransaction
    where TScope : class, ITransactionScope<TTransaction>
    where TScopeManager : ITransactionScopeManager<TTransaction, TScope>, new ()
  {
    [NonSerialized]
    private Stack<TScope> _scopeStack;
    [NonSerialized]
    private TScopeManager _scopeManager;

    /// <summary>Creates a new instance with an empty step list and autoCommit enabled that uses the existing transaction, if one exists.</summary>
    public WxeScopedTransaction ()
        : this (null, true, true)
    {
    }

    /// <summary>Creates a new instance with an empty step list that uses the existing transaction, if one exists.</summary>
    /// <param name="autoCommit">If <see langword="true"/>, the transaction is committed after execution, otherwise it is rolled back.</param>
    public WxeScopedTransaction (bool autoCommit)
        : this (null, autoCommit, true)
    {
    }

    /// <summary>Creates a new instance with an empty step list.</summary>
    /// <param name="autoCommit">If <see langword="true"/>, the transaction is committed after execution, otherwise it is rolled back.</param>
    /// <param name="forceRoot">If <see langword="true"/>, a new root transaction will be created even if a parent transaction exists.</param>
    public WxeScopedTransaction (bool autoCommit, bool forceRoot)
        : this (null, autoCommit, forceRoot)
    {
    }

    /// <summary>Creates a new instance.</summary>
    /// <param name="steps">Initial step list. Can be <see langword="null"/>.</param>
    /// <param name="autoCommit">If <see langword="true"/>, the transaction is committed after execution, otherwise it is rolled back.</param>
    /// <param name="forceRoot">If <see langword="true"/>, a new root transaction will be created even if a parent transaction exists.</param>
    public WxeScopedTransaction (WxeStepList steps, bool autoCommit, bool forceRoot)
        : base (steps, autoCommit, forceRoot)
    {
    }

    private TScopeManager ScopeManager
    {
      get
      {
        if (_scopeManager == null)
          _scopeManager = new TScopeManager ();

        return _scopeManager;
      }
    }

    /// <summary>
    /// Gets the current <typeparamref name="TTransaction"/> or <see langword="null"/> if none is set.
    /// </summary>
    protected override TTransaction CurrentTransaction
    {
      get
      {
        if (ScopeManager.ActiveScope != null)
          return ScopeManager.ActiveScope.ScopedTransaction;
        else
          return null;
      }
    }

    /// <summary>
    /// Checks whether <see cref="CurrentTransaction"/> can be reset and throws an exception if it can't.
    /// </summary>
    /// <exception cref="InvalidOperationException">There is no current transaction or it is read-only or needs to be committed or rolled back.</exception>
    protected override void CheckCurrentTransactionResettable ()
    {
      if (CurrentTransaction == null)
        throw new InvalidOperationException ("There is no current transaction.");

      if (CurrentTransaction.IsReadOnly)
        throw new InvalidOperationException (
            "The current transaction cannot be reset as it is read-only. The reason might be an open child transaction.");

      if (CurrentTransaction.HasUncommittedChanges)
        throw new InvalidOperationException (
            "The current transaction cannot be reset as it is in a dirty state and needs to be committed or rolled back.");

      // else succeed
    }

    private Stack<TScope> ScopeStack
    {
      get
      {
        if (_scopeStack == null)
          _scopeStack = new Stack<TScope>();
        return _scopeStack;
      }
    }

    /// <summary>
    /// Sets a new current transaction.
    /// </summary>
    /// <param name="transaction">The new transaction.</param>
    protected override void SetCurrentTransaction (TTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      ScopeStack.Push (ScopeManager.ActiveScope);
      TScope newScope = ScopeManager.EnterScope (transaction); // set new scope and store old one
      Assertion.IsTrue (ScopeManager.ActiveScope == newScope);
    }

    /// <summary>
    /// Resets the current <typeparamref name="TTransaction"/> to the transaction previously replaced via <see cref="SetCurrentTransaction"/>.
    /// </summary>
    /// <param name="previousTransaction">The transaction previously replaced by <see cref="SetCurrentTransaction"/>.</param>
    protected override void SetPreviousCurrentTransaction (TTransaction previousTransaction)
    {
      if (ScopeStack.Count == 0)
        throw new InvalidOperationException ("RestorePreviousTransaction must never be called more often than SetCurrentTransaction.");

      TTransaction storedPreviousTransaction = ScopeStack.Peek () != null ? ScopeStack.Peek ().ScopedTransaction : null;
      if (storedPreviousTransaction != null && previousTransaction != storedPreviousTransaction)
        throw new ArgumentException ("The given transaction is not the previous transaction.", "previousTransaction");

      if (ScopeManager.ActiveScope == null)
        throw new InconsistentClientTransactionScopeException ("Somebody else has removed the active transaction scope.");

      ScopeManager.ActiveScope.Leave();

      if (ScopeManager.ActiveScope != ScopeStack.Pop())
        throw new InconsistentClientTransactionScopeException ("The active transaction scope does not contain the expected scope.");

      if (previousTransaction != null && ScopeManager.ActiveScope == null)
      {
        // there was a Thread transition during execution of this function, we need to restore the transaction that was active when this whole thing
        // started
        // we cannot restore the same scope we had on the other thread, but we can restore the transaction
        ScopeManager.EnterScope (previousTransaction);
      }

      Assertion.IsTrue (previousTransaction == CurrentTransaction);
    }
  }
}
