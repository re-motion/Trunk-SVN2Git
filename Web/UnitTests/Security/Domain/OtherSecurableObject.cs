using System;
using Remotion.Security;

namespace Remotion.Web.UnitTests.Security.Domain
{
  public class OtherSecurableObject : ISecurableObject
  {
    private IObjectSecurityStrategy _securityStrategy;

    public OtherSecurableObject (IObjectSecurityStrategy securityStrategy)
    {
      _securityStrategy = securityStrategy;
    }

    [DemandMethodPermission (GeneralAccessTypes.Read)]
    public void Show ()
    {
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _securityStrategy;
    }

    public Type GetSecurableType ()
    {
      return GetType ();
    }
  }
}
