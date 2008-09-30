using System;
using Remotion.Data;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  public abstract class TransactionStrategyBase : ITransactionStrategy, IWxeFunctionExecutionListener
  {
    private readonly bool _autoCommit;
    private readonly IWxeFunctionExecutionListener _innerListener;

    protected TransactionStrategyBase (bool autoCommit, IWxeFunctionExecutionListener innerListener)
    {
      ArgumentUtility.CheckNotNull ("innerListener", innerListener);
      _autoCommit = autoCommit;
      _innerListener = innerListener;
    }

    public bool AutoCommit
    {
      get { return _autoCommit; }
    }

    public IWxeFunctionExecutionListener InnerListener
    {
      get { return _innerListener; }
    }

    public abstract void OnExecutionPlay (WxeContext context);
    public abstract void OnExecutionStop (WxeContext context);
    public abstract void OnExecutionPause (WxeContext context);
    public abstract void OnExecutionFail (WxeContext context, Exception exception);

    public abstract ITransaction Transaction { get; }
    public abstract void Commit ();
    public abstract void Rollback ();
    public abstract void Reset ();

    public abstract bool IsNull { get; }
  }
}