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
using System.Reflection;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public class MixinPropertyFinder
  {
    private readonly Type _propertyFinderType;
    private readonly PersistentMixinFinder _persistentMixinFinder;
    private readonly bool _includeBaseProperties;
    private readonly IMappingNameResolver _nameResolver;

    public MixinPropertyFinder (Type propertyFinderType, PersistentMixinFinder persistentMixinFinder, bool includeBaseProperties, IMappingNameResolver nameResolver)
    {
      ArgumentUtility.CheckNotNull ("propertyFinderType", propertyFinderType);
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);

      _propertyFinderType = propertyFinderType;
      _persistentMixinFinder = persistentMixinFinder;
      _nameResolver = nameResolver;
      _includeBaseProperties = includeBaseProperties;
    }

    public IEnumerable<PropertyInfo> FindPropertyInfosOnMixins (ReflectionBasedClassDefinition classDefinition)
    {
      if (_persistentMixinFinder != null)
      {
        Set<Type> processedMixins = new Set<Type> ();
        foreach (Type mixin in _persistentMixinFinder.GetPersistentMixins ())
        {
          foreach (PropertyInfo propertyInfo in FindPropertyInfosOnMixin (classDefinition, mixin, processedMixins))
            yield return propertyInfo;
        }
      }
    }

    private IEnumerable<PropertyInfo> FindPropertyInfosOnMixin (ReflectionBasedClassDefinition classDefinition, Type mixin, Set<Type> processedMixins)
    {
      Type current = mixin;
      while (current != null && !IsMixinBaseClass (current))
      {
        if (!processedMixins.Contains (current) && (!_persistentMixinFinder.IsInParentContext (current) || _includeBaseProperties))
        {
          PropertyFinderBase mixinPropertyFinder = (PropertyFinderBase) TypesafeActivator.CreateInstance (_propertyFinderType)
              .With (
              current,
              false,
              (PersistentMixinFinder) null, // mixins on mixins are not checked
              _nameResolver);

          foreach (PropertyInfo propertyInfo in mixinPropertyFinder.FindPropertyInfosDeclaredOnThisType (classDefinition))
            yield return propertyInfo;

          processedMixins.Add (current);
        }
        current = current.BaseType;
      }
    }

    private bool IsMixinBaseClass (Type type)
    {
      if (!type.IsGenericType)
        return false;
      Type genericTypeDefinition = type.GetGenericTypeDefinition ();
      return genericTypeDefinition == typeof (Mixin<>) || genericTypeDefinition == typeof (Mixin<,>);
    }
  }
}