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
using System.ComponentModel;
using System.Web.UI;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  public partial class CurrentTenantControl : UserControl
  {
    private static readonly string s_isTenantSelectionEnabledKey = typeof (CurrentTenantControl).FullName + "_IsTenantSelectionEnabled";
    private static readonly string s_enableAbstractTenantsKey = typeof (CurrentTenantControl).FullName + "_EnableAbstractTenants";

    private bool _isCurrentTenantFieldReadOnly = true;

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
        DomainObjectCollection tenants = GetPossibleTenants ();
        CurrentTenantField.SetBusinessObjectList (tenants);
        CurrentTenantField.LoadUnboundValue (Tenant.Current, false);

        bool isCurrentTenantTheOnlyTenantInTheCollection = tenants.Count == 1 && Tenant.Current != null && tenants.Contains (Tenant.Current.ID);
        bool isCurrentTenantTheOnlyTenant = tenants.Count == 0 && Tenant.Current != null;
        bool hasExactlyOneTenant = isCurrentTenantTheOnlyTenantInTheCollection || isCurrentTenantTheOnlyTenant;
        IsTenantSelectionEnabled = !hasExactlyOneTenant;
      }

      if (!IsTenantSelectionEnabled)
        CurrentTenantField.Command.Type = CommandType.None;
    }

    private DomainObjectCollection GetPossibleTenants ()
    {
      User user = ApplicationInstance.LoadUserFromSession ();
      DomainObjectCollection tenants;
      if (user == null)
        tenants = new DomainObjectCollection ();
      else
        tenants = user.Tenant.GetHierachy ();

      if (!EnableAbstractTenants)
      {
        for (int i = tenants.Count - 1; i >= 0; i--)
        {
          if (((Tenant) tenants[i]).IsAbstract)
            tenants.RemoveAt (i);
        }
      }

      return tenants;
    }

    protected void CurrentTenantField_SelectionChanged (object sender, EventArgs e)
    {
      string tenantID = CurrentTenantField.BusinessObjectID;
      if (StringUtility.IsNullOrEmpty (tenantID))
      {
        ApplicationInstance.SetCurrentTenant (null);
        _isCurrentTenantFieldReadOnly = false;
      }
      else
      {
        ApplicationInstance.SetCurrentTenant (Tenant.GetObject (ObjectID.Parse (tenantID)));
        _isCurrentTenantFieldReadOnly = true;
      }

      CurrentTenantField.IsDirty = false;
    }

    protected void CurrentTenantField_CommandClick (object sender, BocCommandClickEventArgs e)
    {
      _isCurrentTenantFieldReadOnly = false;
      CurrentTenantField.SetBusinessObjectList (GetPossibleTenants ());
      CurrentTenantField.LoadUnboundValue (Tenant.Current, false);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (_isCurrentTenantFieldReadOnly && Tenant.Current != null)
        CurrentTenantField.ReadOnly = true;
      else
        CurrentTenantField.ReadOnly = false;

      User user = ApplicationInstance.LoadUserFromSession ();
      CurrentUserField.LoadUnboundValue (user, false);
    }

    private bool IsTenantSelectionEnabled
    {
      get { return (bool?) ViewState[s_isTenantSelectionEnabledKey] ?? true; }
      set { ViewState[s_isTenantSelectionEnabledKey] = value; }
    }
  }
}
