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
using System;
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{RequiredType.Type}, Depender = {Depender.Type}")]
  public abstract class DependencyDefinitionBase : IVisitableDefinition
  {
    private readonly UniqueDefinitionCollection<Type, DependencyDefinitionBase> _aggregatedDependencies;

    private readonly RequirementDefinitionBase _requirement; // the required face or base interface
    private readonly MixinDefinition _depender; // the mixin (directly or indirectly) defining the requirement
    private readonly DependencyDefinitionBase _aggregator; // the outer dependency containing this dependency, if defined indirectly

    public DependencyDefinitionBase (RequirementDefinitionBase requirement, MixinDefinition depender, DependencyDefinitionBase aggregator)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("depender", depender);
      ArgumentUtility.CheckType ("aggregator", aggregator, GetType ());

      _requirement = requirement;
      _depender = depender;
      _aggregator = aggregator;

      _aggregatedDependencies = new UniqueDefinitionCollection<Type, DependencyDefinitionBase> (
          delegate (DependencyDefinitionBase d) { return d.RequiredType.Type; },
          HasSameDepender);
    }

    public bool HasSameDepender (DependencyDefinitionBase dependencyToCheck)
    {
      ArgumentUtility.CheckNotNull ("dependencyToCheck", dependencyToCheck);
      return dependencyToCheck.Depender == _depender;
    }

    public RequirementDefinitionBase RequiredType
    {
      get { return _requirement; }
    }

    public MixinDefinition Depender
    {
      get { return _depender; }
    }

    public DependencyDefinitionBase Aggregator
    {
      get { return _aggregator; }
    }

    public string FullName
    {
      get { return RequiredType.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get
      {
        if (Aggregator != null)
        {
          return Aggregator;
        }
        else
        {
          return Depender;
        }
      }
    }

    // aggregates hold nested dependencies
    public bool IsAggregate
    {
      get { return _aggregatedDependencies.Count > 0; }
    }

    public UniqueDefinitionCollection<Type, DependencyDefinitionBase> AggregatedDependencies
    {
      get { return _aggregatedDependencies; }
    }

    public abstract void Accept (IDefinitionVisitor visitor);

    public virtual ClassDefinitionBase GetImplementer()
    {
      if (RequiredType.Type.IsAssignableFrom (_depender.TargetClass.Type))
        return _depender.TargetClass;
      else if (_depender.TargetClass.ReceivedInterfaces.ContainsKey (RequiredType.Type))
        return _depender.TargetClass.ReceivedInterfaces[RequiredType.Type].Implementer;
      else if (!RequiredType.IsEmptyInterface) // duck interface
        return _depender.TargetClass; 
      else
        return null; // empty interface that is neither introduced nor implemented
    }
  }
}
