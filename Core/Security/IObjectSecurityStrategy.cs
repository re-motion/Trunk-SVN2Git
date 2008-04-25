using System;
using System.Security.Principal;

namespace Remotion.Security
{
  //TODO FS: Move to SecurityInterfaces
  /// <summary>Encapsulates the security checks for the business object.</summary>
  /// <remarks>
  /// Typically the <see cref="IObjectSecurityStrategy"/> knows its business object (possibly indirectly) 
  /// and uses the security-relevant object state (as part of the <see cref="SecurityContext"/>) as 
  /// parameter when evaluating the required permissions.
  /// <note type="implementnotes">Implementations are free to decide whether they provide caching.</note>
  /// </remarks>
  public interface IObjectSecurityStrategy
  {
    /// <summary>Determines whether the requested access is granted.</summary>
    /// <param name="securityProvider">The <see cref="ISecurityProvider"/> used to determine the permissions.</param>
    /// <param name="user">The <see cref="IPrincipal"/> on whose behalf the permissions are evaluated.</param>
    /// <param name="requiredAccessTypes">The access rights required for the access to be granted.</param>
    /// <returns><see langword="true"/> if the <paramref name="requiredAccessTypes"/> are granted.</returns>
    /// <remarks>
    /// Typically called via <see cref="O:Remotion.Security.SecurityClient.HasAccess"/> of 
    /// <see cref="T:Remotion.Security.SecurityClient"/>.
    /// The strategy incorporates <see cref="SecurityContext"/> in the permission query.
    /// The <paramref name="requiredAccessTypes"/> are determined by the <see cref="T:Remotion.Security.SecurityClient"/>, 
    /// taking the business object instance and the member name (property or method) into account.
    /// </remarks>
    bool HasAccess (ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes);
  }
}
