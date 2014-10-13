using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// Todo RM-6297: Docs
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