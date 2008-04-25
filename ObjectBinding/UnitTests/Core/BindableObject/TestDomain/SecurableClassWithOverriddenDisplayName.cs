using System;
using Remotion.Security;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  public class SecurableClassWithOverriddenDisplayName : ClassWithOverriddenDisplayName, ISecurableObject
  {
    private readonly IObjectSecurityStrategy _objectSecurityStrategy;

    public SecurableClassWithOverriddenDisplayName (IObjectSecurityStrategy objectSecurityStrategy)
    {
      _objectSecurityStrategy = objectSecurityStrategy;
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _objectSecurityStrategy;
    }

    public Type GetSecurableType ()
    {
      return typeof (SecurableClassWithOverriddenDisplayName);
    }
  }
}