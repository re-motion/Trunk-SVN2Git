using System;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  public class TransactionStrategyBase : ITransactionStrategy
  {
    private readonly bool _autoCommit;
    private readonly IWxeFunctionExecutionListener _innerListener;

    public TransactionStrategyBase (bool autoCommit, IWxeFunctionExecutionListener innerListener)
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
  }
}