using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// Waits for the WXE sequence number to increase by a given amount. In contrast to <see cref="WxePostBackWaitingStrategy"/> it inspects the
  /// sequence number on a specified scope, instead of the given context.
  /// </summary>
  public class WxePostBackInWaitingStrategy : WxeWaitingStrategyBase, IWaitingStrategy
  {
    private readonly ElementScope _scope;
    private readonly int _expectedWxePostBackSequenceNumberIncrease;

    public WxePostBackInWaitingStrategy ([NotNull] ElementScope scope, int expectedWxePostBackSequenceNumberIncrease)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      _scope = scope;
      _expectedWxePostBackSequenceNumberIncrease = expectedWxePostBackSequenceNumberIncrease;
    }

    public object OnBeforeActionPerformed (TestObjectContext context)
    {
      var wxePostBackSequenceNumber = GetWxePostBackSequenceNumber (_scope);
      return wxePostBackSequenceNumber;
    }

    public void PerformWaitAfterActionPerformed (TestObjectContext context, object state)
    {
      var oldWxePostBackSequenceNumber = (int) state;
      var expectedWxePostBackSequenceNumber = oldWxePostBackSequenceNumber + _expectedWxePostBackSequenceNumberIncrease;

      WaitForWxePostBackSequenceNumber (context, _scope, expectedWxePostBackSequenceNumber);
    }
  }
}