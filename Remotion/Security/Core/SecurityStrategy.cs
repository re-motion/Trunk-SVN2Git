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
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Security
{
  [Serializable]
  public class SecurityStrategy : ISecurityStrategy
  {
    [ThreadStatic]
    private static bool s_isEvaluatingAccess;

    private static IGlobalAccessTypeCache GetGlobalAccessTypeCache ()
    {
      return SafeServiceLocator.Current.GetAllInstances<IGlobalAccessTypeCache>()
          .First (() => new InvalidOperationException ("No instance of IGlobalAccessTypeCache has been registered with the ServiceLocator."));
    }

    private readonly ICache<ISecurityPrincipal, AccessType[]> _localCache;
    private readonly IGlobalAccessTypeCache _globalCache;

    public SecurityStrategy ()
        : this (new Cache<ISecurityPrincipal, AccessType[]>(), GetGlobalAccessTypeCache())
    {
    }

    public SecurityStrategy (ICache<ISecurityPrincipal, AccessType[]> localCache, IGlobalAccessTypeCache globalCache)
    {
      ArgumentUtility.CheckNotNull ("localCache", localCache);
      ArgumentUtility.CheckNotNull ("globalCache", globalCache);

      _localCache = localCache;
      _globalCache = globalCache;
    }

    public ICache<ISecurityPrincipal, AccessType[]> LocalCache
    {
      get { return _localCache; }
    }

    public IGlobalAccessTypeCache GlobalCache
    {
      get { return _globalCache; }
    }

    public void InvalidateLocalCache ()
    {
      _localCache.Clear();
    }

    public bool HasAccess (
        ISecurityContextFactory factory,
        ISecurityProvider securityProvider,
        ISecurityPrincipal principal,
        params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("factory", factory);
      ArgumentUtility.CheckNotNull ("securityProvider", securityProvider);
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNull ("requiredAccessTypes", requiredAccessTypes);

      if (s_isEvaluatingAccess)
      {
        throw new InvalidOperationException (
            "Multiple reentrancies on SecurityStrategy.HasAccess(...) are not allowed as they can indicate a possible infinite recursion. "
            + "Use SecurityFreeSection.IsActive to guard the computation of the SecurityContext returned by ISecurityContextFactory.CreateSecurityContext().");
      }

      AccessType[] actualAccessTypes;
      try
      {
        s_isEvaluatingAccess = true;
        actualAccessTypes = GetAccessFromLocalCache (factory, securityProvider, principal);
      }
      finally
      {
        s_isEvaluatingAccess = false;
      }

      // This section is performance critical. No closure should be created, therefor converting this code to Linq is not possible.
      // requiredAccessTypes.All (requiredAccessType => actualAccessTypes.Contains (requiredAccessType));
      // ReSharper disable LoopCanBeConvertedToQuery
      foreach (var requiredAccessType in requiredAccessTypes)
      {
        if (Array.IndexOf (actualAccessTypes, requiredAccessType) < 0)
          return false;
      }

      return true;
      // ReSharper restore LoopCanBeConvertedToQuery
    }

    private AccessType[] GetAccessFromLocalCache (ISecurityContextFactory factory, ISecurityProvider securityProvider, ISecurityPrincipal principal)
    {
      AccessType[] value;

      if (!_localCache.TryGetValue (principal, out value))
        value = _localCache.GetOrCreateValue (principal, delegate { return GetAccessFromGlobalCache (factory, securityProvider, principal); });

      return value;
    }

    private AccessType[] GetAccessFromGlobalCache (ISecurityContextFactory factory, ISecurityProvider securityProvider, ISecurityPrincipal principal)
    {
      var globalCache = _globalCache;
      var context = CreateSecurityContext (factory);
      var key = new GlobalAccessTypeCacheKey (context, principal);

      AccessType[] value;
      if (!globalCache.TryGetValue (key, out value))
        value = globalCache.GetOrCreateValue (key, delegate { return GetAccessFromSecurityProvider (securityProvider, context, principal); });

      return value;
    }

    private ISecurityContext CreateSecurityContext (ISecurityContextFactory factory)
    {
      using (new SecurityFreeSection())
      {
        var context = factory.CreateSecurityContext();
        if (context == null)
          throw new InvalidOperationException ("ISecurityContextFactory.CreateSecurityContext() evaluated and returned null.");
        return context;
      }
    }

    private AccessType[] GetAccessFromSecurityProvider (ISecurityProvider securityProvider, ISecurityContext context, ISecurityPrincipal principal)
    {
      return securityProvider.GetAccess (context, principal) ?? new AccessType[0];
    }
  }
}