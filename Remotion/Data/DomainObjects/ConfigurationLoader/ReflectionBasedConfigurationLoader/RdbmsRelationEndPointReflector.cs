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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="PropertyInfo"/> for types persisted in an <b>RDBMS</b>.</summary>
  public class RdbmsRelationEndPointReflector : RelationEndPointReflector
  {
    public RdbmsRelationEndPointReflector (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, IMappingNameResolver nameResolver)
        : this (classDefinition, propertyInfo, typeof (DBBidirectionalRelationAttribute), nameResolver)
    {
    }

    protected RdbmsRelationEndPointReflector (
        ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, Type bidirectionalRelationAttributeType, IMappingNameResolver nameResolver)
        : base (
            classDefinition,
            propertyInfo,
            ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom (
                "bidirectionalRelationAttributeType", bidirectionalRelationAttributeType, typeof (DBBidirectionalRelationAttribute)),
            nameResolver)
    {
    }

    public DBBidirectionalRelationAttribute DBBidirectionalRelationAttribute
    {
      get { return (DBBidirectionalRelationAttribute) BidirectionalRelationAttribute; }
    }

    public override bool IsVirtualEndRelationEndpoint ()
    {
      if (base.IsVirtualEndRelationEndpoint())
        return true;

      return !ContainsKey();
    }

    private bool ContainsKey ()
    {
      if (!IsBidirectionalRelation)
        return true;

      if (DBBidirectionalRelationAttribute.ContainsForeignKey)
        return true;

      return ReflectionUtility.IsObjectList (GetOppositePropertyInfo().PropertyType);
    }

    protected override string GetSortExpression ()
    {
      if (!IsBidirectionalRelation)
        return null;

      return DBBidirectionalRelationAttribute.SortExpression;
    }
  }
}
