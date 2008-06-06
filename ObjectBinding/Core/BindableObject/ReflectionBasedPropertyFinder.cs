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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  public class ReflectionBasedPropertyFinder : IPropertyFinder
  {
    private readonly Type _concreteType;
    private readonly MultiDictionary<MethodInfo, MethodInfo> _interfaceMethodImplementations; // from implementation to declaration

    public ReflectionBasedPropertyFinder (Type concreteType)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      _concreteType = concreteType;

      _interfaceMethodImplementations = GetInterfaceMethodImplementationCache ();
    }

    private MultiDictionary<MethodInfo, MethodInfo> GetInterfaceMethodImplementationCache ()
    {
      MultiDictionary<MethodInfo, MethodInfo> cache = new MultiDictionary<MethodInfo, MethodInfo> ();
      foreach (Type currentType in GetInheritanceHierarchy ())
      {
        foreach (Type interfaceType in currentType.GetInterfaces ())
        {
          InterfaceMapping mapping = currentType.GetInterfaceMap (interfaceType);
          for (int i = 0; i < mapping.TargetMethods.Length; ++i)
            cache.Add (mapping.TargetMethods[i], mapping.InterfaceMethods[i]);
        }
      }
      return cache;
    }

    public IEnumerable<IPropertyInformation> GetPropertyInfos ()
    {
      PropertyInfoCollection propertyInfos = new PropertyInfoCollection ();
      
      foreach (Type currentType in GetInheritanceHierarchy ())
      {
        foreach (PropertyInfo propertyInfo in currentType.FindMembers (MemberTypes.Property,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, PropertyFilter, null))
        {
          if (!propertyInfos.Contains (propertyInfo.Name))
            propertyInfos.Add (new PropertyInfoAdapter (propertyInfo));
        }
      }
      return propertyInfos;
    }

    private IEnumerable<Type> GetInheritanceHierarchy ()
    {
      for (Type currentType = _concreteType; currentType != null; currentType = currentType.BaseType)
        yield return currentType;
    }

    protected virtual bool PropertyFilter (MemberInfo memberInfo, object filterCriteria)
    {
      ObjectBindingAttribute attribute = AttributeUtility.GetCustomAttribute<ObjectBindingAttribute> (memberInfo, true);
      if (attribute != null && !attribute.Visible)
        return false;

      PropertyInfo propertyInfo = (PropertyInfo) memberInfo;

      // property can be an explicit interface implementation or property must have a public getter
      if (IsNonInfrastructurePublicProperty (propertyInfo))
        return true;
      else
        return IsNonInfrastructureInterfaceProperty (propertyInfo);
    }

    private bool IsNonInfrastructurePublicProperty (PropertyInfo propertyInfo)
    {
      MethodInfo accessor = propertyInfo.GetGetMethod (false); // property must have public getter
      return accessor != null
          && !IsInfrastructureProperty (propertyInfo, accessor);
    }

    private bool IsNonInfrastructureInterfaceProperty (PropertyInfo propertyInfo)
    {
      MethodInfo accessor = propertyInfo.GetGetMethod (true);
      return accessor != null
          && _interfaceMethodImplementations.ContainsKey (accessor)
          && !_interfaceMethodImplementations[accessor].TrueForAll (delegate (MethodInfo m) { return IsInfrastructureProperty (propertyInfo, m); });
    }

    protected virtual bool IsInfrastructureProperty (PropertyInfo propertyInfo, MethodInfo accessorDeclaration)
    {
      return accessorDeclaration.DeclaringType.Assembly == typeof (IBusinessObject).Assembly
          || accessorDeclaration.DeclaringType.Assembly == typeof (BindableObjectClass).Assembly
          || accessorDeclaration.DeclaringType.Assembly == typeof (IMixinTarget).Assembly;
    }
  }
}
