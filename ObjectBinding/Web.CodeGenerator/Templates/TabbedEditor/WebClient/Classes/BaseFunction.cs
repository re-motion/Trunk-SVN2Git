using System;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;

namespace $PROJECT_ROOTNAMESPACE$.Classes
{
  public abstract class BaseFunction: WxeTransactedFunction
  {
    protected BaseFunction (params object[] args)
      : this (WxeTransactionMode.CreateChildIfParent, args)
    {
    }

    protected BaseFunction (WxeTransactionMode transactionMode, params object[] args)
      : base (transactionMode, args)
    {
    }
  }
}
