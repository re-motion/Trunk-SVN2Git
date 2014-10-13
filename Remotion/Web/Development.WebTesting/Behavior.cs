using System;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// <see cref="IActionBehavior"/> implementations which are directly supported by the framework.
  /// </summary>
  public static class Behavior
  {
    public static readonly IActionBehavior WaitForNothing = new ActionBehavior().WaitFor (WaitFor.Nothing);
    public static readonly IActionBehavior WaitForWxePostBack = new ActionBehavior().WaitFor (WaitFor.WxePostBack);
    public static readonly Func<PageObject, IActionBehavior> WaitForWxePostBackIn = po => new ActionBehavior().WaitFor(WaitFor.WxePostBackIn(po));
    public static readonly IActionBehavior WaitForWxeReset = new ActionBehavior().WaitFor(WaitFor.WxeReset);
  }
}