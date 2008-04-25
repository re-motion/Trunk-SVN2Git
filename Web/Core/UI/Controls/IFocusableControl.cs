using System;

namespace Remotion.Web.UI.Controls
{

public interface IFocusableControl
{
  /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
  /// <value> The ID of a focusable HTML element. </value>
  string FocusID { get; }
}

}
