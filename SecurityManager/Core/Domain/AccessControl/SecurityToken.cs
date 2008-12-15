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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public sealed class SecurityToken
  {
    private readonly Principal _principal;
    private readonly Tenant _owningTenant;
    private readonly Group _owningGroup;
    private readonly User _owningUser;
    private readonly ReadOnlyCollection<AbstractRoleDefinition> _abstractRoles;

    public SecurityToken (Principal principal, Tenant owningTenant, Group owningGroup, User owningUser, IList<AbstractRoleDefinition> abstractRoles)
    {
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNullOrItemsNull ("abstractRoles", abstractRoles);

      _principal = principal;
      _owningTenant = owningTenant;
      _owningGroup = owningGroup;
      _owningUser = owningUser;
      _abstractRoles = new ReadOnlyCollection<AbstractRoleDefinition> (abstractRoles);
    }

    public Principal Principal
    {
      get { return _principal; }
    }

    public Tenant OwningTenant
    {
      get { return _owningTenant; }
    }

    public Group OwningGroup
    {
      get { return _owningGroup; }
    }

    public User OwningUser
    {
      get { return _owningUser; }
    }

    public ReadOnlyCollection<AbstractRoleDefinition> AbstractRoles
    {
      get { return _abstractRoles; }
    }
  }
}
