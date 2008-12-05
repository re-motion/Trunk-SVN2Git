// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities.DependencySort
{
  public class DependentObjectSorterAlgorithm<T>
  {
    private IDependencyAnalyzer<T> _analyzer;
    private Set<T> _objects;

    public DependentObjectSorterAlgorithm (IDependencyAnalyzer<T> analyzer, IEnumerable<T> objects)
    {
      ArgumentUtility.CheckNotNull ("objects", objects);
      _analyzer = analyzer;
      _objects = new Set<T> (objects);
    }

    public IEnumerable<T> Execute ()
    {
      while (_objects.Count > 0)
      {
        T root = GetRoot ();
        yield return root;
        _objects.Remove (root);
      }
    }

    private T GetRoot ()
    {
      Set<T> rootCandidates = new Set<T> (_objects);
      foreach (T first in _objects)
      {
        foreach (T second in _objects)
        {
          if (first.Equals (second))
          {
            if (_analyzer.AnalyzeDirectDependency (first, second) != DependencyKind.None)
            {
              string message = string.Format ("Item '{0}' depends on itself.", first);
              throw new CircularDependenciesException<T> (message, new T[] { first });
            }
          }
          else
          {
            switch (_analyzer.AnalyzeDirectDependency (first, second))
            {
              case DependencyKind.FirstOnSecond:
                rootCandidates.Remove (second);
                break;
              case DependencyKind.SecondOnFirst:
                rootCandidates.Remove (first);
                goto nextOuter;
            }
          }
        }
      nextOuter:
        ;
      }
      if (rootCandidates.Count == 0)
      {
        string message = string.Format ("The object graph contains circular dependencies involving items {{{0}}}, no root object can be found.",
            SeparatedStringBuilder.Build (", ", _objects, delegate (T t) { return t.ToString (); })); 
        throw new CircularDependenciesException<T> (message, _objects);
      }
      else if (rootCandidates.Count == 1)
      {
        using (IEnumerator<T> enumerator = rootCandidates.GetEnumerator())
        {
          enumerator.MoveNext ();
          return enumerator.Current;
        }
      }
      else
      {
        Assertion.IsTrue (rootCandidates.Count > 1);
        return _analyzer.ResolveEqualRoots (rootCandidates);
      }
    }
  }
}
