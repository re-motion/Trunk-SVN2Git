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
using System.Web.UI.HtmlControls;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (AccessControlResources))]
  public partial class EditAccessControlEntryControl : BaseControl
  {
    // types

    // static members and constants

    private static readonly object s_deleteEvent = new object ();

    // member fields

    private List<EditPermissionControl> _editPermissionControls = new List<EditPermissionControl> ();

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

    protected AccessControlEntry CurrentAccessControlEntry
    {
      get { return (AccessControlEntry) CurrentObject.BusinessObject; }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      SpecificPositionAndGroupLinkingLabel.Text = AccessControlResources.SpecificPositionAndGroupLinkingLabelText;

      UpdateActualPriority ();
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      LoadPermissions (interim);
      AdjustSpecificTenantField ();
      AdjustPositionFields ();
    }

    public override void SaveValues (bool interim)
    {
      using (new SecurityFreeSection ())
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
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();
      isValid &= ValidatePermissions ();

      return isValid;
    }

    protected void DeleteAccessControlEntryButton_Click (object sender, EventArgs e)
    {
      EventHandler handler = (EventHandler) Events[s_deleteEvent];
      if (handler != null)
        handler (this, e);
    }

    protected void TenantField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificTenantField ();
    }

    protected void SpecificPositionField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustPositionFields ();
    }

    private void AdjustSpecificTenantField ()
    {
      if ((TenantSelection?) TenantSelectionField.Value == TenantSelection.SpecificTenant)
      {
        SpecificTenantField.Visible = true;
      }
      else
      {
        SpecificTenantField.Visible = false;
        SpecificTenantField.Value = null;
      }
    }

    private void AdjustPositionFields ()
    {
      if (SpecificPositionField.BusinessObjectID == null)
        CurrentAccessControlEntry.UserSelection = UserSelection.All;
      else
        CurrentAccessControlEntry.UserSelection = UserSelection.SpecificPosition;

      // TODO: Remove when Group can stand alone during ACE lookup.
      if (SpecificPositionField.BusinessObjectID == null)
      {
        SpecificPositionAndGroupLinkingLabel.Visible = false;
        GroupSelectionField.Visible = false;
        GroupSelectionField.Value = GroupSelection.All;
      }
      else
      {
        SpecificPositionAndGroupLinkingLabel.Visible = true;
        GroupSelectionField.Visible = true;
      }
    }

    private void UpdateActualPriority ()
    {
      foreach (IBusinessObjectBoundEditableWebControl control in CurrentObject.BoundControls)
      {
        if (control.IsDirty)
        {
          BocTextValue bocTextValue = control as BocTextValue;
          if (bocTextValue != null && !bocTextValue.IsValidValue)
            continue;

          using (new SecurityFreeSection ())
          {
            control.SaveValue (false);
          }
          control.IsDirty = true;
        }
      }

      ActualPriorityLabel.Text = string.Format (AccessControlResources.ActualPriorityLabelText, CurrentAccessControlEntry.ActualPriority);
    }

    private void LoadPermissions (bool interim)
    {
      CreateEditPermissionControls (CurrentAccessControlEntry.Permissions);
      foreach (EditPermissionControl control in _editPermissionControls)
        control.LoadValues (interim);
    }

    private void CreateEditPermissionControls (DomainObjectCollection permissions)
    {
      PermissionsPlaceHolder.Controls.Clear ();
      _editPermissionControls.Clear ();

      HtmlGenericControl ul = new HtmlGenericControl ("ul");
      ul.Attributes.Add ("class", "permissionsList");
      PermissionsPlaceHolder.Controls.Add (ul);

      for (int i = 0; i < permissions.Count; i++)
      {
        Permission permission = (Permission) permissions[i];

        EditPermissionControl control = (EditPermissionControl) LoadControl ("EditPermissionControl.ascx");
        control.ID = "P_" + i.ToString ();
        control.BusinessObject = permission;

        HtmlGenericControl li = new HtmlGenericControl ("li");
        li.Attributes.Add ("class", "permissionsList");
        ul.Controls.Add (li);
        li.Controls.Add (control);

        _editPermissionControls.Add (control);
      }
    }

    private bool ValidatePermissions ()
    {
      bool isValid = true;
      foreach (EditPermissionControl control in _editPermissionControls)
        isValid &= control.Validate ();

      return isValid;
    }
  }
}
