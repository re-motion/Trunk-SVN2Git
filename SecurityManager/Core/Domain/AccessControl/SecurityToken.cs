/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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