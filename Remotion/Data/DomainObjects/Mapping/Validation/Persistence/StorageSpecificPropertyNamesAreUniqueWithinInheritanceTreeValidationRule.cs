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
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Persistence
{
  /// <summary>
  /// Validates that each defined persistent property storage specific name is not already defined in a class in the same inheritance hierarchy.
  /// </summary>
  public class StorageSpecificPropertyNamesAreUniqueWithinInheritanceTreeValidationRule : IPersistenceMappingValidationRule
  {
    public StorageSpecificPropertyNamesAreUniqueWithinInheritanceTreeValidationRule ()
    {
    }

    public IEnumerable<MappingValidationResult> Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.BaseClass == null) //if class definition is inheritance root class
      {
        var propertyDefinitionsByName = new Dictionary<string, PropertyDefinition>();

        var derivedPropertyDefinitions = classDefinition.GetAllDerivedClasses().Cast<ClassDefinition>()
            .SelectMany (cd => cd.MyPropertyDefinitions.Cast<PropertyDefinition>());
        var allPropertyDefinitions = classDefinition.MyPropertyDefinitions.Cast<PropertyDefinition> ().Concat (derivedPropertyDefinitions);
        
        foreach (var propertyDefinition in allPropertyDefinitions)
          yield return ValidateStorageSpecificPropertyNames (propertyDefinition, propertyDefinitionsByName);
      }
      else
        yield return MappingValidationResult.CreateValidResult();
    }

    private MappingValidationResult ValidateStorageSpecificPropertyNames (
        PropertyDefinition propertyDefinition, IDictionary<string, PropertyDefinition> propertyDefinitionsByName)
    {
      if (propertyDefinition.StorageClass == StorageClass.Persistent && propertyDefinition.StoragePropertyDefinition != null)
      {
        PropertyDefinition basePropertyDefinition;
        if (propertyDefinitionsByName.TryGetValue (propertyDefinition.StoragePropertyDefinition.Name, out basePropertyDefinition))
        {
          if (!propertyDefinition.PropertyInfo.Equals (basePropertyDefinition.PropertyInfo))
          {
            return MappingValidationResult.CreateInvalidResultForProperty (
                propertyDefinition.PropertyInfo,
                "Property '{0}' of class '{1}' must not define storage specific name '{2}',"
                + " because class '{3}' in same inheritance hierarchy already defines property '{4}' with the same storage specific name.",
                propertyDefinition.PropertyInfo.Name,
                propertyDefinition.ClassDefinition.ClassType.Name,
                propertyDefinition.StoragePropertyDefinition.Name,
                basePropertyDefinition.ClassDefinition.ClassType.Name,
                basePropertyDefinition.PropertyInfo.Name);
          }
        }

        propertyDefinitionsByName[propertyDefinition.StoragePropertyDefinition.Name] = propertyDefinition;
      }

      return MappingValidationResult.CreateValidResult();
    }
  }
}