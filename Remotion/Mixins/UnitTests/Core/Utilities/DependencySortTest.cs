// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Mixins.Utilities.DependencySort;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.Utilities
{
  [TestFixture]
  [Obsolete ("This class will been removed. Use MixinDefinitionSorter instead. (1.13.175.0)")]
  public class DependencySortTest
  {
    private class SimpleDependencyAnalyzer<T> : IDependencyAnalyzer<T>
    {
      private Dictionary<T, Set<T>> _dependencies = new Dictionary<T, Set<T>> ();

      public void AddDependency (T first, T second)
      {
        GetDependencies (first).Add (second);
      }

      private Set<T> GetDependencies (T first)
      {
        if (!_dependencies.ContainsKey (first))
          _dependencies.Add (first, new Set<T>());
        return _dependencies[first];
      }

      public DependencyKind AnalyzeDirectDependency (T first, T second)
      {
        if (GetDependencies (first).Contains (second))
          return DependencyKind.FirstOnSecond;
        else if (GetDependencies (second).Contains (first))
          return DependencyKind.SecondOnFirst;
        else
          return DependencyKind.None;
      }

      public T ResolveEqualRoots (IEnumerable<T> equalRoots)
      {
        List<T> roots = new List<T> (equalRoots);
        roots.Sort();
        return roots[0];
      }
    }

    private IEnumerable<T> AnalyzeDependencies<T> (IEnumerable<T> objectsEnumerable, IDependencyAnalyzer<T> analyzer)
    {
      ArgumentUtility.CheckNotNull ("objectsEnumerable", objectsEnumerable);
      ArgumentUtility.CheckNotNull ("analyzer", analyzer);

      DependentObjectSorter<T> sorter = new DependentObjectSorter<T>(analyzer);
      return sorter.SortDependencies (objectsEnumerable);
    }

    private void AssertListsEqualInOrder<T> (IEnumerable<T> expected, IEnumerable<T> actual)
    {
      List<T> expectedList = new List<T> (expected);
      List<T> actualList = new List<T> (actual);
      Assert.AreEqual (expectedList.Count, actualList.Count);
      for (int i = 0; i < expectedList.Count; ++i)
        Assert.AreEqual (expectedList[i], actualList[i]);
    }

    [Test]
    public void SortDependentObjectsList ()
    {
      string[] dependentObjects = new string[] {"h", "g", "f", "e", "d", "c", "b", "a"};
      SimpleDependencyAnalyzer<string> analyzer = new SimpleDependencyAnalyzer<string>();
      analyzer.AddDependency ("a", "b");
      analyzer.AddDependency ("b", "c");
      analyzer.AddDependency ("c", "d");
      analyzer.AddDependency ("d", "e");
      analyzer.AddDependency ("e", "f");
      analyzer.AddDependency ("f", "g");
      analyzer.AddDependency ("g", "h");

      IEnumerable<string> sortedObjects = AnalyzeDependencies (dependentObjects, analyzer);
      AssertListsEqualInOrder (new string[] { "a", "b", "c", "d", "e", "f", "g", "h" }, sortedObjects);
    }

    [Test]
    public void SortDependentObjectsGraph1 ()
    {
      string[] dependentObjects = new string[] { "h", "g", "f", "e", "d", "c", "b", "a" };
      SimpleDependencyAnalyzer<string> analyzer = new SimpleDependencyAnalyzer<string> ();
      analyzer.AddDependency ("a", "b");
      analyzer.AddDependency ("b", "c");
      analyzer.AddDependency ("c", "d");
      analyzer.AddDependency ("c", "e");
      analyzer.AddDependency ("c", "h");
      analyzer.AddDependency ("d", "f");
      analyzer.AddDependency ("e", "g");
      analyzer.AddDependency ("g", "d");
      analyzer.AddDependency ("g", "d");
      analyzer.AddDependency ("f", "h");

      IEnumerable<string> sortedObjects = AnalyzeDependencies (dependentObjects, analyzer);
      AssertListsEqualInOrder (new string[] { "a", "b", "c", "e", "g", "d", "f", "h" }, sortedObjects);
    }

    [Test]
    public void SortDependentObjectsGraph2 ()
    {
      string[] dependentObjects = new string[] { "h", "g", "f", "e", "d", "c", "b", "a" };
      SimpleDependencyAnalyzer<string> analyzer = new SimpleDependencyAnalyzer<string> ();
      analyzer.AddDependency ("a", "b");
      analyzer.AddDependency ("a", "c");
      analyzer.AddDependency ("a", "d");
      analyzer.AddDependency ("b", "e");
      analyzer.AddDependency ("c", "b");
      analyzer.AddDependency ("c", "f");
      analyzer.AddDependency ("d", "f");
      analyzer.AddDependency ("e", "d");
      analyzer.AddDependency ("e", "g");
      analyzer.AddDependency ("f", "g");
      analyzer.AddDependency ("g", "h");

      IEnumerable<string> sortedObjects = AnalyzeDependencies (dependentObjects, analyzer);
      AssertListsEqualInOrder (new string[] { "a", "c", "b", "e", "d", "f", "g", "h" }, sortedObjects);
    }

    [Test]
    public void SortDependentObjectsGraphWithMultiRoots1 ()
    {
      string[] dependentObjects = new string[] { "h", "g", "f", "e", "d", "c", "b", "a" };
      SimpleDependencyAnalyzer<string> analyzer = new SimpleDependencyAnalyzer<string> ();
      analyzer.AddDependency ("a", "d");
      analyzer.AddDependency ("b", "e");
      analyzer.AddDependency ("c", "f");
      analyzer.AddDependency ("d", "f");
      analyzer.AddDependency ("e", "d");
      analyzer.AddDependency ("f", "g");
      analyzer.AddDependency ("f", "h");

      IEnumerable<string> sortedObjects = AnalyzeDependencies (dependentObjects, analyzer);
      AssertListsEqualInOrder (new string[] { "a", "b", "c", "e", "d", "f", "g", "h" }, sortedObjects);
    }

    [Test]
    public void SortDependentObjectsGraphWithMultiRoots2 ()
    {
      string[] dependentObjects = new string[] { "h", "g", "f", "e", "d", "c", "b", "a" };
      SimpleDependencyAnalyzer<string> analyzer = new SimpleDependencyAnalyzer<string> ();
      analyzer.AddDependency ("a", "c");
      analyzer.AddDependency ("b", "d");
      analyzer.AddDependency ("c", "g");
      analyzer.AddDependency ("d", "c");
      analyzer.AddDependency ("f", "e");

      IEnumerable<string> sortedObjects = AnalyzeDependencies (dependentObjects, analyzer);
      AssertListsEqualInOrder (new string[] { "a", "b", "d", "c", "f", "e", "g", "h" }, sortedObjects);
    }

    [Test]
    [ExpectedException (typeof (CircularDependenciesException<string>),
        ExpectedMessage = "The object graph contains circular dependencies involving items {e, d, c}, no root object can be found.")]
    public void ThrowsOnCirculars1 ()
    {
      string[] dependentObjects = new string[] { "h", "g", "f", "e", "d", "c", "b", "a" };
      SimpleDependencyAnalyzer<string> analyzer = new SimpleDependencyAnalyzer<string> ();
      analyzer.AddDependency ("a", "c");
      analyzer.AddDependency ("c", "d");
      analyzer.AddDependency ("d", "e");
      analyzer.AddDependency ("e", "c");

      new List<string> (AnalyzeDependencies (dependentObjects, analyzer));
    }

    [Test]
    [ExpectedException (typeof (CircularDependenciesException<string>), ExpectedMessage = "Item 'a' depends on itself.")]
    public void ThrowsOnCirculars2 ()
    {
      string[] dependentObjects = new string[] { "h", "g", "f", "e", "d", "c", "b", "a" };
      SimpleDependencyAnalyzer<string> analyzer = new SimpleDependencyAnalyzer<string> ();
      analyzer.AddDependency ("a", "a");
      analyzer.AddDependency ("a", "b");

      new List<string> (AnalyzeDependencies (dependentObjects, analyzer));
    }
  }
}
