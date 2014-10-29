namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations bearing a <see cref="T:Remotion.Web.UI.Controls.ListMenu"/>.
  /// </summary>
  public interface IListMenuHost
  {
    /// <summary>
    /// Returns the <see cref="T:Remotion.Web.UI.Controls.ListMenu"/> control object.
    /// </summary>
    ListMenuControlObject GetListMenu ();
  }
}