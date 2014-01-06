// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Security;

namespace Remotion.Data.DomainObjects.Security
{
  [Flags]
  public enum RequiredSecurityForStates
  {
    None = 0,
    New = 1,
    Deleted = 2,
    NewAndDeleted = 3
  }

  [Serializable]
  public class DomainObjectSecurityStrategy : ObjectSecurityStrategy
  {
    // types

    // static members

    // member fields

    private RequiredSecurityForStates _requiredSecurityForStates;

    // construction and disposing

    public DomainObjectSecurityStrategy (
        RequiredSecurityForStates requiredSecurityForStates,
        IDomainObjectSecurityContextFactory securityContextFactory, 
        ISecurityStrategy securityStrategy)
      : base (securityContextFactory, securityStrategy)
    {
      _requiredSecurityForStates = requiredSecurityForStates;
    }

    public DomainObjectSecurityStrategy (
        RequiredSecurityForStates requiredSecurityForStates, 
        IDomainObjectSecurityContextFactory securityContextFactory)
      : base (securityContextFactory)
    {
      _requiredSecurityForStates = requiredSecurityForStates;
    }

    // methods and properties

    public override bool HasAccess (ISecurityProvider securityService, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      if (SecurityContextFactory.IsInvalid)
        return true;

      bool isSecurityRequiredForNew = RequiredSecurityForStates.New == (RequiredSecurityForStates.New & _requiredSecurityForStates);
      if (!isSecurityRequiredForNew && SecurityContextFactory.IsNew)
        return true;

      bool isSecurityRequiredForDeleted = RequiredSecurityForStates.Deleted == (RequiredSecurityForStates.Deleted & _requiredSecurityForStates);
      if (!isSecurityRequiredForDeleted && SecurityContextFactory.IsDeleted)
        return true;

      return base.HasAccess (securityService, principal, requiredAccessTypes);
    }

    public new IDomainObjectSecurityContextFactory SecurityContextFactory
    {
      get { return (IDomainObjectSecurityContextFactory) base.SecurityContextFactory; }
    }

    public RequiredSecurityForStates RequiredSecurityForStates
    {
      get { return _requiredSecurityForStates; }
    }
  }
}
