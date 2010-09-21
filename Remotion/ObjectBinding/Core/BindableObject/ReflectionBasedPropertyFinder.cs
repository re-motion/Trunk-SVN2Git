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
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Reflection;
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
      var cache = new MultiDictionary<MethodInfo, MethodInfo> ();
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
      var propertyNames = new HashSet<string>();
      
      foreach (Type currentType in GetInheritanceHierarchy ())
      {
        var propertyInfos = currentType.FindMembers (
            MemberTypes.Property, 
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, 
            PropertyFilter, 
            null);
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
          if (!propertyNames.Contains (propertyInfo.Name))
          {
            var introducedMemberAttributes = propertyInfo.GetCustomAttributes (typeof (IntroducedMemberAttribute), true);
            if (introducedMemberAttributes.Length > 0)
            {
              var introducedMemberAttribute = (IntroducedMemberAttribute) introducedMemberAttributes[0];
              var interfaceProperty = new PropertyInfoAdapter (
                  introducedMemberAttribute.IntroducedInterface.GetProperty (introducedMemberAttribute.InterfaceMemberName));
              var mixinProperty = interfaceProperty.FindInterfaceImplementation (introducedMemberAttribute.Mixin);

              yield return new BindableObjectMixinIntroducedPropertyInformation (mixinProperty, currentType, propertyInfo);
            }
            else
            {
              var propertyInfoAdapter = new PropertyInfoAdapter (propertyInfo);
              // TODO Review 3292: Check whether propertyInfoAdapter.FindInterfaceDeclaration() != null. If so, return an InterfaceImplementationPropertyInformation. Add a test checking this. After that change, the ReferenceType.GetMetadata_WithReadWriteExplicitInterfaceScalar() test should work again.
              yield return propertyInfoAdapter;
            }

            propertyNames.Add (propertyInfo.Name);
          }
        }
      }
    }

    private IEnumerable<Type> GetInheritanceHierarchy ()
    {
      for (Type currentType = _concreteType; currentType != null; currentType = currentType.BaseType)
        yield return currentType;
    }

    protected virtual bool PropertyFilter (MemberInfo memberInfo, object filterCriteria)
    {
      // properties explicitly marked invisible are ignored
      var attribute = AttributeUtility.GetCustomAttribute<ObjectBindingAttribute> (memberInfo, true);
      if (attribute != null && !attribute.Visible)
        return false;

      var propertyInfo = (PropertyInfo) memberInfo;

      // indexed properties are ignored
      if (propertyInfo.GetIndexParameters ().Length > 0)
        return false;

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
          && !_interfaceMethodImplementations[accessor].TrueForAll (m => IsInfrastructureProperty (propertyInfo, m));
    }

    protected virtual bool IsInfrastructureProperty (PropertyInfo propertyInfo, MethodInfo accessorDeclaration)
    {
      return accessorDeclaration.DeclaringType.Assembly == typeof (IBusinessObject).Assembly
          || accessorDeclaration.DeclaringType.Assembly == typeof (BindableObjectClass).Assembly
          || accessorDeclaration.DeclaringType.Assembly == typeof (IMixinTarget).Assembly;
    }
  }
}
