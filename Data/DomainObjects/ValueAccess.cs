using System;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// An value indicating whether the original or current value of a <see cref="PropertyValue"/> is being accessed.
  /// </summary>
  public enum ValueAccess
  {
    /// <summary>
    /// The original value is being accessed.
    /// </summary>
    Original = 0,

    /// <summary>
    /// The current value is being accessed.
    /// </summary>
    Current = 1
  }
}
