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
using System.Web.UI.WebControls;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes.AccessControl;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (AccessControlResources))]
  public partial class EditStatefulAccessControlListControl : EditAccessControlListControlBase<StatefulAccessControlList>
  {
    // types

    // static members and constants

    // member fields
    
    private readonly List<EditStateCombinationControl> _editStateCombinationControls = new List<EditStateCombinationControl> ();
    private bool _isCreatingNewStateCombination;

    // construction and disposing

    // methods and properties

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override ControlCollection GetAccessControlEntryControls ()
    {
      return AccessControlEntryControls.Controls;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      EnableNewStateCombinationButton ();
    }

    private void EnableNewStateCombinationButton ()
    {
      DomainObjectCollection stateProperties = CurrentAccessControlList.Class.StateProperties;
      if (stateProperties.Count > 1)
        throw new NotSupportedException ("Only classes with a zero or one StatePropertyDefinition are supported.");

      int possibleStateCombinations = 1;
      if (stateProperties.Count > 0)
        possibleStateCombinations = ((StatePropertyDefinition) stateProperties[0]).DefinedStates.Count;
      NewStateCombinationButton.Enabled = CurrentAccessControlList.Class.StateCombinations.Count < possibleStateCombinations;
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      LoadStateCombinations (interim);
    }

    private void LoadStateCombinations (bool interim)
    {
      CreateEditStateCombinationControls (CurrentAccessControlList.StateCombinations);
      foreach (var control in _editStateCombinationControls)
        control.LoadValues (interim);
    }

    private void CreateEditStateCombinationControls (ObjectList<StateCombination> stateCombinations)
    {
      StateCombinationControls.Controls.Clear ();
      _editStateCombinationControls.Clear ();

      for (int i = 0; i < stateCombinations.Count; i++)
      {
        StateCombination stateCombination = stateCombinations[i];

        EditStateCombinationControl editStateCombinationControl = (EditStateCombinationControl) LoadControl ("EditStateCombinationControl.ascx");
        editStateCombinationControl.ID = "SC_" + i;
        editStateCombinationControl.BusinessObject = stateCombination;
        editStateCombinationControl.Delete += EditStateCombinationControl_Delete;

        StateCombinationControls.Controls.Add (editStateCombinationControl);

        _editStateCombinationControls.Add (editStateCombinationControl);
      }
    }

    public override void SaveValues (bool interim)
    {
      base.SaveValues (interim);

      SaveStateCombinations (interim);
    }

    private void SaveStateCombinations (bool interim)
    {
      foreach (EditStateCombinationControl control in _editStateCombinationControls)
        control.SaveValues (interim);
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= ValidateStateCombinations ();

      return isValid;
    }

    private bool ValidateStateCombinations (params EditStateCombinationControl[] excludedControls)
    {
      List<EditStateCombinationControl> excludedControlList = new List<EditStateCombinationControl> (excludedControls);

      bool isValid = true;
      foreach (EditStateCombinationControl control in _editStateCombinationControls)
      {
        if (!excludedControlList.Contains (control))
          isValid &= control.Validate ();
      }

      if (!_isCreatingNewStateCombination)
      {
        MissingStateCombinationsValidator.Validate ();
        isValid &= MissingStateCombinationsValidator.IsValid;
      }

      return isValid;
    }

    protected void MissingStateCombinationsValidator_ServerValidate (object source, ServerValidateEventArgs args)
    {
      args.IsValid = CurrentAccessControlList.StateCombinations.Count > 0;
    }

    protected void NewStateCombinationButton_Click (object sender, EventArgs e)
    {
      _isCreatingNewStateCombination = true;
      Page.PrepareValidation ();
      bool isValid = Validate ();
      if (!isValid)
      {
        return;
      }
      SaveValues (false);
      Page.IsDirty = true;

      CurrentAccessControlList.CreateStateCombination ();

      LoadStateCombinations (false);
      _isCreatingNewStateCombination = false;
    }

    void EditStateCombinationControl_Delete (object sender, EventArgs e)
    {
      EditStateCombinationControl editStateCombinationControl = (EditStateCombinationControl) sender;
      Page.PrepareValidation ();
      bool isValid = ValidateStateCombinations (editStateCombinationControl);
      if (!isValid)
        return;

      _editStateCombinationControls.Remove (editStateCombinationControl);
      StateCombination accessControlEntry = (StateCombination) editStateCombinationControl.DataSource.BusinessObject;
      accessControlEntry.Delete ();

      SaveStateCombinations (false);
      Page.IsDirty = true;

      LoadStateCombinations (false);
    }
  }
}
