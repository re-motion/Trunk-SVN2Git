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

namespace Remotion.Mixins.Definitions
{
  public interface IDefinitionVisitor
  {
    void Visit (TargetClassDefinition targetClass);
    void Visit (MixinDefinition mixin);
    void Visit (InterfaceIntroductionDefinition interfaceIntroduction);
    void Visit (NonIntroducedInterfaceDefinition definition);
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
    void Visit (SuppressedAttributeIntroductionDefinition suppressedAttributeIntroduction);
  }
}
