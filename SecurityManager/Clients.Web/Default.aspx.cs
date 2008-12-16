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
using System.Linq;
using System.Web.UI;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using SecurityManagerUser = Remotion.SecurityManager.Domain.OrganizationalStructure.User;

namespace Remotion.SecurityManager.Clients.Web
{
  public partial class DefaultPage : Page
  {
    private ClientTransaction _clientTransaction;

    protected Global ApplicationInstance
    {
      get { return (Global) Context.ApplicationInstance; }
    }

    protected override void OnLoad (EventArgs e)
    {
      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _clientTransaction.EnterDiscardingScope();
      if (!IsPostBack)
      {
        using (new SecurityFreeSection())
        {
          var users = (from u in QueryFactory.CreateLinqQuery<SecurityManagerUser>() orderby u.UserName select u).ToArray();

          SecurityManagerUser user;
          if (SecurityManagerPrincipal.Current != null)
            user = SecurityManagerPrincipal.Current.User;
          else
            user = null;

          UsersField.SetBusinessObjectList (users);
          UsersField.LoadUnboundValue (user, false);
        }
      }
    }

    protected void UsersField_SelectionChanged (object sender, EventArgs e)
    {
      if (UsersField.BusinessObjectID == null)
      {
        ApplicationInstance.SetCurrentPrincipal (null);
      }
      else
      {
        var user = SecurityManagerUser.GetObject (ObjectID.Parse (UsersField.BusinessObjectID));
        ApplicationInstance.SetCurrentPrincipal (new SecurityManagerPrincipal (user.Tenant, user, null));
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      _clientTransaction.EnterDiscardingScope();
      base.OnPreRender (e);
    }

    protected override void OnUnload (EventArgs e)
    {
      base.OnUnload (e);
      ClientTransactionScope.ResetActiveScope();
    }
  }
}