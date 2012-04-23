// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Reflection;
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
    private readonly LockingCacheDecorator<IPropertyInformation, PropertyAccessorData> _cachedAccessorDataByMember;

    public PropertyAccessorDataCache (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      _classDefinition = classDefinition;
      _cachedAccessorData = new DoubleCheckedLockingContainer<Dictionary<string, PropertyAccessorData>> (BuildAccessorDataDictionary);
      _cachedAccessorDataByMember = CacheFactory.CreateWithLocking<IPropertyInformation, PropertyAccessorData>();
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

    public PropertyAccessorData ResolvePropertyAccessorData<TDomainObject, TResult> (Expression<Func<TDomainObject, TResult>> propertyAccessExpression) 
        where TDomainObject : DomainObject
    {
      ArgumentUtility.CheckNotNull ("propertyAccessExpression", propertyAccessExpression);

      PropertyInfo propertyInfo;
      try
      {
        propertyInfo = MemberInfoFromExpressionUtility.GetProperty (propertyAccessExpression);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException ("The expression must identify a property.", "propertyAccessExpression", ex);
      }

      return ResolvePropertyAccessorData (PropertyInfoAdapter.Create (propertyInfo));
    }

    public PropertyAccessorData ResolvePropertyAccessorData (IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);

      return _cachedAccessorDataByMember.GetOrCreateValue (
          propertyInformation,
          pi => ReflectionBasedPropertyResolver.ResolveDefinition (pi, _classDefinition, GetPropertyAccessorData));
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

    public PropertyAccessorData ResolveMandatoryPropertyAccessorData<TDomainObject, TResult> (Expression<Func<TDomainObject, TResult>> propertyAccessExpression)
        where TDomainObject : DomainObject
    {
      ArgumentUtility.CheckNotNull ("propertyAccessExpression", propertyAccessExpression);

      var data = ResolvePropertyAccessorData (propertyAccessExpression);
      if (data == null)
      {
        var message = string.Format (
            "The domain object type '{0}' does not have a mapping property identified by expression '{1}'.",
            _classDefinition.ClassType,
            propertyAccessExpression);
        throw new MappingException (message);
      }

      return data;
    }

    public PropertyAccessorData FindPropertyAccessorData (Type typeToStartSearch, string shortPropertyName)
    {
      ArgumentUtility.CheckNotNull ("typeToStartSearch", typeToStartSearch);
      ArgumentUtility.CheckNotNullOrEmpty ("shortPropertyName", shortPropertyName);

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