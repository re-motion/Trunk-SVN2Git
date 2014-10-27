using System;
using Coypu;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// Base class for all WXE-based waiting strategies which wait for a new root WXE function to execute.
  /// </summary>
  public abstract class WxeResetWaitingStrategyBase : WxeWaitingStrategyBase
  {
    private const string c_wxeFunktionTokenId = "WxeFunctionToken";

    protected string GetWxeFunctionToken (ElementScope scope)
    {
      return scope.FindId (c_wxeFunktionTokenId).Value;
    }

    protected void WaitForWxeFunctionTokenToBeDifferent (TestObjectContext context, ElementScope scope, string oldWxeFunctionToken)
    {
      context.Window.Query (() => GetWxeFunctionToken (scope) != oldWxeFunctionToken, true);

      Assertion.IsTrue (
          GetWxeFunctionToken (scope) != oldWxeFunctionToken,
          string.Format ("Expected WxeFunctionToken to be different to '{0}', but it is equal.", oldWxeFunctionToken));
    }
  }
}