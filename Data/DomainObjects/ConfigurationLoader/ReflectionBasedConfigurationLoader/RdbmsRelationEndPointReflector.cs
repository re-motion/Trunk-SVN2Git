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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
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

    protected override void ValidatePropertyInfo ()
    {
      base.ValidatePropertyInfo();

      if (ReflectionUtility.IsObjectList (PropertyInfo.PropertyType) && ContainsKey())
      {
        throw CreateMappingException (
            null,
            PropertyInfo,
            "Only relation end points with a property type of '{0}' can contain the foreign key.",
            typeof (DomainObject));
      }

      if (!ReflectionUtility.IsObjectList (PropertyInfo.PropertyType) && !string.IsNullOrEmpty (GetSortExpression()))
      {
        throw CreateMappingException (
            null,
            PropertyInfo,
            "Only relation end points with a property type of '{0}' can have a sort expression.",
            typeof (ObjectList<>));
      }
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
