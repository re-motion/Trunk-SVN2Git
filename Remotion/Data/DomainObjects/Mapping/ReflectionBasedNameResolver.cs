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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Resolves <see cref="PropertyInfo"/> objects into property names and the other way around.
  /// </summary>
  public class ReflectionBasedNameResolver : IMappingNameResolver
  {
    private static readonly LockingCacheDecorator<IPropertyInformation, string> s_propertyNameCache =
        CacheFactory.CreateWithLocking<IPropertyInformation, string>();

    /// <summary>
    /// Returns the mapping name for the given <paramref name="propertyInformation"/>.
    /// </summary>
    /// <param name="propertyInformation">The property whose mapping name should be retrieved.</param>
    /// <returns>The name of the given <paramref name="propertyInformation"/> as used internally by the mapping.</returns>
    public string GetPropertyName (IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);

      return s_propertyNameCache.GetOrCreateValue (
          propertyInformation, pi => GetPropertyName (pi.GetOriginalDeclaringType(), pi.Name));
    }

    private string GetPropertyName (ITypeInformation type, string shortPropertyName)
    {
      if (type.IsGenericType && !type.IsGenericTypeDefinition)
        type = type.GetGenericTypeDefinition ();

      return type.FullName + "." + shortPropertyName;
    }
  }
}