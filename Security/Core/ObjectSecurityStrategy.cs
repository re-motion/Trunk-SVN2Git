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
