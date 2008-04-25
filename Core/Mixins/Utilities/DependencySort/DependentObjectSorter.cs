using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities.DependencySort
{
  public class DependentObjectSorter<T>
  {
    private IDependencyAnalyzer<T> _analyzer;

    public DependentObjectSorter (IDependencyAnalyzer<T> analyzer)
    {
      ArgumentUtility.CheckNotNull ("analyzer", analyzer);
      _analyzer = analyzer;
    }

    public IEnumerable<T> SortDependencies (IEnumerable<T> dependentObjects)
    {
      ArgumentUtility.CheckNotNull ("dependentObjects", dependentObjects);
      DependentObjectSorterAlgorithm<T> algorithm = new DependentObjectSorterAlgorithm<T> (_analyzer, dependentObjects);
      return algorithm.Execute ();
    }
  }
}