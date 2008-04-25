using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (SearchGroupControlResources))]
  public partial class SearchGroupControl : BaseControl
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new SearchGroupFormFunction CurrentFunction
    {
      get { return (SearchGroupFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      GroupList.LoadUnboundValue (Group.FindByTenantID (CurrentTenantID), false);
    }

    protected void GroupList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (e.Column.ItemID == "ApplyItem")
      {
        CurrentFunction.SelectedGroup = (Group) e.BusinessObject;
        Page.ExecuteNextStep ();
      }
    }
  }
}