using System;
using Remotion.Collections;

namespace Remotion.Security
{
  /// <summary>
  /// Cache for the <see cref="AccessType"/> array, using the <see cref="SecurityContext"/> and the user name (<see cref="string"/>) as key.
  /// These are used as parameters for each call to the <see cref="ISecurityProvider.GetAccess"/> method of <see cref="ISecurityProvider"/>.
  /// </summary>
  /// <remarks><note type="implementnotes">Implementations are free to implement their own best practice for keeping the cache up to date.</note></remarks>
  public interface IGlobalAccessTypeCacheProvider : INullObject
  {
    /// <summary>
    /// Gets the <see cref="ICache{T, S}"/> for the <see cref="SecurityContext"/> and user name (<see cref="string"/>) key pair.
    /// </summary>
    /// <returns>The <see cref="ICache{T, S}"/> in use.</returns>
    ICache<Tuple<ISecurityContext, string>, AccessType[]> GetCache ();
  }
}