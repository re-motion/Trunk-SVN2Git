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
  /// <summary>The <see cref="PropertyFinderBase"/> is used to find all <see cref="PropertyInfo"/> objects relevant for the mapping.</summary>
  /// <remarks>Derived classes must have a cosntructor with a matching the <see cref="PropertyFinderBase"/>'s constructor signature. </remarks>
  public abstract class PropertyFinderBase
  {
    public const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private readonly Type _type;
    private readonly bool _includeBaseProperties;
    private readonly Set<MethodInfo> _explicitInterfaceImplementations;
    private readonly PersistentMixinFinder _persistentMixinFinder;
    private readonly IMappingNameResolver _nameResolver;

    protected PropertyFinderBase (Type type, bool includeBaseProperties, PersistentMixinFinder persistentMixinFinder, IMappingNameResolver nameResolver)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);

      _type = type;
      _persistentMixinFinder = persistentMixinFinder;
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

    public PropertyInfo[] FindPropertyInfos (ReflectionBasedClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      List<PropertyInfo> propertyInfos = new List<PropertyInfo>();

      if (_includeBaseProperties && _type.BaseType != typeof (DomainObject))
      {
        PropertyFinderBase propertyFinder = (PropertyFinderBase) TypesafeActivator.CreateInstance (GetType()).With (_type.BaseType, true,
            (PersistentMixinFinder) null, // mixins are only checked on the top level, we get all inherited mixins anyway
            NameResolver);
        propertyInfos.AddRange (propertyFinder.FindPropertyInfos (classDefinition));
      }

      propertyInfos.AddRange (FindPropertyInfosInternal (classDefinition));

      FindPropertyInfosOnMixins(classDefinition, propertyInfos);

      return propertyInfos.ToArray();
    }

    private void FindPropertyInfosOnMixins (ReflectionBasedClassDefinition classDefinition, List<PropertyInfo> propertyInfos)
    {
      if (_persistentMixinFinder != null)
      {
        foreach (Type mixin in _persistentMixinFinder.GetPersistentMixins())
        {
          Type current = mixin;
          // we need to manually walk over the base types because the ordinary ClassDefinition/parent algorithm won't be applied to mixins
          // and PropertyFinder can only work with properties declared directly on the type
          while (current != null && !IsMixinBaseClass (current))
          {
            if (!_persistentMixinFinder.IsInParentContext (current) || IncludeBaseProperties)
            {
              PropertyFinderBase mixinPropertyFinder = (PropertyFinderBase) TypesafeActivator.CreateInstance (GetType())
                  .With (
                  current,
                  false,
                  (PersistentMixinFinder) null, // mixins on mixins are not checked
                  NameResolver);
              propertyInfos.AddRange (mixinPropertyFinder.FindPropertyInfosInternal (classDefinition));
            }
            current = current.BaseType;
          }
        }
      }
    }

    private bool IsMixinBaseClass (Type type)
    {
      if (!type.IsGenericType)
        return false;
      Type genericTypeDefinition = type.GetGenericTypeDefinition ();
      return genericTypeDefinition == typeof (Mixin<>) || genericTypeDefinition == typeof (Mixin<,>);
    }

    protected virtual bool FindPropertiesFilter (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (!IsOriginalDeclaringType (propertyInfo))
      {
        CheckForMappingAttributes (propertyInfo);
        return false;
      }

      if (IsUnmanagedProperty (propertyInfo))
        return false;

      if (IsUnmanagedExplictInterfaceImplementation (propertyInfo))
        return false;

      return true;
    }

    protected void CheckForMappingAttributes (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      IMappingAttribute[] mappingAttributes = AttributeUtility.GetCustomAttributes<IMappingAttribute> (propertyInfo, false);
      if (mappingAttributes.Length > 0)
      {
        throw new MappingException (
            string.Format (
                "The '{0}' is a mapping attribute and may only be applied at the property's base definition.\r\n  Type: {1}, property: {2}",
                mappingAttributes[0].GetType().FullName,
                propertyInfo.DeclaringType.FullName,
                propertyInfo.Name));
      }
    }

    protected bool IsOriginalDeclaringType (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      Type originalDeclaringType = Remotion.Utilities.ReflectionUtility.GetOriginalDeclaringType (propertyInfo);
      if (originalDeclaringType.IsGenericType && propertyInfo.DeclaringType.IsGenericType)
        return originalDeclaringType.GetGenericTypeDefinition() == propertyInfo.DeclaringType.GetGenericTypeDefinition();
      return originalDeclaringType == propertyInfo.DeclaringType;
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

    private IList<PropertyInfo> FindPropertyInfosInternal (ReflectionBasedClassDefinition classDefinition)
    {
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
