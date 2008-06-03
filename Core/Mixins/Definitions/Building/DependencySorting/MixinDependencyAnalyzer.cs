/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Mixins.Utilities.DependencySort;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building.DependencySorting
{
  public class MixinDependencyAnalyzer : IDependencyAnalyzer<MixinDefinition>
  {
    public DependencyKind AnalyzeDirectDependency (MixinDefinition first, MixinDefinition second)
    {
      foreach (DependencyDefinitionBase dependency in first.GetOrderRelevantDependencies ())
      {
        if (dependency.GetImplementer () == second)
          return DependencyKind.FirstOnSecond;
      }
      foreach (DependencyDefinitionBase dependency in second.GetOrderRelevantDependencies ())
      {
        if (dependency.GetImplementer () == first)
          return DependencyKind.SecondOnFirst;
      }
      return DependencyKind.None;
    }

    public MixinDefinition ResolveEqualRoots (IEnumerable<MixinDefinition> equalRoots)
    {
      MixinDefinition root = ResolveAlphabetically (equalRoots);
      if (root != null)
        return root;
      else
        throw CreateCannotResolveException (equalRoots);
    }

    private MixinDefinition ResolveAlphabetically (IEnumerable<MixinDefinition> equalRoots)
    {
      List<Tuple<string, MixinDefinition>> mixinsByTypeName = GetMixinsByTypeName(equalRoots);
      if (mixinsByTypeName == null)
        return null;
      else
      {
        SortAlphabetically (mixinsByTypeName);
        return mixinsByTypeName[0].B;
      }
    }

    private List<Tuple<string, MixinDefinition>> GetMixinsByTypeName (IEnumerable<MixinDefinition> equalRoots)
    {
      List<Tuple<string, MixinDefinition>> mixinsByTypeName = new List<Tuple<string, MixinDefinition>>();
      int nonAlphabetics = 0;
      foreach (MixinDefinition mixin in equalRoots)
      {
        if (!mixin.AcceptsAlphabeticOrdering)
          ++nonAlphabetics;

        if (nonAlphabetics > 1)
          return null;

        mixinsByTypeName.Add (Tuple.NewTuple (mixin.FullName, mixin));
      }
      return mixinsByTypeName;
    }

    public void SortAlphabetically (List<Tuple<string, MixinDefinition>> mixinsByTypeName)
    {
      mixinsByTypeName.Sort (
          delegate (Tuple<string, MixinDefinition> t1, Tuple<string, MixinDefinition> t2)
          {
            return StringComparer.Ordinal.Compare(t1.A, t2.A);
          });
    }

    private Exception CreateCannotResolveException (IEnumerable<MixinDefinition> equalRoots)
    {
      IEnumerator<MixinDefinition> equalRootsEnumerator = equalRoots.GetEnumerator ();
      bool hasFirst = equalRootsEnumerator.MoveNext ();
      Assertion.IsTrue (hasFirst);
      MixinDefinition first = equalRootsEnumerator.Current;

      string message = string.Format ("The following mixins are applied to the same base class {0} and require a clear base call ordering, but do not "
          + "provide enough dependency information: {1}.{2}Please supply additional dependencies to the mixin definitions, use the "
          + "AcceptsAlphabeticOrderingAttribute, or adjust the mixin configuration accordingly.", first.TargetClass.FullName,
          SeparatedStringBuilder.Build (", ", equalRoots, delegate (MixinDefinition m) { return m.FullName; }),
          Environment.NewLine);
      return new ConfigurationException (message);
    }
  }
}
