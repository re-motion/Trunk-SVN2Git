// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
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
using Remotion.SecurityManager.Clients.Web.WxeFunctions;

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
        EditUserFormFunction editUserFormFunction = new EditUserFormFunction (WxeTransactionMode.None, ((User) e.BusinessObject).ID);
        Page.ExecuteFunction (editUserFormFunction, WxeCallArguments.Default);
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
        EditUserFormFunction editUserFormFunction = new EditUserFormFunction (WxeTransactionMode.None, null);
        Page.ExecuteFunction (editUserFormFunction, WxeCallArguments.Default);
      }
      else
      {
        if (!((EditUserFormFunction) Page.ReturningFunction).HasUserCancelled)
          UserList.LoadUnboundValue (User.FindByTenantID (CurrentFunction.TenantID), false);
      }
    }
  }
}
