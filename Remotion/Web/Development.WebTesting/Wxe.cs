using System;
using Remotion.Web.Development.WebTesting.CompletionDetectionImplementation;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// WXE-based <see cref="ICompletionDetectionStrategy"/> implementations which are directly supported by the framework.
  /// </summary>
  public static class Wxe
  {
    public static readonly WxePostBackCompletionDetectionStrategy PostBackCompleted = new WxePostBackCompletionDetectionStrategy (1);

    public static readonly Func<PageObject, WxePostBackInCompletionDetectionStrategy> PostBackCompletedIn =
        po => new WxePostBackInCompletionDetectionStrategy (po.Context, 1);

    public static readonly WxeResetCompletionDetectionStrategy Reset = new WxeResetCompletionDetectionStrategy();

    public static readonly Func<PageObject, WxeResetInCompletionDetectionStrategy> ResetIn =
        po => new WxeResetInCompletionDetectionStrategy (po.Context);
  }
}