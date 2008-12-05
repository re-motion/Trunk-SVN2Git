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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public sealed class SecurityToken
  {
    private readonly User _principal;
    private readonly Tenant _owningTenant;
    private readonly Group _owningGroup;
    private readonly User _owningUser;
    private readonly ReadOnlyCollection<AbstractRoleDefinition> _abstractRoles;

    public SecurityToken (User principal, Tenant owningTenant, Group owningGroup, User owningUser, IList<AbstractRoleDefinition> abstractRoles)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("abstractRoles", abstractRoles);

      _principal = principal;
      _owningTenant = owningTenant;
      _owningGroup = owningGroup;
      _owningUser = owningUser;
      _abstractRoles = new ReadOnlyCollection<AbstractRoleDefinition> (abstractRoles);
    }

    public User Principal
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
