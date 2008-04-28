using System;
using System.Collections.Specialized;
using Remotion.Configuration;

namespace Remotion.Security.UnitTests.Core.Configuration
{
  public class SecurityProviderMock : ExtendedProviderBase, ISecurityProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing


    public SecurityProviderMock (string name, NameValueCollection config)
        : base (name, config)
    {
    }
    
     // methods and properties

    public AccessType[] GetAccess (ISecurityContext context, System.Security.Principal.IPrincipal user)
    {
      return new AccessType[0];
    }

    public int GetRevision ()
    {
      return 0;
    }

    public bool IsNull
    {
      get { return false; }
    }
  }
}