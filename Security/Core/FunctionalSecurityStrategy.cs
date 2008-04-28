using System;
using System.Security.Principal;
using Remotion.Collections;
using Remotion.Security.Configuration;
using Remotion.Utilities;

namespace Remotion.Security
{
  public class FunctionalSecurityStrategy : IFunctionalSecurityStrategy
  {
    private readonly ISecurityStrategy _securityStrategy;

    public FunctionalSecurityStrategy (ISecurityStrategy securityStrategy)
    {
      ArgumentUtility.CheckNotNull ("securityStrategy", securityStrategy);

      _securityStrategy = securityStrategy;
    }

    public FunctionalSecurityStrategy ()
      : this (new SecurityStrategy (new NullCache<string, AccessType[]> (), SecurityConfiguration.Current.GlobalAccessTypeCacheProvider))
    {
    }

    public ISecurityStrategy SecurityStrategy
    {
      get { return _securityStrategy; }
    }

    public bool HasAccess (Type type, ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableObject));
      ArgumentUtility.CheckNotNull ("securityProvider", securityProvider);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      return _securityStrategy.HasAccess (new FunctionalSecurityContextFactory (type), securityProvider, user, requiredAccessTypes);
    }
  }
}
