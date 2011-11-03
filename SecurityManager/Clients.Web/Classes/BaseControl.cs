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
using System.Web.UI;
using Microsoft.Practices.ServiceLocation;
using Remotion.Data.DomainObjects;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  public abstract class BaseControl : DataEditUserControl, IObjectWithResources
  {
    // types

    // static members and constants

    private static readonly string s_currentTenantIDKey = typeof (BaseControl).FullName + "_CurrentTenantID";

    // member fields

    private bool _hasTenantChanged;

    // construction and disposing

    // methods and properties

    public new BasePage Page
    {
      get { return (BasePage) base.Page; }
      set { base.Page = value; }
    }

    protected BaseTransactedFunction CurrentFunction
    {
      get { return Page.CurrentFunction; }
    }

    public virtual IFocusableControl InitialFocusControl
    {
      get { return null; }
    }

    protected ObjectID CurrentTenantID
    {
      get { return (ObjectID) ViewState[s_currentTenantIDKey]; }
      set { ViewState[s_currentTenantIDKey] = value; }
    }

    protected bool HasTenantChanged
    {
      get { return _hasTenantChanged; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
        CurrentTenantID = CurrentFunction.TenantID;
    }

    protected override void OnPreRender (EventArgs e)
    {
      if (CurrentTenantID != CurrentFunction.TenantID)
      {
        CurrentTenantID = CurrentFunction.TenantID;
        _hasTenantChanged = true;
      }

      ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));

      base.OnPreRender (e);
    }

    IResourceManager IObjectWithResources.GetResourceManager ()
    {
      return this.GetResourceManager();
    }

    protected virtual IResourceManager GetResourceManager ()
    {
      Type type = this.GetType();

      if (MultiLingualResources.ExistsResource (type))
        return MultiLingualResources.GetResourceManager (type, true);
      else
        return null;
    }

    protected IServiceLocator ServiceLocator
    {
      get { return SafeServiceLocator.Current; }
    }

    protected IResourceUrlFactory ResourceUrlFactory
    {
      get { return ServiceLocator.GetInstance<IResourceUrlFactory>(); }
    }

    protected TControl GetControl<TControl> (string controlID, string propertyIdentifier)
        where TControl : Control, IBusinessObjectBoundWebControl, IFocusableControl
    {
      ArgumentUtility.CheckNotNullOrEmpty ("controlID", controlID);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);

      var control = FindControl (controlID);
      
      if (control == null)
      {
        throw new InvalidOperationException (string.Format ("No control with the ID '{0}' found on {1}.", controlID, GetType().Name));
      }

      if (!(control is TControl))
      {
        throw new InvalidOperationException (
            string.Format ("Control '{0}' on {2} must be of type '{1}'.", controlID, typeof (TControl).FullName, GetType().Name));
      }

      if (!(control is IFocusableControl))
      {
        throw new InvalidOperationException (
            string.Format ("Control '{0}' on {2} must implement the '{1}' interface.", controlID, typeof (IFocusableControl).FullName, GetType().Name));
      }

      var boundEditableWebControl = (TControl) control;
      if (boundEditableWebControl.Property == null || boundEditableWebControl.Property.Identifier != propertyIdentifier)
      {
        throw new InvalidOperationException (
            string.Format ("Control '{0}' on {2} is not bound to property '{1}'.", controlID, propertyIdentifier, GetType().Name));
      }

      return boundEditableWebControl;
    }
  }
}