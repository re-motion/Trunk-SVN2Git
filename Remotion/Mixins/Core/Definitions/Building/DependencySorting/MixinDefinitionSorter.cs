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
using Remotion.Mixins.Utilities.DependencySort;
using Remotion.Text;

namespace Remotion.Mixins.Definitions.Building.DependencySorting
{
  /// <summary>
  /// Sorts the mixin definitions of a <see cref="TargetClassDefinition"/> by first grouping them into independent groups via 
  /// <see cref="DependentMixinGrouper"/> and then sorting the mixins in the groups via <see cref="DependentObjectSorter{T}"/>. 
  /// The groups are alphabetically sorted according to the full name of the first mixin in the group.
  /// </summary>
  public class MixinDefinitionSorter : IMixinDefinitionSorter
  {
    private readonly IDependentMixinGrouper _grouper;
    private readonly IDependentMixinSorter _sorter;

    public MixinDefinitionSorter (IDependentMixinGrouper grouper, IDependentMixinSorter sorter)
    {
      _grouper = grouper;
      _sorter = sorter;
    }

    public IDependentMixinGrouper Grouper
    {
      get { return _grouper; }
    }

    public IDependentMixinSorter Sorter
    {
      get { return _sorter; }
    }

    public IEnumerable<MixinDefinition> SortMixins (IEnumerable<MixinDefinition> mixinDefinitions)
    {
      var sortedMixinGroups = PartitionAndSortMixins (mixinDefinitions);

      // flatten ordered groups of sorted mixins
      return sortedMixinGroups.SelectMany (mixinGroup => mixinGroup);
    }

    private IEnumerable<List<MixinDefinition>> PartitionAndSortMixins (IEnumerable<MixinDefinition> mixinDefinitions)
    {
      // partition mixins into independent groups
      var sortedMixinGroups = _grouper
          .GroupMixins (mixinDefinitions)
          .Select (
              mixinGroup =>
              {
                try
                {
                  return _sorter.SortDependencies (mixinGroup).ToList();
                }
                catch (CircularDependenciesException<MixinDefinition> ex)
                {
                  string message = string.Format (
                      "The following group of mixins contains circular dependencies:{1}{0}.",
                      SeparatedStringBuilder.Build ("," + Environment.NewLine, ex.Circulars, m => "'" + m.FullName + "'"),
                      Environment.NewLine);
                  throw new InvalidOperationException (message, ex);
                }
              })
          .ToList();

      // order groups alphabetically
      sortedMixinGroups.Sort ((one, two) => System.String.Compare(one[0].FullName, two[0].FullName, System.StringComparison.Ordinal));
      return sortedMixinGroups;
    }
  }
}
