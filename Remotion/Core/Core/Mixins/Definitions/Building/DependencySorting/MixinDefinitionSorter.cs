// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
    private readonly DependentObjectSorter<MixinDefinition> _sorter = new DependentObjectSorter<MixinDefinition> (new MixinDependencyAnalyzer ());
    private readonly DependentMixinGrouper _grouper = new DependentMixinGrouper ();

    public List<MixinDefinition> SortMixins (IEnumerable<MixinDefinition> unsortedMixins)
    {
      var sortedMixinGroups = new List<List<MixinDefinition>> ();

      // partition mixins into independent groups
      foreach (Set<MixinDefinition> mixinGroup in _grouper.GroupMixins (unsortedMixins))
      {
        try
        {
          IEnumerable<MixinDefinition> sortedGroup = _sorter.SortDependencies (mixinGroup);
          sortedMixinGroups.Add (new List<MixinDefinition> (sortedGroup));
        }
        catch (CircularDependenciesException<MixinDefinition> ex)
        {
          string message = string.Format ("The following group of mixins contains circular dependencies: {0}.",
              SeparatedStringBuilder.Build (", ", ex.Circulars, delegate (MixinDefinition m) { return m.FullName; }));
          throw new ConfigurationException (message, ex);
        }
      }

      // order groups alphabetically
      sortedMixinGroups.Sort (delegate (List<MixinDefinition> one, List<MixinDefinition> two) { return one[0].FullName.CompareTo (two[0].FullName); });

      // flatten ordered groups of sorted mixins
      var result = new List<MixinDefinition> ();
      foreach (List<MixinDefinition> mixinGroup in sortedMixinGroups)
      {
        foreach (MixinDefinition mixin in mixinGroup)
          result.Add (mixin);
      }
      return result;
    }
  }
}