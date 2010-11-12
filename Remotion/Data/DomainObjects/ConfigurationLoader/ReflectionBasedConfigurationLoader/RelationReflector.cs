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
    private readonly IRelationEndPointDefinitionFactory _endPointDefinitionFactory;

    public RelationReflector (
        ReflectionBasedClassDefinition classDefinition,
        PropertyInfo propertyInfo,
        IMappingNameResolver nameResolver,
        IRelationEndPointDefinitionFactory endPointDefinitionFactory)
      : base (classDefinition, propertyInfo, typeof (BidirectionalRelationAttribute), nameResolver)
    {
      ArgumentUtility.CheckNotNull ("endPointDefinitionFactory", endPointDefinitionFactory);

      _endPointDefinitionFactory = endPointDefinitionFactory;
    }

    public RelationDefinition GetMetadata (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      var firstEndPoint = _endPointDefinitionFactory.CreateEndPoint (ClassDefinition, PropertyInfo, NameResolver);
      var secondEndPoint = CreateOppositeEndPointDefinition (classDefinitions);

      var relationID = GetRelationID (firstEndPoint, secondEndPoint);
      return new RelationDefinition (relationID, firstEndPoint, secondEndPoint);
    }

    private string GetRelationID (IRelationEndPointDefinition first, IRelationEndPointDefinition second)
    {
      bool isFirstEndPointReal = !first.IsVirtual && !first.IsAnonymous;
      var endPoints = isFirstEndPointReal ? new { Left = first, Right = second } : new { Left = second, Right = first };
      var nameGivingEndPoint = endPoints.Left.PropertyInfo!=null ? endPoints.Left : endPoints.Right;
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

      PropertyInfo oppositePropertyInfo = GetOppositePropertyInfo();
      if (oppositePropertyInfo == null)
      {
        var oppositeClassDefinition = GetOppositeClassDefinition (classDefinitions, null);
        return new PropertyNotFoundRelationEndPointDefinition (oppositeClassDefinition, BidirectionalRelationAttribute.OppositeProperty);
      }
      else
      {
        var oppositeClassDefinition = GetOppositeClassDefinition (classDefinitions, oppositePropertyInfo);
        return _endPointDefinitionFactory.CreateEndPoint (oppositeClassDefinition, oppositePropertyInfo, NameResolver);
      }
    }

    private AnonymousRelationEndPointDefinition CreateOppositeAnonymousRelationEndPointDefinition (ClassDefinitionCollection classDefinitions)
    {
      var oppositeClassDefinition = GetOppositeClassDefinition (classDefinitions, null);
      return new AnonymousRelationEndPointDefinition (oppositeClassDefinition);
    }

    private ReflectionBasedClassDefinition GetOppositeClassDefinition (ClassDefinitionCollection classDefinitions, PropertyInfo optionalOppositePropertyInfo)
    {
      // TODO 3424: Validation rule => use InvalidClassDefinition if classDefinitions[...] is null
      try
      {
        var oppositeClassDefinition = classDefinitions.GetMandatory (ReflectionUtility.GetRelatedObjectTypeFromRelationProperty (PropertyInfo));

        if (optionalOppositePropertyInfo != null)
        {
          while (oppositeClassDefinition.BaseClass != null && oppositeClassDefinition.ClassType != optionalOppositePropertyInfo.DeclaringType)
          {
            oppositeClassDefinition = oppositeClassDefinition.BaseClass;
          }
        }

        return (ReflectionBasedClassDefinition) oppositeClassDefinition;
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