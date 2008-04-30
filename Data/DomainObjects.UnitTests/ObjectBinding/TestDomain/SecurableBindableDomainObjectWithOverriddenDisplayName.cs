using System;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [Instantiable]
  [Serializable]
  public abstract class SecurableBindableDomainObjectWithOverriddenDisplayName : BindableDomainObjectWithOverriddenDisplayName, ISecurableObject
  {
    public static SecurableBindableDomainObjectWithOverriddenDisplayName NewObject (IObjectSecurityStrategy objectSecurityStrategy)
    {
      return NewObject<SecurableBindableDomainObjectWithOverriddenDisplayName> ().With (objectSecurityStrategy);
    }

    public static new SecurableBindableDomainObjectWithOverriddenDisplayName GetObject (ObjectID id)
    {
      return GetObject<SecurableBindableDomainObjectWithOverriddenDisplayName> (id);
    }

    private readonly IObjectSecurityStrategy _objectSecurityStrategy;

    protected SecurableBindableDomainObjectWithOverriddenDisplayName (IObjectSecurityStrategy objectSecurityStrategy)
    {
      _objectSecurityStrategy = objectSecurityStrategy;
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _objectSecurityStrategy;
    }

    public Type GetSecurableType ()
    {
      return typeof (SecurableBindableDomainObjectWithOverriddenDisplayName);
    }
  }
}
