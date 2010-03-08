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
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditUserControlResources))]
  public partial class EditUserControl : BaseControl
  {
    private bool _isNewSubstitution;

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new EditUserFormFunction CurrentFunction
    {
      get { return (EditUserFormFunction) base.CurrentFunction; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return UserNameField; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      Page.RegisterRequiresControlState (this);

      SubstitutedByList.EditModeControlFactory = new EditableRowAutoCompleteControlFactory();
      var editModeColumn = (BocRowEditModeColumnDefinition) SubstitutedByList.FixedColumns[0];
      editModeColumn.EditIcon = GetIcon ("EditItem.gif", GlobalResources.Edit);
      editModeColumn.SaveIcon = GetIcon ("SaveButton.gif", GlobalResources.Save);
      editModeColumn.CancelIcon = GetIcon ("CancelButton.gif", GlobalResources.Cancel);
    }

    private IconInfo GetIcon (string resourceUrl, string alternateText)
    {
      return new IconInfo (
          ResourceUrlResolver.GetResourceUrl (this, typeof (EditUserControl), ResourceType.Image, ResourceTheme, resourceUrl))
             { AlternateText = alternateText };
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        RolesList.SetSortingOrder (
            new BocListSortingOrderEntry ((IBocSortableColumnDefinition) RolesList.FixedColumns[0], SortingDirection.Ascending),
            new BocListSortingOrderEntry ((IBocSortableColumnDefinition) RolesList.FixedColumns[1], SortingDirection.Ascending));
      }

      if (RolesList.IsReadOnly)
        RolesList.Selection = RowSelection.Disabled;
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }

    protected void RolesList_MenuItemClick (object sender, WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditRole (null, CurrentFunction.User, null, null);
        }
        else
        {
          EditRoleFormFunction returningFunction = (EditRoleFormFunction) Page.ReturningFunction;

          RolesList.LoadValue (!returningFunction.HasUserCancelled);
          if (returningFunction.HasUserCancelled)
            returningFunction.Role.Delete ();
          else
            RolesList.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "EditItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditRole ((Role) RolesList.GetSelectedBusinessObjects ()[0], CurrentFunction.User, null, null);
        }
        else
        {
          EditRoleFormFunction returningFunction = (EditRoleFormFunction) Page.ReturningFunction;

          if (!returningFunction.HasUserCancelled)
            RolesList.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "DeleteItem")
      {
        foreach (Role role in RolesList.GetSelectedBusinessObjects ())
        {
          RolesList.RemoveRow (role);
          role.Delete ();
        }
      }

      RolesList.ClearSelectedRows ();
    }

    private void EditRole (Role role, User user, Group group, Position position)
    {
      EditRoleFormFunction editRoleFormFunction = new EditRoleFormFunction (WxeTransactionMode.None, (role != null) ? role.ID : null, user, group);
      Page.ExecuteFunction (editRoleFormFunction, WxeCallArguments.Default);
    }

    protected void SubstitutedByList_MenuItemClick (object sender, WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        SubstitutedByList.AddAndEditRow (Substitution.NewObject());
        _isNewSubstitution = true;
      }

      if (e.Item.ItemID == "DeleteItem")
      {
        foreach (Role role in SubstitutedByList.GetSelectedBusinessObjects())
        {
          SubstitutedByList.RemoveRow (role);
          role.Delete();
        }
      }

      SubstitutedByList.ClearSelectedRows();
    }

    protected void SubstitutedByList_EditableRowChangesCanceled (object sender, BocListItemEventArgs e)
    {
      Substitution substitution = (Substitution) e.BusinessObject;
      if (_isNewSubstitution)
        substitution.Delete();
      _isNewSubstitution = false;
    }

    protected void SubstitutedByList_EditableRowChangesSaved (object sender, BocListItemEventArgs e)
    {
      _isNewSubstitution = false;
    }

    protected override void LoadControlState (object savedState)
    {
      object[] controlState = (object[])savedState;

      base.LoadControlState (controlState[0]);
      _isNewSubstitution = (bool) controlState[1];
    }

    protected override object SaveControlState ()
    {
      object[] controlState = new object[2];
      controlState[0] = base.SaveControlState ();
      controlState[1] = _isNewSubstitution;
      
      return controlState;
    }
  }
}
