using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Threading;
using Remotion.Configuration;

namespace Remotion.Security
{
  public class ThreadUserProvider: ExtendedProviderBase, IUserProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public ThreadUserProvider()
        : this ("Thread", new NameValueCollection())
    {
    }

    public ThreadUserProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    // methods and properties

    public IPrincipal GetUser()
    {
      return Thread.CurrentPrincipal;
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}