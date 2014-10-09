using System;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocListUserControlTestOutput : UserControl
  {
    public void SetInfoForNormalBocList (BocList bocList)
    {
      SelectedIndicesLabel.Text = GetSelectedRowIndicesAsString (bocList);
      SelectedViewLabel.Text = bocList.SelectedView.ItemID;
      EditModeLabel.Text = bocList.IsRowEditModeActive.ToString();
    }

    private string GetSelectedRowIndicesAsString (BocList bocList)
    {
      var selectedRows = string.Join (", ", bocList.GetSelectedRows());
      if (string.IsNullOrEmpty (selectedRows))
        selectedRows = "NoneSelected";
      return selectedRows;
    }

    public void SetActionPerformed (string bocListId, int rowIndex, string action, string parameter)
    {
      ActionPerformedSenderLabel.Text = bocListId;
      ActionPerformedSenderRowLabel.Text = rowIndex.ToString();
      ActionPerformedLabel.Text = action;
      ActionPerformedParameterLabel.Text = parameter;
    }
  }
}