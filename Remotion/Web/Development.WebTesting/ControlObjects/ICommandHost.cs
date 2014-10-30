using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations bearing a <see cref="T:Remotion.Web.UI.Controls.Command"/>.
  /// </summary>
  public interface ICommandHost
  {
    /// <summary>
    /// Returns the <see cref="T:Remotion.Web.UI.Controls.Command"/> control object.
    /// </summary>
    CommandControlObject GetCommand ();

    /// <summary>
    /// Shortcut, directly executes the command retrieved by <see cref="GetCommand"/>. See <see cref="CommandControlObject.Click"/> for more
    /// information.
    /// </summary>
    UnspecifiedPageObject ExecuteCommand ([CanBeNull] ICompletionDetection completionDetection = null);
  }
}