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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Holds all <see cref="PropertyAccessorData"/> object for a <see cref="ClassDefinition"/>, providing fast access via full property name or
  /// declaring type and short (.NET) property name.
  /// </summary>
  public class PropertyAccessorDataCache
  {
    private readonly ClassDefinition _classDefinition;
    private readonly DoubleCheckedLockingContainer<Dictionary<string, PropertyAccessorData>> _cachedAccessorData;

    public PropertyAccessorDataCache (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      _classDefinition = classDefinition;
      _cachedAccessorData = new DoubleCheckedLockingContainer<Dictionary<string, PropertyAccessorData>> (BuildAccessorDataDictionary);
    }

    public PropertyAccessorData GetPropertyAccessorData (string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);

      PropertyAccessorData result;
      _cachedAccessorData.Value.TryGetValue (propertyIdentifier, out result);
      return result;
    }

    public PropertyAccessorData GetPropertyAccessorData (Type domainObjectType, string shortPropertyName)
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
      ArgumentUtility.CheckNotNullOrEmpty ("shortPropertyName", shortPropertyName);

      string propertyIdentifier = GetIdentifierFromTypeAndShortName (domainObjectType, shortPropertyName);
      return GetPropertyAccessorData (propertyIdentifier);
    }

    public PropertyAccessorData GetMandatoryPropertyAccessorData (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      var data = GetPropertyAccessorData (propertyName);
      if (data == null)
      {
        var message = string.Format (
            "The domain object type '{0}' does not have a mapping property named '{1}'.",
            _classDefinition.ClassType,
            propertyName);
        throw new MappingException (message);
      }
      return data;
    }

    public PropertyAccessorData GetMandatoryPropertyAccessorData (Type domainObjectType, string shortPropertyName)
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
      ArgumentUtility.CheckNotNullOrEmpty ("shortPropertyName", shortPropertyName);

      var propertyName = GetIdentifierFromTypeAndShortName (domainObjectType, shortPropertyName);
      return GetMandatoryPropertyAccessorData (propertyName);
    }

    public PropertyAccessorData FindPropertyAccessorData (Type typeToStartSearch, string shortPropertyName)
    {
      Type currentType = typeToStartSearch;
      PropertyAccessorData propertyAccessorData = null;
      while (currentType != null && (propertyAccessorData = GetPropertyAccessorData (currentType, shortPropertyName)) == null)
      {
        if (currentType.IsGenericType && !currentType.IsGenericTypeDefinition)
          currentType = currentType.GetGenericTypeDefinition ();
        else
          currentType = currentType.BaseType;
      }
      return propertyAccessorData;
    }

    private string GetIdentifierFromTypeAndShortName (Type domainObjectType, string shortPropertyName)
    {
      return domainObjectType.FullName + "." + shortPropertyName;
    }

    private Dictionary<string, PropertyAccessorData> BuildAccessorDataDictionary ()
    {
      var propertyDefinitions = _classDefinition.GetPropertyDefinitions ();
      var relationEndPointDefinitions = _classDefinition.GetRelationEndPointDefinitions ();

      var propertyDefinitionNames = from PropertyDefinition pd in propertyDefinitions
                                    select pd.PropertyName;
      var virtualRelationEndPointNames =
          from IRelationEndPointDefinition repd in relationEndPointDefinitions
          where repd.IsVirtual
          select repd.PropertyName;

      var allPropertyNames = propertyDefinitionNames.Concat (virtualRelationEndPointNames);
      var allPropertyAccessorData = allPropertyNames.Select (name => new PropertyAccessorData (_classDefinition, name));
      return allPropertyAccessorData.ToDictionary (data => data.PropertyIdentifier);
    }
  }
}