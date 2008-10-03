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

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Manages a thread's <see cref="CurrentTransaction"/> in a scoped way. Optionally, it can also automatically roll back a transaction at the end
  /// of the scope.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When an instance of this class is created via <see cref="ClientTransaction.EnterScope(DomainObjects.AutoRollbackBehavior)"/>, it sets the
  /// <see cref="ClientTransactionScope.ActiveScope"/> property to the newly created intance. The new instance stores the previous active scope, and
  /// when its <see cref="Leave"/> method is called, it resets <see cref="ClientTransactionScope.ActiveScope"/> to that value (executing the 
  /// <see cref="AutoRollbackBehavior"/> as applicable). Employ a <c>using</c> block to associate a new <see cref="ClientTransaction"/> with the
  /// current thread and to restore the previous transaction (and execute the <see cref="AutoRollbackBehavior"/>) in a scoped way.
  /// </para>
  /// <para>
  /// If <see cref="Leave"/> is not called (and the scope is not disposed of), the previous scope (and thus the previous transaction) is not
  /// automatically restored and the <see cref="AutoRollbackBehavior"/> is not executed; also, the chain of stored previously active transactions
  /// will become a memory leak unless <see cref="ResetActiveScope"/> is used.
  /// </para>
  /// </remarks>
  public class ClientTransactionScope : IDisposable, ITransactionScope, ITransactionScope<ClientTransaction>
  {
    private static readonly SafeContextSingleton<ClientTransactionScope> _scopeSingleton = 
        new SafeContextSingleton<ClientTransactionScope> (typeof (ClientTransactionScope).FullName, delegate { return null; });

    /// <summary>
    /// Gets a value indicating if a <see cref="ClientTransaction"/> is currently set as <see cref="CurrentTransaction"/>. 
    /// </summary>
    /// <remarks>
    /// Even if the value returned by <b>HasCurrentTransaction</b> is false, <see cref="CurrentTransaction"/> will return a
    /// <see cref="ClientTransaction"/>. See <see cref="CurrentTransaction"/> for further information.
    /// </remarks>
    public static bool HasCurrentTransaction
    {
      get { return ClientTransactionScope.ActiveScope != null && ClientTransactionScope.ActiveScope.ScopedTransaction != null; }
    }

    /// <summary>
    /// Gets the <see cref="ClientTransaction"/> associated with the current thread. 
    /// </summary>
    /// <exception cref="InvalidOperationException"><see cref="CurrentTransaction"/> is being used, but no <see cref="ClientTransaction"/> has been
    /// associated with the current thread.</exception>
    /// <remarks>If there is no <see cref="ClientTransaction"/> associated with the current thread, this method throws an exception. It
    /// <b>does not</b> automatically initialize a new transaction. Use a <see cref="ClientTransactionScope"/> to set the current thread's current
    /// transaction.</remarks>
    public static ClientTransaction CurrentTransaction
    {
      get
      {
        if (!HasCurrentTransaction)
          throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread.");
        else
          return ActiveScope.ScopedTransaction;
      }
    }

    /// <summary>
    /// Retrieves the active <see cref="ClientTransactionScope"/> for the current thread.
    /// </summary>
    /// <value>The current thread's active scope, or <see langword="null"/> if no scope is currently active.</value>
    public static ClientTransactionScope ActiveScope
    {
      get { return _scopeSingleton.Current; }
    }

    /// <summary>
    /// Resets the active scope to <see langword="null"/>, causing the previously active scopes to be removed even if they haven't been left.
    /// </summary>
    /// <remarks>This method can be used to implement a custom scoping mechanism, as it circumvents the need of calling a scope's <see cref="Leave"/>
    /// method to avoid memory leaks. It should only be used in very special scenarios, however.</remarks>
    public static void ResetActiveScope ()
    {
      SetActiveScope (null);
    }

    private static void SetActiveScope (ClientTransactionScope scope)
    {
      _scopeSingleton.SetCurrent (scope);
    }

    /// <summary>
    /// Creates a new <see cref="ClientTransactionScope"/> with an empty (<see langword="null"/>) <see cref="ScopedTransaction"/> and makes
    /// it the current thread's <see cref="ActiveScope"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="ClientTransactionScope"/> constructor stores the previous <see cref="ClientTransactionScope.ActiveScope"/>. When this scope's
    /// <see cref="Leave"/> method is called or the scope is disposed of, the previous scope is reactivated.
    /// </para>
    /// </remarks>
    public static ClientTransactionScope EnterNullScope ()
    {
      return new ClientTransactionScope (null, DomainObjects.AutoRollbackBehavior.None);
    }

    private readonly ClientTransactionScope _previousScope;
    private readonly ClientTransaction _scopedTransaction;

    private bool _hasBeenLeft = false;
    private bool _autoEnlistDomainObjects;
    private AutoRollbackBehavior _autoRollbackBehavior;

    /// <summary>
    /// Associates a <see cref="ClientTransaction"/> with the current thread, specifying the scope's automatic rollback behavior.
    /// </summary>
    /// <param name="scopedCurrentTransaction">The <see cref="ClientTransaction"/> object used as the current transaction until the scope is left.</param>
    /// <param name="autoRollbackBehavior">The automatic rollback behavior to be exhibited by this scope.</param>
    /// <remarks>
    /// <para>
    /// The <see cref="ClientTransactionScope"/> constructor stores the previous <see cref="ClientTransactionScope.ActiveScope"/>. When this scope's
    /// <see cref="Leave"/> method is called or the scope is disposed of, the previous scope is reactivated.
    /// </para>
    /// </remarks>
    internal ClientTransactionScope (ClientTransaction scopedCurrentTransaction, AutoRollbackBehavior autoRollbackBehavior)
    {
      _autoRollbackBehavior = autoRollbackBehavior;

      _previousScope = ClientTransactionScope.ActiveScope;

      ClientTransactionScope.SetActiveScope (this);
      _scopedTransaction = scopedCurrentTransaction;

      _autoEnlistDomainObjects = _previousScope != null ? _previousScope.AutoEnlistDomainObjects : false;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this scope will automatically call <see cref="ClientTransaction.Rollback"/> on a transaction
    /// with uncommitted changed objects when the scope's <see cref="Leave"/> method is invoked.
    /// </summary>
    /// <value>An <see cref="AutoRollbackBehavior"/> value indicating how the scope should behave when it is disposed and its transaction's changes
    /// have not been committed.</value>
    public AutoRollbackBehavior AutoRollbackBehavior
    {
      get { return _autoRollbackBehavior; }
      set { _autoRollbackBehavior = value; }
    }

    /// <summary>
    /// Gets the transaction this scope was created for.
    /// </summary>
    /// <value>The transaction passed to the scope's constructor or automatically created by the scope.</value>
    public ClientTransaction ScopedTransaction
    {
      get { return _scopedTransaction; }
    }

    /// <summary>
    /// Gets the transaction managed by this scope.
    /// </summary>
    /// <value>The scoped transaction.</value>
    ITransaction ITransactionScope.ScopedTransaction
    {
      get { return ScopedTransaction; }
    }

    /// <summary>
    /// Gets a flag that describes whether this is the active scope.
    /// </summary>
    bool ITransactionScope.IsActiveScope
    {
      get { return ClientTransactionScope.ActiveScope == this; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="DomainObject"/> instances are automatically enlisted in the current transaction within
    /// this scope.
    /// </summary>
    /// <value>True if <see cref="DomainObject"/> instances are automatically enlisted in the current transaction when accessed;
    /// otherwise, false.</value>
    public bool AutoEnlistDomainObjects
    {
      get { return _autoEnlistDomainObjects; }
      set { _autoEnlistDomainObjects = value; }
    }

    /// <summary>
    /// Resets <see cref="CurrentTransaction"/> to the value it had before this scope was instantiated and performs the
    /// <see cref="AutoRollbackBehavior"/>. This method is ignored when executed more than once.
    /// </summary>
    public void Leave ()
    {
      if (_hasBeenLeft)
        throw new InvalidOperationException ("The ClientTransactionScope has already been left.");

      if (ActiveScope != this)
        throw new InvalidOperationException ("This ClientTransactionScope is not the active scope. Leave the active scope before leaving this one.");

      ExecuteAutoRollbackBehavior ();
      ClientTransactionScope.SetActiveScope (_previousScope);
      _hasBeenLeft = true;
    }

    void IDisposable.Dispose ()
    {
      Leave ();
    }

    private void ExecuteAutoRollbackBehavior ()
    {
      if (AutoRollbackBehavior == AutoRollbackBehavior.Rollback && ScopedTransaction.HasChanged ())
        Rollback ();
      else if (AutoRollbackBehavior == AutoRollbackBehavior.Discard)
        DiscardTransaction ();
    }

    /// <summary>
    /// Commits the transaction scoped by this object. This is equivalent to <c>ScopedTransaction.Commt()</c>.
    /// </summary>
    /// <exception cref="Persistence.PersistenceException">Changes to objects from multiple storage providers were made.</exception>
    /// <exception cref="Persistence.StorageProviderException">An error occurred while committing the changes to the datasource.</exception>
    public void Commit ()
    {
      if (ScopedTransaction != null)
        ScopedTransaction.Commit ();
    }

    /// <summary>
    /// Performs a rollback on the transaction scoped by this object. This is equivalent to <c>ScopedTransaction.Rollback()</c>.
    /// </summary>
    public void Rollback ()
    {
      if (ScopedTransaction != null)
        ScopedTransaction.Rollback ();
    }

    private void DiscardTransaction ()
    {
      if (ScopedTransaction != null)
        ScopedTransaction.Discard ();
    }
  }
}
