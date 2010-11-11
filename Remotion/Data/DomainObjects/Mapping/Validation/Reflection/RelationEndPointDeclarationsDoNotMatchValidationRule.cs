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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Reflection
{
  /// <summary>
  /// Validates that the end point property declaration matches the opposite end point property declaration.
  /// </summary>
  public class RelationEndPointDeclarationsDoNotMatchValidationRule : IRelationDefinitionValidatorRule
  {
    public MappingValidationResult Validate (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      foreach (var endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        var validationResult = Validate (endPointDefinition);
        if (!validationResult.IsValid)
          return MappingValidationResult.CreateInvalidResult(validationResult.Message);
      }

      return MappingValidationResult.CreateValidResult();
    }

    private MappingValidationResult Validate (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      if (relationEndPointDefinition.PropertyInfo!=null)
      {
        var relationAttribute =
            (BidirectionalRelationAttribute)
            AttributeUtility.GetCustomAttribute (relationEndPointDefinition.PropertyInfo, typeof (BidirectionalRelationAttribute), true);
        if (relationAttribute != null)
        {
          var oppositePropertyDomainObjectType = ReflectionUtility.GetDomainObjectTypeFromProperty (relationEndPointDefinition.PropertyInfo);
          var oppositeProperty = GetOppositePropertyInfo (oppositePropertyDomainObjectType, relationAttribute.OppositeProperty);
          if (oppositeProperty != null)
          {
            var oppositeRelationAttribute =
              (BidirectionalRelationAttribute) AttributeUtility.GetCustomAttribute (oppositeProperty, typeof (BidirectionalRelationAttribute), true);

            if (oppositeRelationAttribute == null)
            {
              var message = string.Format (
                  "Opposite relation property '{0}' declared on type '{1}' does not define a matching '{2}'.\r\n\r\n"
                  + "Declaration type: '{3}'",
                  relationAttribute.OppositeProperty,
                  oppositeProperty.DeclaringType.Name,
                  relationAttribute.GetType ().Name,
                  relationEndPointDefinition.ClassDefinition.ClassType.FullName);
              return MappingValidationResult.CreateInvalidResult(message);
            }

            if (!relationEndPointDefinition.PropertyInfo.Name.Equals (oppositeRelationAttribute.OppositeProperty, StringComparison.Ordinal))
            {
              var message =
                  string.Format (
                      "Opposite relation property '{0}' declared on type '{1}' defines a '{2}' whose opposite property does not match.\r\n\r\n"
                      + "Declaration type: '{3}'",
                      relationAttribute.OppositeProperty,
                      oppositeProperty.DeclaringType.Name,
                      relationAttribute.GetType ().Name,
                      relationEndPointDefinition.ClassDefinition.ClassType.FullName);
              return MappingValidationResult.CreateInvalidResult(message);
            }
          }
        }
      }

      return MappingValidationResult.CreateValidResult();
    }

    private PropertyInfo GetOppositePropertyInfo (Type type, string oppositeProperty)
    {
      return type.GetProperty (oppositeProperty, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
  }
}