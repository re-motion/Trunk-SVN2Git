using System;
using Remotion.Web.Development.WebTesting.WaitingStrategyImplementations;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// <see cref="IWaitingStrategy"/> implementations which are directly supported by the framework.
  /// </summary>
  public static class WaitingStrategies
  {
    public static readonly IWaitingStrategy Null = new NullWaitingStrategy();
    public static readonly IWaitingStrategy WxePostBack = new WxePostBackWaitingStrategy();
    public static readonly IWaitingStrategy WxeReset = new WxeResetWaitingStrategy();
  }
}