using System;
using System.Collections;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  public abstract class TransactionStrategyBase : ITransactionStrategy
  {
    private readonly bool _autoCommit;
    private readonly IWxeFunctionExecutionContext _executionContext;
    private readonly ITransactionStrategy _outerTransactionStrategy;
    private TransactionStrategyBase _child;

    protected TransactionStrategyBase (bool autoCommit, ITransactionStrategy outerTransactionStrategy, IWxeFunctionExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("parent", outerTransactionStrategy);
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);

      _autoCommit = autoCommit;
      _outerTransactionStrategy = outerTransactionStrategy;
      _executionContext = executionContext;
  }

    public bool AutoCommit
    {
      get { return _autoCommit; }
    }

    public ITransactionStrategy OuterTransactionStrategy
    {
      get { return _outerTransactionStrategy; }
    }

    public void SetChild (TransactionStrategyBase child)
    {
      _child = child;
    }

    public TransactionStrategyBase Child
    {
      get { return _child; }
    }

    public IWxeFunctionExecutionContext ExecutionContext
    {
      get { return _executionContext; }
    }

    public abstract TTransaction GetNativeTransaction<TTransaction> ();
    public abstract void Commit ();
    public abstract void Rollback ();
    public abstract void Reset ();
    public abstract void RegisterObjects (IEnumerable objects);

    public abstract bool IsNull { get; }
    public abstract IWxeFunctionExecutionListener CreateExecutionListener (IWxeFunctionExecutionListener innerListener);

    public virtual void OnExecutionPlay (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);

      if (Child != null)
        Child.OnExecutionPlay (context,listener);
      else
        listener.OnExecutionPlay (context);
    }

    public virtual void OnExecutionStop (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);

      if (Child != null)
        Child.OnExecutionStop (context, listener);
      else
        listener.OnExecutionStop (context);
    }

    public virtual void OnExecutionPause (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);

      if (Child != null)
        Child.OnExecutionPause (context, listener);
      else
        listener.OnExecutionPause (context);
    }

    public virtual void OnExecutionFail (WxeContext context, IWxeFunctionExecutionListener listener, Exception exception)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("listener", listener);

      if (Child != null)
        Child.OnExecutionFail (context, listener, exception);
      else
        listener.OnExecutionFail (context, exception);
    }
  }
}