using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="RelationDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class RelationReflector : RelationReflectorBase
  {
    public static RelationReflector CreateRelationReflector (
        ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      return new RelationReflector (classDefinition, propertyInfo);
    }

    public RelationReflector (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
        : this (classDefinition, propertyInfo, typeof (BidirectionalRelationAttribute))
    {
    }

    protected RelationReflector (
        ReflectionBasedClassDefinition classDefinition,
        PropertyInfo propertyInfo,
        Type bidirectionalRelationAttributeType)
        : base (classDefinition, propertyInfo, bidirectionalRelationAttributeType)
    {
    }

    public Type DomainObjectTypeDeclaringProperty
    {
      get
      {
        if (IsMixedProperty)
          return ClassDefinition.ClassType;
        else
          return PropertyInfo.DeclaringType;
      }
    }

    public RelationDefinition GetMetadata (ClassDefinitionCollection classDefinitions, RelationDefinitionCollection relationDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("relationDefinitions", relationDefinitions);

      ValidatePropertyInfo();
      RelationEndPointReflector relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (ClassDefinition, PropertyInfo);

      if (relationEndPointReflector.IsVirtualEndRelationEndpoint())
      {
        ValidateVirtualEndPointPropertyInfo (classDefinitions);
        return null;
      }
      else
      {
        string relationID = GetRelationID();
        if (relationDefinitions.Contains (relationID))
          return relationDefinitions[relationID];

        RelationDefinition relationDefinition = new RelationDefinition (
            relationID,
            relationEndPointReflector.GetMetadata (),
            CreateOppositeEndPointDefinition (classDefinitions));

        AddRelationDefinitionToClassDefinitions (relationDefinition);
        relationDefinitions.Add (relationDefinition);

        return relationDefinition;
      }
    }

    private void ValidateVirtualEndPointPropertyInfo (ClassDefinitionCollection classDefinitions)
    {
      RelationEndPointReflector oppositeRelationEndPointReflector = CreateOppositeRelationEndPointReflector (classDefinitions);
      if (oppositeRelationEndPointReflector.IsVirtualEndRelationEndpoint())
      {
        throw CreateMappingException (
            null,
            PropertyInfo,
            "A bidirectional relation can only have one virtual relation end point.",
            BidirectionalRelationAttribute.GetType().FullName);
      }
    }

    protected override void ValidatePropertyInfo ()
    {
      base.ValidatePropertyInfo();

      if (!IsBidirectionalRelation && !typeof (DomainObject).IsAssignableFrom (PropertyInfo.PropertyType))
      {
        throw CreateMappingException (
            null, PropertyInfo, "The property type of an uni-directional relation property must be assignable to {0}.", typeof (DomainObject).FullName);
      }
    }

    protected void ValidateOppositePropertyInfo (PropertyInfo oppositePropertyInfo, ClassDefinitionCollection classDefintions)
    {
      ArgumentUtility.CheckNotNull ("oppositePropertyInfo", oppositePropertyInfo);
      ArgumentUtility.CheckNotNull ("classDefintions", classDefintions);

      ValidateOppositePropertyInfoDeclaringType (oppositePropertyInfo, classDefintions);

      ValidateOppositePropertyInfoBidirectionalRelationAttribute (oppositePropertyInfo);
    }

    private string GetRelationID ()
    {
      if (IsMixedProperty)
      {
        string propertyName = ReflectionUtility.GetPropertyName (PropertyInfo);
        return string.Format ("{0}->{1}", ClassDefinition.ClassType.FullName, propertyName);
      }
      else if (ClassDefinition.BaseClass == null && ClassDefinition.ClassType != PropertyInfo.DeclaringType)
        return ReflectionUtility.GetPropertyName (ClassDefinition.ClassType, PropertyInfo.Name);
      return ReflectionUtility.GetPropertyName (PropertyInfo);
    }

    private IRelationEndPointDefinition CreateOppositeEndPointDefinition (ClassDefinitionCollection classDefinitions)
    {
      if (!IsBidirectionalRelation)
        return CreateOppositeAnonymousRelationEndPointDefinition (classDefinitions);

      RelationEndPointReflector oppositeRelationEndPointReflector = CreateOppositeRelationEndPointReflector (classDefinitions);
      return oppositeRelationEndPointReflector.GetMetadata ();
    }

    private AnonymousRelationEndPointDefinition CreateOppositeAnonymousRelationEndPointDefinition (ClassDefinitionCollection classDefinitions)
    {
      try
      {
        return new AnonymousRelationEndPointDefinition (classDefinitions.GetMandatory (PropertyInfo.PropertyType));
      }
      catch (MappingException e)
      {
        throw CreateMappingException (null, PropertyInfo, e.Message);
      }
    }

    private RelationEndPointReflector CreateOppositeRelationEndPointReflector (ClassDefinitionCollection classDefinitions)
    {
      PropertyInfo oppositePropertyInfo = GetOppositePropertyInfo();
      ValidateOppositePropertyInfo (oppositePropertyInfo, classDefinitions);

      ReflectionBasedClassDefinition classDefinition;
      try
      {
        classDefinition = (ReflectionBasedClassDefinition) classDefinitions.GetMandatory (GetDomainObjectTypeFromRelationProperty (PropertyInfo));
      }
      catch (MappingException e)
      {
        throw CreateMappingException (null, oppositePropertyInfo, e.Message);
      }

      return RelationEndPointReflector.CreateRelationEndPointReflector (classDefinition, oppositePropertyInfo);
    }

    private void AddRelationDefinitionToClassDefinitions (RelationDefinition relationDefinition)
    {
      IRelationEndPointDefinition endPoint1 = relationDefinition.EndPointDefinitions[0];
      IRelationEndPointDefinition endPoint2 = relationDefinition.EndPointDefinitions[1];

      if (!endPoint1.IsNull)
        endPoint1.ClassDefinition.MyRelationDefinitions.Add (relationDefinition);

      if (endPoint1.ClassDefinition != endPoint2.ClassDefinition && !endPoint2.IsNull)
        endPoint2.ClassDefinition.MyRelationDefinitions.Add (relationDefinition);
    }


    private void ValidateOppositePropertyInfoDeclaringType (PropertyInfo oppositePropertyInfo, ClassDefinitionCollection classDefintions)
    {
      Type oppositeDomainObjectType = GetDomainObjectTypeFromRelationProperty (oppositePropertyInfo);
      if (classDefintions.Contains (DomainObjectTypeDeclaringProperty))
      {
        if (DomainObjectTypeDeclaringProperty != oppositeDomainObjectType)
        {
          throw CreateMappingException (
              null,
              PropertyInfo,
              "The declaring type '{0}' does not match the type of the opposite relation propery '{1}' declared on type '{2}'.",
              DomainObjectTypeDeclaringProperty.Name,
              BidirectionalRelationAttribute.OppositeProperty,
              oppositePropertyInfo.DeclaringType.FullName);
        }
      }
      else
      {
        if (DomainObjectTypeDeclaringProperty.IsAssignableFrom (oppositeDomainObjectType))
          return;

        throw CreateMappingException (
            null,
            PropertyInfo,
            "The declaring type '{0}' cannot be assigned to the type of the opposite relation propery '{1}' declared on type '{2}'.",
            DomainObjectTypeDeclaringProperty.Name,
            BidirectionalRelationAttribute.OppositeProperty,
            oppositePropertyInfo.DeclaringType.FullName);
      }
    }

    private void ValidateOppositePropertyInfoBidirectionalRelationAttribute (PropertyInfo oppositePropertyInfo)
    {
      BidirectionalRelationAttribute oppositeBidirectionalRelationAttribute =
          (BidirectionalRelationAttribute) AttributeUtility.GetCustomAttribute (oppositePropertyInfo, BidirectionalRelationAttribute.GetType(), true);

      if (oppositeBidirectionalRelationAttribute == null)
      {
        throw CreateMappingException (
            null,
            PropertyInfo,
            "Opposite relation property '{0}' declared on type '{1}' does not define a matching '{2}'.",
            BidirectionalRelationAttribute.OppositeProperty,
            oppositePropertyInfo.DeclaringType.FullName,
            BidirectionalRelationAttribute.GetType().FullName);
      }

      if (!PropertyInfo.Name.Equals (oppositeBidirectionalRelationAttribute.OppositeProperty, StringComparison.Ordinal))
      {
        throw CreateMappingException (
            null,
            PropertyInfo,
            "Opposite relation property '{0}' declared on type '{1}' defines a '{2}' whose opposite property does not match.",
            BidirectionalRelationAttribute.OppositeProperty,
            oppositePropertyInfo.DeclaringType.FullName,
            BidirectionalRelationAttribute.GetType().FullName);
      }
    }
  }
}