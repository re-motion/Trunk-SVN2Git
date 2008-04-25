using System;
using System.Collections.Specialized;
using System.Security.Principal;
using Remotion.Configuration;

namespace Remotion.Security
{
  /// <summary>
  /// Represents a nullable <see cref="IUserProvider"/> according to the "Null Object Pattern".
  /// </summary>
  public class NullUserProvider: ExtendedProviderBase, IUserProvider
  {
    private NullPrincipal _principal = new NullPrincipal();

    public NullUserProvider()
        : this ("Null", new NameValueCollection())
    {
    }

    public NullUserProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    public IPrincipal GetUser()
    {
      return _principal;
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}