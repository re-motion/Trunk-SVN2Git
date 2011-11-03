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
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditPositionControlResources))]
  public partial class EditPositionControl : BaseControl
  {
    private BocListInlineEditingManager<GroupTypePosition> _groupTypesListInlineEditingManager;


    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return NameField; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      Page.RegisterRequiresControlState (this);

      _groupTypesListInlineEditingManager =
          new BocListInlineEditingManager<GroupTypePosition> (GroupTypesList, GroupTypePosition.NewObject, ResourceUrlFactory);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        GroupTypesList.SetSortingOrder (
            new BocListSortingOrderEntry ((IBocSortableColumnDefinition) GroupTypesList.FixedColumns.Find ("GroupType"), SortingDirection.Ascending));
      }

      if (GroupTypesList.IsReadOnly)
        GroupTypesList.Selection = RowSelection.Disabled;
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate();

      isValid &= FormGridManager.Validate();

      return isValid;
    }

    protected override void LoadControlState (object savedState)
    {
      object[] controlState = (object[]) savedState;

      base.LoadControlState (controlState[0]);
      _groupTypesListInlineEditingManager.LoadControlState (controlState[1]);
    }

    protected override object SaveControlState ()
    {
      object[] controlState = new object[2];
      controlState[0] = base.SaveControlState();
      controlState[1] = _groupTypesListInlineEditingManager.SaveControlState();

      return controlState;
    }
  }
}