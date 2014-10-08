using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public class TestBocTreeViewContextMenu : BocTreeViewMenuItemProvider
  {
    public override WebMenuItem[] InitalizeMenuItems (WebTreeNode node)
    {
      var menuItem = new WebMenuItem
                     {
                         ItemID = "MenuItem",
                         RequiredSelection = RequiredSelection.Any,
                         Text = "Event",
                         Command = { Type = CommandType.Event }
                     };

      return new[] { menuItem };
    }

    public override void OnMenuItemEventCommandClick (WebMenuItem menuItem, WebTreeNode node)
    {
      base.OnMenuItemEventCommandClick (menuItem, node);

      TestOutput.SetActionPerformed (node.TreeView.ID, "NodeContextMenuClick", node.ItemID + "|" + node.Text);
    }

    private BocTreeViewUserControlTestOutput TestOutput
    {
      get { return ((Layout) OwnerControl.Page.Master).GetTestOutputControl<BocTreeViewUserControlTestOutput>(); }
    }
  }
}