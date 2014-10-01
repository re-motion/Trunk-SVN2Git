using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public class TestBocListWithRowMenuItems : BocList
  {
    protected override WebMenuItem[] InitializeRowMenuItems (IBusinessObject businessObject, int listIndex)
    {
      var baseRowMenuItems = base.InitializeRowMenuItems (businessObject, listIndex);

      var rowMenuItems = new WebMenuItem[2];
      rowMenuItems[0] = new WebMenuItem { ItemID = "RowMenuItemCmd1", Text = "Row menu 1" };
      rowMenuItems[1] = new WebMenuItem { ItemID = "RowMenuItemCmd2", Text = "Row menu 2" };

      return ArrayUtility.Combine (baseRowMenuItems, rowMenuItems);
    }

    protected override void OnRowMenuItemEventCommandClick (WebMenuItem menuItem, IBusinessObject businessObject, int listIndex)
    {
      var command = menuItem.ItemID + "|" + menuItem.Text;
      TestOutput.SetActionPerformed (ID, listIndex, "RowContextMenuClick", command);
    }

    private BocListUserControlTestOutput TestOutput
    {
      get { return ((Layout) Page.Master).GetTestOutputControl<BocListUserControlTestOutput>(); }
    }
  }
}