using System;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  public class NestedPageStepTestTransactedFunction : WxeTransactedFunction
  {
    private WxePageStep Step1 = new WxePageStep ("ImmediatelyReturningPage.aspx");
  }
}
