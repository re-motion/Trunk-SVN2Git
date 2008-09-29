using System;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  public class TransactionStrategyBase : ITransactionStrategy
  {
    private readonly IWxeFunctionExecutionListener _innerListener;

    public TransactionStrategyBase (IWxeFunctionExecutionListener innerListener)
    {
      ArgumentUtility.CheckNotNull ("innerListener", innerListener);
      _innerListener = innerListener;
    }

    public IWxeFunctionExecutionListener InnerListener
    {
      get { return _innerListener; }
    }
  }
}