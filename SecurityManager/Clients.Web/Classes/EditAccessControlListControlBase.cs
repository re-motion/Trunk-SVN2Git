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
using Remotion.SecurityManager.Clients.Web.UI.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  public abstract class EditAccessControlListControlBase : BaseControl
  {
    private static readonly object s_deleteEvent = new object ();

    public event EventHandler Delete
    {
      add { Events.AddHandler (s_deleteEvent, value); }
      remove { Events.RemoveHandler (s_deleteEvent, value); }
    }

    protected void DeleteAccessControlListButton_Click (object sender, EventArgs e)
    {
      EventHandler handler = (EventHandler) Events[s_deleteEvent];
      if (handler != null)
        handler (this, e);
    }
  }

  public abstract class EditAccessControlListControlBase<TAccessControlList> : EditAccessControlListControlBase
      where TAccessControlList : AccessControlList
  {
    private readonly List<EditAccessControlEntryControl> _editAccessControlEntryControls = new List<EditAccessControlEntryControl> ();

    protected abstract ControlCollection GetAccessControlEntryControls ();

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      LoadAccessControlEntries (interim);
    }

    protected TAccessControlList CurrentAccessControlList
    {
      get { return (TAccessControlList) DataSource.BusinessObject; }
    }

    private void LoadAccessControlEntries (bool interim)
    {
      CreateEditAccessControlEntryControls (CurrentAccessControlList.AccessControlEntries);
      foreach (EditAccessControlEntryControl control in _editAccessControlEntryControls)
        control.LoadValues (interim);
    }

    private void CreateEditAccessControlEntryControls (DomainObjectCollection accessControlEntries)
    {
      ControlCollection accessControlEntryControls = GetAccessControlEntryControls();
      accessControlEntryControls.Clear ();
      _editAccessControlEntryControls.Clear ();

      for (int i = 0; i < accessControlEntries.Count; i++)
      {
        AccessControlEntry accessControlEntry = (AccessControlEntry) accessControlEntries[i];

        EditAccessControlEntryControl editAccessControlEntryControl = (EditAccessControlEntryControl) LoadControl ("EditAccessControlEntryControl.ascx");
        editAccessControlEntryControl.ID = "Ace_" + i;
        editAccessControlEntryControl.BusinessObject = accessControlEntry;
        editAccessControlEntryControl.Delete += EditAccessControlEntryControl_Delete;

        accessControlEntryControls.Add (editAccessControlEntryControl);

        _editAccessControlEntryControls.Add (editAccessControlEntryControl);
      }
    }

    public override void SaveValues (bool interim)
    {
      base.SaveValues (interim);

      SaveAccessControlEntries (interim);
    }

    private void SaveAccessControlEntries (bool interim)
    {
      foreach (EditAccessControlEntryControl control in _editAccessControlEntryControls)
        control.SaveValues (interim);
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= ValidateAccessControlEntries ();

      return isValid;
    }

    private bool ValidateAccessControlEntries (params EditAccessControlEntryControl[] excludedControls)
    {
      List<EditAccessControlEntryControl> excludedControlList = new List<EditAccessControlEntryControl> (excludedControls);

      bool isValid = true;
      foreach (EditAccessControlEntryControl control in _editAccessControlEntryControls)
      {
        if (!excludedControlList.Contains (control))
          isValid &= control.Validate ();
      }

      return isValid;
    }

    protected void NewAccessControlEntryButton_Click (object sender, EventArgs e)
    {
      Page.PrepareValidation ();
      bool isValid = Validate ();
      if (!isValid)
      {
        return;
      }
      SaveValues (false);
      Page.IsDirty = true;

      CurrentAccessControlList.CreateAccessControlEntry ();

      LoadAccessControlEntries (false);
    }

    private void EditAccessControlEntryControl_Delete (object sender, EventArgs e)
    {
      EditAccessControlEntryControl editAccessControlEntryControl = (EditAccessControlEntryControl) sender;
      Page.PrepareValidation ();
      bool isValid = ValidateAccessControlEntries (editAccessControlEntryControl);
      if (!isValid)
        return;

      _editAccessControlEntryControls.Remove (editAccessControlEntryControl);
      AccessControlEntry accessControlEntry = (AccessControlEntry) editAccessControlEntryControl.DataSource.BusinessObject;
      accessControlEntry.Delete ();

      SaveAccessControlEntries (false);
      Page.IsDirty = true;

      LoadAccessControlEntries (false);
    }
  }
}