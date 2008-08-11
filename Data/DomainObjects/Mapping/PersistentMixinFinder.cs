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
using System.Linq;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class PersistentMixinFinder : IPersistentMixinFinder
  {
    public static bool IsPersistenceRelevant (Type mixinType)
    {
      return Utilities.ReflectionUtility.CanAscribe (mixinType, typeof (DomainObjectMixin<,>));
    }

    private Type[] _persistentMixins;
    private readonly List<ClassContext> _parentClassContexts;

    public PersistentMixinFinder (Type type)
      : this (type, false)
    {
    }

    public PersistentMixinFinder (Type type, bool includeInherited)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      Type = type;
      IncludeInherited = includeInherited;
      MixinConfiguration = TargetClassDefinitionUtility.GetContext (Type, Mixins.MixinConfiguration.ActiveConfiguration, 
          GenerationPolicy.GenerateOnlyIfConfigured);
      if (MixinConfiguration != null)
      {
        ParentClassContext = TargetClassDefinitionUtility.GetContext (
            Type.BaseType, Mixins.MixinConfiguration.ActiveConfiguration, GenerationPolicy.GenerateOnlyIfConfigured);
        _parentClassContexts = GetParentClassContexts ();
      }
    }

    private List<ClassContext> GetParentClassContexts ()
    {
      List<ClassContext> parentClassContexts = new List<ClassContext> ();
      ClassContext current = MixinConfiguration;
      while (current != null && current.Type.BaseType != null)
      {
        ClassContext parent = TargetClassDefinitionUtility.GetContext (current.Type.BaseType, Mixins.MixinConfiguration.ActiveConfiguration, 
            GenerationPolicy.GenerateOnlyIfConfigured);
        if (parent != null)
          parentClassContexts.Add (parent);
        current = parent;
      }
      parentClassContexts.Reverse (); // first base is first
      return parentClassContexts;
    }

    public Type Type { get; private set; }

    public ClassContext MixinConfiguration { get; private set; }
    public ClassContext ParentClassContext { get; private set; }
    public bool IncludeInherited { get; private set; }

    public Type[] GetPersistentMixins ()
    {
      if (_persistentMixins == null)
        _persistentMixins = CalculatePersistentMixins ().ToArray();
      return _persistentMixins;
    }

    public bool IsInParentContext (Type mixinType)
    {
      return (ParentClassContext != null && ParentClassContext.Mixins.ContainsAssignableMixin (mixinType));
    }

    private IEnumerable<Type> CalculatePersistentMixins ()
    {
      // Generic parameter substitution is disallowed on purpose: the reflection-based mapping considers mixins applied to a base class to be
      // part of the base class definition. Therefore, the mixin applied to the derived class must be exactly the same as that on the base class,
      // otherwise the mapping might be inconsistent with the actual property types. With generic parameter substitution, an inherited mixin
      // might change with the derived class, so we can't allow it.
      // (The need to specify all generic arguments is also consistent with the mapping rule disallowing generic domain object types in the mapping;
      // and Extends and Uses both provide a means to specify generic type arguments as a workaround for when substitution doesn't work.)
      if (MixinConfiguration != null)
      {
        CheckForSuppressedMixins ();

        return from mixin in MixinConfiguration.Mixins
               where IsPersistenceRelevant(mixin.MixinType) && (IncludeInherited || !IsInParentContext(mixin.MixinType))
               select CheckNotOpenGenericMixin (mixin).MixinType;
      }
      else
        return Enumerable.Empty<Type>();
    }

    private void CheckForSuppressedMixins ()
    {
      if (ParentClassContext != null)
      {
        var suppressedMixins = from mixin in ParentClassContext.Mixins
                               where IsPersistenceRelevant (mixin.MixinType) && !MixinConfiguration.Mixins.ContainsAssignableMixin (mixin.MixinType)
                               select mixin;
        MixinContext suppressedMixin = suppressedMixins.FirstOrDefault();
        if (suppressedMixin != null)
        {
          string message = string.Format ("Class '{0}' suppresses mixin '{1}' inherited from its base class '{2}'. This is not allowed because "
              + "the mixin adds persistence information to the base class which must also be present in the derived class.", Type.FullName,
              suppressedMixin.MixinType.Name, ParentClassContext.Type.Name);
          throw new MappingException (message);
        }
      }
    }

    private MixinContext CheckNotOpenGenericMixin (MixinContext mixin)
    {
      if (mixin.MixinType.ContainsGenericParameters)
      {
        string message = string.Format ("The persistence-relevant mixin {0} applied to class {1} has open generic type parameters. All type "
            + "parameters of the mixin must be specified when it is applied to a DomainObject.", mixin.MixinType.FullName, Type.FullName);
        throw new MappingException (message);
      }
      else
        return mixin;
    }

    public Type FindOriginalMixinTarget (Type mixinType)
    {
      ClassContext parent = _parentClassContexts.FirstOrDefault (c => c.Mixins.ContainsKey (mixinType));
      if (parent != null)
        return parent.Type;
      else if (MixinConfiguration.Mixins.ContainsKey (mixinType))
        return MixinConfiguration.Type;
      else
        return null;
    }
  }
}
