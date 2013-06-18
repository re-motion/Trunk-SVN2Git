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
using Remotion.Utilities;

namespace Remotion.Security
{
  public sealed class GlobalAccessTypeCacheKey
  {
    private readonly ISecurityContext _securityContext;
    private readonly ISecurityPrincipal _securityPrincipal;

    public GlobalAccessTypeCacheKey (ISecurityContext securityContext, ISecurityPrincipal securityPrincipal)
    {
      ArgumentUtility.CheckNotNull ("securityContext", securityContext);
      ArgumentUtility.CheckNotNull ("securityPrincipal", securityPrincipal);
      
      _securityContext = securityContext;
      _securityPrincipal = securityPrincipal;
    }

    public ISecurityContext SecurityContext
    {
      get { return _securityContext; }
    }

    public ISecurityPrincipal SecurityPrincipal
    {
      get { return _securityPrincipal; }
    }

    public override bool Equals (object other)
    {
      var otherKey = other as GlobalAccessTypeCacheKey;
      return otherKey != null
             && _securityContext.Equals (otherKey._securityContext)
             && _securityPrincipal.Equals (otherKey._securityPrincipal);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_securityContext, _securityPrincipal);
    }
  }
}