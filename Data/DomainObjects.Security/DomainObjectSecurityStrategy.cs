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
using System.Text;
using Remotion.Security;
using Remotion.Utilities;
using System.Security.Principal;

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
      if (SecurityContextFactory.IsDiscarded)
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
