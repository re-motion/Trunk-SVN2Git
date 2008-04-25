using System;
using Rhino.Mocks;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.NullSecurityClientTests
{
  public class NullSecurityClientTestHelper
  {
    public static NullSecurityClientTestHelper CreateForStatelessSecurity()
    {
      return new NullSecurityClientTestHelper();
    }

    public static NullSecurityClientTestHelper CreateForStatefulSecurity()
    {
      return new NullSecurityClientTestHelper();
    }

    private MockRepository _mocks;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private SecurableObject _securableObject;

    private NullSecurityClientTestHelper()
    {
      _mocks = new MockRepository();
      _mockObjectSecurityStrategy = _mocks.CreateMock<IObjectSecurityStrategy>();

      _securableObject = new SecurableObject (_mockObjectSecurityStrategy);
    }

    public NullSecurityClient CreateSecurityClient()
    {
      return new NullSecurityClient();
    }

    public SecurableObject SecurableObject
    {
      get { return _securableObject; }
    }

    public void ReplayAll()
    {
      _mocks.ReplayAll();
    }

    public void VerifyAll()
    {
      _mocks.VerifyAll();
    }
  }
}