using System;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (UserListControlResources))]
  public partial class UserListControl : BaseControl
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

    protected new UserListFormFunction CurrentFunction
    {
      get { return (UserListFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
        UserList.SetSortingOrder (new BocListSortingOrderEntry ((IBocSortableColumnDefinition) UserList.FixedColumns[0], SortingDirection.Ascending));
      UserList.LoadUnboundValue (User.FindByTenantID (CurrentTenantID), IsPostBack);

      if (!SecurityConfiguration.Current.SecurityProvider.IsNull)
      {
        SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
        Type userType = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.GetUserType ();
        NewUserButton.Visible = securityClient.HasConstructorAccess (userType);
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (HasTenantChanged)
        UserList.LoadUnboundValue (User.FindByTenantID (CurrentTenantID), false);
    }

    protected void UserList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditUserFormFunction editUserFormFunction = new EditUserFormFunction (((User) e.BusinessObject).ID);
        editUserFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editUserFormFunction);
      }
      else
      {
        if (!((EditUserFormFunction) Page.ReturningFunction).HasUserCancelled)
          UserList.LoadUnboundValue (User.FindByTenantID (CurrentFunction.TenantID), false);
      }
    }

    protected void NewUserButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditUserFormFunction editUserFormFunction = new EditUserFormFunction (null);
        editUserFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editUserFormFunction);
      }
      else
      {
        if (!((EditUserFormFunction) Page.ReturningFunction).HasUserCancelled)
          UserList.LoadUnboundValue (User.FindByTenantID (CurrentFunction.TenantID), false);
      }
    }
  }
}
