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

namespace Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Persistence
{
  /// <summary>
  /// Validates that each defined persistent property storage specific name is not already defined in a class in the same inheritance hierarchy.
  /// </summary>
  public class StorageSpecificPropertyNamesAreUniqueWithinInheritanceTreeValidationRule : IClassDefinitionValidatorRule
  {
    private readonly IDictionary<string, List<PropertyDefinition>> _persistentPropertyDefinitionsInInheritanceHierarchy =
        new Dictionary<string, List<PropertyDefinition>> ();

    public StorageSpecificPropertyNamesAreUniqueWithinInheritanceTreeValidationRule ()
    {
      
    }

    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.BaseClass == null) //if class definition is inheritance root class
        return ValidateStorageSpecificPropertyNames (classDefinition);
      
      return new MappingValidationResult (true);
    }

    private MappingValidationResult ValidateStorageSpecificPropertyNames (ClassDefinition classDefinition)
    {
      var mappingValidationResult = new MappingValidationResult(true);
      foreach (PropertyDefinition myPropertyDefinition in classDefinition.MyPropertyDefinitions)
      {
        if (myPropertyDefinition.StorageClass == StorageClass.Persistent)
        {
          List<PropertyDefinition> basePropertyDefinitions;
          if (_persistentPropertyDefinitionsInInheritanceHierarchy.TryGetValue (myPropertyDefinition.StorageProperty.Name, out basePropertyDefinitions)
              && basePropertyDefinitions != null
              && basePropertyDefinitions.Count > 0)
          {
            var basePropertyDefinition = basePropertyDefinitions[0];

            if (!myPropertyDefinition.StorageProperty.Equals (basePropertyDefinition.StorageProperty))
            {
              var message = string.Format ("Property '{0}' of class '{1}' must not define storage specific name '{2}',"
                  + " because class '{3}' in same inheritance hierarchy already defines property '{4}' with the same storage specific name.",
                  myPropertyDefinition.PropertyName,
                  classDefinition.ID,
                  myPropertyDefinition.StorageProperty.Name,
                  basePropertyDefinition.ClassDefinition.ID,
                  basePropertyDefinition.PropertyName);
              return new MappingValidationResult (false, message);
            }
          }

          _persistentPropertyDefinitionsInInheritanceHierarchy[myPropertyDefinition.StorageProperty.Name] =
              new List<PropertyDefinition> (new[] { myPropertyDefinition });
        }
      }

      foreach (ClassDefinition derivedClassDefinition in classDefinition.DerivedClasses)
      {
        if (!mappingValidationResult.IsValid)
          break;
        mappingValidationResult = ValidateStorageSpecificPropertyNames (derivedClassDefinition);
      }

      return mappingValidationResult;
    }
  }
}