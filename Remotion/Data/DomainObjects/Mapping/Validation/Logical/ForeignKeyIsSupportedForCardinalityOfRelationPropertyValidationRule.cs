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
using System.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Logical
{
  public class ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule : IRelationDefinitionValidatorRule
  {
    public MappingValidationResult Validate (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      var errorMessages = new StringBuilder (2);
      foreach (var endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        var validationResult = Validate (endPointDefinition);
        if (!validationResult.IsValid)
          errorMessages.AppendLine (validationResult.Message);
      }

      var messages = errorMessages.ToString ().Trim ();
      if (string.IsNullOrEmpty (messages))
        return new MappingValidationResult (true);
      else
        return new MappingValidationResult (false, messages);
    }

    private MappingValidationResult Validate (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      var relationAttribute = (DBBidirectionalRelationAttribute)
          AttributeUtility.GetCustomAttribute (relationEndPointDefinition.PropertyInfo, typeof (DBBidirectionalRelationAttribute), true);
      if (relationAttribute != null && relationAttribute.ContainsForeignKey && relationEndPointDefinition.IsVirtual)
      {
        var message = string.Format ("Only relation end points with a property type of '{0}' can contain the foreign key.", typeof (DomainObject));
        return new MappingValidationResult (false, message);
      }
      return new MappingValidationResult (true);
    }
  }
}