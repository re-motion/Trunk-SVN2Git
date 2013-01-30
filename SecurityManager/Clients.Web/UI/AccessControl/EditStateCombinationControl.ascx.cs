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
using System.Collections.Generic;
using System.Linq;
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

      DeleteStateDefinitionButton.Icon = new IconInfo (
          ResourceUrlFactory.CreateThemedResourceUrl (typeof (EditStateCombinationControl), ResourceType.Image, "DeleteItem.gif").GetUrl());
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
      var stateProperties = CurrentStateCombination.Class.StateProperties;
      if (stateProperties.Count > 1)
        throw new NotSupportedException ("Only classes with a zero or one StatePropertyDefinition are supported.");

      var possibleStateDefinitions = new List<StateDefinition> ();
      if (stateProperties.Count > 0)
        possibleStateDefinitions.AddRange (stateProperties[0].DefinedStates);
      StateDefinitionField.SetBusinessObjectList (possibleStateDefinitions);
    }

    private StateDefinition GetStateDefinition (StateCombination stateCombination)
    {
      return stateCombination.GetStates().SingleOrDefault();
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
        string id = StateDefinitionField.BusinessObjectUniqueIdentifier;
        StateDefinition stateDefinition = ObjectID.Parse (id).GetObject<StateDefinition> ();
        CurrentStateCombination.ClearStates();
          CurrentStateCombination.AttachState (stateDefinition);
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
      args.IsValid = !StringUtility.IsNullOrEmpty (StateDefinitionField.BusinessObjectUniqueIdentifier);
    }
  }
}
