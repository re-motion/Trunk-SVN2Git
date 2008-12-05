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
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Classes.AccessControl;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (AccessControlResources))]
  public partial class EditAccessControlEntryControl : BaseControl
  {
    // types

    // static members and constants

    private static readonly object s_deleteEvent = new object();

    // member fields

    private readonly List<EditPermissionControl> _editPermissionControls = new List<EditPermissionControl>();
    private const string c_grantAllMenuItemID = "GrantAllPermissions";
    private const string c_denyAllMenuItemID = "DenyAllPermissions";
    private const string c_clearAllMenuItemID = "ClearAllPermissions";

    // construction and disposing

    // methods and properties

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    public event EventHandler Delete
    {
      add { Events.AddHandler (s_deleteEvent, value); }
      remove { Events.RemoveHandler (s_deleteEvent, value); }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public string CssClass { get; set; }

    protected bool IsCollapsed
    {
      get { return (bool?) ViewState["IsCollapsed"] ?? CurrentAccessControlEntry.State != StateType.New; }
      set { ViewState["IsCollapsed"] = value; }
    }

    protected AccessControlEntry CurrentAccessControlEntry
    {
      get { return (AccessControlEntry) CurrentObject.BusinessObject; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      AllPermisionsMenu.MenuItems.Add (
          new WebMenuItem
          {
              ItemID = c_clearAllMenuItemID,
              Text = AccessControlResources.AllPermissionsMenu_ClearAllPermissions_Text,
              Icon = new IconInfo (GetIconUrl ("PermissionUndefined.gif"))
          });
      AllPermisionsMenu.MenuItems.Add (
          new WebMenuItem
          {
              ItemID = c_grantAllMenuItemID,
              Text = AccessControlResources.AllPermissionsMenu_GrantAllPermissions_Text,
              Icon = new IconInfo (GetIconUrl ("PermissionGranted.gif"))
          });
      AllPermisionsMenu.MenuItems.Add (
          new WebMenuItem
          {
              ItemID = c_denyAllMenuItemID,
              Text = AccessControlResources.AllPermissionsMenu_DenyAllPermissions_Text,
              Icon = new IconInfo (GetIconUrl ("PermissionDenied.gif"))
          });
      AllPermisionsMenu.EventCommandClick += AllPermisionsMenu_EventCommandClick;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (string.IsNullOrEmpty (SpecificGroupField.ServicePath))
      {
        SpecificGroupField.ServicePath = ResourceUrlResolver.GetResourceUrl (
            this, Context, typeof (SecurityManagerSearchWebService), ResourceType.UI, "SecurityManagerSearchWebService.asmx");
        SpecificGroupField.ServiceMethod = "GetBusinessObjects";
      }

      if (string.IsNullOrEmpty (SpecificUserField.ServicePath))
      {
        SpecificUserField.ServicePath = ResourceUrlResolver.GetResourceUrl (
            this, Context, typeof (SecurityManagerSearchWebService), ResourceType.UI, "SecurityManagerSearchWebService.asmx");
        SpecificUserField.ServiceMethod = "GetBusinessObjects";
      }

      if (IsCollapsed)
      {
        var collapsedRenderer = new CollapsedAccessControlConditionsRenderer (CurrentAccessControlEntry);
        CollapsedTenantInformation.SetRenderMethodDelegate (collapsedRenderer.RenderTenant);
        CollapsedGroupInformation.SetRenderMethodDelegate (collapsedRenderer.RenderGroup);
        CollapsedUserInformation.SetRenderMethodDelegate (collapsedRenderer.RenderUser);
        CollapsedAbstractRoleInformation.SetRenderMethodDelegate (collapsedRenderer.RenderAbstractRole);
      }

      DetailsCell.Attributes.Add ("colspan", (4 + CurrentAccessControlEntry.AccessControlList.Class.AccessTypes.Count + 3).ToString());

      DeleteAccessControlEntryButton.Icon = new IconInfo (GetIconUrl ("DeleteItem.gif"));
      DeleteAccessControlEntryButton.Icon.AlternateText = AccessControlResources.DeleteAccessControlEntryButton_Text;

      if (IsCollapsed)
      {
        ToggleAccessControlEntryButton.Icon.Url = GetIconUrl ("Expand.gif");
        ToggleAccessControlEntryButton.Icon.AlternateText = AccessControlResources.ExpandAccessControlEntryButton_Text;
        DetailsView.Visible = false;
      }
      else
      {
        ToggleAccessControlEntryButton.Icon.Url = GetIconUrl ("Collapse.gif");
        ToggleAccessControlEntryButton.Icon.AlternateText = AccessControlResources.CollapseAccessControlEntryButton_Text;
        DetailsView.Visible = true;
      }
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      LoadPermissions (interim);
      AdjustSpecificTenantField (false);
      AdjustTenantHierarchyConditionField (false);
      AdjustSpecificGroupField (false);
      AdjustGroupHierarchyConditionField (false);
      AdjustSpecificGroupTypeField();
      AdjustSpecificUserField (false);
      AdjustSpecificPositionField();
      AdjustSpecificAbstractRoleField();
    }

    public override void SaveValues (bool interim)
    {
      using (new SecurityFreeSection())
      {
        base.SaveValues (interim);
      }

      SavePermissions (interim);
    }

    private void SavePermissions (bool interim)
    {
      foreach (EditPermissionControl control in _editPermissionControls)
        control.SaveValues (interim);
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate();

      isValid &= FormGridManager.Validate();
      isValid &= ValidatePermissions();

      return isValid;
    }

    protected void DeleteAccessControlEntryButton_Click (object sender, EventArgs e)
    {
      EventHandler handler = (EventHandler) Events[s_deleteEvent];
      if (handler != null)
        handler (this, e);
    }

    protected void TenantConditionField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificTenantField (true);
      AdjustTenantHierarchyConditionField (true);
    }

    protected void SpecificTenantField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificGroupField (true);
    }

    protected void GroupConditionField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificGroupField (false);
      AdjustGroupHierarchyConditionField (true);
      AdjustSpecificGroupTypeField();
    }

    protected void UserConditionField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificUserField (false);
      AdjustSpecificPositionField();
    }

    private void AdjustSpecificTenantField (bool hasTenantConditionChanged)
    {
      var isSpecifciTenantSelected = (TenantCondition?) TenantConditionField.Value == TenantCondition.SpecificTenant;
      if (hasTenantConditionChanged && !isSpecifciTenantSelected)
        SpecificTenantField.Value = null;
      SpecificTenantField.Visible = isSpecifciTenantSelected;
    }

    private void AdjustTenantHierarchyConditionField (bool hasTenantConditionChanged)
    {
      bool isSpecificTenantSelected = (TenantCondition?) TenantConditionField.Value == TenantCondition.SpecificTenant;
      bool isOwningTenantSelected = (TenantCondition?) TenantConditionField.Value == TenantCondition.OwningTenant;

      if (hasTenantConditionChanged)
      {
        if (isOwningTenantSelected)
          TenantHierarchyConditionField.Value = TenantHierarchyCondition.ThisAndParent;
        else if (isSpecificTenantSelected)
          TenantHierarchyConditionField.Value = TenantHierarchyCondition.This;
        else
          TenantHierarchyConditionField.Value = TenantHierarchyCondition.Undefined;
      }

      TenantHierarchyConditionField.Visible = isSpecificTenantSelected || isOwningTenantSelected;
    }

    private void AdjustSpecificGroupField (bool resetValue)
    {
      if (resetValue)
        SpecificGroupField.Value = null;

      if (CurrentFunction.TenantID == null)
        throw new InvalidOperationException ("No current tenant has been set. Possible reason: session timeout");
      SpecificGroupField.Args = SpecificTenantField.BusinessObjectID ?? CurrentFunction.TenantID.ToString();

      SpecificGroupField.Visible = (GroupCondition?) GroupConditionField.Value == GroupCondition.SpecificGroup;
    }

    private void AdjustGroupHierarchyConditionField (bool hasGroupConditionChanged)
    {
      bool isSpecificGroupSelected = (GroupCondition?) GroupConditionField.Value == GroupCondition.SpecificGroup;
      bool isOwningGroupSelected = (GroupCondition?) GroupConditionField.Value == GroupCondition.OwningGroup;

      if (hasGroupConditionChanged)
      {
        if (isSpecificGroupSelected || isOwningGroupSelected)
          GroupHierarchyConditionField.Value = GroupHierarchyCondition.ThisAndParent;
        else
          GroupHierarchyConditionField.Value = GroupHierarchyCondition.Undefined;
      }

      GroupHierarchyConditionField.Visible = isSpecificGroupSelected || isOwningGroupSelected;
    }

    private void AdjustSpecificGroupTypeField ()
    {
      bool isSpecificGroupTypeSelected = (GroupCondition?) GroupConditionField.Value == GroupCondition.AnyGroupWithSpecificGroupType;
      bool isBranchOfOwningGroupSelected = (GroupCondition?) GroupConditionField.Value == GroupCondition.BranchOfOwningGroup;
      SpecificGroupTypeField.Visible = isSpecificGroupTypeSelected || isBranchOfOwningGroupSelected;
    }

    private void AdjustSpecificUserField (bool resetValue)
    {
      if (resetValue)
        SpecificUserField.Value = null;

      if (CurrentFunction.TenantID == null)
        throw new InvalidOperationException ("No current tenant has been set. Possible reason: session timeout");
      SpecificUserField.Args = SpecificTenantField.BusinessObjectID ?? CurrentFunction.TenantID.ToString();

      SpecificUserField.Visible = (UserCondition?) UserConditionField.Value == UserCondition.SpecificUser;
    }

    private void AdjustSpecificPositionField ()
    {
      bool isPositionSelected = (UserCondition?) UserConditionField.Value == UserCondition.SpecificPosition;
      SpecificPositionField.Visible = isPositionSelected;
    }

    private void AdjustSpecificAbstractRoleField ()
    {
      SpecificAbstractRoleField.Visible = CurrentAccessControlEntry.AccessControlList is StatefulAccessControlList;
    }

    private void LoadPermissions (bool interim)
    {
      CreateEditPermissionControls (CurrentAccessControlEntry.Permissions);
      foreach (var control in _editPermissionControls)
        control.LoadValues (interim);
    }

    private void CreateEditPermissionControls (ObjectList<Permission> permissions)
    {
      PermissionsPlaceHolder.Controls.Clear();
      _editPermissionControls.Clear();

      for (int i = 0; i < permissions.Count; i++)
      {
        var permission = permissions[i];

        var control = (EditPermissionControl) LoadControl ("EditPermissionControl.ascx");
        control.ID = "P_" + i;
        control.BusinessObject = permission;

        var td = new HtmlGenericControl ("td");
        td.Attributes.Add ("class", "permissionCell");
        PermissionsPlaceHolder.Controls.Add (td);
        td.Controls.Add (control);

        _editPermissionControls.Add (control);
      }
    }

    private bool ValidatePermissions ()
    {
      bool isValid = true;
      foreach (var control in _editPermissionControls)
        isValid &= control.Validate();

      return isValid;
    }

    private void AllPermisionsMenu_EventCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      bool? isAllowed;
      switch (e.Item.ItemID)
      {
        case c_grantAllMenuItemID:
          isAllowed = true;
          break;
        case c_denyAllMenuItemID:
          isAllowed = false;
          break;
        case c_clearAllMenuItemID:
          isAllowed = null;
          break;
        default:
          throw new InvalidOperationException (string.Format ("The menu item '{0}' is not defined.", e.Item.ItemID));
      }

      foreach (var control in _editPermissionControls)
        control.SetPermissionValue (isAllowed);
    }

    protected void ToggleAccessControlEntryButton_Click (object sender, EventArgs e)
    {
      if (IsCollapsed)
      {
        IsCollapsed = false;
      }
      else if (Validate())
      {
        SaveValues (false);
        LoadValues (false);
        IsCollapsed = true;
      }

    }

    private string GetIconUrl (string url)
    {
      return ResourceUrlResolver.GetResourceUrl (this, typeof (EditAccessControlEntryControl), ResourceType.Image, url);
    }
  }
}
