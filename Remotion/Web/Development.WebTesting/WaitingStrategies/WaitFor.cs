using System;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// <see cref="IWaitingStrategy"/> implementations which are directly supported by the framework.
  /// </summary>
  public static class WaitFor
  {
    public static readonly IWaitingStrategy Nothing = new NullWaitingStrategy();
    public static readonly IWaitingStrategy WxePostBack = new WxePostBackWaitingStrategy (1);
    public static readonly Func<PageObject, IWaitingStrategy> WxePostBackIn = po => new WxePostBackInWaitingStrategy (po.Context.FrameRootElement, 1);
    public static readonly IWaitingStrategy WxeReset = new WxeResetWaitingStrategy();
    public static readonly Func<PageObject, IWaitingStrategy> WxeResetIn = po => new WxeResetInWaitingStrategy (po.Context.FrameRootElement);
  }
}