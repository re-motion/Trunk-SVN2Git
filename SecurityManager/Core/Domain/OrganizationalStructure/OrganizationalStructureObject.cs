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
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.Data.DomainObjects.Security;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [PermanentGuid ("8DBA42FE-ECD9-4b10-8F79-48E7A1119414")]
  [Serializable]
  public abstract class OrganizationalStructureObject : BaseSecurityManagerObject, ISecurableObject, IDomainObjectSecurityContextFactory
  {
    private DomainObjectSecurityStrategy _securityStrategy;

    protected OrganizationalStructureObject ()
    {
    }

    protected virtual string GetOwningTenant ()
    {
      return null;
    }

    protected virtual string GetOwner ()
    {
      return null;
    }

    protected virtual string GetOwningGroup ()
    {
      return null;
    }

    protected virtual IDictionary<string, Enum> GetStates ()
    {
      return new Dictionary<string, Enum>();
    }

    protected virtual IList<Enum> GetAbstractRoles ()
    {
      return new List<Enum>();
    }

    ISecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      using (new SecurityFreeSection())
      {
        return SecurityContext.Create(GetPublicDomainObjectType(), GetOwner(), GetOwningGroup(), GetOwningTenant(), GetStates(), GetAbstractRoles());
      }
    }

    bool IDomainObjectSecurityContextFactory.IsDiscarded
    {
      get { return IsDiscarded; }
    }

    bool IDomainObjectSecurityContextFactory.IsNew
    {
      get { return State == StateType.New; }
    }

    bool IDomainObjectSecurityContextFactory.IsDeleted
    {
      get { return State == StateType.Deleted; }
    }

    IObjectSecurityStrategy ISecurableObject.GetSecurityStrategy ()
    {
      if (_securityStrategy == null)
        _securityStrategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, this);

      return _securityStrategy;
    }

    Type ISecurableObject.GetSecurableType ()
    {
      return GetPublicDomainObjectType();
    }
  }
}
