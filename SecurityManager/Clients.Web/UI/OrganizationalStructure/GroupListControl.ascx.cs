// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
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
        EditGroupFormFunction editGroupFormFunction = new EditGroupFormFunction (WxeTransactionMode.None, ((Group) e.BusinessObject).ID);
        Page.ExecuteFunction (editGroupFormFunction, WxeCallArguments.Default);
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
        EditGroupFormFunction editGroupFormFunction = new EditGroupFormFunction (WxeTransactionMode.None, null);
        Page.ExecuteFunction (editGroupFormFunction, WxeCallArguments.Default);
      }
      else
      {
        if (!((EditGroupFormFunction) Page.ReturningFunction).HasUserCancelled)
          GroupList.LoadUnboundValue (Group.FindByTenantID (CurrentFunction.TenantID), false);
      }
    }
  }
}
