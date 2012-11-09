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
using System.Linq;
using Remotion.Text;

namespace Remotion.Mixins.Definitions.Building.DependencySorting
{
  /// <summary>
  /// Implements <see cref="IMixinDefinitionSorter"/> by sorting the mixins according to their dependencies and using lexicographic ordering when
  /// no dependencies are defined. For all mixins that are sorted lexicographically but overlap in overriding the same method, the algorithm 
  /// requires the <see cref="MixinDefinition.AcceptsAlphabeticOrdering"/> flag to be set. In such a group of mixins overlapping in an overridden 
  /// method, at most one mixin can have the <see cref="MixinDefinition.AcceptsAlphabeticOrdering"/> flag unset. (Rationale: If just one mixin
  /// cares about its ordering, but all others don't, an "undefined", i.e., lexicographical, ordering is acceptable.)
  /// </summary>
  /// <remarks>
  /// The algorithm works as follows:
  /// <list type="bullet">
  /// <item><description>If there are no mixins left, stop.</description></item>
  /// <item>
  /// <description>
  /// From the set of mixins (that have not been processed yet), take those on which there are no ordering dependencies.
  /// <list type="bullet">
  /// <item><description>If you find 0, throw (cyclic dependency).</description></item>
  /// </list>
  /// </description>
  /// </item>
  /// <item><description>If you find 1, append this mixin to the returned sequence.</description></item>
  /// <item><description>If you find more than 1:
  /// <list type="bullet">
  /// <item><description>Group the mixins based on the methods overridden by the mixins.</description></item>
  /// <item><description>Check that within each group, alphabetic ordering is accepted.</description></item>
  /// <item><description>Order the (ungrouped) mixins lexicographicsally, and append the ordered result to the returned sequence.</description></item>
  /// </list>
  /// </description>
  /// </item>
  /// <item><description>Repeat.</description></item>
  /// </list>
  /// </remarks>
  public class MixinDefinitionSorter2 : IMixinDefinitionSorter
  {
    public IEnumerable<MixinDefinition> SortMixins (IEnumerable<MixinDefinition> mixinDefinitions)
    {
      var unprocessedMixins = new HashSet<MixinDefinition> (mixinDefinitions);
      while (unprocessedMixins.Any ())
      {
        var roots = GetRoots (unprocessedMixins).ToList ();
        unprocessedMixins.ExceptWith (roots);
        if (roots.Count == 0)
        {
          var message = string.Format (
              "The following group of mixins contains circular dependencies:{1}{0}.",
              SeparatedStringBuilder.Build ("," + Environment.NewLine, unprocessedMixins, m => "'" + m.FullName + "'"),
              Environment.NewLine);
          throw new InvalidOperationException (message);
        }
        else if (roots.Count == 1)
          yield return roots.Single ();
        else
        {
          foreach (var mixinDefinition in GetOrderedMixinsOnSameLevel (roots))
            yield return mixinDefinition;
        }
      }
    }

    private IEnumerable<MixinDefinition> GetOrderedMixinsOnSameLevel (ICollection<MixinDefinition> mixins)
    {
      var groupedMixins = (from mixin in mixins
                           from ovr in mixin.GetAllMethods ().Select (m => m.Base)
                           where ovr != null
                           select new { Mixin = mixin, OverriddenMethod = ovr })
                           .GroupBy (t => t.OverriddenMethod);

      var badGroups = groupedMixins
          .Where (group => group.Count (m => !m.Mixin.AcceptsAlphabeticOrdering) > 1)
          .ToList ();

      if (badGroups.Any ())
      {
        var badGroupStrings = badGroups.Select (g => SeparatedStringBuilder.Build (", ", g, m => "'" + m.Mixin.FullName + "'"));

        var message = string.Format (
            "The following mixin groups require a clear base call ordering, but do not provide enough dependency information:{1}{0}.{1}"
            + "Please supply additional dependencies to the mixin definitions, use the AcceptsAlphabeticOrderingAttribute, or adjust the mixin "
            + "configuration accordingly.",
            SeparatedStringBuilder.Build ("," + Environment.NewLine, badGroupStrings, groupString => "{" + groupString + "}"),
            Environment.NewLine);
        throw new InvalidOperationException (message);
      }

      return mixins.OrderBy (m => m.FullName, StringComparer.Ordinal);
    }

    private IEnumerable<MixinDefinition> GetRoots (HashSet<MixinDefinition> unprocessedMixins)
    {
      return unprocessedMixins.Where (m => unprocessedMixins.All (other => !HasDependency (other, m)));
    }

    private bool HasDependency (MixinDefinition from, MixinDefinition to)
    {
      return from.GetOrderRelevantDependencies ().Any (dep => dep.GetImplementer () == to);
    }
  }
}