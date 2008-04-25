using System;
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{RequiredType.Type}, Depender = {Depender.Type}")]
  public abstract class DependencyDefinitionBase : IVisitableDefinition
  {
    public readonly UniqueDefinitionCollection<Type, DependencyDefinitionBase> AggregatedDependencies;

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

      AggregatedDependencies = new UniqueDefinitionCollection<Type, DependencyDefinitionBase> (
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
      get { return AggregatedDependencies.Count > 0; }
    }

    public abstract void Accept (IDefinitionVisitor visitor);

    public virtual ClassDefinitionBase GetImplementer()
    {
      if (RequiredType.Type.IsAssignableFrom (_depender.TargetClass.Type))
        return _depender.TargetClass;
      else if (_depender.TargetClass.IntroducedInterfaces.ContainsKey (RequiredType.Type))
        return _depender.TargetClass.IntroducedInterfaces[RequiredType.Type].Implementer;
      else if (!RequiredType.IsEmptyInterface) // duck interface
        return _depender.TargetClass; 
      else
        return null; // empty interface that is neither introduced nor implemented
    }
  }
}