// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;
using SecurityManagerUser = Remotion.SecurityManager.Domain.OrganizationalStructure.User;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  public class SecurityManagerHttpApplication : HttpApplication
  {
    // constants

    // types

    // static members

    private static readonly string s_tenantKey = typeof (SecurityManagerHttpApplication).AssemblyQualifiedName + "_Tenant";
    private static readonly string s_userKey = typeof (SecurityManagerHttpApplication).AssemblyQualifiedName + "_User";

    // member fields

    // construction and disposing

    public SecurityManagerHttpApplication ()
    {
    }

    // methods and properties

    public void SetCurrentUser (SecurityManagerUser user, bool setCurrentTenant)
    {
      IPrincipal principal = GetPrincipal (user);
      HttpContext.Current.User = principal;
      Thread.CurrentPrincipal = principal;
      SaveUserToSession (user, false);
      SecurityManagerUser.Current = user;
      if (setCurrentTenant)
      {
        Tenant tenant;
        using (new SecurityFreeSection ())
        {
          tenant = (user != null) ? user.Tenant : null;
        }
        SetCurrentTenant (tenant);
      }
    }

    protected virtual IPrincipal GetPrincipal (SecurityManagerUser user)
    {
      string userName = (user != null) ? user.UserName : string.Empty;
      return new GenericPrincipal (new GenericIdentity (userName), new string[0]);
    }

    public ObjectID LoadUserIDFromSession ()
    {
      return (ObjectID) Session[s_userKey];
    }

    public User LoadUserFromSession ()
    {
      ObjectID userID = LoadUserIDFromSession ();
      if (userID == null)
        return null;

      return SecurityManagerUser.GetObject (userID);
    }

    public void SaveUserToSession (SecurityManagerUser user, bool saveCurrentTenant)
    {
      if (user == null)
        Session.Remove (s_userKey);
      else
        Session[s_userKey] = user.ID;

      if (saveCurrentTenant)
      {
        Tenant tenant;
        using (new SecurityFreeSection ())
        {
          tenant = (user != null) ? user.Tenant : null;
        }
        SaveTenantToSession (tenant);
      }
    }

    public void SetCurrentTenant (Tenant tenant)
    {
      SaveTenantToSession (tenant);
      Tenant.Current = tenant;
    }

    public ObjectID LoadTenantIDFromSession ()
    {
      return (ObjectID) Session[s_tenantKey];
    }

    public Tenant LoadTenantFromSession ()
    {
      ObjectID tenantID = LoadTenantIDFromSession ();
      if (tenantID == null)
        return null;

      return Tenant.GetObject (tenantID);
    }

    public void SaveTenantToSession (Tenant tenant)
    {
      if (tenant == null)
        Session.Remove (s_tenantKey);
      else
        Session[s_tenantKey] = tenant.ID;
    }

    protected bool HasSessionState
    {
      get { return Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState; }
    }

    public override void Init ()
    {
      base.Init ();

      PostAcquireRequestState += SecurityManagerHttpApplication_PostAcquireRequestState; 
    }

    private void SecurityManagerHttpApplication_PostAcquireRequestState (object sender, EventArgs e)
    {
      if (HasSessionState)
      {
        using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
        {
          SecurityManagerUser user = LoadUserFromSession ();
          if (user == null && Context.User.Identity.IsAuthenticated)
          {
            user = SecurityManagerUser.FindByUserName (Context.User.Identity.Name);
            SetCurrentUser (user, true);
          }
          else
          {
            SetCurrentUser (user, false);
            SetCurrentTenant (LoadTenantFromSession ());
          }
        }
      }
    }
  }
}
