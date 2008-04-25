using System;

namespace Remotion.Security.UnitTests.Core.SampleDomain
{
  public class SecurableObjectWithSecuredInstanceMethods : ISecurableObject
  {
    public SecurableObjectWithSecuredInstanceMethods ()
    {
    }

    [DemandMethodPermission (TestAccessTypes.First)]
    public void InstanceMethod ()
    {
    }

    [DemandMethodPermission (TestAccessTypes.Second)]
    public void InstanceMethod (string value)
    {
    }

    [DemandMethodPermission (TestAccessTypes.Third)]
    public void OtherInstanceMethod (string value)
    {
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public Type GetSecurableType ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
