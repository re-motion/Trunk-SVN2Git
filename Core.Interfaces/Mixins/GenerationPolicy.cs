using System;

namespace Remotion.Mixins
{
  /// <summary>
  /// Defines how the <see cref="TypeFactory"/> and <see cref="ObjectFactory"/> behave when asked to generate a concrete type for a target
  /// type without any mixin configuration information.
  /// </summary>
  public enum GenerationPolicy
  {
    /// <summary>
    /// Specifies that <see cref="TypeFactory"/> and <see cref="ObjectFactory"/> should always generate concrete types, no matter whether
    /// mixin configuration information exists for the given target type or not.
    /// </summary>
    ForceGeneration,
    /// <summary>
    /// Specifies that <see cref="TypeFactory"/> and <see cref="ObjectFactory"/> should only generate concrete types if
    /// mixin configuration information exists for the given target type.
    /// </summary>
    GenerateOnlyIfConfigured
  }
}