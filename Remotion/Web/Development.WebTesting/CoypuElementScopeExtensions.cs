using System;
using Coypu;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various extension methods for Coypu's <see cref="ElementScope"/> class.
  /// </summary>
  public static class CoypuElementScopeExtensions
  {
    ///// <summary>
    ///// Focuses a link before actually clicking it.
    ///// </summary>
    ///// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    //public static void FocusClick (this ElementScope scope)
    //{
    //  scope.FocusLink();
    //  scope.Click();
    //}

    ///// <summary>
    ///// Focuses a link.
    ///// </summary>
    ///// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    //private static void FocusLink (this ElementScope scope)
    //{
    //  scope.SendKeys ("");
    //}

    ///// <summary>
    ///// Todo RM-6297: Docs
    ///// </summary>
    //public static void ClickAndWait (this ElementScope scope, TestObjectContext context, IWaitForActionCompletedStrategy waitStrategy)
    //{
    //  scope.ClickUsingWaitStrategy (context, waitStrategy);
    //  waitStrategy.PerformWaitAfterActionPerformed (context);
    //}

    ///// <summary>
    ///// Todo RM-6297: Docs
    ///// </summary>
    ///// <param name="scope"></param>
    ///// <param name="context"></param>
    ///// <param name="waitStrategy"></param>
    //public static void ClickUsingWaitStrategy (this ElementScope scope, TestObjectContext context, IWaitForActionCompletedStrategy waitStrategy)
    //{
    //  Action<ElementScope> clickAction = s => s.FocusClick();
    //  scope.PerformActionUsingWaitStrategy (clickAction, context, waitStrategy);
    //}

    ///// <summary>
    ///// Performs an <paramref name="action"/> on a DOM element (given by <paramref name="scope"/>), which is part of a control object (represented by
    ///// its <paramref name="context"/>) using the given <paramref name="waitStrategy"/>.
    ///// </summary>
    //public static void PerformActionUsingWaitStrategy (
    //    this ElementScope scope,
    //    Action<ElementScope> action,
    //    TestObjectContext context,
    //    IWaitForActionCompletedStrategy waitStrategy)
    //{
    //  waitStrategy.OnBeforeActionPerformed (context);
    //  action (scope);
    //}
  }
}