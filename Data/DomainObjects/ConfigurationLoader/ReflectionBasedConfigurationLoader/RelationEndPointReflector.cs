using System;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class RelationEndPointReflector: RelationReflectorBase
  {
    public static RelationEndPointReflector CreateRelationEndPointReflector (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      return new RdbmsRelationEndPointReflector (classDefinition, propertyInfo);
    }

    public RelationEndPointReflector (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
      : this (classDefinition, propertyInfo, typeof (BidirectionalRelationAttribute))
    {
    }

    protected RelationEndPointReflector (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, Type bidirectionalRelationAttributeType)
      : base (classDefinition, propertyInfo, bidirectionalRelationAttributeType)
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