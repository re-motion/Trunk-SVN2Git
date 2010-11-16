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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Persistence
{
  /// <summary>
  /// Validates that the entity-name of a class is the same as the inherited entity-name.
  /// </summary>
  public class EntityNameMatchesParentEntityNameValidationRule : IClassDefinitionValidationRule
  {
    public EntityNameMatchesParentEntityNameValidationRule ()
    {
      
    }

    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.BaseClass != null && classDefinition.MyEntityName != null && classDefinition.BaseClass.GetEntityName() != null
          && classDefinition.MyEntityName != classDefinition.BaseClass.GetEntityName())
      {
        string message = string.Format(
            "Class '{0}' must not specify an entity name '{1}' which is different from inherited entity name '{2}'.\r\n\r\nDeclaring type: '{3}",
            classDefinition.ClassType.Name,
            classDefinition.MyEntityName,
            classDefinition.BaseClass.GetEntityName(),
            classDefinition.ClassType.FullName);
        return MappingValidationResult.CreateInvalidResult(message);
      }
      return MappingValidationResult.CreateValidResult();
    }
  }
}