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
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Security
{
  [ImplementationFor (typeof (IFunctionalSecurityStrategy), Lifetime = LifetimeKind.Singleton)]
  public class FunctionalSecurityStrategy : IFunctionalSecurityStrategy
  {
    private static IGlobalAccessTypeCache GetGlobalAccessTypeCache ()
    {
      return SafeServiceLocator.Current.GetAllInstances<IGlobalAccessTypeCache>()
          .First (() => new InvalidOperationException ("No instance of IGlobalAccessTypeCache has been registered with the ServiceLocator."));
    }

    public static FunctionalSecurityStrategy CreateWithCustomSecurityStrategy (ISecurityStrategy securityStrategy)
    {
      ArgumentUtility.CheckNotNull ("securityStrategy", securityStrategy);
      
      return new FunctionalSecurityStrategy (securityStrategy);
    }

    private readonly ISecurityStrategy _securityStrategy;

    [PublicAPI]
    protected FunctionalSecurityStrategy (ISecurityStrategy securityStrategy)
    {
      ArgumentUtility.CheckNotNull ("securityStrategy", securityStrategy);

      _securityStrategy = securityStrategy;
    }

    public FunctionalSecurityStrategy ()
        : this (new SecurityStrategy (new NullCache<ISecurityPrincipal, AccessType[]>(), GetGlobalAccessTypeCache()))
    {
    }

    public ISecurityStrategy SecurityStrategy
    {
      get { return _securityStrategy; }
    }

    public bool HasAccess (Type type, ISecurityProvider securityProvider, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableObject));
      ArgumentUtility.CheckNotNull ("securityProvider", securityProvider);
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      return _securityStrategy.HasAccess (new FunctionalSecurityContextFactory (type), securityProvider, principal, requiredAccessTypes);
    }
  }
}