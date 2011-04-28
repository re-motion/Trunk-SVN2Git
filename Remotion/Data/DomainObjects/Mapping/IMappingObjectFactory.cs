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

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// <see cref="IMappingObjectFactory"/> defines the API for creating maping objects.
  /// </summary>
  public interface IMappingObjectFactory
  {
    ClassDefinition CreateClassDefinition (Type type, ClassDefinition baseClass);
    PropertyDefinition CreatePropertyDefinition (ClassDefinition classDefinition, PropertyInfo propertyInfo);
    RelationDefinition CreateRelationDefinition (
        IDictionary<Type, ClassDefinition> classDefinitions, ClassDefinition classDefinition, PropertyInfo propertyInfo);
    IRelationEndPointDefinition CreateRelationEndPointDefinition (ClassDefinition classDefinition, PropertyInfo propertyInfo);

    ClassDefinition[] CreateClassDefinitionCollection (IEnumerable<Type> types);
    PropertyDefinitionCollection CreatePropertyDefinitionCollection (ClassDefinition classDefinition, IEnumerable<PropertyInfo> propertyInfos);
    RelationDefinition[] CreateRelationDefinitionCollection (IDictionary<Type, ClassDefinition> classDefinitions);
    RelationEndPointDefinitionCollection CreateRelationEndPointDefinitionCollection (ClassDefinition classDefinition);
  }
}