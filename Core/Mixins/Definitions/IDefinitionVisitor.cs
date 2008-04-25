using System;

namespace Remotion.Mixins.Definitions
{
  public interface IDefinitionVisitor
  {
    void Visit (TargetClassDefinition targetClass);
    void Visit (MixinDefinition mixin);
    void Visit (InterfaceIntroductionDefinition interfaceIntroduction);
    void Visit (SuppressedInterfaceIntroductionDefinition definition);
    void Visit (MethodIntroductionDefinition methodIntroduction);
    void Visit (PropertyIntroductionDefinition propertyIntroduction);
    void Visit (EventIntroductionDefinition eventIntroduction);
    void Visit (MethodDefinition method);
    void Visit (PropertyDefinition property);
    void Visit (EventDefinition eventDefintion);
    void Visit (RequiredFaceTypeDefinition requiredFaceType);
    void Visit (RequiredBaseCallTypeDefinition requiredBaseCallType);
    void Visit (RequiredMixinTypeDefinition requiredMixinType);
    void Visit (RequiredMethodDefinition definition);
    void Visit (ThisDependencyDefinition thisDependency);
    void Visit (BaseDependencyDefinition baseDependency);
    void Visit (MixinDependencyDefinition mixinDependency);
    void Visit (AttributeDefinition attribute);
    void Visit (AttributeIntroductionDefinition attributeIntroduction);
  }
}
