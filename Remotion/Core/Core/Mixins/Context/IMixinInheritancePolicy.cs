using System;
using System.Collections.Generic;

namespace Remotion.Mixins.Context
{
  /// <summary>
  /// Provides a public interface for objects encapsulating the rules governing mixin inheritance for target types.
  /// </summary>
  public interface IMixinInheritancePolicy
  {
    /// <summary>
    /// Gets the types this <paramref name="targetType"/> inherits mixins from. A target type inherits mixins from its generic type definition,
    /// its base class, and its interfaces.
    /// </summary>
    /// <param name="targetType">The type whose "base" types should be retrieved.</param>
    /// <returns>The types from which the given <paramref name="targetType"/> inherits its mixins.</returns>
    IEnumerable<Type> GetTypesToInheritFrom (Type targetType);

    /// <summary>
    /// Gets the class contexts this <paramref name="targetType"/> inherits mixins from. A target type inherits mixins from its generic type 
    /// definition, its base class, and its interfaces.
    /// </summary>
    /// <param name="targetType">The type whose "base" <see cref="ClassContext"/> instances should be retrieved.</param>
    /// <param name="classContextRetriever">A function returning the <see cref="ClassContext"/> for a given type.</param>
    /// <returns><see cref="ClassContext"/> objects for the types from which the given <paramref name="targetType"/> inherits its mixins.</returns>
    IEnumerable<ClassContext> GetClassContextsToInheritFrom (Type targetType, Func<Type, ClassContext> classContextRetriever);
  }
}