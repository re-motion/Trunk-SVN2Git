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
using Remotion.Utilities;

namespace Remotion.Security
{
  [Serializable]
  public class ObjectSecurityStrategy : IObjectSecurityStrategy
  {
    private ISecurityStrategy _securityStrategy;
    private ISecurityContextFactory _securityContextFactory;

    public ObjectSecurityStrategy (ISecurityContextFactory securityContextFactory, ISecurityStrategy securityStrategy)
    {
      ArgumentUtility.CheckNotNull ("securityContextFactory", securityContextFactory);
      ArgumentUtility.CheckNotNull ("securityStrategy", securityStrategy);
      
      _securityContextFactory = securityContextFactory;
      _securityStrategy = securityStrategy;
    }

    public ObjectSecurityStrategy (ISecurityContextFactory securityContextFactory)
      : this (securityContextFactory, new SecurityStrategy ())
    {
    }

    public ISecurityStrategy SecurityStrategy
    {
      get { return _securityStrategy; }
    }

    public ISecurityContextFactory SecurityContextFactory
    {
      get { return _securityContextFactory; }
    }

    public void InvalidateLocalCache ()
    {
      _securityStrategy.InvalidateLocalCache ();
    }

    public virtual bool HasAccess (ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securityProvider", securityProvider);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      return _securityStrategy.HasAccess (_securityContextFactory, securityProvider, user, requiredAccessTypes);
    }
  }
}
