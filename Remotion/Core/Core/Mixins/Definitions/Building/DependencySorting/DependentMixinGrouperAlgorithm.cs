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

namespace Remotion.Mixins.Definitions.Building.DependencySorting
{
  internal class DependentMixinGrouperAlgorithm
  {
    private readonly Set<MixinDefinition> _mixins;

    public DependentMixinGrouperAlgorithm (IEnumerable<MixinDefinition> mixins)
    {
      _mixins = new Set<MixinDefinition> (mixins);
    }

    public IEnumerable<Set<MixinDefinition>> Execute ()
    {
      while (_mixins.Count > 0)
      {
        MixinDefinition current = _mixins.GetAny ();
        yield return FormGroup (current);
      }
    }

    private Set<MixinDefinition> FormGroup (MixinDefinition startingPoint)
    {
      var group = new Set<MixinDefinition> ();
      AddToGroupRecursive (startingPoint, group);
      AddAllWithDependenciesIntoGroup (group);
      return group;
    }

    private void AddToGroupRecursive (MixinDefinition mixin, Set<MixinDefinition> group)
    {
      if (_mixins.Remove (mixin))
      {
        group.Add (mixin);
        foreach (DependencyDefinitionBase dependency in mixin.GetOrderRelevantDependencies ())
        {
          var implementer = dependency.GetImplementer() as MixinDefinition;
          if (implementer != null)
            AddToGroupRecursive (implementer, group);
        }

        foreach (MemberDefinitionBase overridingMember in mixin.GetAllOverrides ())
        {
          MemberDefinitionBase overriddenMember = overridingMember.BaseAsMember;
          AddOtherOverridersToGroup (overriddenMember, group);
        }
      }
    }

    private void AddOtherOverridersToGroup (MemberDefinitionBase overriddenMember, Set<MixinDefinition> group)
    {
      foreach (MemberDefinitionBase overridingMember in overriddenMember.Overrides)
      {
        var overrider = overridingMember.DeclaringClass as MixinDefinition;
        if (overrider != null)
          AddToGroupRecursive (overrider, group);
      }
    }

    private void AddAllWithDependenciesIntoGroup (Set<MixinDefinition> group)
    {
      // need to clone the set because we modify _mixins while iterating
      var remainingMixinsClone = new Set<MixinDefinition> (_mixins);

      foreach (MixinDefinition mixin in remainingMixinsClone)
      {
        foreach (DependencyDefinitionBase dependency in mixin.GetOrderRelevantDependencies())
        {
          var implementer = dependency.GetImplementer () as MixinDefinition;
          if (implementer != null && group.Contains (implementer))
            AddToGroupRecursive (mixin, group);
        }
      }
    }
  }
}
