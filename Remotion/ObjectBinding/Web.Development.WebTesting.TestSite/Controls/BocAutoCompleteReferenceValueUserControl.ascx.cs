using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocAutoCompleteReferenceValueUserControl : DataEditUserControl
  {
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      PartnerField.CommandClick += PartnerField_CommandClick;
      PartnerField.MenuItemClick += MenuItemClickHandler;
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      var masterPage = ((Layout) Page.Master);
      masterPage.AddTestOutput ("Current BOUI: " + PartnerField.BusinessObjectUniqueIdentifier);
    }

    void PartnerField_CommandClick (object sender, BocCommandClickEventArgs e)
    {
      var masterPage = ((Layout) Page.Master);
      masterPage.AddTestOutput ("CommandClick");
    }

    private void MenuItemClickHandler (object sender, WebMenuItemClickEventArgs e)
    {
      var masterPage = ((Layout) Page.Master);
      masterPage.AddTestOutput ("MenuItemClick: " + e.Item.Text);
    }
  }
}