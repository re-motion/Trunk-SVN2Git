// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.ObjectModel;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// The <see cref="Principal"/> type encapsulates a <see cref="Tenant"/> object, a <see cref="User"/> object, 
  /// and one or more <see cref="Role"/> objects. Together, they specify the principal for which the permissions are evaluated.
  /// </summary>
  public class Principal
  {
    private readonly Tenant _tenant;
    private readonly User _user;
    private readonly IList<Role> _roles;

    public Principal (Tenant tenant, User user, IList<Role> roles)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("roles", roles);

      _tenant = tenant;
      _user = user;
      _roles = new ReadOnlyCollection<Role> (roles);
    }

    public Tenant Tenant
    {
      get { return _tenant; }
    }

    public User User
    {
      get { return _user; }
    }

    public IList<Role> Roles
    {
      get { return _roles; }
    }
  }
}
