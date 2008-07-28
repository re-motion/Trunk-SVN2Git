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
using Remotion.Data;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{

  /// <summary> Creates a scope for a transaction. </summary>
  /// <typeparam name="TTransaction">The <see cref="ITransaction"/> implementation wrapped by this <see cref="WxeTransactionBase{TTransaction}"/>. </typeparam>
  [Serializable]
  public abstract class WxeTransactionBase<TTransaction> : WxeStepList
    where TTransaction : class, ITransaction
  {
    private static readonly ILog s_log = LogManager.GetLogger ("Remotion.Web.ExecutionEngine.WxeTransactionBase");

    /// <summary> Finds out wheter the specified step is part of a <see cref="WxeTransactionBase{TTransaction}"/>. </summary>
    /// <returns> 
    ///   <see langword="true"/>, if one of the parents of the specified Step is a <see cref="WxeTransactionBase{TTransaction}"/>, 
    ///   otherwise <see langword="false"/>. 
    /// </returns>
    public static bool HasWxeTransaction (WxeStep step)
    {
      return WxeStep.GetStepByType (step, typeof (WxeTransactionBase<TTransaction>)) != null;
    }

    private readonly bool _autoCommit;
    private readonly bool _forceRoot;

    private TTransaction _transaction = null;
    private TTransaction _previousCurrentTransaction = null;
    private bool _isPreviousCurrentTransactionRestored = true; // start with true, in the beginning, there is no previous transaction to be restored

    /// <summary> Creates a new instance. </summary>
    /// <param name="steps"> Initial step list. Can be <see langword="null"/>. </param>
    /// <param name="autoCommit">
    ///   If <see langword="true"/>, the transaction is committed after execution, otherwise it is rolled back. 
    /// </param>
    /// <param name="forceRoot"> 
    ///   If <see langword="true"/>, a new root transaction will be created even if a parent transaction exists. 
    /// </param>
    /// <remarks>
    ///   If the <typeparamref name="TTransaction"/> implementation does not support child transactions 
    /// </remarks>
    public WxeTransactionBase (WxeStepList steps, bool autoCommit, bool forceRoot)
    {
      _autoCommit = autoCommit;
      _forceRoot = forceRoot;
      if (steps != null)
        AddStepList (steps);
    }

    /// <summary> Gets the current <typeparamref name="TTransaction"/>. </summary>
    /// <value> 
    ///   An instance of a type implementing <typeparamref name="TTransaction"/> or <see langword="null"/> if no current
    ///   transaction exists.
    /// </value>
    /// <remarks> 
    ///   <note type="inotes">
    ///     If the <typeparamref name="TTransaction"/> implementation does not support the concept of a current transaction,
    ///     it is valid to always return <see langword="null"/>.
    ///   </note>
    /// </remarks>
    protected abstract TTransaction CurrentTransaction { get; }

    /// <summary> Sets the current <typeparamref name="TTransaction"/> to <paramref name="transaction"/> and stores the previous transaction
    /// for later restoration. </summary>
    /// <param name="transaction"> The new current transaction. </param>
    /// <remarks> 
    ///  <note type="caution">
    ///    This method must not throw an exception.
    ///  </note>
    ///   <note type="inotes">
    ///     It the <typeparamref name="TTransaction"/> implementation does not support the concept of a current transaction,
    ///     it is valid to implement an empty method.
    ///   </note>
    ///   <para>
    ///    Implementers can rely on each call to this method being paired with a call to <see cref="SetPreviousCurrentTransaction"/>.
    ///    However, <see cref="SetCurrentTransaction"/> can be called multiple times, before <see cref="SetPreviousCurrentTransaction"/>
    ///    is invoked. In that case, <see cref="SetPreviousCurrentTransaction"/> must operate in a Last-In-First-Out (stack-like) fashion if they
    ///    store a list of previous transactions.
    ///  </para>
    /// </remarks>
    protected abstract void SetCurrentTransaction (TTransaction transaction);

    /// <summary> Resets the current <typeparamref name="TTransaction"/> to the transaction previously replaced via 
    /// <see cref="SetCurrentTransaction"/>.</summary>
    /// <param name="previousTransaction"> The transaction previously replaced by <see cref="SetCurrentTransaction"/>. </param>
    /// <remarks> 
    ///  <note type="caution">
    ///    This method must not throw an exception.
    ///  </note>
    ///   <note type="inotes">
    ///     It the <typeparamref name="TTransaction"/> implementation does not support the concept of a current transaction,
    ///     it is valid to implement an empty method.
    ///   </note>
    ///   <para>
    ///     Implementes can rely on this method being called exactly once for each call of <see cref="SetCurrentTransaction"/>.
    ///    However, <see cref="SetCurrentTransaction"/> can be called multiple times, before <see cref="SetPreviousCurrentTransaction"/>
    ///    is invoked. Therefore, <see cref="SetPreviousCurrentTransaction"/> must operate in a Last-In-First-Out (stack-like) fashion if they
    ///    store a list of previous transactions.
    ///   </para>
    /// </remarks>
    protected abstract void SetPreviousCurrentTransaction (TTransaction previousTransaction);

    /// <summary>
    /// Checks whether <see cref="CurrentTransaction"/> can be resettable and throws an exception if it isn't.
    /// </summary>
    /// <exception cref="InvalidOperationException">The current transaction cannot be reset or the current transaction is <see langword="null"/>.</exception>
    /// <remarks>When implementing this method, subclasses should check the current transaction for any state issues preventing it from just being
    /// thrown away when this <see cref="WxeTransactionBase{TTransaction}"/> is reset. For example, implementers should throw an exception if the current transaction
    /// is read-only, has an open child transaction, or similar.</remarks>
    protected abstract void CheckCurrentTransactionResettable ();

    /// <summary> Creates a new transaction. </summary>
    /// <returns> A new instance of type <typeparamref name="TTransaction"/>. </returns>
    /// <exception cref="InvalidOperationException"> Thrown if <see langword="null"/> were to be returned as the child transaction. </exception>
    protected TTransaction CreateTransaction ()
    {
      WxeTransactionBase<TTransaction> parentTransaction = GetParentTransaction ();
      TTransaction transaction;

      bool isParentTransactionNull = parentTransaction == null || parentTransaction.Transaction == null;
      bool useParentTransaction = !_forceRoot && !isParentTransactionNull;
      if (useParentTransaction)
      {
        bool hasCurrentTransaction = CurrentTransaction != null;
        if (hasCurrentTransaction && parentTransaction.Transaction != CurrentTransaction)
          throw new InvalidOperationException ("The parent transaction does not match the current transaction.");

        OnTransactionCreating ();
        transaction = CreateChildTransaction (parentTransaction.Transaction);
        s_log.Debug ("Created child " + this.GetType ().Name + ".");
      }
      else
      {
        OnTransactionCreating ();
        transaction = GetRootTransactionFromFunction ();
        if (transaction == null)
          throw new InvalidOperationException (string.Format ("{0}.GetRootTransactionFromFunction() evaluated and returned null.", GetType ().Name));
        s_log.Debug ("Created root " + this.GetType ().Name + ".");
      }

      try
      {
        OnTransactionCreated (transaction);
      }
      catch
      {
        transaction.Release ();
        throw;
      }
      return transaction;
    }

    /// <summary> Called before the <see cref="Transaction"/> is created. </summary>
    /// <remarks> Raises the <see cref="TransactionCreating"/> event. </remarks>
    protected virtual void OnTransactionCreating ()
    {
      if (TransactionCreating != null)
        TransactionCreating (this, EventArgs.Empty);
    }
    
    /// <summary> Called after the <see cref="Transaction"/> has been created. </summary>
    /// <param name="transaction"> The <typeparamref name="TTransaction"/> that has been created. </param>
    /// <remarks> Raises the <see cref="TransactionCreated"/> event. Note that the transaction has not yet been made current when this method is
    /// called; overriders need to perform manual transaction management if they need to use <paramref name="transaction"/>.</remarks>
    protected virtual void OnTransactionCreated (TTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      
      if (TransactionCreated != null)
        TransactionCreated (this, new WxeTransactionEventArgs<TTransaction> (transaction));
    }

    /// <summary>Retrieves a root transaction from the parent transacted function.</summary>
    /// <returns>An instance of <typeparamref name="TTransaction"/> which will be encapsulated by this object.</returns>
    protected virtual TTransaction GetRootTransactionFromFunction ()
    {
      return GetParentTransactedFunction().CreateRootTransaction ();
    }

    /// <summary>
    /// Gets the parent transacted function of this <see cref="WxeTransactionBase{TTransaction}"/>.
    /// </summary>
    /// <returns>This transaction's parent transacted function.</returns>
    /// <exception cref="InvalidOperationException">This object has not yet been encapsulated by a transacted function.</exception>
    public WxeTransactedFunctionBase<TTransaction> GetParentTransactedFunction ()
    {
      WxeTransactedFunctionBase<TTransaction> parentFunction = ParentFunction as WxeTransactedFunctionBase<TTransaction>;
      if (parentFunction == null)
        throw new InvalidOperationException ("This object has not yet been encapsulated by a transacted function.");
      else
        return parentFunction;
    }

    /// <summary> Creates a new <typeparamref name="TTransaction"/> using the <paramref name="parentTransaction"/> as parent. </summary>
    /// <param name="parentTransaction"> The <typeparamref name="TTransaction"/> to be used as parent. </param>
    /// <returns> A new instance of a type implementing <typeparamref name="TTransaction"/>. </returns>
    /// <remarks> 
    ///   The created transaction will be created as a root transaction if the <typeparamref name="TTransaction"/> 
    ///   implementation of the <paramref name="parentTransaction"/> does not support child transactions.
    /// </remarks>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if <see langword="null"/> where to be returned as the child transaction.
    /// </exception>
    protected TTransaction CreateChildTransaction (TTransaction parentTransaction)
    {
      ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);

      TTransaction transaction;
      if (parentTransaction.CanCreateChild)
      {
        transaction = (TTransaction) parentTransaction.CreateChild ();
        if (transaction == null)
        {
          throw new InvalidOperationException (string.Format (
              "{0}.CreateChild() evaluated and returned null.", parentTransaction.GetType ().Name));
        }
      }
      else
      {
        transaction = GetRootTransactionFromFunction ();
        if (transaction == null)
        {
          throw new InvalidOperationException (string.Format (
              "{0}.GetRootTransactionFromFunction() evaluated and returned null.", GetType ().Name));
        }
      }
      return transaction;
    }

    /// <summary> Gets the first parent of type <see cref="WxeTransactionBase{TTransaction}"/>. </summary>
    /// <value> 
    ///   A <see cref="WxeTransactionBase{TTransaction}"/> object or <see langword="null"/> if the current transaction is the 
    ///   topmost transaction.
    /// </value>
    protected WxeTransactionBase<TTransaction> GetParentTransaction()
    {
      return (WxeTransactionBase<TTransaction>) WxeStep.GetStepByType (ParentStep, typeof (WxeTransactionBase<TTransaction>));
    }

    /// <summary>
    /// Resets this <see cref="WxeTransactionBase{TTransaction}"/> instance by discarding of the current <see cref="Transaction"/> and
    /// creating a new instance of <typeparamref name="TTransaction"/>. This method can only be called from within <see cref="Execute"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Execution of this <see cref="WxeTransactionBase{TTransaction}"/> hasn't started yet or
    /// has already finished, or <see cref="Transaction"/> is not the <see cref="CurrentTransaction"/>.</exception>
    protected internal virtual void Reset ()
    {
      if (!ExecutionStarted)
        throw new InvalidOperationException ("Transaction cannot be reset before its execution has started.");

      if (_transaction == null)
        throw new InvalidOperationException ("Transaction cannot be reset after its execution has finished.");

      if (Transaction != CurrentTransaction)
        throw new InvalidOperationException ("A WxeFunction's transaction can only be reset when it is the current transaction.");

      CheckCurrentTransactionResettable ();

      CheckAndRestorePreviousCurrentTransaction ();
      _transaction.Release ();
      _transaction = null;
      InitializeTransaction ();
    }

    /// <summary> Gets the underlying <typeparamref name="TTransaction"/>. </summary>
    public TTransaction Transaction
    {
      get { return _transaction; }
    }

    public override void Execute (WxeContext context)
    {
      if (!ExecutionStarted)
      {
        s_log.Debug ("Initializing execution of " + this.GetType ().FullName + ".");
        InitializeTransaction();
      }
      else
      {
        s_log.Debug (string.Format ("Resuming execution of " + this.GetType ().FullName + "."));
        MakeTransactionCurrent ();
      }

      try
      {
        base.Execute (context);
      }
      catch (Exception e)
      {
        if (e is System.Threading.ThreadAbortException)
        {
          CheckAndRestorePreviousCurrentTransaction(); // leave stack in good order
          throw;
        }

        s_log.Debug ("Aborting execution of " + this.GetType ().Name + " because of exception: \"" + e.Message + "\" (" + e.GetType ().FullName + ").");

        RollbackAndRestoreTransactionForException(e);
        throw;
      }

      try
      {
        if (_autoCommit)
          CommitAndReleaseTransaction ();
        else
          RollbackAndReleaseTransaction ();
        CheckAndRestorePreviousCurrentTransaction ();
      }
      catch (Exception ex)
      {
        RollbackAndRestoreTransactionForException (ex);
        throw;
      }

      s_log.Debug ("Ending execution of " + this.GetType ().Name);
    }

    private void RollbackAndRestoreTransactionForException (Exception e)
    {
      try
      {
        RollbackAndReleaseTransaction ();
      }
      catch (Exception transactionException)
      {
        s_log.Debug (
            "Non-recoverable transaction exception in rollback " + transactionException.GetType ().FullName + " hiding original exception "
                + e.GetType ().FullName + ". Original message: \"" + e.Message + "\" Transaction exception message:\""
                    + transactionException.Message
                        + "\"");
        throw new WxeNonRecoverableTransactionException (e, transactionException);
      }
      finally
      {
        RestorePreviousCurrentTransactionForException(e);
      }
    }

    private void RestorePreviousCurrentTransactionForException (Exception e)
    {
      try
      {
        CheckAndRestorePreviousCurrentTransaction ();
      }
      catch (Exception rollbackException)
      {
        s_log.Debug (
            "Non-recoverable transaction exception in restore" + rollbackException.GetType ().FullName + " hiding original exception "
                + e.GetType ().FullName + ". Original message: \"" + e.Message + "\" Transaction exception message:\""
                    + rollbackException.Message
                        + "\"");
        throw new WxeNonRecoverableTransactionException (e, rollbackException);
      }
    }

    private void InitializeTransaction ()
    {
      _previousCurrentTransaction = CurrentTransaction;
      if (_transaction == null)
        _transaction = CreateTransaction ();
      MakeTransactionCurrent();
    }

    private void MakeTransactionCurrent ()
    {
      if (_transaction == null)
        throw new WxeTransactionAlreadyReleasedException ("Function cannot be executed again because its transaction has already been released.");

      CheckAndSetCurrentTransaction (_transaction);
      _isPreviousCurrentTransactionRestored = false; // we've just set the current transaction, so we need the old one to be restored later on
    }

    protected override void AbortRecursive ()
    {
      s_log.Debug ("Aborting " + this.GetType ().Name);
      base.AbortRecursive ();
      
      // TODO: ExecutionStarted might not be true here
      RollbackAndReleaseTransaction ();
      CheckAndRestorePreviousCurrentTransaction ();
    }

    /// <summary> Commits encasulated <typeparamref name="TTransaction"/> and releases it afterwards. </summary>
    protected void CommitAndReleaseTransaction ()
    {
      if (_transaction != null)
      {
        s_log.Debug ("Committing " + _transaction.GetType ().Name + ".");
        TTransaction previousTransaction = CurrentTransaction;
        CheckAndSetCurrentTransaction (_transaction);
        try
        {
          CommitTransaction (_transaction);
        }
        finally
        {
          SetPreviousCurrentTransaction (previousTransaction);
          Assertion.IsTrue (CurrentTransaction == previousTransaction);

          _transaction.Release ();
          _transaction = null;
        }
      }
    }

    /// <summary> Commits the <paramref name="transaction"/>. </summary>
    /// <param name="transaction"> The <typeparamref name="TTransaction"/> to be committed. </param>
    /// <remarks> 
    ///   Calls the <see cref="OnTransactionCommitting"/> method before committing the transaction
    ///   and the <see cref="OnTransactionCommitted"/> method after the transaction has been committed.
    /// </remarks>
    protected void CommitTransaction (TTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      OnTransactionCommitting ();
      transaction.Commit ();
      OnTransactionCommitted ();
    }

    /// <summary> Called before the <see cref="Transaction"/> is committed. </summary>
    /// <remarks> Raises the <see cref="TransactionCommitting"/> event. </remarks>
    protected virtual void OnTransactionCommitting ()
    {
      if (TransactionCommitting != null)
        TransactionCommitting (this, EventArgs.Empty);
    }

    /// <summary> Called after the <see cref="Transaction"/> has been committed. </summary>
    /// <remarks> Raises the <see cref="TransactionCommitted"/> event. </remarks>
    protected virtual void OnTransactionCommitted ()
    {
      if (TransactionCommitted != null)
        TransactionCommitted (this, EventArgs.Empty);
    }

    /// <summary> Rolls the encasulated <typeparamref name="TTransaction"/> back and relases it afterwards. </summary>
    protected void RollbackAndReleaseTransaction ()
    {
      if (_transaction != null)
      {
        s_log.Debug ("Rolling back " + _transaction.GetType ().Name + ".");
        TTransaction previousTransaction = CurrentTransaction;
        CheckAndSetCurrentTransaction (_transaction);
        try
        {
          RollbackTransaction (_transaction);
        }
        finally
        {
          SetPreviousCurrentTransaction (previousTransaction);
          Assertion.IsTrue (CurrentTransaction == previousTransaction);

          _transaction.Release ();
          _transaction = null;
        }
      }
    }

    /// <summary> Rolls the <paramref name="transaction"/> back. </summary>
    /// <param name="transaction"> The <typeparamref name="TTransaction"/> to be rolled back. </param>
    /// <remarks> 
    ///   Calls the <see cref="OnTransactionRollingBack"/> method before rolling back the transaction 
    ///   and the <see cref="OnTransactionRolledBack"/> method after the transaction has been rolled back.
    /// </remarks>
    protected void RollbackTransaction (TTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      OnTransactionRollingBack ();
      transaction.Rollback ();
      OnTransactionRolledBack ();
    }

    /// <summary> Called before the <see cref="Transaction"/> is rolled back. </summary>
    /// <remarks> Raises the <see cref="TransactionRollingBack"/> event. </remarks>
    protected virtual void OnTransactionRollingBack ()
    {
      if (TransactionRollingBack != null)
        TransactionRollingBack (this, EventArgs.Empty);
    }

    /// <summary> Called after the <see cref="Transaction"/> has been rolled back. </summary>
    /// <remarks> Raises the <see cref="TransactionRolledBack"/> event. </remarks>
    protected virtual void OnTransactionRolledBack ()
    {
      if (TransactionRolledBack != null)
        TransactionRolledBack (this, EventArgs.Empty);
    }

    /// <summary> Sets the backed up transaction as the old and new current transaction. </summary>
    protected void CheckAndSetCurrentTransaction (TTransaction transaction)
    {
      try
      {
        SetCurrentTransaction (transaction);
      }
      catch (Exception ex)
      {
        s_log.Fatal ("SetCurrentTransaction threw an exception.", ex);
        throw new WxeNonRecoverableTransactionException (
            this.GetType().Name + ".SetCurrentTransaction threw an exception. See InnerException property.", ex);
      }
    }

    /// <summary> Sets the backed up transaction as the old and new current transaction. </summary>
    protected void CheckAndRestorePreviousCurrentTransaction ()
    {
      if (!_isPreviousCurrentTransactionRestored)
      {
        try
        {
          SetPreviousCurrentTransaction (_previousCurrentTransaction);
        }
        catch (Exception ex)
        {
          s_log.Fatal ("SetPreviousCurrentTransaction threw an exception.", ex);
          throw new WxeNonRecoverableTransactionException (
              this.GetType().Name + ".SetPreviousCurrentTransaction threw an exception. See InnerException property.", ex);
        }
        Assertion.IsTrue (_previousCurrentTransaction == CurrentTransaction);
        // Note: do not set _previousCurrentTransaction to null, we might need it again later
        _isPreviousCurrentTransactionRestored = true;
      }
    }

    protected bool AutoCommit
    {
      get { return _autoCommit; }
    }

    protected bool ForceRoot
    {
      get { return _forceRoot; }
    }

    /// <summary> Is fired before the <see cref="Transaction"/> is created. </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler TransactionCreating;

    /// <summary> Is fired after the <see cref="Transaction"/> has been created. </summary>
    /// <remarks>
    ///   <para>
      ///   <note type="caution">
      ///     The event handler must be reattached after the <see cref="WxeTransactionBase{TTransaction}"/> has been deserialized.
      ///   </note>
    ///   </para>
    ///   <para>
    ///   Note that the transaction has not yet been made current when this event is raised; subscribers need to perform manual transaction management 
    ///   if they need to use the transaction.
    ///   </para>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler<WxeTransactionEventArgs<TTransaction>> TransactionCreated;

    /// <summary> Is fired before the <see cref="Transaction"/> is committed. </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler TransactionCommitting;

    /// <summary> Is fired after the <see cref="Transaction"/> has been committed. </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler TransactionCommitted;

    /// <summary> Is fired before the <see cref="Transaction"/> is rolled back. </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler TransactionRollingBack;

    /// <summary> Is fired after the <see cref="Transaction"/> has been rolled back. </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler TransactionRolledBack;
  }
}
