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
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Clients.Web.Test.Domain
{
  [PermanentGuid ("C9FC9EC0-9F41-4636-9A4C-4927A9B47E85")]
  public abstract class BaseSecurableObject : BaseObject, ISecurableObject, ISecurityContextFactory
  {
    private IObjectSecurityStrategy _objectSecurityStrategy;

    protected BaseSecurableObject ()
    {
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      if (_objectSecurityStrategy == null)
        _objectSecurityStrategy = new ObjectSecurityStrategy (this);

      return _objectSecurityStrategy;
    }

    public Type GetSecurableType ()
    {
      return GetPublicDomainObjectType ();
    }

    public ISecurityContext CreateSecurityContext ()
    {
      return SecurityContext.Create(GetPublicDomainObjectType(), GetOwnerName(), GetOwnerGroupName(), GetOwnerTenantName(), GetStates(), GetAbstractRoles());
    }

    private string GetOwnerTenantName ()
    {
      Tenant tenant = GetOwnerTenant();
      if (tenant == null)
        return null;
      return tenant.Name;
    }

    private string GetOwnerGroupName ()
    {
      Group group = GetOwnerGroup();
      if (group == null)
        return null;
      return group.Name;
    }

    private string GetOwnerName ()
    {
      User user = GetOwner();
      if (user == null)
        return null;
      return user.UserName;
    }

    public abstract User GetOwner ();

    public abstract Group GetOwnerGroup ();

    public abstract Tenant GetOwnerTenant ();

    public virtual IDictionary<string, Enum> GetStates ()
    {
      return new Dictionary<string, Enum>();
    }

    public virtual ICollection<Enum> GetAbstractRoles ()
    {
      return new Enum[0];
    }
  }
}
