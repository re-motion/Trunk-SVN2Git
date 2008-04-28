using System;

namespace Remotion.Security
{
  /// <summary>Defines the signature for a factory method to create a <see cref="ISecurityContext"/> for a buiness object.</summary>
  /// <remarks><note type="implementnotes">Typically implemented by business objects (acting as their own <see cref="ISecurityContextFactory"/>).</note></remarks>
  public interface ISecurityContextFactory
  {
    /// <summary>Gets the <see cref="ISecurityContext"/> for a business object.</summary>
    /// <returns>The <see cref="ISecurityContext"/> for a business object.</returns>
    ISecurityContext CreateSecurityContext ();
  }
}
