using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// Todo RM-6297: Docs
  /// </summary>
  public class WxeResetInWaitingStrategy : WxeWaitingStrategyBase, IWaitingStrategy
  {
    private readonly ElementScope _scope;

    public WxeResetInWaitingStrategy ([NotNull] ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      _scope = scope;
    }

    public object OnBeforeActionPerformed (TestObjectContext context)
    {
      return null;
    }

    public void PerformWaitAfterActionPerformed (TestObjectContext context, object state)
    {
      const int expectedWxePostBackSequenceNumber = 2;
      WaitForWxePostBackSequenceNumber (context, _scope, expectedWxePostBackSequenceNumber);
    }
  }
}