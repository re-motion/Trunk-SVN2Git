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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

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

    protected bool IsCollapsed
    {
      get { return (bool?) ViewState["IsCollapsed"] ?? CurrentAccessControlEntry.State != StateType.New; }
      set { ViewState["IsCollapsed"] = value; }
    }

    protected AccessControlEntry CurrentAccessControlEntry
    {
      get { return (AccessControlEntry) CurrentObject.BusinessObject; }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      CollapsedView.Visible = IsCollapsed;
      ExpandedView.Visible = !IsCollapsed;

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

      DeleteAccessControlEntryButton.Icon = new IconInfo (GetIconUrl ("DeleteItem.gif"));
      DeleteAccessControlEntryButton.Icon.AlternateText = AccessControlResources.DeleteAccessControlEntryButton_Text;

      CollapseAccessControlEntryButton.Icon.Url = GetIconUrl ("Collapse.gif");
      CollapseAccessControlEntryButton.Icon.AlternateText = AccessControlResources.CollapseAccessControlEntryButton_Text;

      ExpandAccessControlEntryButton.Icon.Url = GetIconUrl ("Expand.gif");
      ExpandAccessControlEntryButton.Icon.AlternateText = AccessControlResources.ExpandAccessControlEntryButton_Text;
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      CreateCollapsedTenantInformation();
      CreateCollapsedGroupInformation();
      CreateCollapsedUserInformation();
      CreateCollapsedPermissionControls (CurrentAccessControlEntry.Permissions);

      ExpandedCell.Attributes.Add ("colspan", (6 + CurrentAccessControlEntry.AccessControlList.Class.AccessTypes.Count).ToString());

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

    private void CreateCollapsedTenantInformation ()
    {
      CollapsedTenantInformation.SetRenderMethodDelegate (
          delegate (HtmlTextWriter writer, Control container)
          {
            switch (CurrentAccessControlEntry.TenantCondition)
            {
              case TenantCondition.None:
                break;
              case TenantCondition.OwningTenant:
                RenderTenantHierarchyIcon (writer);
                RenderPropertyPathString (writer, "TenantCondition");
                break;
              case TenantCondition.SpecificTenant:
                RenderTenantHierarchyIcon (writer);
                RenderPropertyPathString (writer, "SpecificTenant.DisplayName");
                break;
              default:
                throw new ArgumentOutOfRangeException();
            }
          });
    }

    private void CreateCollapsedGroupInformation ()
    {
      CollapsedGroupInformation.SetRenderMethodDelegate (
          delegate (HtmlTextWriter writer, Control container)
          {
            switch (CurrentAccessControlEntry.GroupCondition)
            {
              case GroupCondition.None:
                break;
              case GroupCondition.OwningGroup:
                RenderGroupHierarchyIcon (writer);
                writer.RenderBeginTag (HtmlTextWriterTag.Em);
                RenderPropertyPathString (writer, "GroupCondition");
                writer.RenderEndTag();
                break;
              case GroupCondition.SpecificGroup:
                RenderGroupHierarchyIcon (writer);
                writer.Write (HtmlUtility.HtmlEncode ("Group"));
                writer.Write (" ");
                writer.RenderBeginTag (HtmlTextWriterTag.Em);
                RenderPropertyPathString (writer, "SpecificGroup.ShortName");
                writer.RenderEndTag();
                break;
              case GroupCondition.BranchOfOwningGroup:
                writer.Write (HtmlUtility.HtmlEncode ("Same"));
                writer.Write (" ");
                writer.RenderBeginTag (HtmlTextWriterTag.Em);
                RenderPropertyPathString (writer, "SpecificGroupType.DisplayName");
                writer.RenderEndTag();
                break;
              case GroupCondition.AnyGroupWithSpecificGroupType:
                writer.Write (HtmlUtility.HtmlEncode ("GT"));
                writer.Write (" ");
                writer.RenderBeginTag (HtmlTextWriterTag.Em);
                RenderPropertyPathString (writer, "SpecificGroupType.DisplayName");
                writer.RenderEndTag();
                break;
              default:
                throw new ArgumentOutOfRangeException();
            }
          });
    }

    private void CreateCollapsedUserInformation ()
    {
      CollapsedUserInformation.SetRenderMethodDelegate (
          delegate (HtmlTextWriter writer, Control container)
          {
            switch (CurrentAccessControlEntry.UserCondition)
            {
              case UserCondition.None:
                break;
              case UserCondition.Owner:
                RenderPropertyPathString (writer, "UserCondition");
                break;
              case UserCondition.SpecificUser:
                RenderPropertyPathString (writer, "SpecificUser.DisplayName");
                break;
              case UserCondition.SpecificPosition:
                RenderPropertyPathString (writer, "SpecificPosition.DisplayName");
                break;
              default:
                throw new ArgumentOutOfRangeException();
            }
          });
    }

    private void CreateCollapsedPermissionControls (ObjectList<Permission> permissions)
    {
      var grantedIcon = new IconInfo (GetIconUrl ("PermissionGranted.gif"));
      var deniedIcon = new IconInfo (GetIconUrl ("PermissionDenied.gif"));
      var undefinedIcon = new IconInfo (GetIconUrl ("PermissionUndefined.gif"));

      CollapsedPermissionCells.SetRenderMethodDelegate (
          delegate (HtmlTextWriter writer, Control container)
          {
            foreach (var permission in permissions)
            {
              writer.AddAttribute (HtmlTextWriterAttribute.Class, "permissionCell");
              writer.RenderBeginTag (HtmlTextWriterTag.Td);
              if (permission.Allowed.HasValue)
              {
                if (permission.Allowed.Value)
                  grantedIcon.Render (writer);
                else
                  deniedIcon.Render (writer);
              }
              else
                undefinedIcon.Render (writer);
              writer.RenderEndTag();
            }
          });
    }

    private void RenderPropertyPathString (HtmlTextWriter writer, string propertyPathIdentifier)
    {
      IBusinessObject businessObject = CurrentAccessControlEntry;
      var propertyPath = BusinessObjectPropertyPath.Parse (businessObject.BusinessObjectClass, propertyPathIdentifier);
      writer.Write (HtmlUtility.HtmlEncode (propertyPath.GetString (businessObject, null)));
    }

    private void RenderTenantHierarchyIcon (HtmlTextWriter writer)
    {
      string url;
      switch (CurrentAccessControlEntry.TenantHierarchyCondition)
      {
        case TenantHierarchyCondition.Undefined:
          throw new InvalidOperationException();
        case TenantHierarchyCondition.This:
          url = "HierarchyThis.gif";
          break;
        case TenantHierarchyCondition.Parent:
          throw new InvalidOperationException();
        case TenantHierarchyCondition.ThisAndParent:
          url = "HierarchyThisAndParent.gif";
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      var icon = new IconInfo (GetIconUrl (url));
      icon.Render (writer);
    }

    private void RenderGroupHierarchyIcon (HtmlTextWriter writer)
    {
      string url;
      switch (CurrentAccessControlEntry.GroupHierarchyCondition)
      {
        case GroupHierarchyCondition.Undefined:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.This:
          url = "HierarchyThis.gif";
          break;
        case GroupHierarchyCondition.Parent:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.Children:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.ThisAndParent:
          url = "HierarchyThisAndParent.gif";
          break;
        case GroupHierarchyCondition.ThisAndChildren:
          url = "HierarchyThisAndChildren.gif";
          break;
        case GroupHierarchyCondition.ThisAndParentAndChildren:
          url = "HierarchyThisAndParentAndChildren.gif";
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      var icon = new IconInfo (GetIconUrl (url));
      icon.Render (writer);
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

      var ul = new HtmlGenericControl ("ul");
      ul.Attributes.Add ("class", "permissionsList");
      PermissionsPlaceHolder.Controls.Add (ul);

      for (int i = 0; i < permissions.Count; i++)
      {
        var permission = permissions[i];

        var control = (EditPermissionControl) LoadControl ("EditPermissionControl.ascx");
        control.ID = "P_" + i;
        control.BusinessObject = permission;

        var li = new HtmlGenericControl ("li");
        li.Attributes.Add ("class", "permissionsList");
        ul.Controls.Add (li);
        li.Controls.Add (control);

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

    protected void AllPermissionsField_CheckedChange (object sender, EventArgs e)
    {
      bool? isAllowed = ((BocBooleanValue) sender).Value;
      foreach (var control in _editPermissionControls)
        control.SetPermissionValue (isAllowed);
    }

    protected void ExpandAccessControlEntryButton_Click (object sender, EventArgs e)
    {
      IsCollapsed = false;
    }

    protected void CollapseAccessControlEntryButton_Click (object sender, EventArgs e)
    {
      if (Validate())
      {
        SaveValues (false);
        LoadValues (false);
        IsCollapsed = true;
      }
    }

    private string GetIconUrl (string url)
    {
      return ResourceUrlResolver.GetResourceUrl (this, Context, typeof (EditAccessControlEntryControl), ResourceType.Image, url);
    }
  }
}