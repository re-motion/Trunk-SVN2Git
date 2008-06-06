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
using System.Collections;
using System.Collections.Generic;
using Remotion.Collections;

namespace Remotion.Mixins.Definitions.Building.DependencySorting
{
  // groups mixins based on dependencies and common overridden methods
  public class DependentMixinGrouper
  {
    public IEnumerable<Set<MixinDefinition>> GroupMixins (IEnumerable<MixinDefinition> mixins)
    {
      DependentMixinGrouperAlgorithm algorithm = new DependentMixinGrouperAlgorithm (mixins);
      return algorithm.Execute ();
    }
  }

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
      Set<MixinDefinition> group = new Set<MixinDefinition> ();
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
          MixinDefinition implementer = dependency.GetImplementer() as MixinDefinition;
          if (implementer != null)
            AddToGroupRecursive (implementer, group);
        }

        foreach (MemberDefinition overridingMember in mixin.GetAllOverrides ())
        {
          MemberDefinition overriddenMember = overridingMember.BaseAsMember;
          AddOtherOverridersToGroup (overriddenMember, group);
        }
      }
    }

    private void AddOtherOverridersToGroup (MemberDefinition overriddenMember, Set<MixinDefinition> group)
    {
      foreach (MemberDefinition overridingMember in overriddenMember.Overrides)
      {
        MixinDefinition overrider = overridingMember.DeclaringClass as MixinDefinition;
        if (overrider != null)
          AddToGroupRecursive (overrider, group);
      }
    }

    private void AddAllWithDependenciesIntoGroup (Set<MixinDefinition> group)
    {
      // need to clone the set because we modify _mixins while iterating
      Set<MixinDefinition> remainingMixinsClone = new Set<MixinDefinition> (_mixins);

      foreach (MixinDefinition mixin in remainingMixinsClone)
      {
        foreach (DependencyDefinitionBase dependency in mixin.GetOrderRelevantDependencies())
        {
          MixinDefinition implementer = dependency.GetImplementer () as MixinDefinition;
          if (implementer != null && group.Contains (implementer))
            AddToGroupRecursive (mixin, group);
        }
      }
    }
  }
}
