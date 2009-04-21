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

namespace Remotion.Mixins.Definitions
{
  public interface IDefinitionVisitor
  {
    void Visit (TargetClassDefinition targetClass);
    void Visit (MixinDefinition mixin);
    void Visit (InterfaceIntroductionDefinition interfaceIntroduction);
    void Visit (NonInterfaceIntroductionDefinition nonIntroductionDefinition);
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
    void Visit (NonAttributeIntroductionDefinition nonAttributeIntroduction);
    void Visit (SuppressedAttributeIntroductionDefinition suppressedAttributeIntroduction);
  }
}
