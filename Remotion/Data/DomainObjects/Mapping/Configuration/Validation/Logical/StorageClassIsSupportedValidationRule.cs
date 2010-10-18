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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Logical
{
  /// <summary>
  /// Validates that the given storage class is supported.
  /// </summary>
  public class StorageClassIsSupportedValidationRule : IPropertyDefintionValidator
  {
    public MappingValidationResult Validate (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      return Validate (propertyDefinition.PropertyInfo);
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