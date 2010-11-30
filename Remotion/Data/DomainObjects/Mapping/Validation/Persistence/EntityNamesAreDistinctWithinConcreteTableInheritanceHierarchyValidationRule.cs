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
  /// Validates that a specified entity name within concrete table inheritance hierarchy classes in different inheritance brachnes is unique.
  /// </summary>
  public class EntityNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule : IPersistenceMappingValidationRule
  {
    public EntityNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule ()
    {
      
    }

    public IEnumerable<MappingValidationResult> Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var allDistinctConcreteEntityNames = new HashSet<string>();
      foreach (string entityName in classDefinition.GetAllConcreteEntityNames ())
      {
        if (allDistinctConcreteEntityNames.Contains(entityName))
        {
          yield return MappingValidationResult.CreateInvalidResultForType (
              classDefinition.ClassType,
              "At least two classes in different inheritance branches derived from abstract class '{0}'"
              + " specify the same entity name '{1}', which is not allowed.",
              classDefinition.ClassType.Name,
              entityName);
        }
        else
        {
          yield return MappingValidationResult.CreateValidResult();
        }

        allDistinctConcreteEntityNames.Add (entityName);
      }
    }
  }
}