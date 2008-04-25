using System;

namespace Remotion.Security
{
  //TODO FS: Move to SecurityInterfaces
  /// <summary>The base interface for all business objects that need security features.</summary>
  public interface ISecurableObject
  {
    /// <summary>Gets the <see cref="IObjectSecurityStrategy"/> used by that business object.</summary>
    /// <remarks>Primarily used by a <see cref="T:Remotion.Security.SecurityClient"/> to dispatch security checks.</remarks>
    /// <returns>Returns the <see cref="IObjectSecurityStrategy"/>.</returns>
    IObjectSecurityStrategy GetSecurityStrategy ();

    /// <summary>Gets the <see cref="Type"/> representing the <see cref="ISecurableObject"/> in the security infrastructure.</summary>
    /// <returns>Return a <see cref="Type"/> object.</returns>
    Type GetSecurableType ();
  }
}
