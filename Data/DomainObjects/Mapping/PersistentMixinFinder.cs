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
      ClassContext mixinConfiguration = TargetClassDefinitionUtility.GetContext (type, MixinConfiguration.ActiveConfiguration, GenerationPolicy.GenerateOnlyIfConfigured);
      List<Type> persistentMixins = new List<Type> ();
      if (mixinConfiguration != null)
      {
        ClassContext parentClassContext =
            TargetClassDefinitionUtility.GetContext (type.BaseType, MixinConfiguration.ActiveConfiguration, GenerationPolicy.GenerateOnlyIfConfigured);

        foreach (MixinContext mixin in mixinConfiguration.Mixins)
        {
          if (mixin.MixinType.ContainsGenericParameters)
          {
            string message = string.Format ("The persistence-relevant mixin {0} applied to class {1} has open generic type parameters. All type "
                + "parameters of the mixin must be specified when it is applied to a DomainObject.", mixin.MixinType.FullName, type.FullName);
            throw new MappingException (message);
          }

          if (Utilities.ReflectionUtility.CanAscribe (mixin.MixinType, typeof (DomainObjectMixin<,>))
              && (parentClassContext == null || !parentClassContext.Mixins.ContainsAssignableMixin (mixin.MixinType)))
            persistentMixins.Add (mixin.MixinType);
        }
      }
      return persistentMixins;
    }
  }
}
