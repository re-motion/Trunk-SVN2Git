// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// The <see cref="ISecurityManagerPrincipal"/> interface is represent the current <see cref="Tenant"/>, <see cref="User"/>, 
  /// and optional <see cref="Substitution"/>. 
  /// See <see cref="SecurityManagerPrincipal"/> for the <see cref="SecurityManagerPrincipal.Current"/> instance.
  /// </summary>
  /// <remarks>Implementations of the interface must be threadsafe.</remarks>
  /// <threadsafety static="true" instance="true"/>
  public interface ISecurityManagerPrincipal : INullObject
  {
    TenantProxy Tenant { get; }
    UserProxy User { get; }
    SubstitutionProxy Substitution { get; }
    void Refresh ();
    ISecurityPrincipal GetSecurityPrincipal ();
    TenantProxy[] GetTenants (bool includeAbstractTenants);
    SubstitutionProxy[] GetActiveSubstitutions ();
  }
}