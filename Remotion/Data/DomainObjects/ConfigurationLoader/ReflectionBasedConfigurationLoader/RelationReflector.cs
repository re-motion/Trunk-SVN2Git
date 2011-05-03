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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="RelationDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class RelationReflector : RelationReflectorBase<BidirectionalRelationAttribute>
  {
    public RelationReflector (ClassDefinition classDefinition, PropertyInfo propertyInfo, IMappingNameResolver nameResolver)
        : base (classDefinition, propertyInfo, nameResolver)
    {
    }

    public RelationDefinition GetMetadata (IDictionary<Type, ClassDefinition> classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      var firstEndPoint = GetEndPointDefinition (ClassDefinition, PropertyInfo);
      var secondEndPoint = GetOppositeEndPointDefinition (classDefinitions);

      var relationID = GetRelationID (firstEndPoint, secondEndPoint);
      return new RelationDefinition (relationID, firstEndPoint, secondEndPoint);
    }

    private string GetRelationID (IRelationEndPointDefinition first, IRelationEndPointDefinition second)
    {
      bool isFirstEndPointReal = !first.IsVirtual && !first.IsAnonymous;
      var endPoints = isFirstEndPointReal ? new { Left = first, Right = second } : new { Left = second, Right = first };
      
      var nameGivingEndPoint = endPoints.Left.PropertyInfo != null ? endPoints.Left : endPoints.Right;
      var leftPropertyName = NameResolver.GetPropertyName (nameGivingEndPoint.PropertyInfo);

      if (endPoints.Right.PropertyInfo == null)
      {
        return string.Format ("{0}:{1}", nameGivingEndPoint.ClassDefinition.ClassType.FullName, leftPropertyName);
      }
      else
      {
        var rightPropertyName = NameResolver.GetPropertyName (endPoints.Right.PropertyInfo);
        return string.Format ("{0}:{1}->{2}", nameGivingEndPoint.ClassDefinition.ClassType.FullName, leftPropertyName, rightPropertyName);
      }
    }

    private IRelationEndPointDefinition GetOppositeEndPointDefinition (IDictionary<Type, ClassDefinition> classDefinitions)
    {
      if (!IsBidirectionalRelation)
        return CreateOppositeAnonymousRelationEndPointDefinition (classDefinitions);

      var oppositePropertyInfo = GetOppositePropertyInfo();
      if (oppositePropertyInfo == null)
      {
        var oppositeClassDefinition = GetOppositeClassDefinition (classDefinitions, null);
        return new PropertyNotFoundRelationEndPointDefinition (oppositeClassDefinition, BidirectionalRelationAttribute.OppositeProperty);
      }
      else
      {
        var oppositeClassDefinition = GetOppositeClassDefinition (classDefinitions, oppositePropertyInfo);
        return GetEndPointDefinition (oppositeClassDefinition, oppositePropertyInfo);
      }
    }

    private AnonymousRelationEndPointDefinition CreateOppositeAnonymousRelationEndPointDefinition (IDictionary<Type, ClassDefinition> classDefinitions)
    {
      var oppositeClassDefinition = GetOppositeClassDefinition (classDefinitions, null);
      return new AnonymousRelationEndPointDefinition (oppositeClassDefinition);
    }

    private IRelationEndPointDefinition GetEndPointDefinition (ClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      var endPointDefinition = classDefinition.GetRelationEndPointDefinition (NameResolver.GetPropertyName (new PropertyInfoAdapter (propertyInfo)));
      if (endPointDefinition != null)
        return endPointDefinition;

      return new PropertyNotFoundRelationEndPointDefinition (classDefinition, propertyInfo.Name);
    }

    private ClassDefinition GetOppositeClassDefinition (IDictionary<Type,ClassDefinition> classDefinitions, PropertyInfo optionalOppositePropertyInfo)
    {
      var type = ReflectionUtility.GetRelatedObjectTypeFromRelationProperty (PropertyInfo);
      var oppositeClassDefinition = classDefinitions.GetValueOrDefault (type);
      if (oppositeClassDefinition == null)
      {
        var notFoundClassDefinition = new ClassDefinitionForUnresolvedRelationPropertyType (type.Name, type, PropertyInfo);
        notFoundClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection ());
        notFoundClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
        return notFoundClassDefinition;
      }

      if (optionalOppositePropertyInfo != null)
      {
        while (oppositeClassDefinition.BaseClass != null && oppositeClassDefinition.ClassType != optionalOppositePropertyInfo.DeclaringType)
          oppositeClassDefinition = oppositeClassDefinition.BaseClass;
      }

      return oppositeClassDefinition;
    }
  }
}