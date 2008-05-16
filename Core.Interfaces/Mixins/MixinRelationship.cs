using System;

namespace Remotion.Mixins
{
  /// <summary>
  /// Describes how a mixin influences its target class.
  /// </summary>
  public enum MixinKind
  {
    /// <summary>
    /// The mixin extends the target class from the outside, the target class might not know about being mixed. The mixin therefore has the
    /// possibility to override attributes (with <see cref="AttributeUsageAttribute.AllowMultiple"/> set to false) and interfaces declared
    /// or implemented by the target class.
    /// </summary>
    Extending,
    /// <summary>
    /// The mixin is explicitly used by the target class. The mixin therefore behaves more like a base class, eg. attributes (with 
    /// <see cref="AttributeUsageAttribute.AllowMultiple"/> set to false) and interfaces introduced by the mixin can be overridden by the 
    /// target class.
    /// </summary>
    Used
  }
}