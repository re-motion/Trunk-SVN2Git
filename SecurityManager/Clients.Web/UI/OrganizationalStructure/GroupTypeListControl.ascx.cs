/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (GroupTypeListControlResources))]
  public partial class GroupTypeListControl : BaseControl
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

    protected new GroupTypeListFormFunction CurrentFunction
    {
      get { return (GroupTypeListFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
        GroupTypeList.SetSortingOrder (new BocListSortingOrderEntry ((IBocSortableColumnDefinition) GroupTypeList.FixedColumns[0], SortingDirection.Ascending));
      GroupTypeList.LoadUnboundValue (GroupType.FindAll (), false);

      if (SecurityConfiguration.Current.SecurityProvider != null)
      {
        SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
        NewGroupTypeButton.Visible = securityClient.HasConstructorAccess (typeof (GroupType));
      }
    }

    protected void GroupTypeList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditGroupTypeFormFunction editGroupTypeFormFunction = new EditGroupTypeFormFunction (WxeTransactionMode.None, ((GroupType) e.BusinessObject).ID);
        Page.ExecuteFunction (editGroupTypeFormFunction, WxeCallArguments.Default);
      }
      else
      {
        if (!((EditGroupTypeFormFunction) Page.ReturningFunction).HasUserCancelled)
          GroupTypeList.LoadUnboundValue (GroupType.FindAll (), false);
      }
    }

    protected void NewGroupTypeButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditGroupTypeFormFunction editGroupTypeFormFunction = new EditGroupTypeFormFunction (WxeTransactionMode.None, null);
        Page.ExecuteFunction (editGroupTypeFormFunction, WxeCallArguments.Default);
      }
      else
      {
        if (!((EditGroupTypeFormFunction) Page.ReturningFunction).HasUserCancelled)
          GroupTypeList.LoadUnboundValue (GroupType.FindAll (), false);
      }
    }
  }
}
