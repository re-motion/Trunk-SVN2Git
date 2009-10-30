using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.Reflection.TypeDiscovery;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Provides a non-generic interface for the <see cref="ExtensibleEnumDefinition{T}"/> class.
  /// </summary>
  /// <remarks>
  /// Do not implement this interface yourself, use it only when working with extensible enums in a reflective context, e.g. via the 
  /// <see cref="ExtensibleEnumDefinitionCache"/> class.
  /// </remarks>
  public interface IExtensibleEnumDefinition
  {
    /// <summary>
    /// Gets the values defined by the extensible enum type.
    /// </summary>
    /// <returns>A <see cref="ReadOnlyCollection{T}"/> holding the values for the extensible enum type.</returns>
    /// <remarks>
    /// The values are retrieved by scanning all types found by <see cref="ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService"/>
    /// and discovering the extension methods defining values via <see cref="ExtensibleEnumValueDiscoveryService"/>.
    /// </remarks>
    ReadOnlyCollection<IExtensibleEnum> GetValues ();

    /// <summary>
    /// Gets the enum value identified by <paramref name="id"/>, throwing an exception if the value cannot be found.
    /// </summary>
    /// <param name="id">The identifier of the enum value to return.</param>
    /// <returns>The enum value identified by <paramref name="id"/>.</returns>
    /// <exception cref="KeyNotFoundException">No enum value with the given <paramref name="id"/> exists.</exception>
    IExtensibleEnum GetValueByID (string id);

    /// <summary>
    /// Gets the enum value identified by <paramref name="id"/>, returning a boolean value indicating whether 
    /// such a value could be found.
    /// </summary>
    /// <param name="id">The identifier of the enum value to return.</param>
    /// <param name="value">The enum value identified by <paramref name="id"/>, or <see langword="null" /> if no such value exists.</param>
    /// <returns>
    /// <see langword="true" /> if a value with the given <paramref name="id"/> could be found; <see langword="false" /> otherwise.
    /// </returns>
    bool TryGetValueByID (string id, out IExtensibleEnum value);
  }
}