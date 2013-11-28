// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using System.Reflection;
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
    private readonly HashSet<IMethodInformation> _explicitInterfaceImplementations;
    private readonly IMemberInfoNameResolver _nameResolver;
    private readonly bool _includeMixinProperties;
    private readonly IPersistentMixinFinder _persistentMixinFinder;

    protected PropertyFinderBase (
        Type type,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMemberInfoNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull ("persistentMixinFinder", persistentMixinFinder);

      _type = type;
      _nameResolver = nameResolver;
      _includeBaseProperties = includeBaseProperties;
      _includeMixinProperties = includeMixinProperties;
      _persistentMixinFinder = persistentMixinFinder;
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

    public bool IncludeMixinProperties
    {
      get { return _includeMixinProperties; }
    }

    public IMemberInfoNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    protected abstract PropertyFinderBase CreateNewFinder (
        Type type,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMemberInfoNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder);

    public IPropertyInformation[] FindPropertyInfos ()
    {
      var propertyInfos = new List<IPropertyInformation>();

      if (_includeBaseProperties && _type.BaseType != typeof (DomainObject) && _type.BaseType != null)
      {
        // Use a new PropertyFinder of the same type as this one to get all properties from above this class. Mixins are not included for the base
        // classes; the mixin finder below will include the mixins for those classes anyway.
        var propertyFinder = CreateNewFinder (_type.BaseType, true, false, NameResolver, _persistentMixinFinder);
        propertyInfos.AddRange (propertyFinder.FindPropertyInfos());
      }

      propertyInfos.AddRange (FindPropertyInfosDeclaredOnThisType ());

      if (IncludeMixinProperties)
      {
        // Base mixins are included only when base properties are included.
        var mixinPropertyFinder = new MixinPropertyFinder (CreateNewFinder, _persistentMixinFinder, IncludeBaseProperties, NameResolver);
        propertyInfos.AddRange (mixinPropertyFinder.FindPropertyInfosOnMixins ());
      }

      return propertyInfos.ToArray();
    }

    protected virtual bool FindPropertiesFilter (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      if (!propertyInfo.IsOriginalDeclaration())
        return false;

      if (IsUnmanagedProperty (propertyInfo))
        return false;

      if (IsUnmanagedExplictInterfaceImplementation (propertyInfo))
        return false;

      return true;
    }

    protected bool IsUnmanagedProperty (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      var storageClassAttribute = propertyInfo.GetCustomAttribute<StorageClassAttribute> (false);
      if (storageClassAttribute == null)
        return false;

      return storageClassAttribute.StorageClass == StorageClass.None;
    }

    protected bool IsUnmanagedExplictInterfaceImplementation (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      bool isExplicitInterfaceImplementation = Array.Exists (
          propertyInfo.GetAccessors (true),
          accessor => _explicitInterfaceImplementations.Contains (accessor));
      if (!isExplicitInterfaceImplementation)
        return false;

      var storageClassAttribute = propertyInfo.GetCustomAttribute<StorageClassAttribute> (false);
      if (storageClassAttribute == null)
        return true;

      return storageClassAttribute.StorageClass == StorageClass.None;
    }

    public IEnumerable<IPropertyInformation> FindPropertyInfosDeclaredOnThisType ()
    {
      MemberInfo[] memberInfos = _type.FindMembers (
          MemberTypes.Property,
          PropertyBindingFlags | BindingFlags.DeclaredOnly,
          FindPropertiesFilter, 
          null);

      var propertyInfos = Array.ConvertAll (memberInfos, input => PropertyInfoAdapter.Create((PropertyInfo) input));

      return propertyInfos;
    }

    private bool FindPropertiesFilter (MemberInfo member, object filterCriteria)
    {
      return FindPropertiesFilter (PropertyInfoAdapter.Create((PropertyInfo) member));
    }

    private HashSet<IMethodInformation> GetExplicitInterfaceImplementations (Type type)
    {
      var explicitInterfaceImplementationSet = new HashSet<IMethodInformation> ();

      foreach (Type interfaceType in type.GetInterfaces())
      {
        InterfaceMapping interfaceMapping = type.GetInterfaceMap (interfaceType);
        MethodInfo[] explicitInterfaceImplementations = Array.FindAll (
            interfaceMapping.TargetMethods,
            targetMethod => targetMethod.IsSpecialName && !targetMethod.IsPublic);
        explicitInterfaceImplementationSet.UnionWith (explicitInterfaceImplementations.Select (mi => (IMethodInformation) MethodInfoAdapter.Create(mi)));
      }

      return explicitInterfaceImplementationSet;
    }
  }
}