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

      PartnerField_Normal.CommandClick += PartnerField_CommandClick;
      PartnerField_Normal.MenuItemClick += MenuItemClickHandler;
      PartnerField_Normal_AlternativeRendering.CommandClick += PartnerField_CommandClick;
      PartnerField_Normal_AlternativeRendering.MenuItemClick += MenuItemClickHandler;
      PartnerField_ReadOnly.CommandClick += PartnerField_CommandClick;
      PartnerField_ReadOnly.MenuItemClick += MenuItemClickHandler;
      PartnerField_ReadOnly_AlternativeRendering.CommandClick += PartnerField_CommandClick;
      PartnerField_ReadOnly_AlternativeRendering.MenuItemClick += MenuItemClickHandler;
      PartnerField_Disabled.CommandClick += PartnerField_CommandClick;
      PartnerField_Disabled.MenuItemClick += MenuItemClickHandler;
      PartnerField_NoAutoPostBack.CommandClick += PartnerField_CommandClick;
      PartnerField_NoAutoPostBack.MenuItemClick += MenuItemClickHandler;
      PartnerField_NoCommandNoMenu.CommandClick += PartnerField_CommandClick;
      PartnerField_NoCommandNoMenu.MenuItemClick += MenuItemClickHandler;
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      var masterPage = ((Layout) Page.Master);
      masterPage.SetBOUINormal (PartnerField_Normal.BusinessObjectUniqueIdentifier);
      masterPage.SetBOUINoAutoPostBack (PartnerField_NoAutoPostBack.BusinessObjectUniqueIdentifier);
    }

    private void PartnerField_CommandClick (object sender, BocCommandClickEventArgs e)
    {
      var masterPage = ((Layout) Page.Master);
      masterPage.SetActionPerformed ("CommandClick", "", e.Command.OwnerControl.ID);
    }

    private void MenuItemClickHandler (object sender, WebMenuItemClickEventArgs e)
    {
      var masterPage = ((Layout) Page.Master);
      masterPage.SetActionPerformed ("MenuItemClick", e.Item.Text, e.Command.OwnerControl.ID);
    }
  }
}