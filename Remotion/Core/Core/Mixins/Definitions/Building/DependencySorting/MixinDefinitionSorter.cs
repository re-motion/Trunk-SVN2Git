// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq;
using Remotion.Mixins.Utilities.DependencySort;
using Remotion.Text;

namespace Remotion.Mixins.Definitions.Building.DependencySorting
{
  /// <summary>
  /// Sorts the given mixin definitions by first grouping them into independent groups via <see cref="DependentMixinGrouper"/> and then sorting the
  /// mixins in the groups via <see cref="DependentObjectSorter{T}"/>. The groups are alphabetically sorted according to the full name of the first 
  /// mixin in the group.
  /// </summary>
  public class MixinDefinitionSorter
  {
    private readonly IDependentMixinGrouper _grouper;
    private readonly IDependentObjectSorter<MixinDefinition> _sorter;

    public MixinDefinitionSorter (IDependentMixinGrouper grouper, IDependentObjectSorter<MixinDefinition> sorter)
    {
      _grouper = grouper;
      _sorter = sorter;
    }

    public IDependentMixinGrouper Grouper
    {
      get { return _grouper; }
    }

    public IDependentObjectSorter<MixinDefinition> Sorter
    {
      get { return _sorter; }
    }

    /// <summary>
    /// Sorts the mixins of <paramref name="targetClassDefinition"/>. The <see cref="TargetClassDefinition"/> is required because without a target
    /// class, the dependencies are not defined.
    /// </summary>
    /// <param name="targetClassDefinition">The target class definition holding the mixins.</param>
    /// <returns>A list with the mixins held by <paramref name="targetClassDefinition"/>, but in the correct order.</returns>
    public List<MixinDefinition> SortMixins (TargetClassDefinition targetClassDefinition)
    {
      var sortedMixinGroups = PartitionAndSortMixins (targetClassDefinition.Mixins);

      // flatten ordered groups of sorted mixins
      return sortedMixinGroups.SelectMany (mixinGroup => mixinGroup).ToList();
    }

    private List<List<MixinDefinition>> PartitionAndSortMixins (IEnumerable<MixinDefinition> unsortedMixins)
    {
      var sortedMixinGroups = new List<List<MixinDefinition>> ();

      // partition mixins into independent groups
      foreach (HashSet<MixinDefinition> mixinGroup in _grouper.GroupMixins (unsortedMixins))
      {
        try
        {
          IEnumerable<MixinDefinition> sortedGroup = _sorter.SortDependencies (mixinGroup);
          sortedMixinGroups.Add (new List<MixinDefinition> (sortedGroup));
        }
        catch (CircularDependenciesException<MixinDefinition> ex)
        {
          string message = string.Format ("The following group of mixins contains circular dependencies: {0}.",
                                          SeparatedStringBuilder.Build (", ", ex.Circulars, m => m.FullName));
          throw new ConfigurationException (message, ex);
        }
      }

      // order groups alphabetically
      sortedMixinGroups.Sort ((one, two) => one[0].FullName.CompareTo (two[0].FullName));

      return sortedMixinGroups;
    }
  }
}
