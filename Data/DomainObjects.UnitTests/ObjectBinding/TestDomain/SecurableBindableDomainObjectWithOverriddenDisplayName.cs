using System;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [Instantiable]
  [Serializable]
  public abstract class SecurableBindableDomainObjectWithOverriddenDisplayName : BindableDomainObjectWithOverriddenDisplayName, ISecurableObject
  {
    public static SecurableBindableDomainObjectWithOverriddenDisplayName NewObject (IObjectSecurityStrategy objectSecurityStrategy)
    {
      return DomainObject.NewObject<SecurableBindableDomainObjectWithOverriddenDisplayName> ().With (objectSecurityStrategy);
    }

    public static new SecurableBindableDomainObjectWithOverriddenDisplayName GetObject (ObjectID id)
    {
      return DomainObject.GetObject<SecurableBindableDomainObjectWithOverriddenDisplayName> (id);
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
