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
using System.Reflection;
using System.Text;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Logical
{
  /// <summary>
  /// Validates that a property type is supported.
  /// </summary>
  public class PropertyTypeIsSupportedValidationRule : IClassDefinitionValidatorRule
  {
    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var errorMessages = new StringBuilder();
      foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
      {
        var validationResult = Validate (propertyDefinition.PropertyInfo, classDefinition);
        if (!validationResult.IsValid)
          errorMessages.AppendLine (validationResult.Message);
      }

      var messages = errorMessages.ToString().Trim();
      return string.IsNullOrEmpty (messages) ? new MappingValidationResult (true) : new MappingValidationResult (false, messages);
    }

    private MappingValidationResult Validate (PropertyInfo propertyInfo, ClassDefinition classDefinition)
    {
      var nativePropertyType = ReflectionUtility.IsDomainObject (propertyInfo.PropertyType) ? typeof (ObjectID) : propertyInfo.PropertyType;
      if (!PropertyValue.IsTypeSupported (nativePropertyType))
      {
        var message = string.Format (
            "The property type {0} is not supported.\r\n\r\nDeclaring type: {1}\r\nProperty: {2}",
            nativePropertyType,
            classDefinition.ClassType.FullName,
            propertyInfo.Name);
        return new MappingValidationResult (false, message);
      }

      return new MappingValidationResult (true);
    }
  }
}