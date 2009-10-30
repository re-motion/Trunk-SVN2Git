using System;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Defines a common interface for extensible enum values.
  /// </summary>
  public interface IExtensibleEnum
  {
    /// <summary>
    /// Gets the identifier representing this extensible enum value.
    /// </summary>
    /// <value>The ID of this value.</value>
    string ID { get; }
  }
}