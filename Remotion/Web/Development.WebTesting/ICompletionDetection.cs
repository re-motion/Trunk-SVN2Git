using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Interface for test-writers to specify additional behavior when performing an action like a button click.
  /// </summary>
  public interface ICompletionDetection
  {
    /// <summary>
    /// Instructs the system to use a given <paramref name="waitingStrategy"/> to properly wait for all responses of all requests the action
    /// has triggered.
    /// </summary>
    ICompletionDetection WaitFor ([NotNull] IWaitingStrategy waitingStrategy);

    /// <summary>
    /// Accepts the modal browser dialog which is triggrered by the corresponding action.
    /// </summary>
    ICompletionDetection AcceptModalDialog ();

    /// <summary>
    /// Cancels the modal browser dialog which is triggered by the corresponding action.
    /// </summary>
    ICompletionDetection CancelModalDialog ();

    /// <summary>
    /// Informs the system that the action is closing the window.
    /// </summary>
    ICompletionDetection ClosesWindow ();
  }
}