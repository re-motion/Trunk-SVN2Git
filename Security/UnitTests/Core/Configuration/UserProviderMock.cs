using System;
using System.Collections.Specialized;
using System.Security.Principal;
using Remotion.Configuration;

namespace Remotion.Security.UnitTests.Core.Configuration
{
  public class UserProviderMock : ExtendedProviderBase, IUserProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing


    public UserProviderMock (string name, NameValueCollection config)
        : base (name, config)
    {
    }
    
     // methods and properties

    public IPrincipal GetUser()
    {
      throw new NotImplementedException();
    }

    public bool IsNull
    {
      get { return false; }
    }
  }
}