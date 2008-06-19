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
using Remotion.Mixins;
using Remotion.Mixins.Context;

namespace Remotion.Data.DomainObjects.Mapping
{
  public static class PersistentMixinFinder
  {
    public static List<Type> GetPersistentMixins (Type type)
    {
      // Generic parameter substitution is disallowed on purpose: the reflection-based mapping considers mixins applied to a base class to be
      // part of the base class definition. Therefore, the mixin applied to the derived class must be exactly the same as that on the base class,
      // otherwise the mapping might be inconsistent with the actual property types. With generic parameter substitution, an inherited mixin
      // might change with the derived class, so we can't allow it.
      // (The need to specify all generic arguments is also consistent with the mapping rule disallowing generic domain object types in the mapping;
      // and Extends and Uses both provide a means to specify generic type arguments as a workaround for when substitution doesn't work.)
      ClassContext mixinConfiguration = TargetClassDefinitionUtility.GetContext (type, MixinConfiguration.ActiveConfiguration, GenerationPolicy.GenerateOnlyIfConfigured);
      List<Type> persistentMixins = new List<Type> ();
      if (mixinConfiguration != null)
      {
        ClassContext parentClassContext =
            TargetClassDefinitionUtility.GetContext (type.BaseType, MixinConfiguration.ActiveConfiguration, GenerationPolicy.GenerateOnlyIfConfigured);

        foreach (MixinContext mixin in mixinConfiguration.Mixins)
        {
          if (IsPersistenceRelevant(mixin) && IsNotInParentContext(parentClassContext, mixin))
          {
            CheckNotOpenGenericMixin (mixin, type);
            persistentMixins.Add (mixin.MixinType);
          }
        }
      }
      return persistentMixins;
    }

    private static bool IsNotInParentContext (ClassContext parentClassContext, MixinContext mixin)
    {
      return (parentClassContext == null || !parentClassContext.Mixins.ContainsAssignableMixin (mixin.MixinType));
    }

    private static bool IsPersistenceRelevant (MixinContext mixin)
    {
      return Utilities.ReflectionUtility.CanAscribe (mixin.MixinType, typeof (DomainObjectMixin<,>));
    }

    private static void CheckNotOpenGenericMixin (MixinContext mixin, Type targetType)
    {
      if (mixin.MixinType.ContainsGenericParameters)
      {
        string message = string.Format ("The persistence-relevant mixin {0} applied to class {1} has open generic type parameters. All type "
            + "parameters of the mixin must be specified when it is applied to a DomainObject.", mixin.MixinType.FullName, targetType.FullName);
        throw new MappingException (message);
      }
    }
  }
}
