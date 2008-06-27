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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class PersistentMixinFinder
  {
    public static bool IsPersistenceRelevant (Type mixinType)
    {
      return Utilities.ReflectionUtility.CanAscribe (mixinType, typeof (DomainObjectMixin<,>));
    }

    private readonly Type _type;
    private readonly ClassContext _mixinConfiguration;
    private readonly ClassContext _parentClassContext;
    
    private List<Type> _persistentMixins;

    public PersistentMixinFinder (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _type = type;
      _mixinConfiguration = TargetClassDefinitionUtility.GetContext (_type, Mixins.MixinConfiguration.ActiveConfiguration, GenerationPolicy.GenerateOnlyIfConfigured);
      if (_mixinConfiguration != null)
        _parentClassContext = TargetClassDefinitionUtility.GetContext (_type.BaseType, Mixins.MixinConfiguration.ActiveConfiguration, GenerationPolicy.GenerateOnlyIfConfigured);
    }

    public Type Type
    {
      get { return _type; }
    }

    public ClassContext MixinConfiguration
    {
      get { return _mixinConfiguration; }
    }

    public ClassContext ParentClassContext
    {
      get { return _parentClassContext; }
    }

    public List<Type> GetPersistentMixins ()
    {
      if (_persistentMixins == null)
        _persistentMixins = CalculatePersistentMixins ();
      return _persistentMixins;
    }

    public bool IsInParentContext (Type mixinType)
    {
      return (_parentClassContext != null && _parentClassContext.Mixins.ContainsAssignableMixin (mixinType));
    }

    private List<Type> CalculatePersistentMixins ()
    {
      // Generic parameter substitution is disallowed on purpose: the reflection-based mapping considers mixins applied to a base class to be
      // part of the base class definition. Therefore, the mixin applied to the derived class must be exactly the same as that on the base class,
      // otherwise the mapping might be inconsistent with the actual property types. With generic parameter substitution, an inherited mixin
      // might change with the derived class, so we can't allow it.
      // (The need to specify all generic arguments is also consistent with the mapping rule disallowing generic domain object types in the mapping;
      // and Extends and Uses both provide a means to specify generic type arguments as a workaround for when substitution doesn't work.)
      List<Type> persistentMixins = new List<Type> ();
      if (_mixinConfiguration != null)
      {
        foreach (MixinContext mixin in _mixinConfiguration.Mixins)
        {
          if (IsPersistenceRelevant(mixin.MixinType) && !IsInParentContext(mixin.MixinType))
          {
            CheckNotOpenGenericMixin (mixin);
            persistentMixins.Add (mixin.MixinType);
          }
        }

        CheckForSuppressedMixins ();
      }
      return persistentMixins;
    }

    private void CheckForSuppressedMixins ()
    {
      if (_parentClassContext != null)
      {
        foreach (MixinContext mixin in _parentClassContext.Mixins)
        {
          if (IsPersistenceRelevant (mixin.MixinType) && !_mixinConfiguration.Mixins.ContainsAssignableMixin (mixin.MixinType))
          {
            string message = string.Format ("Class '{0}' suppresses mixin '{1}' inherited from its base class '{2}'. This is not allowed because "
                + "the mixin adds persistence information to the base class which must also be present in the derived class.", _type.FullName,
                mixin.MixinType.Name, _parentClassContext.Type.Name);
            throw new MappingException (message);
          }
        }
      }
    }

    private void CheckNotOpenGenericMixin (MixinContext mixin)
    {
      if (mixin.MixinType.ContainsGenericParameters)
      {
        string message = string.Format ("The persistence-relevant mixin {0} applied to class {1} has open generic type parameters. All type "
            + "parameters of the mixin must be specified when it is applied to a DomainObject.", mixin.MixinType.FullName, _type.FullName);
        throw new MappingException (message);
      }
    }
  }
}
