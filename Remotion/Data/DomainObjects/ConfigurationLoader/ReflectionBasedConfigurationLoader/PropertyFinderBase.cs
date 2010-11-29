// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>The <see cref="PropertyFinderBase"/> is used to find all <see cref="PropertyInfo"/> objects relevant for the mapping.</summary>
  /// <remarks>Derived classes must have a cosntructor with a matching the <see cref="PropertyFinderBase"/>'s constructor signature. </remarks>
  public abstract class PropertyFinderBase
  {
    public const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private readonly Type _type;
    private readonly bool _includeBaseProperties;
    private readonly Set<MethodInfo> _explicitInterfaceImplementations;
    private readonly IMappingNameResolver _nameResolver;
    private readonly ReflectionBasedClassDefinition _classDefinition;

    protected PropertyFinderBase (Type type, ReflectionBasedClassDefinition classDefinition, bool includeBaseProperties, IMappingNameResolver nameResolver)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);

      _type = type;
      _classDefinition = classDefinition;
      _nameResolver = nameResolver;
      _includeBaseProperties = includeBaseProperties;
      _explicitInterfaceImplementations = GetExplicitInterfaceImplementations (type);
    }

    public Type Type
    {
      get { return _type; }
    }

    public bool IncludeBaseProperties
    {
      get { return _includeBaseProperties; }
    }

    public IMappingNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    public PropertyInfo[] FindPropertyInfos ()
    {
      var propertyInfos = new List<PropertyInfo>();

      if (_includeBaseProperties && _type.BaseType != typeof (DomainObject))
      {
        var propertyFinder = (PropertyFinderBase) TypesafeActivator.CreateInstance (GetType()).With (_type.BaseType, _classDefinition, true, NameResolver);
        propertyInfos.AddRange (propertyFinder.FindPropertyInfos ());
      }

      propertyInfos.AddRange (FindPropertyInfosDeclaredOnThisType (_classDefinition));

      // Mixins are currently only checked when the current type equals the classDefinition's type. This means we do not check for mixins when we
      // are above the inheritance root (where _type will be a base of classDefinition.ClassType).
      // TODO: Substitute this implicit rule with an includeMixins ctor parameter.
      if (_type == _classDefinition.ClassType)
      {
        var mixinPropertyFinder = new MixinPropertyFinder (GetType(), _classDefinition.PersistentMixinFinder, IncludeBaseProperties, NameResolver);
        propertyInfos.AddRange (mixinPropertyFinder.FindPropertyInfosOnMixins (_classDefinition));
      }

      return propertyInfos.ToArray();
    }
    
    protected virtual bool FindPropertiesFilter (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (!Utilities.ReflectionUtility.IsOriginalDeclaration (propertyInfo))
        return false;
      
      if (IsUnmanagedProperty (propertyInfo))
        return false;

      if (IsUnmanagedExplictInterfaceImplementation (propertyInfo))
        return false;

      return true;
    }

    protected bool IsUnmanagedProperty (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      StorageClassAttribute storageClassAttribute = AttributeUtility.GetCustomAttribute<StorageClassAttribute> (propertyInfo, false);
      if (storageClassAttribute == null)
        return false;

      return storageClassAttribute.StorageClass == StorageClass.None;
    }

    protected bool IsUnmanagedExplictInterfaceImplementation (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      bool isExplicitInterfaceImplementation = Array.Exists (
          propertyInfo.GetAccessors (true),
          delegate (MethodInfo accessor) { return _explicitInterfaceImplementations.Contains (accessor); });
      if (!isExplicitInterfaceImplementation)
        return false;

      StorageClassAttribute storageClassAttribute = AttributeUtility.GetCustomAttribute<StorageClassAttribute> (propertyInfo, false);
      if (storageClassAttribute == null)
        return true;

      return storageClassAttribute.StorageClass == StorageClass.None;
    }

    public IList<PropertyInfo> FindPropertyInfosDeclaredOnThisType (ReflectionBasedClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      MemberInfo[] memberInfos = _type.FindMembers (
          MemberTypes.Property,
          PropertyBindingFlags | BindingFlags.DeclaredOnly,
          FindPropertiesFilter,
          classDefinition);

      PropertyInfo[] propertyInfos = Array.ConvertAll<MemberInfo, PropertyInfo> (
          memberInfos,
          delegate (MemberInfo input) { return (PropertyInfo) input; });

      return propertyInfos;
    }

    private bool FindPropertiesFilter (MemberInfo member, object filterCriteria)
    {
      ReflectionBasedClassDefinition classDefinition = 
          ArgumentUtility.CheckNotNullAndType<ReflectionBasedClassDefinition> ("filterCriteria", filterCriteria);
      return FindPropertiesFilter (classDefinition, (PropertyInfo) member);
    }

    private Set<MethodInfo> GetExplicitInterfaceImplementations (Type type)
    {
      Set<MethodInfo> explicitInterfaceImplementationSet = new Set<MethodInfo>();

      foreach (Type interfaceType in type.GetInterfaces())
      {
        InterfaceMapping interfaceMapping = type.GetInterfaceMap (interfaceType);
        MethodInfo[] explicitInterfaceImplementations = Array.FindAll (
            interfaceMapping.TargetMethods,
            delegate (MethodInfo targetMethod) { return targetMethod.IsSpecialName && !targetMethod.IsPublic; });
        explicitInterfaceImplementationSet.AddRange (explicitInterfaceImplementations);
      }

      return explicitInterfaceImplementationSet;
    }
  }
}
