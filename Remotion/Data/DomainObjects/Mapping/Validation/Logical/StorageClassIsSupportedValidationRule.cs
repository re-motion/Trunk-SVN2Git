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
using System.Reflection;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Logical
{
  /// <summary>
  /// Validates that the given storage class is supported.
  /// </summary>
  public class StorageClassIsSupportedValidationRule : IClassDefinitionValidatorRule
  {
    public StorageClassIsSupportedValidationRule ()
    {
      
    }

    public MappingValidationResult Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      foreach (PropertyDefinition propertyDefinition in classDefinition.GetPropertyDefinitions())
      {
        var validationResult = Validate (propertyDefinition.PropertyInfo);
        if (!validationResult.IsValid)
          return validationResult;
      }
      return new MappingValidationResult (true);
    }

    public MappingValidationResult Validate (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      var storageClassAttribute = AttributeUtility.GetCustomAttribute<StorageClassAttribute> (propertyInfo, true);

      if (storageClassAttribute != null && storageClassAttribute.StorageClass != StorageClass.Persistent
          && storageClassAttribute.StorageClass != StorageClass.Transaction)
      {
        var message = "Only StorageClass.Persistent and StorageClass.Transaction is supported.";
        return new MappingValidationResult (false, message);
      }
      return new MappingValidationResult (true);
    }

    
  }
}