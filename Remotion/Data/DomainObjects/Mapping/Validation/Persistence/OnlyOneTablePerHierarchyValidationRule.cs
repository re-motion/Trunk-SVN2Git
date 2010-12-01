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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Persistence
{
  /// <summary>
  /// Validates that the entity-name of a class is the same as the inherited entity-name.
  /// </summary>
  public class OnlyOneTablePerHierarchyValidationRule : IPersistenceMappingValidationRule
  {
    public OnlyOneTablePerHierarchyValidationRule ()
    {
    }

    public IEnumerable<MappingValidationResult> Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.BaseClass != null && classDefinition.StorageEntityDefinition is TableDefinition)
      {
        var baseClasses = classDefinition.BaseClass.CreateSequence (cd => cd.BaseClass);
        foreach (ClassDefinition baseClass in baseClasses)
        {
          if (baseClass.StorageEntityDefinition is TableDefinition)
          {
            yield return MappingValidationResult.CreateInvalidResultForType (
                classDefinition.ClassType,
                "Class '{0}' must not define a table when its base class '{1}' also defines one.",
                classDefinition.ClassType.Name,
                baseClass.ClassType.Name);
          }
        }
      }
      else
      {
        yield return MappingValidationResult.CreateValidResult();
      }
     
    }
  }
}