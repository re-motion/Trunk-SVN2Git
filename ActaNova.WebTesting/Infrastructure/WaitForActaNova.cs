using System;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.CompletionDetectionImplementation;

namespace ActaNova.WebTesting.Infrastructure
{
  /// <summary>
  /// <see cref="ICompletionDetectionStrategy"/> implementations specific to Acta Nova.
  /// </summary>
  public static class WaitForActaNova
  {
    /// <summary>
    /// Correct waiting implementation in case of a full page reload where the inner frame triggers an asynchronous update of the outer frame.
    /// </summary>
    public static readonly ICompletionDetectionStrategy OuterInnerOuterUpdate = new WxePostBackCompletionDetectionStrategy (2);
  }
}