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
using Remotion.Collections;
using Remotion.Security.Configuration;
using Remotion.Utilities;

namespace Remotion.Security
{
  [Serializable]
  public class SecurityStrategy : ISecurityStrategy
  {
    private readonly ICache<ISecurityPrincipal, AccessType[]> _localCache;
    private readonly IGlobalAccessTypeCacheProvider _globalCacheProvider;

    public SecurityStrategy ()
      : this (new Cache<ISecurityPrincipal, AccessType[]> (), SecurityConfiguration.Current.GlobalAccessTypeCacheProvider)
    {
    }

    public SecurityStrategy (ICache<ISecurityPrincipal, AccessType[]> localCache, IGlobalAccessTypeCacheProvider globalCacheProvider)
    {
      ArgumentUtility.CheckNotNull ("localCache", localCache);
      ArgumentUtility.CheckNotNull ("globalCacheProvider", globalCacheProvider);

      _localCache = localCache;
      _globalCacheProvider = globalCacheProvider;
    }

    public ICache<ISecurityPrincipal, AccessType[]> LocalCache
    {
      get { return _localCache; }
    }

    public IGlobalAccessTypeCacheProvider GlobalCacheProvider
    {
      get { return _globalCacheProvider; }
    }

    public void InvalidateLocalCache ()
    {
      _localCache.Clear ();
    }

    public bool HasAccess (ISecurityContextFactory factory, ISecurityProvider securityProvider, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("factory", factory);
      ArgumentUtility.CheckNotNull ("securityProvider", securityProvider);
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      AccessType[] actualAccessTypes = GetAccessFromLocalCache (factory, securityProvider, principal);

      foreach (AccessType requiredAccessType in requiredAccessTypes)
      {
        if (Array.IndexOf (actualAccessTypes, requiredAccessType) < 0)
          return false;
      }

      return true;
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
      ICache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> globalAccessTypeCache = _globalCacheProvider.GetCache ();
      if (globalAccessTypeCache == null)
        throw new InvalidOperationException ("IGlobalAccesTypeCacheProvider.GetAccessTypeCache() evaluated and returned null.");

      ISecurityContext context = factory.CreateSecurityContext ();
      if (context == null)
        throw new InvalidOperationException ("ISecurityContextFactory.CreateSecurityContext() evaluated and returned null.");

      Tuple<ISecurityContext, ISecurityPrincipal> key = new Tuple<ISecurityContext, ISecurityPrincipal> (context, principal);

      AccessType[] value;
      if (!globalAccessTypeCache.TryGetValue (key, out value))
        value = globalAccessTypeCache.GetOrCreateValue (key, delegate { return GetAccessFromSecurityProvider (securityProvider, context, principal); });
  
      return value;
    }

    private AccessType[] GetAccessFromSecurityProvider (ISecurityProvider securityProvider, ISecurityContext context, ISecurityPrincipal principal)
    {
      AccessType[] actualAccessTypes = securityProvider.GetAccess (context, principal);
      if (actualAccessTypes == null)
        actualAccessTypes = new AccessType[0];
      return actualAccessTypes;
    }
  }
}
