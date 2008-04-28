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
    private readonly ICache<string, AccessType[]> _localCache;
    private readonly IGlobalAccessTypeCacheProvider _globalCacheProvider;

    public SecurityStrategy ()
      : this (new Cache<string, AccessType[]> (), SecurityConfiguration.Current.GlobalAccessTypeCacheProvider)
    {
    }

    public SecurityStrategy (ICache<string, AccessType[]> localCache, IGlobalAccessTypeCacheProvider globalCacheProvider)
    {
      ArgumentUtility.CheckNotNull ("localCache", localCache);
      ArgumentUtility.CheckNotNull ("globalCacheProvider", globalCacheProvider);

      _localCache = localCache;
      _globalCacheProvider = globalCacheProvider;
    }

    public ICache<string, AccessType[]> LocalCache
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

    public bool HasAccess (ISecurityContextFactory factory, ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("factory", factory);
      ArgumentUtility.CheckNotNull ("securityProvider", securityProvider);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      AccessType[] actualAccessTypes = GetAccessFromLocalCache (factory, securityProvider, user);

      foreach (AccessType requiredAccessType in requiredAccessTypes)
      {
        if (Array.IndexOf<AccessType> (actualAccessTypes, requiredAccessType) < 0)
          return false;
      }

      return true;
    }

    private AccessType[] GetAccessFromLocalCache (ISecurityContextFactory factory, ISecurityProvider securityProvider, IPrincipal user)
    {
      return _localCache.GetOrCreateValue (user.Identity.Name, delegate { return GetAccessFromGlobalCache (factory, securityProvider, user); });
    }

    private AccessType[] GetAccessFromGlobalCache (ISecurityContextFactory factory, ISecurityProvider securityProvider, IPrincipal user)
    {
      ICache<Tuple<ISecurityContext, string>, AccessType[]> globalAccessTypeCache = _globalCacheProvider.GetCache ();
      if (globalAccessTypeCache == null)
        throw new InvalidOperationException ("IGlobalAccesTypeCacheProvider.GetAccessTypeCache() evaluated and returned null.");

      ISecurityContext context = factory.CreateSecurityContext ();
      if (context == null)
        throw new InvalidOperationException ("ISecurityContextFactory.CreateSecurityContext() evaluated and returned null.");

      Tuple<ISecurityContext, string> key = new Tuple<ISecurityContext, string> (context, user.Identity.Name);
      return globalAccessTypeCache.GetOrCreateValue (key, delegate { return GetAccessFromSecurityProvider (securityProvider, context, user); });
    }

    private AccessType[] GetAccessFromSecurityProvider (ISecurityProvider securityProvider, ISecurityContext context, IPrincipal user)
    {
      AccessType[] actualAccessTypes = securityProvider.GetAccess (context, user);
      if (actualAccessTypes == null)
        actualAccessTypes = new AccessType[0];
      return actualAccessTypes;
    }
  }
}
