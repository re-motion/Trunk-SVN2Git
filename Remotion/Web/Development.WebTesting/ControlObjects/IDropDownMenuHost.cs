namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations bearing a <see cref="T:Remotion.Web.UI.Controls.DropDownMenu"/>.
  /// </summary>
  public interface IDropDownMenuHost
  {
    /// <summary>
    /// Returns the <see cref="T:Remotion.Web.UI.Controls.DropDownMenu"/> control object.
    /// </summary>
    DropDownMenuControlObject GetDropDownMenu ();
  }
}