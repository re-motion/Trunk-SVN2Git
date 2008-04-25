using System;
using System.Security.Principal;

namespace Remotion.Security
{
  /// <summary>
  /// Represents a nullable <see cref="IPrincipal"/> according to the "Null Object Pattern".
  /// </summary>
  public class NullPrincipal : IPrincipal
  {
    private NullIdentity _identity = new NullIdentity();

    public NullPrincipal()
    {
    }

    public bool IsInRole (string role)
    {
      return false;
    }

    public IIdentity Identity
    {
      get { return _identity; }
    }
  }
}