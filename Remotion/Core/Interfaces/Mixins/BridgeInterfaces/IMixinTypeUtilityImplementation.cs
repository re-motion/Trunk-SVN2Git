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
using System.Collections.Generic;
using Remotion.Implementation;

namespace Remotion.Mixins.BridgeInterfaces
{
  [ConcreteImplementation ("Remotion.Mixins.BridgeImplementations.MixinTypeUtilityImplementation, Remotion, Version=<version>, Culture=neutral, PublicKeyToken=<publicKeyToken>")]
  public interface IMixinTypeUtilityImplementation
  {
    bool IsGeneratedConcreteMixedType (Type type);
    bool IsGeneratedByMixinEngine (Type type);
    Type GetConcreteMixedType (Type targetOrConcreteType);
    Type GetUnderlyingTargetType (Type targetOrConcreteType);
    bool IsAssignableFrom (Type baseOrInterface, Type targetOrConcreteType);
    bool HasMixins (Type targetOrConcreteType);
    bool HasMixin (Type targetOrConcreteType, Type mixinType);
    Type GetAscribableMixinType (Type targetOrConcreteType, Type mixinType);
    bool HasAscribableMixin (Type targetOrConcreteType, Type mixinType);
    IEnumerable<Type> GetMixinTypes (Type targetOrConcreteType);
    Type[] GetMixinTypesExact (Type targetOrConcreteType);
    object CreateInstance (Type targetOrConcreteType, params object[] args);
  }
}
