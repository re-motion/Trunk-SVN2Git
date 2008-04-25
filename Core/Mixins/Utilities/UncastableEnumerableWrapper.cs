using System;
using System.Collections;
using System.Collections.Generic;

namespace Remotion.Mixins.Utilities
{
  /// <summary>
  /// Wraps an enumerable object so that the wrapped object cannot be cast back to its original type.
  /// </summary>
  /// <typeparam name="T">The type returned by the wrapped enumerable object.</typeparam>
  /// <remarks>Use this class when returning an enumerable object from a method to prevent that the object can be cast to its original type.
  /// That way, it will be ensured that the returned object only supports the methods exposed by the <see cref="IEnumerable{T}"/> interface.</remarks>
  public sealed class UncastableEnumerableWrapper<T> : IEnumerable<T>
  {
    private IEnumerable<T> _wrapped;

    public UncastableEnumerableWrapper (IEnumerable<T> wrapped)
    {
      _wrapped = wrapped;
    }

    public IEnumerator<T> GetEnumerator ()
    {
      return _wrapped.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}
