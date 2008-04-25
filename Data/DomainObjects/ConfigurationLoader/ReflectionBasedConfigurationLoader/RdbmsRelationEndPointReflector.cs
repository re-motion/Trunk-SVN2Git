using System;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="PropertyInfo"/> for types persisted in an <b>RDBMS</b>.</summary>
  public class RdbmsRelationEndPointReflector : RelationEndPointReflector
  {
    public RdbmsRelationEndPointReflector (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
        : this (classDefinition, propertyInfo, typeof (DBBidirectionalRelationAttribute))
    {
    }

    protected RdbmsRelationEndPointReflector (
        ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, Type bidirectionalRelationAttributeType)
        : base (
            classDefinition,
            propertyInfo,
            ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom (
                "bidirectionalRelationAttributeType", bidirectionalRelationAttributeType, typeof (DBBidirectionalRelationAttribute)))
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