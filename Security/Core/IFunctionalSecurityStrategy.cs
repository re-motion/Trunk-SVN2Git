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
using System.Security.Principal;

namespace Remotion.Security
{
  /// <summary>Encapsulates the security checks for static access to the business object.</summary>
  /// <remarks><note type="inotes">Implementations are free to decide whether they provide object-independent caching.</note></remarks>
  public interface IFunctionalSecurityStrategy
  {
    /// <summary>Determines whether the requested access is granted.</summary>
    /// <param name="type">The <see cref="Type"/> of the business object.</param>
    /// <param name="securityProvider">The <see cref="ISecurityProvider"/> used to determine the permissions.</param>
    /// <param name="user">The <see cref="IPrincipal"/> on whose behalf the permissions are evaluated.</param>
    /// <param name="requiredAccessTypes">The access rights required for the access to be granted.</param>
    /// <returns><see langword="true"/> if the <paramref name="requiredAccessTypes"/> are granted.</returns>
    /// <remarks>
    /// Typically called via <see cref="O:Remotion.Security.SecurityClient.HasAccess"/> of 
    /// <see cref="T:Remotion.Security.SecurityClient"/>.
    /// The strategy incorporates <see cref="SecurityContext"/> in the permission query.
    /// The <paramref name="requiredAccessTypes"/> are determined by the <see cref="T:Remotion.Security.SecurityClient"/>, 
    /// taking the business object instance and the member name (property or method) into account.
    /// </remarks>
    bool HasAccess (Type type, ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes);
  }
}
