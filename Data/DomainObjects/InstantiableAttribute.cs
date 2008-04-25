using System;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// When the <see cref="InstantiableAttribute"/> is defined on a type, it signals that this type can be instantiated by the
  /// <see cref="DomainObject"/> infrastructure even though it declared as <see langword="abstract"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class InstantiableAttribute: Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="InstantiableAttribute"/> class.
    /// </summary>
    public InstantiableAttribute()
    {
    }
  }
}