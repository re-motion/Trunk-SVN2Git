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
using System.Web.UI.WebControls;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Globalization;
using System.Linq;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (AccessControlResources))]
  public partial class EditPermissionsForm : BaseEditPage
  {

    // types

    // static members and constants

    // member fields

    private readonly List<EditAccessControlListControlBase> _editAccessControlListControls = new List<EditAccessControlListControlBase> ();

    // construction and disposing

    // methods and properties

    protected new EditPermissionsFormFunction CurrentFunction
    {
      get { return (EditPermissionsFormFunction) base.CurrentFunction; }
    }

    protected override void OnPreRenderComplete (EventArgs e)
    {
      HtmlHeadAppender.Current.SetTitle (string.Format (AccessControlResources.Title, CurrentFunction.SecurableClassDefinition.DisplayName));
      base.OnPreRenderComplete (e);
    }

    protected override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      LoadAccessControlLists (interim);

      CurrentObjectHeaderControls.BusinessObject = CurrentFunction.SecurableClassDefinition;
      CurrentObjectHeaderControls.LoadValues (interim);

      CurrentObject.BusinessObject = CurrentFunction.SecurableClassDefinition;
      CurrentObject.LoadValues (interim);
    }

    private void LoadAccessControlLists (bool interim)
    {
      var accessControlLists = new List<AccessControlList> (CurrentFunction.SecurableClassDefinition.StatefulAccessControlLists.Cast<AccessControlList>());
      if (CurrentFunction.SecurableClassDefinition.StatelessAccessControlList != null)
        accessControlLists.Insert (0, CurrentFunction.SecurableClassDefinition.StatelessAccessControlList);

      CreateEditAccessControlListControls (accessControlLists.ToArray());
      foreach (var control in _editAccessControlListControls)
        control.LoadValues (interim);
    }

    private void CreateEditAccessControlListControls (AccessControlList[] accessControlLists)
    {
      AccessControlListsPlaceHolder.Controls.Clear ();
      _editAccessControlListControls.Clear ();
      for (int i = 0; i < accessControlLists.Length; i++)
      {
        var accessControlList = accessControlLists[i];

        string controlName = string.Format ("Edit{0}Control.ascx", accessControlList.GetPublicDomainObjectType().Name);

        var editStatefulAccessControlListControl = (EditAccessControlListControlBase) LoadControl (controlName);
        editStatefulAccessControlListControl.ID = "Acl_" + i;
        editStatefulAccessControlListControl.BusinessObject = accessControlList;
        editStatefulAccessControlListControl.Delete += EditAccessControlListControl_Delete;

        var div = new HtmlGenericControl ("div");
        div.Attributes.Add ("class", "accessControlListContainer");
        AccessControlListsPlaceHolder.Controls.Add (div);
        div.Controls.Add (editStatefulAccessControlListControl);

        _editAccessControlListControls.Add (editStatefulAccessControlListControl);
      }
    }

    protected override void SaveValues (bool interim)
    {
      base.SaveValues (interim);

      SaveAccessControlLists (interim);

      CurrentObjectHeaderControls.SaveValues (interim);
      CurrentObject.SaveValues (interim);
    }

    private void SaveAccessControlLists (bool interim)
    {
      foreach (var control in _editAccessControlListControls)
        control.SaveValues (interim);
    }

    protected override bool ValidatePage ()
    {
      bool isValid = true;
      isValid &= base.ValidatePage ();
      isValid &= ValidateAccessControlLists ();
      isValid &= CurrentObjectHeaderControls.Validate ();
      isValid &= CurrentObject.Validate ();

      return isValid;
    }

    protected override bool ValidatePagePostSaveValues ()
    {
      bool isValid = true;
      isValid &= base.ValidatePagePostSaveValues ();
      DuplicateStateCombinationsValidator.Validate ();
      isValid &= DuplicateStateCombinationsValidator.IsValid;

      return isValid;
    }

    private bool ValidateAccessControlLists (params EditAccessControlListControlBase[] excludedControls)
    {
      var excludedControlList = new List<EditAccessControlListControlBase> (excludedControls);

      bool isValid = true;
      foreach (var control in _editAccessControlListControls)
      {
        if (!excludedControlList.Contains (control))
        {
          isValid &= control.Validate ();
        }
      }

      return isValid;
    }

    protected void DuplicateStateCombinationsValidator_ServerValidate (object source, ServerValidateEventArgs args)
    {
      if (CurrentFunction.SecurableClassDefinition.StateProperties.Count > 1)
        throw new NotSupportedException ("Only classes with a zero or one StatePropertyDefinition are supported.");

      var usedStates = new Dictionary<StateDefinition, object> ();
      foreach (var accessControlList in CurrentFunction.SecurableClassDefinition.StatefulAccessControlLists)
      {
        foreach (var stateCombination in accessControlList.StateCombinations)
        {
          if (stateCombination.StateUsages.Count == 1)
          {
            StateUsage stateUsage = stateCombination.StateUsages[0];
            if (usedStates.ContainsKey (stateUsage.StateDefinition))
              args.IsValid = false;
            else
              usedStates.Add (stateUsage.StateDefinition, null);
          }

          if (!args.IsValid)
            break;
        }

        if (!args.IsValid)
          break;
      }
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      CurrentFunction.Transaction.Rollback ();
      throw new WxeUserCancelException ();
    }

    protected void NewStatelessAccessControlListButton_Click (object sender, EventArgs e)
    {
      PrepareValidation ();
      bool isValid = ValidateAccessControlLists ();
      if (!isValid)
        return;

      SaveAccessControlLists (false);
      IsDirty = true;

      CurrentFunction.SecurableClassDefinition.CreateStatelessAccessControlList ();

      LoadAccessControlLists (false);
      //AccessControlListsRepeater.LoadValue (false);
      //AccessControlListsRepeater.IsDirty = true;
    }

    protected void NewStatefulAccessControlListButton_Click (object sender, EventArgs e)
    {
      PrepareValidation ();
      bool isValid = ValidateAccessControlLists ();
      if (!isValid)
        return;

      SaveAccessControlLists (false);
      IsDirty = true;

      CurrentFunction.SecurableClassDefinition.CreateStatefulAccessControlList ();

      LoadAccessControlLists (false);
      //AccessControlListsRepeater.LoadValue (false);
      //AccessControlListsRepeater.IsDirty = true;
    }

    private void EditAccessControlListControl_Delete (object sender, EventArgs e)
    {
      var editStatefulAccessControlListControl = (EditAccessControlListControlBase) sender;
      PrepareValidation ();
      bool isValid = ValidateAccessControlLists (editStatefulAccessControlListControl);
      if (!isValid)
        return;

      _editAccessControlListControls.Remove (editStatefulAccessControlListControl);
      var accessControlList = (AccessControlList) editStatefulAccessControlListControl.DataSource.BusinessObject;
      accessControlList.Delete ();

      SaveAccessControlLists (false);
      IsDirty = true;

      LoadAccessControlLists (false);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      EnableNewAccessControlListButton ();
    }

    private void EnableNewAccessControlListButton ()
    {
      DomainObjectCollection stateProperties = CurrentFunction.SecurableClassDefinition.StateProperties;
      if (stateProperties.Count > 1)
        throw new NotSupportedException ("Only classes with a zero or one StatePropertyDefinition are supported.");

      int possibleStateCombinations = 1;
      if (stateProperties.Count > 0)
        possibleStateCombinations = ((StatePropertyDefinition) stateProperties[0]).DefinedStates.Count;
      NewStatefulAccessControlListButton.Enabled = CurrentFunction.SecurableClassDefinition.StateCombinations.Count < possibleStateCombinations;

      NewStatelessAccessControlListButton.Enabled = CurrentFunction.SecurableClassDefinition.StatelessAccessControlList == null;
    }
  }
}
