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
using System.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Logical
{
  /// <summary>
  /// Validates that the virtual relation end point cardinality matches the property type.
  /// </summary>
  public class VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule : IRelationDefinitionValidatorRule
  {
    public VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule ()
    {
    }

    public MappingValidationResult Validate (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      var errorMessages = new StringBuilder();
      foreach (var endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        var validationResult = Validate (endPointDefinition);
        if (!validationResult.IsValid)
        {
          if (!validationResult.IsValid)
            errorMessages.AppendLine (validationResult.Message);
        }
      }

      var messages = errorMessages.ToString().Trim();
      return string.IsNullOrEmpty (messages) ? new MappingValidationResult (true) : new MappingValidationResult (false, messages);
    }

    private MappingValidationResult Validate (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      var endPointDefinitionAsVirtualRelationEndPointDefinition = relationEndPointDefinition as VirtualRelationEndPointDefinition;
      if (endPointDefinitionAsVirtualRelationEndPointDefinition != null)
      {
        if (endPointDefinitionAsVirtualRelationEndPointDefinition.Cardinality == CardinalityType.One &&
            !endPointDefinitionAsVirtualRelationEndPointDefinition.PropertyType.IsSubclassOf (typeof (DomainObject)))
        {
          var message = "The property type of a virtual end point of a one-to-one relation"
                        + " must be derived from 'Remotion.Data.DomainObjects.DomainObject'.\r\n\r\n"
                        + string.Format (
                            "Declaring type: '{0}'\r\nProperty: '{1}'",
                            endPointDefinitionAsVirtualRelationEndPointDefinition.ClassDefinition.ClassType.FullName,
                            endPointDefinitionAsVirtualRelationEndPointDefinition.PropertyInfo.Name);
          return new MappingValidationResult (false, message);
        }

        if (endPointDefinitionAsVirtualRelationEndPointDefinition.Cardinality == CardinalityType.Many &&
            endPointDefinitionAsVirtualRelationEndPointDefinition.PropertyType != typeof (DomainObjectCollection) &&
            ! endPointDefinitionAsVirtualRelationEndPointDefinition.PropertyType.IsSubclassOf (typeof (DomainObjectCollection)))
        {
          var message = "The property type of a virtual end point of a one-to-many relation"
                        + " must be or be derived from 'Remotion.Data.DomainObjects.DomainObjectCollection'.\r\n\r\n"
                        + string.Format (
                            "Declaring type: '{0}'\r\nProperty: '{1}'",
                            endPointDefinitionAsVirtualRelationEndPointDefinition.ClassDefinition.ClassType.FullName,
                            endPointDefinitionAsVirtualRelationEndPointDefinition.PropertyInfo.Name);
          return new MappingValidationResult (false, message);
        }
      }
      return new MappingValidationResult (true);
    }
  }
}