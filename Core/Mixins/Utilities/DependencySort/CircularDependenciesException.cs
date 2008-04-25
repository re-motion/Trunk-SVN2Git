using System;
using System.Collections.Generic;

namespace Remotion.Mixins.Utilities.DependencySort
{
  public class CircularDependenciesException<T> : Exception
  {
    public readonly IEnumerable<T> Circulars;

    public CircularDependenciesException (string message, IEnumerable<T> circulars)
        : base (message)
    {
      Circulars = circulars;
    }

    public CircularDependenciesException (string message, IEnumerable<T> circulars, Exception inner)
        : base (message, inner)
    {
      Circulars = circulars;
    }
  }
}