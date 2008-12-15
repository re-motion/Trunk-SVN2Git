// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.
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
      ArgumentUtility.CheckNotNull ("tenant", tenant);
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