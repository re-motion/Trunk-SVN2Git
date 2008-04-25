using System;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public class BaseDependencyDefinitionBuilder : DependencyDefinitionBuilderBase
  {
    public BaseDependencyDefinitionBuilder (MixinDefinition mixin)
        : base (mixin)
    {
    }

    protected override RequirementDefinitionBase GetRequirement (Type type, TargetClassDefinition targetClass)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);

      return targetClass.RequiredBaseCallTypes[type];
    }

    protected override RequirementDefinitionBase CreateRequirement (Type type, MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("mixin", mixin);

      Assertion.IsTrue (type != typeof (object), "This method will not be called for typeof (object).");

      if (!type.IsInterface)
      {
        string message = string.Format ("Base call dependencies must be interfaces (or System.Object), but mixin {0} (on class {1} has a dependency "
            + "on a class: {2}.", mixin.FullName, mixin.TargetClass.FullName, type.FullName);
        throw new ConfigurationException (message);
      }

      return new RequiredBaseCallTypeDefinition (mixin.TargetClass, type);
    }

    protected override void AddRequirement (RequirementDefinitionBase requirement, TargetClassDefinition targetClass)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);

      targetClass.RequiredBaseCallTypes.Add ((RequiredBaseCallTypeDefinition) requirement);
    }

    protected override DependencyDefinitionBase CreateDependency (RequirementDefinitionBase requirement, MixinDefinition mixin,
        DependencyDefinitionBase aggregator)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("mixin", mixin);

      return new BaseDependencyDefinition ((RequiredBaseCallTypeDefinition) requirement, mixin, (BaseDependencyDefinition)aggregator);
    }

    protected override void AddDependency (MixinDefinition mixin, DependencyDefinitionBase dependency)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      ArgumentUtility.CheckNotNull ("dependency", dependency);
      if (!mixin.BaseDependencies.ContainsKey (dependency.RequiredType.Type))
        mixin.BaseDependencies.Add ((BaseDependencyDefinition) dependency);
    }
  }
}
