using System;
using System.Security.Principal;

namespace Remotion.Security
{
  //TODO FS: Move to SecurityInterfaces
  /// <summary>Encapsulates the security checks.</summary>
  /// <remarks><note type="implementnotes">Implementations are free to decide whether they provide caching.</note></remarks>
  public interface ISecurityStrategy
  {
    /// <summary>Determines whether the requested access is granted.</summary>
    /// <param name="factory">The <see cref="ISecurityContextFactory"/> to be used.</param>
    /// <param name="securityProvider">The <see cref="ISecurityProvider"/> used to determine the permissions.</param>
    /// <param name="user">The <see cref="IPrincipal"/> on whose behalf the permissions are evaluated.</param>
    /// <param name="requiredAccessTypes">The access rights required for the access to be granted.</param>
    /// <returns><see langword="true"/> if the <paramref name="requiredAccessTypes"/> are granted.</returns>
    /// <remarks>
    /// <note type="implementnotes">
    /// When caching is provided by the implementation, <see cref="ISecurityContextFactory.CreateSecurityContext"/> of the <paramref name="factory"/>
    /// shall only be called when the local cache does not already have a reference to a <see cref="SecurityContext"/>.
    /// </note>
    /// </remarks>
    bool HasAccess (ISecurityContextFactory factory, ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes);
    
    /// <summary>Clears the cached access types of the <see cref="ISecurableObject"/> associated with this <see cref="ISecurityStrategy"/>.</summary>
    /// <remarks>Called by application code when <see cref="ISecurableObject"/> properties that are relevant for the <see cref="SecurityContext"/> change.</remarks>
    void InvalidateLocalCache ();
  }
}
