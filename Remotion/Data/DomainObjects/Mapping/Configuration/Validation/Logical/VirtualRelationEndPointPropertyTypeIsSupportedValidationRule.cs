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
  /// Validates that the virtual property definition is derived from DomainObject, DomainObjectValidation or DomainObjectCollection.
  /// </summary>
  public class VirtualRelationEndPointPropertyTypeIsSupportedValidationRule : IRelationEndPointValidator
  {
    public MappingValidationResult Validate (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      if (relationEndPointDefinition.PropertyType!=null && 
          relationEndPointDefinition.PropertyType != typeof (DomainObjectCollection) && 
          !relationEndPointDefinition.PropertyType.IsSubclassOf (typeof (DomainObjectCollection)) && 
          !relationEndPointDefinition.PropertyType.IsSubclassOf (typeof (DomainObject)))
      {
        var message = string.Format(
            "Relation definition error: Virtual property '{0}' of class '{1}' is of type"
                + "'{2}', but must be derived from 'Remotion.Data.DomainObjects.DomainObject' or "
                    + "'Remotion.Data.DomainObjects.DomainObjectCollection' or must be"
                        + " 'Remotion.Data.DomainObjects.DomainObjectCollection'.",
            relationEndPointDefinition.PropertyName,
            relationEndPointDefinition.ClassDefinition.ID,
            relationEndPointDefinition.PropertyType);
        return new MappingValidationResult (false, message);
      }
      return new MappingValidationResult (true);
    }
  }
}