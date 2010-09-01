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
using System.Linq;
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Provides functionality to resolve <see cref="PropertyInfo"/> objects into child objects of <see cref="ReflectionBasedClassDefinition"/>.
  /// </summary>
  public static class ReflectionBasedPropertyResolver
  {
    public static T ResolveDefinition<T> (PropertyInfo property, ReflectionBasedClassDefinition classDefinition, Func<string, T> definitionGetter) 
        where T : class
    {
      ArgumentUtility.CheckNotNull ("property", property);
      ArgumentUtility.CheckNotNull ("definitionGetter", definitionGetter);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (property.DeclaringType.IsInterface)
      {
        Type implementingType = GetImplementingType(classDefinition, property);
        if (implementingType == null)
          return null;

        property = FindPropertyImplementationOnType (property, implementingType);
      }

      string propertyIdentifier = MappingConfiguration.Current.NameResolver.GetPropertyName (new PropertyInfoAdapter (property));
      return definitionGetter (propertyIdentifier);
    }

    private static Type GetImplementingType (ReflectionBasedClassDefinition classDefinition, PropertyInfo interfaceProperty)
    {
      Assertion.IsTrue (interfaceProperty.DeclaringType.IsInterface);

      Type implementingType;
      if (interfaceProperty.DeclaringType.IsAssignableFrom (classDefinition.ClassType))
        implementingType = classDefinition.ClassType;
      else
      {
        var allPersistentMixins = classDefinition
            .CreateSequence (cd => (ReflectionBasedClassDefinition) cd.BaseClass)
            .SelectMany (cd => cd.PersistentMixins);
        implementingType = allPersistentMixins.Where (m => interfaceProperty.DeclaringType.IsAssignableFrom (m)).SingleOrDefault();
      }
      return implementingType;
    }

    private static PropertyInfo FindPropertyImplementationOnType (PropertyInfo interfaceProperty, Type implementationType)
    {
      Assertion.IsTrue (interfaceProperty.DeclaringType.IsInterface);
      
      var interfaceMap = implementationType.GetInterfaceMap (interfaceProperty.DeclaringType);
      var interfaceAccessorMethod = interfaceProperty.GetGetMethod (false) ?? interfaceProperty.GetSetMethod (false);

      var accessorIndex = interfaceMap.InterfaceMethods
          .Select ((m, i) => new { Method = m, Index = i })
          .Single (tuple => tuple.Method == interfaceAccessorMethod)
          .Index;
      var implementationMethod = interfaceMap.TargetMethods[accessorIndex];

      var implementationProperty = implementationType
          .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
          .Single (pi => (pi.GetGetMethod (true) ?? pi.GetSetMethod (true)) == implementationMethod);
      return implementationProperty;
    }
  }
}