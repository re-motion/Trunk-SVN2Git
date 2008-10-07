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
    private readonly ITransactionStrategy _parent;

    protected TransactionStrategyBase (bool autoCommit, ITransactionStrategy parent, IWxeFunctionExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("parent", parent);
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);

      _autoCommit = autoCommit;
      _parent = parent;
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

    public IWxeFunctionExecutionContext ExecutionContext
    {
      get { return _executionContext; }
    }

    public abstract void OnExecutionPlay (WxeContext context, IWxeFunctionExecutionListener listener);
    public abstract void OnExecutionStop (WxeContext context, IWxeFunctionExecutionListener listener);
    public abstract void OnExecutionPause (WxeContext context, IWxeFunctionExecutionListener listener);
    public abstract void OnExecutionFail (WxeContext context, IWxeFunctionExecutionListener listener, Exception exception);

    public abstract TTransaction GetNativeTransaction<TTransaction> ();
    public abstract void Commit ();
    public abstract void Rollback ();
    public abstract void Reset ();
    public abstract void RegisterObjects (IEnumerable objects);

    public abstract bool IsNull { get; }
    public abstract IWxeFunctionExecutionListener CreateExecutionListener (IWxeFunctionExecutionListener innerListener);
  }
}