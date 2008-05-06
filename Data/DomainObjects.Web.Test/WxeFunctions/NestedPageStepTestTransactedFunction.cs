using System;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  using WxeTransactedFunction = WxeScopedTransactedFunction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>;

  public class NestedPageStepTestTransactedFunction : WxeTransactedFunction
  {
    private WxePageStep Step1 = new WxePageStep ("ImmediatelyReturningPage.aspx");
  }
}
