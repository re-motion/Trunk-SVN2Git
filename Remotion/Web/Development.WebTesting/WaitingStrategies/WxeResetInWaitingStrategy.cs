using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// Waits for the execution of a new WXE function and the reset of the corresponding WXE sequence number. In contrast to
  /// <see cref="WxeResetWaitingStrategy"/> it inspects the WXE function and the WXE sequence number on a specified scope, instead of the given
  /// context.
  /// </summary>
  public class WxeResetInWaitingStrategy : WxeResetWaitingStrategyBase, IWaitingStrategy
  {
    private readonly ElementScope _scope;

    public WxeResetInWaitingStrategy ([NotNull] ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      _scope = scope;
    }

    public object OnBeforeActionPerformed (TestObjectContext context)
    {
      var wxeFunctionToken = GetWxeFunctionToken (_scope);
      return wxeFunctionToken;
    }

    public void PerformWaitAfterActionPerformed (TestObjectContext context, object state)
    {
      var oldWxeFunctionToken = (string) state;
      WaitForWxeFunctionTokenToBeDifferent (context, _scope, oldWxeFunctionToken);

      const int expectedWxePostBackSequenceNumber = 2;
      WaitForWxePostBackSequenceNumber (context, _scope, expectedWxePostBackSequenceNumber);
    }
  }
}