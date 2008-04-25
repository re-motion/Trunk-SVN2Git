using System;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public class MixinDependencyDefinitionBuilder : DependencyDefinitionBuilderBase
  {
    public MixinDependencyDefinitionBuilder (MixinDefinition mixin)
        : base (mixin)
    {
    }

    protected override RequirementDefinitionBase GetRequirement (Type type, TargetClassDefinition targetClass)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);

      return targetClass.RequiredMixinTypes[type];
    }

    protected override RequirementDefinitionBase CreateRequirement (Type type, MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("mixin", mixin);

      return new RequiredMixinTypeDefinition (mixin.TargetClass, type);
    }

    protected override void AddRequirement (RequirementDefinitionBase requirement, TargetClassDefinition targetClass)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);

      targetClass.RequiredMixinTypes.Add ((RequiredMixinTypeDefinition) requirement);
    }

    protected override DependencyDefinitionBase CreateDependency (RequirementDefinitionBase requirement, MixinDefinition mixin,
        DependencyDefinitionBase aggregator)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("mixin", mixin);

      return new MixinDependencyDefinition ((RequiredMixinTypeDefinition) requirement, mixin, (MixinDependencyDefinition) aggregator);
    }

    protected override void AddDependency (MixinDefinition mixin, DependencyDefinitionBase dependency)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      ArgumentUtility.CheckNotNull ("dependency", dependency);

      if (!mixin.MixinDependencies.ContainsKey (dependency.RequiredType.Type))
        mixin.MixinDependencies.Add ((MixinDependencyDefinition) dependency);
    }
  }
}
