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
using System.Reflection;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Resolves <see cref="PropertyInfo"/> objects into property names and the other way around.
  /// </summary>
  public class ReflectionBasedNameResolver : IMappingNameResolver
  {
    private static readonly InterlockedCache<Tuple<Type, string>, string> s_propertyNameCache = new InterlockedCache<Tuple<Type, string>, string>();

    /// <summary>
    /// Returns the mapping name for the given <paramref name="property"/>.
    /// </summary>
    /// <param name="property">The property whose mapping name should be retrieved.</param>
    /// <returns>The name of the given <paramref name="property"/> as used internally by the mapping.</returns>
    public string GetPropertyName (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      Type originalDeclaringType = Remotion.Utilities.ReflectionUtility.GetOriginalDeclaringType (property);
      return GetPropertyName (originalDeclaringType, property.Name);
    }

    /// <summary>
    /// Returns the mapping name for a property with the given <paramref name="shortPropertyName"/> on the <paramref name="originalDeclaringType"/>.
    /// </summary>
    /// <param name="originalDeclaringType">The type on which the property was first declared.</param>
    /// <param name="shortPropertyName">The short property name of the property.</param>
    public string GetPropertyName (Type originalDeclaringType, string shortPropertyName)
    {
      ArgumentUtility.CheckNotNull ("originalDeclaringType", originalDeclaringType);
      ArgumentUtility.CheckNotNull ("shortPropertyName", shortPropertyName);

      return s_propertyNameCache.GetOrCreateValue (
          Tuple.Create (originalDeclaringType, shortPropertyName), 
          key => GetPropertyNameInternal (key.Item1, key.Item2));
    }

    private string GetPropertyNameInternal (Type originalDeclaringType, string shortPropertyName)
    {
      if (originalDeclaringType.IsGenericType && !originalDeclaringType.IsGenericTypeDefinition)
        originalDeclaringType = originalDeclaringType.GetGenericTypeDefinition ();

      return originalDeclaringType.FullName + "." + shortPropertyName;
    }
  }
}
