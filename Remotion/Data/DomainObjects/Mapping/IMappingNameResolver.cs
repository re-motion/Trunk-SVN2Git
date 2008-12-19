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
using System.Reflection;

namespace Remotion.Data.DomainObjects.Mapping
{
  public interface IMappingNameResolver
  {
    /// <summary>
    /// Returns the mapping name for the given <paramref name="property"/>.
    /// </summary>
    /// <param name="property">The property whose mapping name should be retrieved.</param>
    /// <returns>The name of the given <paramref name="property"/> as used internally by the mapping.</returns>
    string GetPropertyName (PropertyInfo property);

    /// <summary>
    /// Returns the mapping name for a property with the given <paramref name="shortPropertyName"/> on the <paramref name="originalDeclaringType"/>.
    /// </summary>
    /// <param name="originalDeclaringType">The type on which the property was first declared.</param>
    /// <param name="shortPropertyName">The short property name of the property.</param>
    string GetPropertyName (Type originalDeclaringType, string shortPropertyName);

    /// <summary>
    /// Returns the property identified by the given mapping property name on the given type.
    /// </summary>
    /// <param name="concreteType">The type on which to search for the property. This can be the same type whose name is encoded in 
    /// <paramref name="propertyName"/> or a derived type or generic specialization.</param>
    /// <param name="propertyName">The long mapping property name of the property to be retrieved.</param>
    /// <returns>The <see cref="PropertyInfo"/> corresponding to the given mapping property.</returns>
    PropertyInfo GetProperty (Type concreteType, string propertyName);
  }
}