using System;
using Remotion.Security;

namespace Remotion.Web.UnitTests.Security.Domain
{
  public class SecurableObject : ISecurableObject
  {
    public enum Method
    {
      Delete,
      Show,
      Search
    }

    [DemandMethodPermission (GeneralAccessTypes.Search)]
    public static void Search ()
    {
    }

    private IObjectSecurityStrategy _securityStrategy;

    public SecurableObject (IObjectSecurityStrategy securityStrategy)
    {
      _securityStrategy = securityStrategy;
    }

    [DemandMethodPermission (GeneralAccessTypes.Read)]
    public void Show ()
    {
    }

    [DemandMethodPermission (GeneralAccessTypes.Delete)]
    public void Delete ()
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
