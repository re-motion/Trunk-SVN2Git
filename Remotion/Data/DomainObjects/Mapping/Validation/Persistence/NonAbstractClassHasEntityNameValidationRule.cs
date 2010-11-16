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
  /// Validates that each non-abstract class in the mapping can resolve it's entity-name.
  /// </summary>
  public class NonAbstractClassHasEntityNameValidationRule : IPersistenceMappingValidationRule
  {
    public NonAbstractClassHasEntityNameValidationRule ()
    {
      
    }

    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.IsClassTypeResolved)
      {
        if (classDefinition.GetEntityName () == null && !classDefinition.IsAbstract)
        {
          string message = string.Format("Neither class '{0}' nor its base classes specify an entity name. "
            +"Make class '{0}' abstract or apply a 'DBTable' attribute to it or one of its base classes.\r\n\r\n"
            +"Declaring type: '{1}'",
              classDefinition.ClassType.Name,
              classDefinition.ClassType.FullName);
          return MappingValidationResult.CreateInvalidResult(message);
        }
      }
      return MappingValidationResult.CreateValidResult();
    }
  }
}