using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.Infrastructure.Interception
{
  /// <summary>
  /// This exception is thrown when the property interception mechanism cannot be applied to a specific <see cref="DomainObject"/> type
  /// because of problems with that type's declaration.
  /// </summary>
  public class NonInterceptableTypeException : Exception
  {
    /// <summary>
    /// The type that cannot be intercepted.
    /// </summary>
    public readonly Type Type;

    /// <summary>
    /// Initializes a new instance of the <see cref="NonInterceptableTypeException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="type">The type that cannot be intercepted.</param>
    public NonInterceptableTypeException (string message, Type type)
        : base (message)
    {
      Type = type;
    }
  }
}