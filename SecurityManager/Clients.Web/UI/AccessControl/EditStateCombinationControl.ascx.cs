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
using System.Web.UI.WebControls;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (AccessControlResources))]
  public partial class EditStateCombinationControl : BaseControl
  {
    // types

    // static members and constants

    private static readonly object s_deleteEvent = new object ();

    // member fields

    // construction and disposing

    // methods and properties
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected StateCombination CurrentStateCombination
    {
      get { return (StateCombination) CurrentObject.BusinessObject; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      DeleteStateDefinitionButton.Icon = new IconInfo (ResourceUrlResolver.GetResourceUrl (
          this, typeof (EditStateCombinationControl), ResourceType.Image, "DeleteItem.gif"));
      DeleteStateDefinitionButton.Icon.AlternateText = Globalization.UI.AccessControl.AccessControlResources.DeleteStateCombinationButton_Text;
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      if (CurrentStateCombination.Class.StateProperties.Count == 1)
      {
        if (!interim)
          FillStateDefinitionField();

        StateDefinition currentStateDefinition = GetStateDefinition (CurrentStateCombination);
        StateDefinitionField.LoadUnboundValue (currentStateDefinition, interim);
        StateDefinitionContainer.Visible = true;
      }
      else
      {
        StateDefinitionContainer.Visible = false;
      }
    }

    private void FillStateDefinitionField ()
    {
      DomainObjectCollection stateProperties = CurrentStateCombination.Class.StateProperties;
      if (stateProperties.Count > 1)
        throw new NotSupportedException ("Only classes with a zero or one StatePropertyDefinition are supported.");

      List<StateDefinition> possibleStateDefinitions = new List<StateDefinition> ();
      if (stateProperties.Count > 0)
      {
        StatePropertyDefinition statePropertyDefinition = (StatePropertyDefinition) stateProperties[0];
        foreach (StateDefinition stateDefinition in statePropertyDefinition.DefinedStates)
        {
          possibleStateDefinitions.Add (stateDefinition);
        }
      }
      StateDefinitionField.SetBusinessObjectList (possibleStateDefinitions);
    }

    private StateDefinition GetStateDefinition (StateCombination stateCombination)
    {
      if (stateCombination.StateUsages.Count == 0)
        return null;

      StateUsage stateUsage = stateCombination.StateUsages[0];
      return stateUsage.StateDefinition;
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= ValidateStateCombination ();

      return isValid;
    }

    private bool ValidateStateCombination ()
    {
      bool isValid = true;
      if (CurrentStateCombination.Class.StateProperties.Count == 1)
      {
        RequiredStateCombinationValidator.Validate();
        isValid &= RequiredStateCombinationValidator.IsValid;
      }
      return isValid;
    }

    public override void SaveValues (bool interim)
    {
      base.SaveValues (interim);

      if (CurrentStateCombination.Class.StateProperties.Count == 1)
      {
        string id = StateDefinitionField.BusinessObjectID;
        StateDefinition stateDefinition = StateDefinition.GetObject (ObjectID.Parse (id));
        StateUsage stateUsage;
        if (CurrentStateCombination.StateUsages.Count == 0)
        {
          stateUsage = StateUsage.NewObject();
          stateUsage.StateCombination = CurrentStateCombination;
        }
        else
        {
          stateUsage = CurrentStateCombination.StateUsages[0];
        }
        if (stateUsage.StateDefinition != stateDefinition)
          stateUsage.StateDefinition = stateDefinition;
      }
    }

    protected void DeleteStateDefinitionButton_Click (object sender, EventArgs e)
    {
      EventHandler handler = (EventHandler) Events[s_deleteEvent];
      if (handler != null)
        handler (this, EventArgs.Empty);
    }

    public event EventHandler Delete
    {
      add { Events.AddHandler (s_deleteEvent, value); }
      remove { Events.RemoveHandler (s_deleteEvent, value); }
    }

    protected void RequiredStateCombinationValidator_ServerValidate (object source, ServerValidateEventArgs args)
    {
      args.IsValid = !StringUtility.IsNullOrEmpty (StateDefinitionField.BusinessObjectID);
    }
  }
}
