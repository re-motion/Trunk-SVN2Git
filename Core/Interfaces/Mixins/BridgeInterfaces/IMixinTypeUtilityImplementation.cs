// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using Remotion.Implementation;

namespace Remotion.Mixins.BridgeInterfaces
{
  [ConcreteImplementation ("Remotion.Mixins.BridgeImplementations.MixinTypeUtilityImplementation, Remotion, Version=<version>, Culture=neutral, PublicKeyToken=<publicKeyToken>")]
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
