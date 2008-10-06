using System;
using System.Collections;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  public abstract class TransactionStrategyBase : ITransactionStrategy, IWxeFunctionExecutionListener
  {
    private readonly bool _autoCommit;
    private readonly IWxeFunctionExecutionListener _innerListener;
    private readonly IWxeFunctionExecutionContext _executionContext;
    private readonly ITransactionStrategy _parent;

    protected TransactionStrategyBase (bool autoCommit, ITransactionStrategy parent, IWxeFunctionExecutionListener innerListener, IWxeFunctionExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("parent", parent);
      ArgumentUtility.CheckNotNull ("innerListener", innerListener);
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);

      _autoCommit = autoCommit;
      _parent = parent;
      _innerListener = innerListener;
      _executionContext = executionContext;
  }

    public bool AutoCommit
    {
      get { return _autoCommit; }
    }

    public ITransactionStrategy Parent
    {
      get { return _parent; }
    }

    public IWxeFunctionExecutionListener InnerListener
    {
      get { return _innerListener; }
    }

    public IWxeFunctionExecutionContext ExecutionContext
    {
      get { return _executionContext; }
    }

    public abstract void OnExecutionPlay (WxeContext context);
    public abstract void OnExecutionStop (WxeContext context);
    public abstract void OnExecutionPause (WxeContext context);
    public abstract void OnExecutionFail (WxeContext context, Exception exception);

    public abstract TTransaction GetNativeTransaction<TTransaction> ();
    public abstract void Commit ();
    public abstract void Rollback ();
    public abstract void Reset ();
    public abstract void RegisterObjects (IEnumerable objects);

    public abstract bool IsNull { get; }
  }
}