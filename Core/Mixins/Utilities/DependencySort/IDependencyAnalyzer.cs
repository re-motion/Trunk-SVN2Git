using System;
using System.Collections.Generic;

namespace Remotion.Mixins.Utilities.DependencySort
{
  public interface IDependencyAnalyzer<T>
  {
    DependencyKind AnalyzeDirectDependency (T first, T second);
    T ResolveEqualRoots (IEnumerable<T> equalRoots);
  }
}