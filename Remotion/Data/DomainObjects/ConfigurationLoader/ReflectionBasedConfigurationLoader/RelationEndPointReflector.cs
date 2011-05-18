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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="IPropertyInformation"/>.</summary>
  public static class RelationEndPointReflector
  {
    public static RdbmsRelationEndPointReflector CreateRelationEndPointReflector (
        ClassDefinition classDefinition,
        IPropertyInformation propertyInfo, 
        IMappingNameResolver nameResolver)
    {
      return new RdbmsRelationEndPointReflector (classDefinition, propertyInfo, nameResolver);
    }
  }

  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="IPropertyInformation"/>.</summary>
  public abstract class RelationEndPointReflector<T>: RelationReflectorBase<T> where T: BidirectionalRelationAttribute
  {
    protected RelationEndPointReflector (ClassDefinition classDefinition, IPropertyInformation propertyInfo, IMappingNameResolver nameResolver)
        : base (classDefinition, propertyInfo, nameResolver)
    {
    }

    public IRelationEndPointDefinition GetMetadata ()
    {
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

    private IRelationEndPointDefinition CreateRelationEndPointDefinition (ClassDefinition classDefinition)
    {
      var propertyName = GetPropertyName();

      PropertyDefinition propertyDefinition = classDefinition[propertyName];
      if (!propertyDefinition.IsObjectID)
        return new TypeNotObjectIDRelationEndPointDefinition (classDefinition, propertyName, propertyDefinition.PropertyType);
      else
        return new RelationEndPointDefinition (propertyDefinition, !IsNullable());
    }

    private IRelationEndPointDefinition CreateVirtualRelationEndPointDefinition (ClassDefinition classDefinition)
    {
      return new VirtualRelationEndPointDefinition (
          classDefinition, GetPropertyName(), !IsNullable(), GetCardinality(), PropertyInfo.PropertyType, GetSortExpression(), PropertyInfo);
    }

    private CardinalityType GetCardinality()
    {
      return ReflectionUtility.IsObjectList (PropertyInfo.PropertyType) ? CardinalityType.Many : CardinalityType.One;
    }

    protected string GetSortExpression ()
    {
      if (!IsBidirectionalRelation)
        return null;

      return BidirectionalRelationAttribute.SortExpression;
    }

    private bool IsNullable ()
    {
      return IsNullableFromAttribute ();
    }
  }
}
