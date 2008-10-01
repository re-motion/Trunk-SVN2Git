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
using Remotion.Mixins.BridgeInterfaces;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;
using ReflectionUtility=Remotion.Utilities.ReflectionUtility;

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

    public Type GetConcreteMixedType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      if (IsGeneratedConcreteMixedType (baseType))
        return baseType;
      else
        return TypeFactory.GetConcreteType (baseType, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    public Type GetUnderlyingTargetType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (IsGeneratedConcreteMixedType (type))
        return MixinReflector.GetClassContextFromConcreteType (type).Type;
      else
        return type;
    }

    public bool IsAssignableFrom (Type baseOrInterface, Type typeToAssign)
    {
      ArgumentUtility.CheckNotNull ("baseOrInterface", baseOrInterface);
      ArgumentUtility.CheckNotNull ("typeToAssign", typeToAssign);

      return baseOrInterface.IsAssignableFrom (GetConcreteMixedType (typeToAssign));
    }

    public bool HasMixins (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      ClassContext classContext = GetConcreteClassContext(type);
      return classContext != null && classContext.Mixins.Count > 0;
    }

    public bool HasMixin (Type typeToCheck, Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("typeToCheck", typeToCheck);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      ClassContext classContext = GetConcreteClassContext (typeToCheck);
      return classContext != null && classContext.Mixins.ContainsKey (mixinType);
    }

    public Type GetAscribableMixinType (Type typeToCheck, Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("typeToCheck", typeToCheck);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      foreach (Type configuredMixinType in GetMixinTypes (typeToCheck))
      {
        if (ReflectionUtility.CanAscribe (configuredMixinType, mixinType))
          return configuredMixinType;
      }
      return null;
    }

    public bool HasAscribableMixin (Type typeToCheck, Type mixinType)
    {
      return GetAscribableMixinType (typeToCheck, mixinType) != null;
    }

    private ClassContext GetConcreteClassContext (Type type)
    {
      if (IsGeneratedConcreteMixedType (type))
        return MixinReflector.GetClassContextFromConcreteType (type);
      else
        return MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (type);
    }

    public IEnumerable<Type> GetMixinTypes (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      ClassContext classContext = GetConcreteClassContext (type);
      if (classContext != null)
      {
        foreach (MixinContext mixinContext in classContext.Mixins)
          yield return mixinContext.MixinType;
      }
    }

    public object CreateInstance (Type type, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("args", args);

      return Activator.CreateInstance (GetConcreteMixedType (type), args);
    }
  }
}
