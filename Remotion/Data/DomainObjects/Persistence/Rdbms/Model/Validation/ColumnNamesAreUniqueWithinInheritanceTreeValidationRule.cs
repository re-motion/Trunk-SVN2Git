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
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation
{
  /// <summary>
  /// Validates that each defined persistent property storage specific name is not already defined in a class in the same inheritance hierarchy.
  /// </summary>
  public class ColumnNamesAreUniqueWithinInheritanceTreeValidationRule : IPersistenceMappingValidationRule
  {
    private class ColumnDefinitionVisitor : IColumnDefinitionVisitor
    {
      public static IEnumerable<KeyValuePair<string, List<PropertyDefinition>>> GroupByName (IEnumerable<PropertyDefinition> propertyDefinitions)
      {
        var visitor = new ColumnDefinitionVisitor();
        foreach (var propertyDefinition in propertyDefinitions)
        {
          visitor._currentProperty = propertyDefinition;
          ((IColumnDefinition) propertyDefinition.StoragePropertyDefinition).Accept (visitor);
        }

        return visitor._propertyDefinitionsByName;
      }


      private readonly MultiDictionary<string, PropertyDefinition> _propertyDefinitionsByName;
      private PropertyDefinition _currentProperty;

      private ColumnDefinitionVisitor ()
      {
        _propertyDefinitionsByName = new MultiDictionary<string, PropertyDefinition>();
      }

      void IColumnDefinitionVisitor.VisitSimpleColumnDefinition (SimpleColumnDefinition simpleColumnDefinition)
      {
        _propertyDefinitionsByName[simpleColumnDefinition.Name].Add (_currentProperty);
      }

      void IColumnDefinitionVisitor.VisitIDColumnDefinition (IDColumnDefinition idColumnDefinition)
      {
        idColumnDefinition.ObjectIDColumn.Accept (this);
        if (idColumnDefinition.HasClassIDColumn)
          idColumnDefinition.ClassIDColumn.Accept (this);
      }

      void IColumnDefinitionVisitor.VisitNullColumnDefinition (NullColumnDefinition nullColumnDefinition)
      {
      }
    }

    public ColumnNamesAreUniqueWithinInheritanceTreeValidationRule ()
    {
    }

    public IEnumerable<MappingValidationResult> Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.BaseClass == null) //if class definition is inheritance root class
      {
        var derivedPropertyDefinitions = classDefinition.GetAllDerivedClasses().Cast<ClassDefinition>()
            .SelectMany (cd => cd.MyPropertyDefinitions);
        var allPropertyDefinitions =
            classDefinition.MyPropertyDefinitions.Concat(derivedPropertyDefinitions).Where (pd => pd.StorageClass == StorageClass.Persistent);

        var groupedPropertyDefinitionsByName = ColumnDefinitionVisitor.GroupByName (allPropertyDefinitions);
        foreach (var keyVakuePair in groupedPropertyDefinitionsByName)
        {
          if (keyVakuePair.Value.Count > 1 && !keyVakuePair.Value[0].PropertyInfo.Equals (keyVakuePair.Value[1].PropertyInfo))
          {
            yield return MappingValidationResult.CreateInvalidResultForProperty (
                keyVakuePair.Value[0].PropertyInfo,
                "Property '{0}' of class '{1}' must not define storage specific name '{2}',"
                + " because class '{3}' in same inheritance hierarchy already defines property '{4}' with the same storage specific name.",
                keyVakuePair.Value[0].PropertyInfo.Name,
                keyVakuePair.Value[0].ClassDefinition.ClassType.Name,
                keyVakuePair.Value[0].StoragePropertyDefinition.Name,
                keyVakuePair.Value[1].ClassDefinition.ClassType.Name,
                keyVakuePair.Value[1].PropertyInfo.Name);
          }
        }
      }
      yield return MappingValidationResult.CreateValidResult();
    }
  }
}