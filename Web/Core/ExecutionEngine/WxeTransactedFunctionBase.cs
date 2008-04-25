using System;
using System.Runtime.Serialization;
using Remotion.Data;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{

  /// <summary> A <see cref="WxeFunction"/> with an excapsulated <see cref="WxeTransactionBase{TTransaction}"/>. </summary>
  /// <typeparam name="TTransaction">
  ///   The <see cref="ITransaction"/> implementation wrapped by this <see cref="WxeTransactedFunctionBase{TTransaction}"/>. 
  /// </typeparam>
  [Serializable]
  public abstract class WxeTransactedFunctionBase<TTransaction> : WxeFunction, IDeserializationCallback
    where TTransaction : class, ITransaction
  {
    private WxeTransactionBase<TTransaction> _wxeTransaction = null;

    /// <summary> Creates a new instance. </summary>
    /// <param name="actualParameters"> The actual function parameters. </param>
    public WxeTransactedFunctionBase (params object[] actualParameters)
      : base (actualParameters)
    {
    }

    /// <summary> Creates the <see cref="WxeTransactionBase{TTransaction}"/> to be encapsulated. </summary>
    /// <returns>
    ///   The <see cref="WxeTransactionBase{TTransaction}"/> instance to be encapsulated in this 
    ///   <see cref="WxeTransactedFunctionBase{TTransaction}"/> or <see langword="null"/> if the 
    ///   <see cref="WxeTransactedFunctionBase{TTransaction}"/> does not have its own transaction.
    /// </returns>
    /// <remarks>
    ///   Called during the first invokation of <see cref="Execute"/>
    ///   <note type="inotes">
    ///     Override this method to initialize your <see cref="WxeTransactionBase{TTransaction}"/> implementation.
    ///   </note>
    /// </remarks>
    protected abstract WxeTransactionBase<TTransaction> CreateWxeTransaction ();

    /// <summary>Creates a new root transaction. </summary>
    /// <returns>A new root instance of type <typeparamref name="TTransaction"/>.</returns>
    /// <remarks>This method is called via <see cref="WxeTransactionBase{TTransaction}"/> when a new root <typeparamref name="TTransaction"/> is needed.</remarks>
    protected internal abstract TTransaction CreateRootTransaction ();

    /// <summary> Gets the underlying <typeparamref name="TTransaction"/> owned by this <see cref="WxeTransactedFunctionBase{TTransaction}"/>.</summary>
    protected TTransaction MyTransaction
    {
      get
      {
        if (_wxeTransaction == null)
          return null;
        else
          return _wxeTransaction.Transaction;
      }
    }

    /// <summary> Gets the underlying <typeparamref name="TTransaction"/> used when this <see cref="WxeTransactedFunctionBase{TTransaction}"/>
    /// is executed. This is either <see cref="MyTransaction"/> or, when this function does not have an own transaction, the
    /// <see cref="Transaction"/> of this function's parent function.</summary>
    protected TTransaction Transaction
    {
      get
      {
        if (MyTransaction != null)
          return MyTransaction;
        else
        {
          WxeTransactedFunctionBase<TTransaction> parent = GetStepByType<WxeTransactedFunctionBase<TTransaction>> (ParentFunction);
          if (parent != null)
            return parent.Transaction;
          else
            return null;
        }
      }
    }

    public override void Execute (WxeContext context)
    {
      if (!ExecutionStarted)
      {
        _wxeTransaction = CreateWxeTransaction ();

        if (_wxeTransaction != null)
        {
          Encapsulate (_wxeTransaction);
          InitializeEvents (_wxeTransaction);
        }
      }

      base.Execute (context);
    }

    /// <summary>
    /// Discards of the <see cref="MyTransaction"/> instance managed by this <see cref="WxeTransactedFunctionBase{TTransaction}"/> and replaces
    /// it by a newly created instance of <typeparamref name="TTransaction"/>. This method can only be called from within <see cref="Execute"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Execution of this <see cref="WxeTransactedFunctionBase{TTransaction}"/> hasn't started yet or
    /// has already finished, or this function does not have its own <see cref="MyTransaction"/>, or <see cref="MyTransaction"/> is not the
    /// <see cref="WxeTransactionBase{TTransaction}.CurrentTransaction"/>.</exception>
    /// <remarks>
    /// Use this method if you need to replace this <see cref="WxeTransactedFunctionBase{TTransaction}"/>'s transaction by a new, empty one while the
    /// function is executing.
    /// </remarks>
    public virtual void ResetTransaction ()
    {
      if (!ExecutionStarted)
        throw new InvalidOperationException ("Transaction cannot be reset before the function has started execution.");

      if (_wxeTransaction == null)
        throw new InvalidOperationException ("This function does not have a transaction to be reset.");

      _wxeTransaction.Reset ();
    }

    private void InitializeEvents (WxeTransactionBase<TTransaction> wxeTransaction)
    {
      wxeTransaction.TransactionCreating += delegate { OnTransactionCreating (); };
      wxeTransaction.TransactionCreated += delegate (object sender, WxeTransactionEventArgs<TTransaction> args) 
          { 
            OnTransactionCreated (args.Transaction); 
          };
      wxeTransaction.TransactionCommitting += delegate { OnCommitting (); };
      wxeTransaction.TransactionCommitted += delegate { OnCommitted(); };
      wxeTransaction.TransactionRollingBack += delegate { OnRollingBack (); };
      wxeTransaction.TransactionRolledBack += delegate { OnRolledBack (); };
    }

    void IDeserializationCallback.OnDeserialization (Object sender)
    {
      OnDeserialization (sender);
    }

    protected virtual void OnDeserialization (Object sender)
    {
      if (_wxeTransaction != null)
        InitializeEvents (_wxeTransaction);
    }

    /// <summary> 
    ///   Called before creating the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/>. 
    /// </summary>
    protected virtual void OnTransactionCreating ()
    {
      if (TransactionCreating != null)
        TransactionCreating (this, EventArgs.Empty);
    }

    /// <summary> 
    ///   Called after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> has been created.
    /// </summary>
    protected virtual void OnTransactionCreated (TTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      if (TransactionCreated != null)
        TransactionCreated (this, new WxeTransactedFunctionEventArgs<TTransaction> (transaction));
    }

    /// <summary> 
    ///   Called before committing the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/>. 
    /// </summary>
    protected virtual void OnCommitting ()
    {
      if (Committing != null)
        Committing (this, EventArgs.Empty);
    }

    /// <summary> 
    ///   Called after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> has been committed. 
    /// </summary>
    protected virtual void OnCommitted ()
    {
      if (Committed != null)
        Committed (this, EventArgs.Empty);
    }

    /// <summary> 
    ///   Called before rolling the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> back. 
    /// </summary>
    protected virtual void OnRollingBack ()
    {
      if (RollingBack != null)
        RollingBack (this, EventArgs.Empty);
    }

    /// <summary> 
    ///   Called after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> has been rolled back. 
    /// </summary>
    protected virtual void OnRolledBack ()
    {
      if (RolledBack != null)
        RolledBack (this, EventArgs.Empty);
    }
    /// <summary> 
    ///   Is fired before the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> is created. 
    /// </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler TransactionCreating;

    /// <summary> 
    ///   Is fired after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> is created. 
    /// </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler<WxeTransactedFunctionEventArgs<TTransaction>> TransactionCreated;

    /// <summary> 
    ///   Is fired before the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> is committed. 
    /// </summary>
    /// <remarks> 
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler Committing;

    /// <summary> 
    ///   Is fired after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> has been committed. 
    /// </summary>
    /// <remarks> 
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler Committed;

    /// <summary> 
    ///   Is fired before the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> is rolled back. 
    /// </summary>
    /// <remarks> 
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler RollingBack;

    /// <summary> 
    ///   Is fired after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> has been rolled back. 
    /// </summary>
    /// <remarks> 
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler RolledBack;
  }

}

