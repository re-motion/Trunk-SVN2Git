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

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      SetTestOutput();
    }

    private void PartnerField_CommandClick (object sender, BocCommandClickEventArgs e)
    {
      TestOutput.SetActionPerformed ("CommandClick", "", e.Command.OwnerControl.ID);
    }

    private void MenuItemClickHandler (object sender, WebMenuItemClickEventArgs e)
    {
      TestOutput.SetActionPerformed ("MenuItemClick", e.Item.ItemID + "|" + e.Item.Text, e.Command.OwnerControl.ID);
    }

    private void SetTestOutput ()
    {
      TestOutput.SetBOUINormal (PartnerField_Normal.BusinessObjectUniqueIdentifier);
      TestOutput.SetBOUINoAutoPostBack (PartnerField_NoAutoPostBack.BusinessObjectUniqueIdentifier);
    }

    private BocAutoCompleteReferenceValueUserControlTestOutput TestOutput
    {
      get { return ((Layout) Page.Master).GetTestOutputControl<BocAutoCompleteReferenceValueUserControlTestOutput>(); }
    }
  }
}