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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="RelationDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class RelationReflector : RelationReflectorBase
  {
    public static RelationReflector CreateRelationReflector (
        ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, IMappingNameResolver nameResolver)
    {
      return new RelationReflector (classDefinition, propertyInfo, nameResolver);
    }

    public RelationReflector (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, IMappingNameResolver nameResolver)
        : this (classDefinition, propertyInfo, typeof (BidirectionalRelationAttribute), nameResolver)
    {
    }

    protected RelationReflector (
        ReflectionBasedClassDefinition classDefinition,
        PropertyInfo propertyInfo,
        Type bidirectionalRelationAttributeType,
        IMappingNameResolver nameResolver)
        : base (classDefinition, propertyInfo, bidirectionalRelationAttributeType, nameResolver)
    {
    }

    public RelationDefinition GetMetadata (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      RelationEndPointReflector relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (
          ClassDefinition, PropertyInfo, NameResolver);

      var firstEndPoint = relationEndPointReflector.GetMetadata();
      var secondEndPoint = CreateOppositeEndPointDefinition (classDefinitions);
      var relationID = GetRelationID (firstEndPoint, secondEndPoint);
      return new RelationDefinition (relationID, firstEndPoint, secondEndPoint);
    }

    private string GetRelationID (IRelationEndPointDefinition first, IRelationEndPointDefinition second)
    {
      bool isFirstEndPointReal = !first.IsVirtual && !first.IsAnonymous;
      var endPoints = isFirstEndPointReal ? new { Left = first, Right = second } : new { Left = second, Right = first };
      var nameGivingEndPoint = endPoints.Left;
      var leftPropertyName = NameResolver.GetPropertyName (new PropertyInfoAdapter (nameGivingEndPoint.PropertyInfo));
      if (endPoints.Right.IsAnonymous)
        return string.Format ("{0}:{1}", nameGivingEndPoint.ClassDefinition.ClassType.FullName, leftPropertyName);
      else
      {
        var rightPropertyName = NameResolver.GetPropertyName (new PropertyInfoAdapter (endPoints.Right.PropertyInfo));
        return string.Format ("{0}:{1}->{2}", nameGivingEndPoint.ClassDefinition.ClassType.FullName, leftPropertyName, rightPropertyName);
      }
    }

    private IRelationEndPointDefinition CreateOppositeEndPointDefinition (ClassDefinitionCollection classDefinitions)
    {
      if (!IsBidirectionalRelation)
        return CreateOppositeAnonymousRelationEndPointDefinition (classDefinitions);

      RelationEndPointReflector oppositeRelationEndPointReflector = CreateOppositeRelationEndPointReflector (classDefinitions);
      return oppositeRelationEndPointReflector.GetMetadata();
    }

    private AnonymousRelationEndPointDefinition CreateOppositeAnonymousRelationEndPointDefinition (ClassDefinitionCollection classDefinitions)
    {
      var oppositeClassDefinition = GetOppositeClassDefinition (classDefinitions);
      return new AnonymousRelationEndPointDefinition (oppositeClassDefinition);
    }

    // TODO 3424: 2. Inline this method
    private RelationEndPointReflector CreateOppositeRelationEndPointReflector (ClassDefinitionCollection classDefinitions)
    {
      PropertyInfo oppositePropertyInfo = GetOppositePropertyInfo ();
      //TODO 3424: create rule. In the rule, get the oppositePropertyInfo via endPoint.GetOppositeEndPointDefinition().PropertyInfo (if not null)

      Type oppositeDomainObjectType = ReflectionUtility.GetRelatedObjectTypeFromRelationProperty (oppositePropertyInfo);
      bool isPropertyDeclaredByThisClassDefinition = DeclaringDomainObjectTypeForProperty == ClassDefinition.ClassType;
      if (isPropertyDeclaredByThisClassDefinition)
      {
        // Case where property is declared on this ClassDefinition => it is declared below/on the inheritance root
        // In this case, the opposite property's return type must exactly match this ClassDefinition's type.
        if (ClassDefinition.ClassType != oppositeDomainObjectType)
        {
          throw CreateMappingException (
              null,
              PropertyInfo,
              "The declaring type '{0}' does not match the type of the opposite relation propery '{1}' declared on type '{2}'.\r\n",
              DeclaringDomainObjectTypeForProperty.Name,
              BidirectionalRelationAttribute.OppositeProperty,
              oppositePropertyInfo.DeclaringType.Name);
        }
      }
      else
      {
        // Case where property is not declared on this ClassDefinition => it must be declared above the inheritance root
        // In this case, the opposite property's return type must be assignable to the type declaring the property. This enables the following 
        // scenario:
        // - ClassAboveInheritanceRoot has a relation property P1 to RelationTarget
        // - RelationTarget has a relation property P2 back to the InheritanceRoot derived from ClassAboveInheritanceRoot
        // In that case, when reflecting P1, DeclaringDomainObjectTypeForProperty will be ClassAboveInheritanceRoot, oppositeDomainObjectType will be
        // InheritanceRoot. ClassAboveInheritanceRoot is assignable from InheritanceRoo, so the check passes.

        // This is the only case where the two sides of a bidirectional relation can point to subclasses of each other.
        // (The scenario this was actually needed for is to allow for generic base classes above the inheritance root defining relation properties.)
        if (!DeclaringDomainObjectTypeForProperty.IsAssignableFrom (oppositeDomainObjectType))
        {
          Assertion.IsTrue (DeclaringDomainObjectTypeForProperty.IsAssignableFrom (ClassDefinition.ClassType));
          throw CreateMappingException (
              null,
              PropertyInfo,
              "The declaring type '{0}' cannot be assigned to the type of the opposite relation propery '{1}' declared on type '{2}'.\r\n",
              DeclaringDomainObjectTypeForProperty.Name,
              BidirectionalRelationAttribute.OppositeProperty,
              oppositePropertyInfo.DeclaringType.Name);
        }
      }

      // TODO 3424: 4. If oppositePropertyInfo == null, return new PropertyNotFoundRelationEndPoint (info that used to be in exception) => after inlining this method and changing GetOppositePropertyInfo to return null
      // TODO 3424: 5. Add new rule: if an end point is a PropertyNotFoundRelationEndPoint, return a validation error

      var oppositeClassDefinition = GetOppositeClassDefinition (classDefinitions);
      return RelationEndPointReflector.CreateRelationEndPointReflector (oppositeClassDefinition, oppositePropertyInfo, NameResolver);
    }

    private ReflectionBasedClassDefinition GetOppositeClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      // TODO 3424: Validation rule => use InvalidClassDefinition if classDefinitions[...] is null
      try
      {
        return (ReflectionBasedClassDefinition) classDefinitions.GetMandatory (ReflectionUtility.GetRelatedObjectTypeFromRelationProperty (PropertyInfo));
      }
      catch (MappingException e)
      {
        // This is the case where the PropertyInfo's related object type is a DomainObject type above the inheritance root.
        // TODO 3424: Tests missing for bidirectional and unidirectional case
        throw CreateMappingException (null, PropertyInfo, e.Message);
      }
    }
  }
}