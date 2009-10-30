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
    /// Gets the identifier representing this extensible enum value.
    /// </summary>
    /// <value>The ID of this value.</value>
    string ID { get; }
  }
}