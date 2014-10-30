using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// A completion detector ensures that an action (e.g. a click on a button) and all its effects (e.g. a postback, an AJAX request, etc.) have been
  /// completed and execution of the next action can begin safely.
  /// </summary>
  /// <remarks>
  /// Completion detection is an important part of race condition prevention in web tests. When beginning an action, we must be sure that the
  /// previous action has completed and the DOM is not modified anymore. Other frameworks simply wait until the new DOM element is available before
  /// interacting with it. However, that is especially not safe for ASP.NET WebForms pages where postbacks often display the very same DOM elements
  /// again.
  /// </remarks>
  public interface ICompletionDetector
  {
    /// <summary>
    /// Called immediately before the action is performed. This method should capture all state (e.g. a page sequence number) required for
    /// subsequently determining whether the action has completed.
    /// </summary>
    void PrepareWaitForCompletion ([NotNull] TestObjectContext context);

    /// <summary>
    /// Called immediately after the action has been performed. This method should block until the DOM fulfills certain characteristics (i.e. a page
    /// sequence number has been increased).
    /// </summary>
    void WaitForCompletion ([NotNull] TestObjectContext context);
  }
}