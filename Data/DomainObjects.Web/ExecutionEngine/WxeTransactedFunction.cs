using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using System.Collections;

namespace Remotion.Data.DomainObjects.Web.ExecutionEngine
{
  /// <summary>
  /// A <see cref="WxeFunction"/> that will always have a <see cref="Remotion.Data.DomainObjects.ClientTransaction"/>.
  /// </summary>
  /// <remarks>
  /// <para>A <b>WxeTransactedFunction</b> always creates a new <see cref="ClientTransaction"/>, unless <see cref="WxeTransactionMode.None"/>
  /// is passed to the constructor. <see cref="DomainObject">DomainObjects</see> passed to a <see cref="WxeTransactedFunction"/> as In parameters are
  /// automatically enlisted in the new transaction; <see cref="DomainObject">DomainObjects</see> returned as Out parameters are automatically
  /// enlisted in the surrounding transaction (if any).
  /// </para>
  /// <para>
  /// Override <see cref="CreateRootTransaction"/> if you wish to replace the default behavior of creating new <see cref="ClientTransaction"/>
  /// instances.
  /// </para>
  /// <para>A <see cref="WxeTransactedFunction"/> has <see cref="AutoCommit"/> set to <see langword="true"/> by default. <br />
  /// To change this behavior for a function you can overwrite this property.</para>
  /// </remarks>
  [Serializable]
  public class WxeTransactedFunction : WxeTransactedFunctionBase<ClientTransaction>
  {
    private WxeTransactionMode _transactionMode;

    /// <summary>
    /// Creates a new <b>WxeTransactedFunction</b> that has a new <see cref="ClientTransaction"/>.
    /// </summary>
    /// <param name="actualParameters">Parameters that are passed to the <see cref="WxeFunction"/>.</param>
    public WxeTransactedFunction (params object[] actualParameters)
      : this (WxeTransactionMode.CreateRoot, actualParameters)
    {
    }

    /// <summary>
    /// Creates a new <b>WxeTransactedFunction</b>
    /// </summary>
    /// <param name="transactionMode">A value indicating the behavior of the WxeTransactedFunction.</param>
    /// <param name="actualParameters">Parameters that are passed to the <see cref="WxeFunction"/>.</param>
    public WxeTransactedFunction (WxeTransactionMode transactionMode, params object[] actualParameters)
      : base (actualParameters)
    {
      ArgumentUtility.CheckValidEnumValue ("transactionMode", transactionMode);

      _transactionMode = transactionMode;
    }

    /// <summary>
    /// Gets or sets the <see cref="WxeTransactionMode"/> of the <see cref="WxeTransactedFunction"/>.
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
          throw new InvalidOperationException ("CreateTransactionMode must not be set after execution of this function has started.");

        _transactionMode = value;
      }
    }

    /// <summary>
    /// Creates a new <see cref="WxeScopedTransaction{TTransaction,TScope,TScopeManager}"/> depending on the value of <see cref="WxeTransactionMode"/>. 
    /// </summary>
    /// <returns>A new WxeTransaction, if <see cref="WxeTransactionMode"/> has a value of <b>CreateRoot</b>; otherwise <see langword="null"/>.</returns>
    protected override sealed WxeTransactionBase<ClientTransaction> CreateWxeTransaction ()
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
    /// Creates a new <see cref="Remotion.Data.DomainObjects.ClientTransaction"/> object.
    /// </summary>
    /// <returns>A new <see cref="Remotion.Data.DomainObjects.ClientTransaction"/>.</returns>
    /// <remarks>Derived class should override this method to provide specific implemenations of <see cref="ClientTransaction"/>s.</remarks>
    protected override ClientTransaction CreateRootTransaction ()
    {
      return ClientTransaction.NewRootTransaction ();
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
    protected virtual WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> CreateWxeTransaction (bool autoCommit, bool forceRoot)
    {
      return new WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> (autoCommit, forceRoot);
    }

    /// <summary>
    /// Gets a value indicating if the function performs an automatic commit on the <see cref="ClientTransaction"/>.
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
    protected override void OnTransactionCreated (ClientTransaction transaction)
    {
      Assertion.IsNotNull (transaction);
      EnlistInParameters (transaction);
      base.OnTransactionCreated (transaction);
    }

    /// <summary>
    /// Executes the current function.
    /// </summary>
    /// <param name="context">The execution context.</param>
    public override void Execute (WxeContext context)
    {
      base.Execute (context);
      if (ClientTransactionScope.HasCurrentTransaction)
        EnlistOutParameters (ClientTransactionScope.CurrentTransaction);
    }

    /// <summary>
    /// Discards of the <see cref="WxeTransactedFunctionBase{TTransaction}.MyTransaction"/> instance managed by this <see cref="WxeTransactedFunctionBase{TTransaction}"/> and replaces
    /// it by a newly created instance of <see cref="ClientTransaction"/>, optionally copying event handlers. This method can only be called from
    /// within <see cref="Execute"/>, when the current transaction has no open subtransaction, and when it holds no new or changed objects.
    /// </summary>
    /// <param name="copyEventHandlers">If true, the method will copy all event handlers registered on the old transaction to the new transaction.
    /// Additionally, it will copy event handlers for <see cref="DomainObjectCollection"/> properties on <see cref="DomainObject"/> instances from
    /// the old transaction to the new transaction. This ensures event listeners are still subscribed to the events even though the transaction is
    /// replaced. Note that setting this parameter to true causes all the objects from the old transaction to be reloaded in the new one (if they
    /// still exist); if you specify false, they will only be loaded on first access.</param>
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
    /// All <see cref="DomainObject"/> instances enlisted in this function's transaction prior to the call to this method are automatically enlisted
    /// in the new transaction. To reset a transaction that contains new, changed, or deleted objects, first roll back or commit the transaction.
    /// </para>
    /// </remarks>
    public void ResetTransaction (bool copyEventHandlers)
    {
      ClientTransaction oldTransaction = MyTransaction;

      base.ResetTransaction ();

      Assertion.IsNotNull (oldTransaction, "base method should have thrown if there was no transaction");
      Assertion.IsNotNull (MyTransaction, "base method should have created a new transaction");
      Assertion.IsFalse (oldTransaction.HasChanged (), "WxeTransaction should have thrown if the transaction had been changed");

      MyTransaction.EnlistSameDomainObjects (oldTransaction, copyEventHandlers);
      if (copyEventHandlers)
        MyTransaction.CopyTransactionEventHandlers (oldTransaction);
    }

    /// <summary>
    /// Discards of the <see cref="WxeTransactedFunctionBase{TTransaction}.MyTransaction"/> instance managed by this 
    /// <see cref="WxeTransactedFunctionBase{TTransaction}"/> and replaces
    /// it by a newly created instance of <see cref="ClientTransaction"/>. This method can only be called from within <see cref="Execute"/>, when
    /// the current transaction has no open subtransaction, and when it holds no new or changed objects.
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
    /// All <see cref="DomainObject"/> instances enlisted in this function's transaction prior to the call to this method are automatically enlisted
    /// in the new transaction. To reset a transaction that contains new, changed, or deleted objects, first roll back or commit the transaction.
    /// </para>
    /// <para>
    /// Note that event handlers registered directly on the old transaction or on <see cref="DomainObjectCollection"/> properties will not be copied
    /// to the newly created transaction. Use the <see cref="ResetTransaction(bool)"/> overload to copy those event handlers.
    /// </para>
    /// </remarks>
    public override void ResetTransaction ()
    {
      ResetTransaction (false);
    }

    private void EnlistInParameters (ClientTransaction transaction)
    {
      Set<Tuple<WxeParameterDeclaration, DomainObject>> enlistedObjects = new Set<Tuple<WxeParameterDeclaration, DomainObject>> ();

      WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;
      for (int i = 0; i < parameterDeclarations.Length; i++)
      {
        WxeParameterDeclaration parameterDeclaration = parameterDeclarations[i];
        if (parameterDeclaration.IsIn)
        {
          object parameter = parameterDeclaration.GetValue (Variables);
          EnlistParameter (parameterDeclaration, parameter, transaction, enlistedObjects);
        }
      }

      LoadAllEnlistedObjects (transaction, enlistedObjects);
    }

    private void EnlistOutParameters (ClientTransaction transaction)
    {
      Set<Tuple<WxeParameterDeclaration, DomainObject>> enlistedObjects = new Set<Tuple<WxeParameterDeclaration, DomainObject>> ();

      WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;
      for (int i = 0; i < parameterDeclarations.Length; i++)
      {
        WxeParameterDeclaration parameterDeclaration = parameterDeclarations[i];
        if (parameterDeclaration.IsOut)
        {
          object parameter = parameterDeclaration.GetValue (Variables);
          EnlistParameter (parameterDeclaration, parameter, transaction, enlistedObjects);
        }
      }

      LoadAllEnlistedObjects (transaction, enlistedObjects);
    }

    private void EnlistParameter (WxeParameterDeclaration parameterDeclaration, object parameter, ClientTransaction transaction,
        Set<Tuple<WxeParameterDeclaration, DomainObject>> enlistedObjects)
    {
      if (!TryEnlistAsDomainObject (parameterDeclaration, parameter as DomainObject, transaction, enlistedObjects))
        TryEnlistAsEnumerable (parameterDeclaration, parameter as IEnumerable, transaction, enlistedObjects);
    }

    private bool TryEnlistAsDomainObject (WxeParameterDeclaration parameterDeclaration, DomainObject domainObject, ClientTransaction transaction,
        Set<Tuple<WxeParameterDeclaration, DomainObject>> enlistedObjects)
    {
      if (domainObject != null)
      {
        transaction.EnlistDomainObject (domainObject);
        enlistedObjects.Add (Tuple.NewTuple (parameterDeclaration, domainObject));
        return true;
      }
      else
        return false;
    }

    private bool TryEnlistAsEnumerable (WxeParameterDeclaration parameterDeclaration, IEnumerable enumerable, ClientTransaction transaction,
         Set<Tuple<WxeParameterDeclaration, DomainObject>> enlistedObjects)
    {
      if (enumerable != null)
      {
        foreach (object innerParameter in enumerable)
          EnlistParameter (parameterDeclaration, innerParameter, transaction, enlistedObjects);
        return true;
      }
      else
        return false;
    }

    private void LoadAllEnlistedObjects (ClientTransaction transaction, IEnumerable<Tuple<WxeParameterDeclaration, DomainObject>> objectsToLoad)
    {
      using (transaction.EnterNonDiscardingScope ())
      {
        foreach (Tuple<WxeParameterDeclaration, DomainObject> objectToLoad in objectsToLoad)
        {
          DomainObject loadedObject;
          try
          {
            loadedObject = RepositoryAccessor.GetObject (objectToLoad.B.ID, false);
          }
          catch (Exception ex)
          {
            string message = string.Format (
                "The domain object '{0}' cannot be enlisted in the function's transaction. Maybe it was newly created "
                + "and has not yet been committed, or it was deleted.",
                objectToLoad.B.ID);
            throw new ArgumentException (message, objectToLoad.A.Name, ex);
          }
          Assertion.IsTrue (object.ReferenceEquals (loadedObject, objectToLoad.B));
        }
      }
    }
  }
}