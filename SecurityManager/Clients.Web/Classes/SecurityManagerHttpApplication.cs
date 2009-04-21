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
using System.Web;
using System.Web.SessionState;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain;
using Remotion.Utilities;
using SecurityManagerUser = Remotion.SecurityManager.Domain.OrganizationalStructure.User;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  public class SecurityManagerHttpApplication : HttpApplication
  {
    private static readonly string s_principalKey = typeof (SecurityManagerHttpApplication).AssemblyQualifiedName + "_Principal";

    public SecurityManagerHttpApplication ()
    {
    }

    public void SetCurrentPrincipal (ISecurityManagerPrincipal securityManagerPrincipal)
    {
      ArgumentUtility.CheckNotNull ("securityManagerPrincipal", securityManagerPrincipal);

      SecurityManagerPrincipal.Current = securityManagerPrincipal;
      SavePrincipalToSession (securityManagerPrincipal);
    }

    protected ISecurityManagerPrincipal LoadPrincipalFromSession ()
    {
      return (ISecurityManagerPrincipal) Session[s_principalKey] ?? SecurityManagerPrincipal.Null;
    }

    protected void SavePrincipalToSession (ISecurityManagerPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("principal", principal);

      Session[s_principalKey] = principal;
    }

    protected bool HasSessionState
    {
      get { return Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState; }
    }

    public override void Init ()
    {
      base.Init();

      PostAcquireRequestState += SecurityManagerHttpApplication_PostAcquireRequestState;
    }

    private void SecurityManagerHttpApplication_PostAcquireRequestState (object sender, EventArgs e)
    {
      if (HasSessionState)
      {
        var principal = LoadPrincipalFromSession();
        if (principal.IsNull && Context.User.Identity.IsAuthenticated)
        {
          using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
          {
            SecurityManagerUser user = SecurityManagerUser.FindByUserName (Context.User.Identity.Name);
            if (user != null)
              SetCurrentPrincipal (new SecurityManagerPrincipal (user.Tenant, user, null));
            else
              SetCurrentPrincipal (SecurityManagerPrincipal.Null);
          }
        }
        else
        {
          principal.Refresh();
          SetCurrentPrincipal (principal);
        }
      }
    }
  }
}
