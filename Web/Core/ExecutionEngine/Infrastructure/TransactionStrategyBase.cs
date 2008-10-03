using System;
using System.Collections;
using Remotion.Data;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  public abstract class TransactionStrategyBase : ITransactionStrategy, IWxeFunctionExecutionListener
  {
    private readonly bool _autoCommit;
    private readonly IWxeFunctionExecutionListener _innerListener;
    private readonly IWxeFunctionExecutionContext _executionContext;

    protected TransactionStrategyBase (bool autoCommit, IWxeFunctionExecutionListener innerListener, IWxeFunctionExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("innerListener", innerListener);
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);

      _autoCommit = autoCommit;
      _innerListener = innerListener;
      _executionContext = executionContext;
    }

    public bool AutoCommit
    {
      get { return _autoCommit; }
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