using System;
using Remotion.ExtensibleEnums.Infrastructure;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Defines a non-generic interface for the <see cref="ExtensibleEnum{T}"/> class.
  /// </summary>
  /// <remarks>
  /// Do not implement this interface yourself, use it only when working with extensible enums in a reflective context, e.g. via the 
  /// <see cref="ExtensibleEnumDefinitionCache"/> class.
  /// </remarks>
  public interface IExtensibleEnum
  {
    /// <summary>
    /// Gets the identifier representing this extensible enum value. This is the combination of <see cref="IDPrefix"/> and <see cref="ShortID"/>.
    /// </summary>
    /// <value>The ID of this value.</value>
    string ID { get; }

    /// <summary>
    /// Gets the ID prefix. This is used to form the <see cref="ID"/> representing this extensible enum value.
    /// </summary>
    /// <value>The ID prefix of this value.</value>
    string IDPrefix { get; }

    /// <summary>
    /// Gets the short ID. This is used to form the <see cref="ID"/> representing this extensible enum value.
    /// </summary>
    /// <value>The short ID of this value.</value>
    string ShortID { get; }
  }
}