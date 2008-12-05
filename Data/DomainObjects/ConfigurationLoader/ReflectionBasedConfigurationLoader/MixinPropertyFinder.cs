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
using System.Linq;
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
    private readonly IPersistentMixinFinder _persistentMixinFinder;
    private readonly bool _includeBaseProperties;
    private readonly IMappingNameResolver _nameResolver;

    public MixinPropertyFinder (Type propertyFinderType, IPersistentMixinFinder persistentMixinFinder, bool includeBaseProperties, IMappingNameResolver nameResolver)
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
        var processedMixins = new Set<Type> ();
        return from mixin in _persistentMixinFinder.GetPersistentMixins ()
               from propertyInfo in FindPropertyInfosOnMixin (classDefinition, mixin, processedMixins)
               select propertyInfo;
      }
      else
        return Enumerable.Empty<PropertyInfo> ();
    }

    private IEnumerable<PropertyInfo> FindPropertyInfosOnMixin (ReflectionBasedClassDefinition classDefinition, Type mixin, Set<Type> processedMixins)
    {
      Type current = mixin;
      while (current != null && !IsMixinBaseClass (current))
      {
        if (!processedMixins.Contains (current) && (_includeBaseProperties || !_persistentMixinFinder.IsInParentContext (current)))
        {
          var mixinPropertyFinder = (PropertyFinderBase) TypesafeActivator.CreateInstance (_propertyFinderType).With (current, false, _nameResolver);
          // Note: mixins on mixins are not checked
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
