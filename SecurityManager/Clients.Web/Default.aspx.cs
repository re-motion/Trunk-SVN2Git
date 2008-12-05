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
using System.Web.UI;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;
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
      _clientTransaction = ClientTransaction.CreateRootTransaction ();
      _clientTransaction.EnterDiscardingScope ();
      if (!IsPostBack)
      {
        using (new SecurityFreeSection ())
        {
          DomainObjectCollection users =
              SecurityManagerUser.FindByTenantID (ObjectID.Parse ("Tenant|00000001-0000-0000-0000-000000000000|System.Guid"));
          users.Combine (SecurityManagerUser.FindByTenantID (ObjectID.Parse ("Tenant|00000001-0000-0000-0000-000000000001|System.Guid")));
          users.Combine (SecurityManagerUser.FindByTenantID (ObjectID.Parse ("Tenant|00000001-0000-0000-0000-000000000002|System.Guid")));

          UsersField.SetBusinessObjectList (users);
          UsersField.LoadUnboundValue (ApplicationInstance.LoadUserFromSession (), false);
        }
      }
    }

    protected void UsersField_SelectionChanged (object sender, EventArgs e)
    {
      if (StringUtility.IsNullOrEmpty (UsersField.BusinessObjectID))
        ApplicationInstance.SetCurrentUser (null, true);
      else
        ApplicationInstance.SetCurrentUser (SecurityManagerUser.GetObject (ObjectID.Parse (UsersField.BusinessObjectID)), true);
    }

    protected override void OnPreRender (EventArgs e)
    {
      _clientTransaction.EnterDiscardingScope ();
      base.OnPreRender (e);
    }

    protected override void OnUnload (EventArgs e)
    {
      base.OnUnload (e);
      ClientTransactionScope.ResetActiveScope ();
    }

  }
}
