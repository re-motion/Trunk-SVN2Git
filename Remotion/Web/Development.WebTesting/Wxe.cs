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

    public static readonly Func<TestObject, WxePostBackInCompletionDetectionStrategy> PostBackCompletedIn =
        to => new WxePostBackInCompletionDetectionStrategy (to.Context.FrameRootElement, 1);

    public static readonly WxeResetCompletionDetectionStrategy Reset = new WxeResetCompletionDetectionStrategy();

    public static readonly Func<TestObject, WxeResetInCompletionDetectionStrategy> ResetIn =
        to => new WxeResetInCompletionDetectionStrategy (to.Context.FrameRootElement);
  }
}