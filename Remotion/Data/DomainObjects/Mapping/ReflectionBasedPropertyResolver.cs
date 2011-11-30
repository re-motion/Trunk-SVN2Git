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
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Provides functionality to resolve <see cref="PropertyInfo"/> objects into child objects of <see cref="ClassDefinition"/>.
  /// </summary>
  public static class ReflectionBasedPropertyResolver
  {
    public static T ResolveDefinition<T> (IPropertyInformation propertyInformation, ClassDefinition classDefinition, Func<string, T> definitionGetter) 
        where T : class
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("definitionGetter", definitionGetter);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      IEnumerable<IPropertyInformation> propertyImplementationCandidates;
      if (propertyInformation.DeclaringType.IsInterface)
      {
        var implementingTypes = GetImplementingType (classDefinition, propertyInformation);
        propertyImplementationCandidates = implementingTypes
            .Select (propertyInformation.FindInterfaceImplementation)
            .Where (pi => pi != null);
      }
      else
      {
        propertyImplementationCandidates = new[] { propertyInformation };
      }

      return (from pi in propertyImplementationCandidates
              let propertyIdentifier = MappingConfiguration.Current.NameResolver.GetPropertyName (pi)
              let definition = definitionGetter (propertyIdentifier)
              where definition != null
              select definition).FirstOrDefault();
    }

    private static IEnumerable<Type> GetImplementingType (ClassDefinition classDefinition, IPropertyInformation interfaceProperty)
    {
      Assertion.IsTrue (interfaceProperty.DeclaringType.IsInterface);

      if (interfaceProperty.DeclaringType.IsAssignableFrom (TypeAdapter.Create (classDefinition.ClassType)))
        return new[] { classDefinition.ClassType };
      else
      {
        var allPersistentMixins = classDefinition
            .CreateSequence (cd => cd.BaseClass)
            .SelectMany (cd => cd.PersistentMixins);
        return allPersistentMixins.Where (m => interfaceProperty.DeclaringType.IsAssignableFrom (TypeAdapter.Create (m))).ToArray();
      }
    }
  }
}