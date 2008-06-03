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
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public abstract class DependencyDefinitionBuilderBase
  {
    private readonly MixinDefinition _mixin;

    public DependencyDefinitionBuilderBase(MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      _mixin = mixin;
    }

    protected abstract RequirementDefinitionBase GetRequirement (Type type, TargetClassDefinition targetClass);
    protected abstract RequirementDefinitionBase CreateRequirement (Type type, MixinDefinition mixin);
    protected abstract void AddRequirement (RequirementDefinitionBase requirement, TargetClassDefinition targetClass);
    protected abstract DependencyDefinitionBase CreateDependency (RequirementDefinitionBase requirement, MixinDefinition mixin, DependencyDefinitionBase aggregator);
    protected abstract void AddDependency (MixinDefinition mixin, DependencyDefinitionBase dependency);

    public void Apply (IEnumerable<Type> dependencyTypes)
    {
      ArgumentUtility.CheckNotNull ("dependencyTypes", dependencyTypes);

      foreach (Type type in dependencyTypes)
      {
        if (!type.Equals (typeof (object))) // dependencies to System.Object are always fulfilled and not explicitly added to the configuration
        {
          DependencyDefinitionBase dependency = BuildDependency (type, null);
          AddDependency (_mixin, dependency);
        }
      }
    }

    private DependencyDefinitionBase BuildDependency(Type type, DependencyDefinitionBase aggregator)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      RequirementDefinitionBase requirement = GetRequirement (type, _mixin.TargetClass);
      if (requirement == null)
      {
        requirement = CreateRequirement (type, _mixin);
        AddRequirement(requirement, _mixin.TargetClass);
      }
      DependencyDefinitionBase dependency = CreateDependency (requirement, _mixin, aggregator);
      requirement.RequiringDependencies.Add (dependency);
      CheckForAggregate (dependency);
      return dependency;
    }

    private void CheckForAggregate (DependencyDefinitionBase dependency)
    {
      ArgumentUtility.CheckNotNull ("dependency", dependency);

      if (dependency.RequiredType.IsAggregatorInterface)
      {
        foreach (Type type in dependency.RequiredType.Type.GetInterfaces ())
        {
          DependencyDefinitionBase innerDependency = BuildDependency (type, dependency);
          dependency.AggregatedDependencies.Add (innerDependency);
        }
      }
    }
  }
}
