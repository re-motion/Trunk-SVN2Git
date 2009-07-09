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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  public partial class CurrentTenantControl : UserControl
  {
    private static readonly string s_isTenantSelectionEnabledKey = typeof (CurrentTenantControl).FullName + "_IsTenantSelectionEnabled";
    private static readonly string s_enableAbstractTenantsKey = typeof (CurrentTenantControl).FullName + "_EnableAbstractTenants";
    private static readonly string s_isSubstitutionSelectionEnabledKey = typeof (CurrentTenantControl).FullName + "_IsSubstitutionSelectionEnabled";

    private bool _isCurrentTenantFieldReadOnly = true;
    private bool _isCurrentSubstitutionFieldReadOnly = true;

    [DefaultValue (true)]
    public bool EnableAbstractTenants
    {
      get { return (bool?) ViewState[s_enableAbstractTenantsKey] ?? true; }
      set { ViewState[s_enableAbstractTenantsKey] = value; }
    }

    protected SecurityManagerHttpApplication ApplicationInstance
    {
      get { return (SecurityManagerHttpApplication) Context.ApplicationInstance; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        Tenant[] tenants = GetPossibleTenants();
        CurrentTenantField.SetBusinessObjectList (tenants);
        Tenant currentTenant = SecurityManagerPrincipal.Current.Tenant;

        CurrentTenantField.LoadUnboundValue (currentTenant, false);

        bool isCurrentTenantTheOnlyTenantInTheCollection = tenants.Length == 1 && currentTenant != null && tenants[0].ID == currentTenant.ID;
        bool isCurrentTenantTheOnlyTenant = tenants.Length == 0 && currentTenant != null;
        bool hasExactlyOneTenant = isCurrentTenantTheOnlyTenantInTheCollection || isCurrentTenantTheOnlyTenant;
        IsTenantSelectionEnabled = !hasExactlyOneTenant;

        Substitution[] substitutions = GetPossibleSubstitutions();
        CurrentSubstitutionField.SetBusinessObjectList (substitutions);
        Substitution currentSubstitution = SecurityManagerPrincipal.Current.Substitution;

        CurrentSubstitutionField.LoadUnboundValue (currentSubstitution, false);
        IsSubstitutionSelectionEnabled = substitutions.Length > 0;
      }

      if (!IsTenantSelectionEnabled)
        CurrentTenantField.Command.Type = CommandType.None;

      if (!IsSubstitutionSelectionEnabled)
        CurrentSubstitutionField.Command.Type = CommandType.None;
    }

    private Tenant[] GetPossibleTenants ()
    {
      if (SecurityManagerPrincipal.Current.IsNull)
        return new Tenant[0];

      IEnumerable<Tenant> tenants = SecurityManagerPrincipal.Current.User.Tenant.GetHierachy();

      if (!EnableAbstractTenants)
        tenants = tenants.Where (t => !t.IsAbstract);

      return tenants.ToArray();
    }

    private Substitution[] GetPossibleSubstitutions ()
    {
      if (SecurityManagerPrincipal.Current.IsNull)
        return new Substitution[0];

      return SecurityManagerPrincipal.Current.User.GetActiveSubstitutions().ToArray();
    }

    protected void CurrentTenantField_SelectionChanged (object sender, EventArgs e)
    {
      string tenantID = CurrentTenantField.BusinessObjectUniqueIdentifier;
      Assertion.IsNotNull (tenantID);

      ApplicationInstance.SetCurrentPrincipal (
          new SecurityManagerPrincipal (
              Tenant.GetObject (ObjectID.Parse (tenantID)),
              SecurityManagerPrincipal.Current.User,
              SecurityManagerPrincipal.Current.Substitution));

      _isCurrentTenantFieldReadOnly = true;
      CurrentTenantField.IsDirty = false;
    }

    protected void CurrentSubstitutionField_SelectionChanged (object sender, EventArgs e)
    {
      string substitutionID = CurrentSubstitutionField.BusinessObjectUniqueIdentifier;

      ApplicationInstance.SetCurrentPrincipal (
          new SecurityManagerPrincipal (
              SecurityManagerPrincipal.Current.Tenant,
              SecurityManagerPrincipal.Current.User,
              substitutionID != null ? (Substitution) Substitution.GetObject (ObjectID.Parse (substitutionID)) : null));

      _isCurrentSubstitutionFieldReadOnly = true;
      CurrentSubstitutionField.IsDirty = false;
    }

    protected void CurrentTenantField_CommandClick (object sender, BocCommandClickEventArgs e)
    {
      _isCurrentTenantFieldReadOnly = false;
      CurrentTenantField.SetBusinessObjectList (GetPossibleTenants());
      CurrentTenantField.LoadUnboundValue (SecurityManagerPrincipal.Current.Tenant, false);
    }

    protected void CurrentSubstitutionField_CommandClick (object sender, BocCommandClickEventArgs e)
    {
      _isCurrentSubstitutionFieldReadOnly = false;
      CurrentSubstitutionField.SetBusinessObjectList (GetPossibleSubstitutions());
      CurrentSubstitutionField.LoadUnboundValue (SecurityManagerPrincipal.Current.Substitution, false);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (_isCurrentTenantFieldReadOnly && !SecurityManagerPrincipal.Current.IsNull)
        CurrentTenantField.ReadOnly = true;
      else
        CurrentTenantField.ReadOnly = false;

      if (_isCurrentSubstitutionFieldReadOnly && !SecurityManagerPrincipal.Current.IsNull)
        CurrentSubstitutionField.ReadOnly = true;
      else
        CurrentSubstitutionField.ReadOnly = false;
      //For now: the subsitution is permanenty editable.
      CurrentSubstitutionField.ReadOnly = false;

      CurrentUserField.LoadUnboundValue (SecurityManagerPrincipal.Current.User, false);
    }

    private bool IsTenantSelectionEnabled
    {
      get { return (bool?) ViewState[s_isTenantSelectionEnabledKey] ?? true; }
      set { ViewState[s_isTenantSelectionEnabledKey] = value; }
    }

    private bool IsSubstitutionSelectionEnabled
    {
      get { return (bool?) ViewState[s_isSubstitutionSelectionEnabledKey] ?? true; }
      set { ViewState[s_isSubstitutionSelectionEnabledKey] = value; }
    }
  }
}
