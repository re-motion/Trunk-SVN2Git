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
using System.Collections.Generic;
using Remotion.Utilities;

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

    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.BaseClass == null) //if class definition is inheritance root class
        return ValidateStorageSpecificPropertyNames (classDefinition, new Dictionary<string, PropertyDefinition>());
      
      return MappingValidationResult.CreateValidResult();
    }

    private MappingValidationResult ValidateStorageSpecificPropertyNames (
        ClassDefinition classDefinition, IDictionary<string, PropertyDefinition> propertyDefinitionsByName)
    {
      var mappingValidationResult = MappingValidationResult.CreateValidResult();
      foreach (PropertyDefinition myPropertyDefinition in classDefinition.MyPropertyDefinitions)
      {
        if (myPropertyDefinition.StorageClass == StorageClass.Persistent)
        {
          PropertyDefinition basePropertyDefinition;
          if (propertyDefinitionsByName.TryGetValue (myPropertyDefinition.StoragePropertyDefinition.Name, out basePropertyDefinition))
          {
            if (!myPropertyDefinition.PropertyInfo.Equals (basePropertyDefinition.PropertyInfo))
            {
              return MappingValidationResult.CreateInvalidResultForProperty (
                  myPropertyDefinition.PropertyInfo,
                  "Property '{0}' of class '{1}' must not define storage specific name '{2}',"
                  + " because class '{3}' in same inheritance hierarchy already defines property '{4}' with the same storage specific name.",
                  myPropertyDefinition.PropertyInfo.Name,
                  classDefinition.ClassType.Name,
                  myPropertyDefinition.StoragePropertyDefinition.Name,
                  basePropertyDefinition.ClassDefinition.ClassType.Name,
                  basePropertyDefinition.PropertyInfo.Name);
            }
          }

          propertyDefinitionsByName[myPropertyDefinition.StoragePropertyDefinition.Name] =  myPropertyDefinition;
        }
      }

      foreach (ClassDefinition derivedClassDefinition in classDefinition.DerivedClasses)
      {
        if (!mappingValidationResult.IsValid)
          break;
        mappingValidationResult = ValidateStorageSpecificPropertyNames (derivedClassDefinition, propertyDefinitionsByName);
      }

      return mappingValidationResult;
    }
  }
}