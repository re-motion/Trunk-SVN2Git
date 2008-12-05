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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class RelationEndPointReflector: RelationReflectorBase
  {
    public static RelationEndPointReflector CreateRelationEndPointReflector (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, IMappingNameResolver nameResolver)
    {
      return new RdbmsRelationEndPointReflector (classDefinition, propertyInfo, nameResolver);
    }

    public RelationEndPointReflector (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, IMappingNameResolver nameResolver)
      : this (classDefinition, propertyInfo, typeof (BidirectionalRelationAttribute), nameResolver)
    {
    }

    protected RelationEndPointReflector (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, Type bidirectionalRelationAttributeType, IMappingNameResolver nameResolver)
      : base (classDefinition, propertyInfo, bidirectionalRelationAttributeType, nameResolver)
    {
    }

    public IRelationEndPointDefinition GetMetadata ()
    {
      ValidatePropertyInfo();
      if (IsVirtualEndRelationEndpoint())
        return CreateVirtualRelationEndPointDefinition (ClassDefinition);
      else
        return CreateRelationEndPointDefinition (ClassDefinition);
    }

    public virtual bool IsVirtualEndRelationEndpoint()
    {
      if (!IsBidirectionalRelation)
        return false;
      return ReflectionUtility.IsObjectList (PropertyInfo.PropertyType);
    }

    private RelationEndPointDefinition CreateRelationEndPointDefinition (ClassDefinition classDefinition)
    {
      return new RelationEndPointDefinition (classDefinition, GetPropertyName(), !IsNullable());
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinition (ClassDefinition classDefinition)
    {
      return
          new ReflectionBasedVirtualRelationEndPointDefinition (
              classDefinition, GetPropertyName(), !IsNullable(), GetCardinality(), PropertyInfo.PropertyType, GetSortExpression(), PropertyInfo);
    }

    private CardinalityType GetCardinality()
    {
      return ReflectionUtility.IsObjectList (PropertyInfo.PropertyType) ? CardinalityType.Many : CardinalityType.One;
    }

    protected virtual string GetSortExpression ()
    {
      return null;
    }

    private bool IsNullable ()
    {
      return IsNullableFromAttribute ();
    }
  }
}
