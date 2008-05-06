using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (GroupListControlResources))]
  public partial class GroupListControl : BaseControl
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

    protected new GroupListFormFunction CurrentFunction
    {
      get { return (GroupListFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
        GroupList.SetSortingOrder (new BocListSortingOrderEntry ((IBocSortableColumnDefinition) GroupList.FixedColumns[0], SortingDirection.Ascending));
      GroupList.LoadUnboundValue (Group.FindByTenantID (CurrentTenantID), IsPostBack);

      if (!SecurityConfiguration.Current.SecurityProvider.IsNull)
      {
        SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
        Type groupType = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.GetGroupType ();
        NewGroupButton.Visible = securityClient.HasConstructorAccess (groupType);
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (HasTenantChanged)
        GroupList.LoadUnboundValue (Group.FindByTenantID (CurrentTenantID), false);
    }

    protected void GroupList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditGroupFormFunction editGroupFormFunction = new EditGroupFormFunction (((Group) e.BusinessObject).ID);
        editGroupFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editGroupFormFunction);
      }
      else
      {
        if (!((EditGroupFormFunction) Page.ReturningFunction).HasUserCancelled)
          GroupList.LoadUnboundValue (Group.FindByTenantID (CurrentFunction.TenantID), false);
      }
    }

    protected void NewGroupButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditGroupFormFunction editGroupFormFunction = new EditGroupFormFunction (null);
        editGroupFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editGroupFormFunction);
      }
      else
      {
        if (!((EditGroupFormFunction) Page.ReturningFunction).HasUserCancelled)
          GroupList.LoadUnboundValue (Group.FindByTenantID (CurrentFunction.TenantID), false);
      }
    }
  }
}
