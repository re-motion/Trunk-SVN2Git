using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Security.Principal;
using System.Web;
using Remotion.Configuration;
using Remotion.Security;

namespace Remotion.Web.Security
{
  public class HttpContextUserProvider : ExtendedProviderBase, IUserProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public HttpContextUserProvider()
        : this ("HttpContext", new NameValueCollection())
    {
    }

    public HttpContextUserProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }
    
     // methods and properties

    public IPrincipal GetUser()
    {
      if (HttpContext.Current == null)
        return null;
      else
        return HttpContext.Current.User;
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}