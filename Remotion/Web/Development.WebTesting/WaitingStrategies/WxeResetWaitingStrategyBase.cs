using System;
using Coypu;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// Base class for all WXE-reset-bsed (new root WXE function execution) waiting strategies.
  /// </summary>
  public abstract class WxeResetWaitingStrategyBase : WxeWaitingStrategyBase
  {
    private const string c_wxeFunktionTokenId = "WxeFunctionToken";

    protected string GetWxeFunctionToken (ElementScope scope)
    {
      // Todo RM-6297: make exception safe.
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