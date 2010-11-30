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
using System.Reflection;
using System.Text;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Persistence
{
  /// <summary>
  /// Validates that a persistent property type is supported by the storage provider.
  /// </summary>
  public class PropertyTypeIsSupportedByStorageProviderValidationRule : IPersistenceMappingValidationRule
  {
    public IEnumerable<MappingValidationResult> Validate (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return from PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions
             select Validate (propertyDefinition.PropertyInfo, classDefinition);
    }

    private MappingValidationResult Validate (PropertyInfo propertyInfo, ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      var storageAttribute = AttributeUtility.GetCustomAttribute<StorageClassAttribute> (propertyInfo, true);
      if (storageAttribute != null && storageAttribute.StorageClass == StorageClass.Persistent)
      {
        var nativePropertyType = ReflectionUtility.IsDomainObject (propertyInfo.PropertyType) ? typeof (ObjectID) : propertyInfo.PropertyType;

        if (!ReflectionUtility.IsTypeSupportedByStorageProvider (nativePropertyType, classDefinition.StorageEntityDefinition.StorageProviderDefinition.Name))
        {
          return MappingValidationResult.CreateInvalidResultForProperty (
              propertyInfo,
              "The property type '{0}' is not supported by this storage provider.",
              nativePropertyType.Name);
        }
      }
      return MappingValidationResult.CreateValidResult();
    }
  }
}