using System;
using System.Collections;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  public abstract class TransactionStrategyBase : ITransactionStrategy
  {
    protected TransactionStrategyBase ()
    {
    }

    public abstract TTransaction GetNativeTransaction<TTransaction> ();
    public abstract void Commit ();
    public abstract void Rollback ();
    public abstract void Reset ();
    public abstract void RegisterObjects (IEnumerable objects);
    public abstract bool IsNull { get; }

    public abstract TransactionStrategyBase OuterTransactionStrategy { get; }
    public abstract ChildTransactionStrategy CreateChildTransactionStrategy (bool autoCommit, IWxeFunctionExecutionContext executionContext);
    public abstract IWxeFunctionExecutionListener CreateExecutionListener (IWxeFunctionExecutionListener innerListener);
    public abstract void OnExecutionPlay (WxeContext context, IWxeFunctionExecutionListener listener);
    public abstract void OnExecutionStop (WxeContext context, IWxeFunctionExecutionListener listener);
    public abstract void OnExecutionPause (WxeContext context, IWxeFunctionExecutionListener listener);
    public abstract void OnExecutionFail (WxeContext context, IWxeFunctionExecutionListener listener, Exception exception);
  }
}