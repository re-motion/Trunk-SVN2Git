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
using Remotion.Mixins.BridgeInterfaces;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;
using ReflectionUtility=Remotion.Utilities.ReflectionUtility;
using System.Linq;

namespace Remotion.Mixins.BridgeImplementations
{
  public class MixinTypeUtilityImplementation : IMixinTypeUtilityImplementation
  {
    public bool IsGeneratedConcreteMixedType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return typeof (IMixinTarget).IsAssignableFrom (type);
    }

    public bool IsGeneratedByMixinEngine (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return IsGeneratedConcreteMixedType (type)
          || typeof (IGeneratedMixinType).IsAssignableFrom (type)
              || typeof (IGeneratedBaseCallProxyType).IsAssignableFrom (type);
    }

    public Type GetConcreteMixedType (Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);
      if (IsGeneratedConcreteMixedType (targetOrConcreteType))
        return targetOrConcreteType;
      else
        return TypeFactory.GetConcreteType (targetOrConcreteType, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    public Type GetUnderlyingTargetType (Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);
      if (IsGeneratedConcreteMixedType (targetOrConcreteType))
        return MixinReflector.GetClassContextFromConcreteType (targetOrConcreteType).Type;
      else
        return targetOrConcreteType;
    }

    public bool IsAssignableFrom (Type baseOrInterface, Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull ("baseOrInterface", baseOrInterface);
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);

      return baseOrInterface.IsAssignableFrom (GetConcreteMixedType (targetOrConcreteType));
    }

    public bool HasMixins (Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);

      ClassContext classContext = MixinConfiguration.ActiveConfiguration.GetContext (targetOrConcreteType);
      return classContext != null && classContext.Mixins.Count > 0;
    }

    public bool HasMixin (Type targetOrConcreteType, Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      ClassContext classContext = MixinConfiguration.ActiveConfiguration.GetContext (targetOrConcreteType);
      return classContext != null && classContext.Mixins.ContainsKey (mixinType);
    }

    public Type GetAscribableMixinType (Type targetOrConcreteType, Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      foreach (Type configuredMixinType in GetMixinTypes (targetOrConcreteType))
      {
        if (ReflectionUtility.CanAscribe (configuredMixinType, mixinType))
          return configuredMixinType;
      }
      return null;
    }

    public bool HasAscribableMixin (Type targetOrConcreteType, Type mixinType)
    {
      return GetAscribableMixinType (targetOrConcreteType, mixinType) != null;
    }

    public IEnumerable<Type> GetMixinTypes (Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);

      ClassContext classContext = MixinConfiguration.ActiveConfiguration.GetContext (targetOrConcreteType);
      if (classContext != null)
      {
        foreach (MixinContext mixinContext in classContext.Mixins)
          yield return mixinContext.MixinType;
      }
    }

    public Type[] GetMixinTypesExact (Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);

      var concreteType = GetConcreteMixedType (targetOrConcreteType);
      var types = MixinReflector.GetOrderedMixinTypesFromConcreteType (concreteType);
      return types ?? Type.EmptyTypes;
    }

    public object CreateInstance (Type targetOrConcreteType, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);
      ArgumentUtility.CheckNotNull ("args", args);

      return Activator.CreateInstance (GetConcreteMixedType (targetOrConcreteType), args);
    }
  }
}
