using System;
using System.Collections.Generic;
using Remotion.Data;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// A <see cref="WxeFunction"/> that will always have a scoped transaction associated with it.
  /// </summary>
  /// <remarks>
  /// <para>A <see cref="WxeScopedTransactedFunction{TTransaction,TTransactionScope,TTransactionScopeManager}"/> always creates a new transaction, 
  /// unless <see cref="WxeTransactionMode.None"/> is passed to the constructor. 
  /// In parameters passed to a <see cref="WxeScopedTransactedFunction{TTransaction,TTransactionScope,TTransactionScopeManager}"/> are
  /// automatically enlisted in the new transaction if the transaction supports enlistment; object returned as Out parameters are automatically
  /// enlisted in the surrounding transaction (if any and if supported) as well.
  /// </para>
  /// <para>
  /// Override <see cref="CreateRootTransaction"/> if you wish to replace the default behavior of creating new transaction instances.
  /// </para>
  /// <para>A <see cref="WxeScopedTransactedFunction{TTransaction,TTransactionScope,TTransactionScopeManager}"/> has <see cref="AutoCommit"/> set 
  /// to <see langword="true"/> by default. <br />
  /// To change this behavior for a function you can overwrite this property.</para>
  /// </remarks>
  [Serializable]
  public class WxeScopedTransactedFunction<TTransaction, TScope, TTransactionScopeManager> : WxeTransactedFunctionBase<TTransaction>
      where TTransaction : class, ITransaction
      where TScope : class, ITransactionScope<TTransaction>
      where TTransactionScopeManager : ITransactionScopeManager<TTransaction, TScope>, new()
  {
    private WxeTransactionMode _transactionMode;

    [NonSerialized]
    private TTransactionScopeManager _scopeManager;

    /// <summary>
    /// Creates a new <b>WxeScopedTransactedFunction</b> that has a new scoped <typeparamref name="TTransaction"/>.
    /// </summary>
    /// <param name="actualParameters">Parameters that are passed to the <see cref="WxeFunction"/>.</param>
    public WxeScopedTransactedFunction (params object[] actualParameters)
      : this (WxeTransactionMode.CreateRoot, actualParameters)
    {
    }

    /// <summary>
    /// Creates a new <b>WxeScopedTransactedFunction</b>
    /// </summary>
    /// <param name="transactionMode">A value indicating the behavior of the WxeScopedTransactedFunction.</param>
    /// <param name="actualParameters">Parameters that are passed to the <see cref="WxeFunction"/>.</param>
    public WxeScopedTransactedFunction (WxeTransactionMode transactionMode, params object[] actualParameters)
      : base (actualParameters)
    {
      ArgumentUtility.CheckValidEnumValue ("transactionMode", transactionMode);

      _transactionMode = transactionMode;
    }

    private TTransactionScopeManager ScopeManager
    {
      get
      {
        if (_scopeManager == null)
          _scopeManager = new TTransactionScopeManager ();
        return _scopeManager;
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="WxeTransactionMode"/> of the <see cref="WxeScopedTransactedFunction{TTransaction,TTransactionScope,TTransactionScopeManager}"/>.
    /// </summary>
    /// <remarks>The property must not be set after execution of the function has started.</remarks>
    /// <exception cref="System.InvalidOperationException">An attempt to set the property is performed after execution of the function has started.</exception>
    public WxeTransactionMode TransactionMode
    {
      get { return _transactionMode; }
      set
      {
        ArgumentUtility.CheckValidEnumValue ("transactionMode", value);

        if (ExecutionStarted)
          throw new InvalidOperationException ("TransactionMode must not be set after execution of this function has started.");

        _transactionMode = value;
      }
    }

    /// <summary>
    /// Creates a new <see cref="WxeScopedTransaction{TTransaction,TScope,TScopeManager}"/> depending on the value of <see cref="WxeTransactionMode"/>. 
    /// </summary>
    /// <returns>A new WxeTransaction, if <see cref="WxeTransactionMode"/> has a value of <b>CreateRoot</b>; otherwise <see langword="null"/>.</returns>
    protected override sealed WxeTransactionBase<TTransaction> CreateWxeTransaction ()
    {
      switch (_transactionMode)
      {
        case WxeTransactionMode.CreateRoot:
          return CreateWxeTransaction (AutoCommit, true);
        case WxeTransactionMode.CreateChildIfParent:
          return CreateWxeTransaction (AutoCommit, false);
        default:
          return null;
      }
    }

    /// <summary>
    /// Creates a new <see cref="WxeScopedTransaction{TTransaction,TScope,TScopeManager}"/>. Derived classes should override this method to use their 
    /// own <see cref="WxeScopedTransaction{TTransaction,TScope,TScopeManager}"/>.
    /// </summary>
    /// <param name="autoCommit"><b>autoCommit</b> is forwarded to <see cref="WxeScopedTransaction{TTransaction,TScope,TScopeManager}"/>'s 
    /// constructor. For further information see <see cref="WxeScopedTransaction{TTransaction,TScope,TScopeManager}"/>.</param>
    /// <param name="forceRoot"><b>forceRoot</b> is forwarded to <see cref="WxeScopedTransaction{TTransaction,TScope,TScopeManager}"/>'s 
    /// constructor. For further information see <see cref="WxeScopedTransaction{TTransaction,TScope,TScopeManager}"/>.</param>
    /// <returns>A new WxeTransaction.</returns>
    protected virtual WxeScopedTransaction<TTransaction, TScope, TTransactionScopeManager> CreateWxeTransaction (bool autoCommit, bool forceRoot)
    {
      return new WxeScopedTransaction<TTransaction, TScope, TTransactionScopeManager> (autoCommit, forceRoot);
    }

    /// <summary>
    /// Creates a new <typeparamref name="TTransaction"/> object representing a root transaction.
    /// </summary>
    /// <returns>A new root transaction.</returns>
    /// <remarks>Derived class should override this method if they want to provide specific transaction objects instead of having new instances
    /// instantiated for them.</remarks>
    protected internal override TTransaction CreateRootTransaction ()
    {
      return ScopeManager.CreateRootTransaction ();
    }

    /// <summary>
    /// Gets a value indicating if the function performs an automatic commit on the <typeparamref name="TTransaction"/>.
    /// </summary>
    /// <note type="inheritinfo">Overwrite this property to change the behavior of the function.</note>
    protected virtual bool AutoCommit
    {
      get { return true; }
    }

    /// <summary> 
    ///   Called after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> has been created.
    /// </summary>
    /// <param name="transaction">The transaction that has been created.</param>
    protected override void OnTransactionCreated (TTransaction transaction)
    {
      Assertion.IsNotNull (transaction);
      WxeTransactedFunctionParameterEnlister<TTransaction, TScope, TTransactionScopeManager> enlister = 
          new WxeTransactedFunctionParameterEnlister<TTransaction, TScope, TTransactionScopeManager> (transaction, ScopeManager);

      IEnumerable<WxeParameterDeclaration> inParameters =
          EnumerableUtility.Where (ParameterDeclarations, delegate (WxeParameterDeclaration p) { return p.IsIn; });

      enlister.EnlistParameters (inParameters, Variables);
      enlister.LoadAllEnlistedObjects ();

      base.OnTransactionCreated (transaction);
    }

    /// <summary>
    /// Executes the current function.
    /// </summary>
    /// <param name="context">The execution context.</param>
    public override void Execute (WxeContext context)
    {
      base.Execute (context);
      OnExecutionFinished();
    }

    protected virtual void OnExecutionFinished ()
    {
      if (ScopeManager.ActiveScope != null && ScopeManager.ActiveScope.ScopedTransaction != null)
      {
        WxeTransactedFunctionParameterEnlister<TTransaction, TScope, TTransactionScopeManager> enlister =
            new WxeTransactedFunctionParameterEnlister<TTransaction, TScope, TTransactionScopeManager> (ScopeManager.ActiveScope.ScopedTransaction, ScopeManager);

        IEnumerable<WxeParameterDeclaration> inParameters =
            EnumerableUtility.Where (ParameterDeclarations, delegate (WxeParameterDeclaration p) { return p.IsOut; });

        enlister.EnlistParameters (inParameters, Variables);
        enlister.LoadAllEnlistedObjects ();
      }
    }

    /// <summary>
    /// Discards of the <see cref="WxeTransactedFunctionBase{TTransaction}.MyTransaction"/> instance managed by this <see cref="WxeTransactedFunctionBase{TTransaction}"/> and replaces
    /// it by a newly created instance of <see cref="TTransaction"/>, optionally copying event handlers. This method can only be called from
    /// within <see cref="Execute"/>, when the current transaction has no open subtransaction, and when it holds no new or changed objects.
    /// </summary>
    /// <param name="copyEventHandlers">If true, the method will copy all event handlers registered on the old transaction to the new transaction.
    /// The Remotion.Data.DomainObjects implementation of this method will additionally copy event handlers for <c>DomainObjectCollection</c> 
    /// properties on domain object instances from the old transaction to the new transaction. This ensures event listeners are still subscribed to 
    /// the events even though the transaction is replaced. Note that setting this parameter to true causes all the objects from the old transaction 
    /// to be reloaded in the new one (if they still exist); if you specify false, they will only be loaded on first access.</param>
    /// <exception cref="InvalidOperationException">Execution of this <see cref="WxeTransactedFunctionBase{TTransaction}"/> hasn't started yet or
    /// has already finished, or this function does not have its own <see cref="WxeTransactedFunctionBase{TTransaction}.MyTransaction"/>, or 
    /// <see cref="WxeTransactedFunctionBase{TTransaction}.MyTransaction"/> is not the 
    /// <see cref="WxeTransactionBase{TTransaction}.CurrentTransaction"/>.</exception>
    /// <remarks>
    /// <para>
    /// Use this method if you need to replace this <see cref="WxeTransactedFunctionBase{TTransaction}"/>'s transaction by a new, empty one while the
    /// function is executing.
    /// </para>
    /// <para>
    /// All objects enlisted in this function's transaction prior to the call to this method are automatically enlisted
    /// in the new transaction. To reset a transaction that contains new, changed, or deleted objects, first roll back or commit the transaction.
    /// </para>
    /// </remarks>
    public void ResetTransaction (bool copyEventHandlers)
    {
      TTransaction oldTransaction = MyTransaction;

      base.ResetTransaction ();

      Assertion.IsNotNull (oldTransaction, "base method should have thrown if there was no transaction");
      Assertion.IsNotNull (MyTransaction, "base method should have created a new transaction");
      Assertion.IsFalse (oldTransaction.HasUncommittedChanges, "WxeTransaction should have thrown if the transaction had been changed");

      ScopeManager.EnlistSameObjects (oldTransaction, MyTransaction, copyEventHandlers);
      if (copyEventHandlers)
        ScopeManager.CopyTransactionEventHandlers (oldTransaction, MyTransaction);
    }

    /// <summary>
    /// Discards of the <see cref="WxeTransactedFunctionBase{TTransaction}.MyTransaction"/> instance managed by this 
    /// <see cref="WxeTransactedFunctionBase{TTransaction}"/> and replaces
    /// it by a newly created transaction instance. This method can only be called from within <see cref="Execute"/>, when
    /// the current transaction is writeable and holds no new or changed objects.
    /// </summary>
    /// <exception cref="InvalidOperationException">Execution of this <see cref="WxeTransactedFunctionBase{TTransaction}"/> hasn't started yet or
    /// has already finished, or this function does not have its own <see cref="WxeTransactedFunctionBase{TTransaction}.MyTransaction"/>, or 
    /// <see cref="WxeTransactedFunctionBase{TTransaction}.MyTransaction"/> is not the
    /// <see cref="WxeTransactionBase{TTransaction}.CurrentTransaction"/>.</exception>
    /// <remarks>
    /// <para>
    /// Use this method if you need to replace this <see cref="WxeTransactedFunctionBase{TTransaction}"/>'s transaction by a new, empty one while the
    /// function is executing.
    /// </para>
    /// <para>
    /// All object instances enlisted in this function's transaction prior to the call to this method are automatically enlisted
    /// in the new transaction. To reset a transaction that contains new, changed, or deleted objects, first roll back or commit the transaction.
    /// </para>
    /// <para>
    /// Note that event handlers registered directly on the old transaction or on collection properties will not be copied
    /// to the newly created transaction. Use the <see cref="ResetTransaction(bool)"/> overload to copy those event handlers.
    /// </para>
    /// </remarks>
    public override void ResetTransaction ()
    {
      ResetTransaction (false);
    }
  }
}