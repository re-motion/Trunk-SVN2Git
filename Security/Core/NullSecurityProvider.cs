using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Security.Principal;
using Remotion.Configuration;

namespace Remotion.Security
{
  /// <summary>
  /// Provides an implementation of a nullable object according to the "Null Object Pattern", 
  /// extending <see cref="ProviderBase"/> and implementing <see cref="ISecurityProvider"/>.
  /// </summary>
  public class NullSecurityProvider : ExtendedProviderBase, ISecurityProvider
  {
    public NullSecurityProvider ()
      : this ("Null", new NameValueCollection ())
    {
    }

    public NullSecurityProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    /// <summary>
    /// The "Null Object" implementation always returns an empty array.
    /// </summary>
    /// <returns>Always returns an empty array.</returns>
    public AccessType[] GetAccess (SecurityContext context, IPrincipal user)
    {
      return new AccessType[0];
    }

    /// <summary>
    /// The "Null Object" implementation always returns 0.
    /// </summary>
    /// <returns>Always returns 0 for the revision.</returns>
    public int GetRevision ()
    {
      return 0;
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}