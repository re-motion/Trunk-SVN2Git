using System;
using Remotion.Data.DomainObjects;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  using WxeTransactedFunction = WxeScopedTransactedFunction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>;

  [Serializable]
  public class RemoveCurrentTransactionScopeFunction : WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public RemoveCurrentTransactionScopeFunction ()
        : base (WxeTransactionMode.CreateRoot)
    {
    }

    // methods and properties

    private void Step1 ()
    {
      ClientTransactionScope.ActiveScope.Leave();
    }
  }
}