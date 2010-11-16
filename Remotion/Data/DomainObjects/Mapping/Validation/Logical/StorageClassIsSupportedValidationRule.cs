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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Logical
{
  /// <summary>
  /// Validates that the given storage class is supported.
  /// </summary>
  public class StorageClassIsSupportedValidationRule : IPropertyDefinitionValidationRule
  {
    public StorageClassIsSupportedValidationRule ()
    {
    }

    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var errorMessages = new StringBuilder();
      foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
      {
        var validationResult = Validate (propertyDefinition.PropertyInfo);
        if (!validationResult.IsValid)
        {
          if (errorMessages.Length > 0)
            errorMessages.AppendLine (new string ('-', 10));
          errorMessages.AppendLine (validationResult.Message);
        }
      }
      var messages = errorMessages.ToString().Trim();
      return string.IsNullOrEmpty (messages) ? MappingValidationResult.CreateValidResult() : MappingValidationResult.CreateInvalidResult(messages);
    }

    private MappingValidationResult Validate (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      var storageClassAttribute = AttributeUtility.GetCustomAttribute<StorageClassAttribute> (propertyInfo, true);

      if (storageClassAttribute != null && storageClassAttribute.StorageClass != StorageClass.Persistent
          && storageClassAttribute.StorageClass != StorageClass.Transaction)
      {
        return MappingValidationResult.CreateInvalidResult (
          "Only StorageClass.Persistent and StorageClass.Transaction are supported for property '{0}' of class '{1}'.\r\n\r\n"
            +"Declaring type: '{2}'\r\nProperty: '{3}'",
            propertyInfo.Name,
            propertyInfo.DeclaringType.Name,
            propertyInfo.DeclaringType.FullName,
            propertyInfo.Name);
      }
      return MappingValidationResult.CreateValidResult ();
    }
  }
}