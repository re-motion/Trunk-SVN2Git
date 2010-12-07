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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;
using System.Linq;

// TODO Review 3545: Move all persistence rules to Persistence\Rdbms\Model\Validation namespace
namespace Remotion.Data.DomainObjects.Mapping.Validation.Persistence
{
  // TODO Review 3545: Rename to TableNamesAreDistinct...
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

      if (classDefinition.BaseClass == null) //if class definition is inheritance root class
      {
        var allDistinctTableNames = new HashSet<string> ();
        foreach (var tableName in FindAllTableDefinitions (classDefinition).Select (td => td.TableName))
        {
          if (allDistinctTableNames.Contains (tableName))
          {
            yield return MappingValidationResult.CreateInvalidResultForType (
                classDefinition.ClassType,
                "At least two classes in different inheritance branches derived from abstract class '{0}'"
                + " specify the same entity name '{1}', which is not allowed.",
                classDefinition.ClassType.Name,
                tableName);
          }
          else
          {
            yield return MappingValidationResult.CreateValidResult ();
          }

          allDistinctTableNames.Add (tableName);
        }
      }
    }

    private IEnumerable<TableDefinition> FindAllTableDefinitions (ClassDefinition classDefinition)
    {
      var tableDefinition = classDefinition.StorageEntityDefinition as TableDefinition;
      if (tableDefinition != null)
        yield return tableDefinition;

      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
      {
        var tableDefinitionsInDerivedClass = FindAllTableDefinitions (derivedClass);
        foreach (var tableDefinitionInDerivedClass in tableDefinitionsInDerivedClass)
          yield return tableDefinitionInDerivedClass;
      }
    }
  }
}