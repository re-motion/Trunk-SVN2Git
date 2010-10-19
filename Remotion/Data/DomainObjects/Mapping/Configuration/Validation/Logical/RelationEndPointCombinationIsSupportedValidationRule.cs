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
  /// Validates that a relation has valid relation endpoint combinations.
  /// </summary>
  public class RelationEndPointCombinationIsSupportedValidationRule : IRelationDefinitionValidator
  {
    public RelationEndPointCombinationIsSupportedValidationRule ()
    {
      
    }

    public MappingValidationResult Validate (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      var endPointDefinition1 = relationDefinition.EndPointDefinitions[0];
      var endPointDefinition2 = relationDefinition.EndPointDefinitions[1];

      if (endPointDefinition1.IsAnonymous && endPointDefinition2.IsAnonymous)
      {
        var message = string.Format("Relation '{0}' cannot have two anonymous end points.", relationDefinition.ID);
        return new MappingValidationResult (false, message);
      }

      if (endPointDefinition1.IsVirtual && endPointDefinition2.IsVirtual)
      {
        var message = string.Format ("Relation '{0}' cannot have two virtual end points.", relationDefinition.ID);
        return new MappingValidationResult (false, message);
      }

      if (!endPointDefinition1.IsVirtual && !endPointDefinition2.IsVirtual)
      {
        var message = string.Format("Relation '{0}' cannot have two non-virtual end points.", relationDefinition.ID);
        return new MappingValidationResult (false, message);
      }

      return new MappingValidationResult (true);
    }
  }
}