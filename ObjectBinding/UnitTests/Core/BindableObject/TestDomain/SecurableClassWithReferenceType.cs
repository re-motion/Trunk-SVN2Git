using System;
using Remotion.Security;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  public class SecurableClassWithReferenceType<T> : ClassWithReferenceType<T>, ISecurableObject
      where T: class
  {
    private readonly IObjectSecurityStrategy _objectSecurityStrategy;

    public SecurableClassWithReferenceType (IObjectSecurityStrategy objectSecurityStrategy)
    {
      _objectSecurityStrategy = objectSecurityStrategy;
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _objectSecurityStrategy;
    }

    public Type GetSecurableType ()
    {
      return typeof (SecurableClassWithReferenceType<T>);
    }
  }
}