using System;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Test
{
  public class BaseFunction: WxeFunction
  {
    public BaseFunction (params object[] arguments)
      : base (new NoneTransactionMode (), arguments)
    {
    }

    protected BaseFunction (ITransactionMode transactionMode, params object[] args)
      : base (transactionMode, args)
    {
    }
  }
}
