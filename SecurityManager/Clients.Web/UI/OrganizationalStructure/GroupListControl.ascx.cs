// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
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
  public partial class GroupListControl : BaseListControl
  {
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        GroupList.SetSortingOrder (
            new BocListSortingOrderEntry ((IBocSortableColumnDefinition) GroupList.FixedColumns[0], SortingDirection.Ascending));
      }
      GroupList.LoadUnboundValue (GetValues(), IsPostBack);

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      Type groupType = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.GetGroupType ();
      NewGroupButton.Visible = securityClient.HasConstructorAccess (groupType);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      ResetListOnTenantChange (GroupList);
    }

    protected void GroupList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      HandleEditItemClick (GroupList, e);
    }

    protected void NewGroupButton_Click (object sender, EventArgs e)
    {
      HandleNewButtonClick (GroupList);
    }

    protected override IList GetValues ()
    {
      return Group.FindByTenant (CurrentFunction.TenantID.GetHandle<Tenant>()).ToArray();
    }

    protected override FormFunction CreateEditFunction (ITransactionMode transactionMode, ObjectID objectID)
    {
      return new EditGroupFormFunction (transactionMode, objectID);
    }
  }
}
