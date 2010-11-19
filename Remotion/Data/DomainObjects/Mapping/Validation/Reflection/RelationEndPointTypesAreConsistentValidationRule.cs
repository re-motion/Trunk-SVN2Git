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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Reflection
{
  /// <summary>
  /// Validates that the relation end point property types are consistent.
  /// </summary>
  public class RelationEndPointTypesAreConsistentValidationRule : IRelationDefinitionValidatorRule
  {
    public MappingValidationResult Validate (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      foreach (var endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        var validationResult = Validate (endPointDefinition);
        if (!validationResult.IsValid)
          return validationResult;
      }

      return MappingValidationResult.CreateValidResult();
    }

    private MappingValidationResult Validate (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      if (relationEndPointDefinition.PropertyInfo != null && relationEndPointDefinition.ClassDefinition is ReflectionBasedClassDefinition)
      {
        var relationAttribute =
            (BidirectionalRelationAttribute)
            AttributeUtility.GetCustomAttribute (relationEndPointDefinition.PropertyInfo, typeof (BidirectionalRelationAttribute), true);
        var oppositePropertyInfo = relationEndPointDefinition.GetOppositeEndPointDefinition().PropertyInfo;
        if (oppositePropertyInfo != null && relationAttribute!=null)
        {
          var classDefinition = relationEndPointDefinition.ClassDefinition as ReflectionBasedClassDefinition;
          var oppositeDomainObjectType = ReflectionUtility.GetRelatedObjectTypeFromRelationProperty (oppositePropertyInfo);
          var declaringDomainObjectTypeForProperty = ReflectionUtility.GetDeclaringDomainObjectTypeForProperty (relationEndPointDefinition.PropertyInfo, classDefinition);
          bool isPropertyDeclaredByThisClassDefinition = declaringDomainObjectTypeForProperty == classDefinition.ClassType;
          if (isPropertyDeclaredByThisClassDefinition)
          {
            // Case where property is declared on this ClassDefinition => it is declared below/on the inheritance root
            // In this case, the opposite property's return type must exactly match this ClassDefinition's type.
            if (classDefinition.ClassType != oppositeDomainObjectType)
            {
              return MappingValidationResult.CreateInvalidResult (
                  classDefinition.ClassType,
                  "The type '{0}' does not match the type of the opposite relation propery '{1}' declared on type '{2}'.\r\n\r\n"
                  + "Declaring type: '{3}'",
                  declaringDomainObjectTypeForProperty.Name,
                  relationAttribute.OppositeProperty,
                  oppositePropertyInfo.DeclaringType.Name,
                  classDefinition.ClassType.FullName);
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
            if (!declaringDomainObjectTypeForProperty.IsAssignableFrom (oppositeDomainObjectType))
            {
              return MappingValidationResult.CreateInvalidResult (
                  classDefinition.ClassType,
                  "The type '{0}' cannot be assigned to the type of the opposite relation propery '{1}' declared on type '{2}'.\r\n\r\n"
                  + "Declaring type: '{3}'",
                  declaringDomainObjectTypeForProperty.Name,
                  relationAttribute.OppositeProperty,
                  oppositePropertyInfo.DeclaringType.Name,
                  classDefinition.ClassType.FullName);
            }
          }
        }
      }
      return MappingValidationResult.CreateValidResult ();
    }
  }
}