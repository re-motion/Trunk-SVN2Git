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
using Remotion.Implementation;

namespace Remotion.Mixins.BridgeInterfaces
{
  [ConcreteImplementation("Remotion.Mixins.BridgeImplementations.MixinTypeUtilityImplementation, Remotion, Culture = neutral, Version = <version>, PublicKeyToken = <publicKeyToken>")]
  public interface IMixinTypeUtilityImplementation
  {
    bool IsGeneratedConcreteMixedType (Type type);
    bool IsGeneratedByMixinEngine (Type type);
    Type GetConcreteMixedType (Type baseType);
    Type GetUnderlyingTargetType (Type type);
    bool IsAssignableFrom (Type baseOrInterface, Type typeToAssign);
    bool HasMixins (Type type);
    bool HasMixin (Type typeToCheck, Type mixinType);
    Type GetAscribableMixinType (Type typeToCheck, Type mixinType);
    bool HasAscribableMixin (Type typeToCheck, Type mixinType);
    IEnumerable<Type> GetMixinTypes (Type type);
    object CreateInstance (Type type, params object[] args);
  }
}
