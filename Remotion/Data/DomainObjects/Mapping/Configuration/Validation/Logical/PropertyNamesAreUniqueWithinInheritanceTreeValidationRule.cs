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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Logical
{
  /// <summary>
  /// Validates that each defined property definition in a class is not already defined in a base class.
  /// </summary>
  public class PropertyNamesAreUniqueWithinInheritanceTreeValidationRule : IClassDefinitionValidator
  {
    public PropertyNamesAreUniqueWithinInheritanceTreeValidationRule ()
    {
      
    }

    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.BaseClass != null)
      {
        PropertyDefinitionCollection basePropertyDefinitions = classDefinition.BaseClass.GetPropertyDefinitions ();
        foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
        {
          if (basePropertyDefinitions.Contains (propertyDefinition.PropertyName))
          {
            string message = string.Format("Class '{0}' must not define property '{1}', because base class '{2}' already defines a property with the same name.",
                classDefinition.ID,
                propertyDefinition.PropertyName,
                basePropertyDefinitions[propertyDefinition.PropertyName].ClassDefinition.ID);
            return new MappingValidationResult (false, message);
          }
        }
      }
      return new MappingValidationResult (true);
    }
  }
}