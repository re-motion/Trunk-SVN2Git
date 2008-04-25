using System;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   Represents the method that handles the <c>Click</c> event raised when clicking on a <see cref="MenuTab"/>.
/// </summary>
public delegate void MenuTabClickEventHandler (object sender, MenuTabClickEventArgs e);

/// <summary>
///   Provides data for the <c>Click</c> event.
/// </summary>
public class MenuTabClickEventArgs: WebTabClickEventArgs
{

  /// <summary> Initializes an instance. </summary>
  public MenuTabClickEventArgs (MenuTab tab)
    : base (tab)
  {
  }

  /// <summary> The <see cref="Command"/> that caused the event. </summary>
  public Command Command
  {
    get { return Tab.Command; }
  }

  /// <summary> The <see cref="MenuTab"/> that was clicked. </summary>
  public new MenuTab Tab
  {
    get { return (MenuTab) base.Tab; }
  }
}

}
