using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.CompletionDetectionImplementation;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Entry point of the fluent interface for the <see cref="ICompletionDetector"/> build.
  /// </summary>
  public static class Continue
  {
    /// <summary>
    /// Specifies a <see cref="ICompletionDetectionStrategy"/> after which executing of the web test should continue.
    /// </summary>
    public static IAdvancedCompletionDetection When ([NotNull] ICompletionDetectionStrategy completionDetectionStrategy)
    {
      var completionDetector = new CompletionDetector();
      completionDetector.And (completionDetectionStrategy);
      return completionDetector;
    }

    /// <summary>
    /// Specifies, that executing of the web test should continue immediately.
    /// </summary>
    public static IAdvancedCompletionDetection Immediately ()
    {
      return When (new NullCompletionDetectionStrategy());
    }

    /// <summary>
    /// Specifies, that the action should automatically determine an appropriate completion detection strategy.
    /// </summary>
    public static IAdvancedCompletionDetection Automatically ()
    {
      var completionDetector = new CompletionDetector();
      return completionDetector;
    }
  }

  /// <summary>
  /// Part of the fluent interface for building <see cref="ICompletionDetector"/> instances. Web test developers may specify various behavior which
  /// must be respected by the completion detector.
  /// </summary>
  public interface ICompletionDetection
  {
    /// <summary>
    /// Builds the <see cref="ICompletionDetector"/> instance.
    /// </summary>
    ICompletionDetector Build ();
  }

  /// <summary>
  /// Part of the fluent interface for building <see cref="ICompletionDetector"/> instances. This, "advanced", interface contains methods which should
  /// be available only after the initial <see cref="ICompletionDetectionStrategy"/> has been specified.
  /// </summary>
  public interface IAdvancedCompletionDetection : ICompletionDetection
  {
    /// <summary>
    /// Adds another <see cref="ICompletionDetectionStrategy"/> to the completion detector. The strategies are executed sequentially: first all
    /// <see cref="ICompletionDetectionStrategy.PrepareWaitForCompletion"/> calls, then the actual action, finally all
    /// <see cref="ICompletionDetectionStrategy.WaitForCompletion"/> calls (in the same order as before).
    /// </summary>
    /// <param name="completionDetectionStrategy">An additional <see cref="ICompletionDetectionStrategy"/>.</param>
    IAdvancedCompletionDetection And ([NotNull] ICompletionDetectionStrategy completionDetectionStrategy);

    /// <summary>
    /// Informs the completion detector to wait for the current window to be closed. Note: this causes the completion detector to execute all
    /// <see cref="ICompletionDetectionStrategy"/>s on the parent window.
    /// </summary>
    IAdvancedCompletionDetection AndWindowHasClosed ();

    /// <summary>
    /// Accepts the modal browser dialog which is triggrered by the action.
    /// </summary>
    IAdvancedCompletionDetection AndAcceptModalDialog ();

    /// <summary>
    /// Cancels the modal browser dialog which is triggered by the action.
    /// </summary>
    IAdvancedCompletionDetection AndCancelModalDialog ();
  }
}