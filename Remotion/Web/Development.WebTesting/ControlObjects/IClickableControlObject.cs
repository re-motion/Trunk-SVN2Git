using System;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing something clickable.
  /// </summary>
  public interface IClickableControlObject
  {
    /// <summary>
    /// Clicks the link, using a given <paramref name="waitingStrategy"/> to wait for the triggered action's results.
    /// </summary>
    /// <param name="waitingStrategy">Waiting strategy to use, implementation uses its default strategy if <see langword="null" /> is passed.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    UnspecifiedPageObject Click (IWaitingStrategy waitingStrategy = null);
  }
}