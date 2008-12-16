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

      using (new SecurityFreeSection())
      {
        IPrincipal principal = GetPrincipal (securityManagerPrincipal.User);
        HttpContext.Current.User = principal;
        Thread.CurrentPrincipal = principal;
      }
    }

    protected virtual IPrincipal GetPrincipal (SecurityManagerUser user)
    {
      string userName = (user != null) ? user.UserName : string.Empty;
      return new GenericPrincipal (new GenericIdentity (userName), new string[0]);
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
          using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
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