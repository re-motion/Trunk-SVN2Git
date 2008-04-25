using System;
using System.Security.Principal;

namespace Remotion.Security
{
  //TODO FS: Move to SecurityInterfaces
  /// <summary>Defines an interface for retrieving the current user.</summary>
  public interface IUserProvider : INullObject
  {
    /// <summary>Gets the current user.</summary>
    /// <returns>The <see cref="IPrincipal"/> representing the current user.</returns>
    IPrincipal GetUser();
  }
}