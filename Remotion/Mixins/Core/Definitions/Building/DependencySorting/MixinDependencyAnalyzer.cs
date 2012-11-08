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
using Remotion.Collections;
using Remotion.Mixins.Utilities.DependencySort;
using Remotion.Text;
using System.Linq;

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
        return mixinsByTypeName[0].Item2;
      }
    }

    private List<Tuple<string, MixinDefinition>> GetMixinsByTypeName (IEnumerable<MixinDefinition> equalRoots)
    {
      var mixinsByTypeName = new List<Tuple<string, MixinDefinition>>();
      int nonAlphabetics = 0;
      foreach (MixinDefinition mixin in equalRoots)
      {
        if (!mixin.AcceptsAlphabeticOrdering)
          ++nonAlphabetics;

        if (nonAlphabetics > 1)
          return null;

        mixinsByTypeName.Add (Tuple.Create (mixin.FullName, mixin));
      }
      return mixinsByTypeName;
    }

    public void SortAlphabetically (List<Tuple<string, MixinDefinition>> mixinsByTypeName)
    {
      mixinsByTypeName.Sort ((t1, t2) => StringComparer.Ordinal.Compare (t1.Item1, t2.Item1));
    }

    private Exception CreateCannotResolveException (IEnumerable<MixinDefinition> equalRoots)
    {
      string message =
          string.Format (
              "The following mixins require a clear base call ordering, but do not "
              + "provide enough dependency information:{1}{0}.{1}Please supply additional dependencies to the mixin definitions, use the "
              + "AcceptsAlphabeticOrderingAttribute, or adjust the mixin configuration accordingly.",
              SeparatedStringBuilder.Build ("," + Environment.NewLine, equalRoots, m => "'" + m.FullName + "'"),
              Environment.NewLine);
      return new InvalidOperationException (message);
    }
  }
}
