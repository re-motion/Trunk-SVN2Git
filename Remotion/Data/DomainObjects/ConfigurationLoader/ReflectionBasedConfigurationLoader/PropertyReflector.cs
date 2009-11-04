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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="PropertyDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  //TODO: Validation: check that only non-virtual relation endpoints are returned as propertydefinition.
  //TODO: Test for null or empty StorageSpecificIdentifier
  public class PropertyReflector: MemberReflectorBase
  {
    private readonly ReflectionBasedClassDefinition _classDefinition;

    public PropertyReflector (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, IMappingNameResolver nameResolver)
        : base (propertyInfo, nameResolver)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      _classDefinition = classDefinition;
    }

    public ReflectionBasedPropertyDefinition GetMetadata ()
    {
      ValidatePropertyInfo();
      CheckValidPropertyType();

      return new ReflectionBasedPropertyDefinition (
          _classDefinition, PropertyInfo, GetPropertyName(),
          GetStorageSpecificIdentifier(),
          IsRelationProperty() ? typeof (ObjectID) : PropertyInfo.PropertyType,
          IsNullable(),
          GetMaxLength(),
          StorageClass);
    }

    private void CheckValidPropertyType()
    {
      if (StorageClass == StorageClass.Persistent)
      {
        Type nativePropertyType = GetNativePropertyType();

        if (!IsTypeSupportedByStorageProvider (nativePropertyType))
          throw CreateMappingException (null, PropertyInfo, "The property type {0} is not supported.", nativePropertyType);
      }
    }

    private Type GetNativePropertyType()
    {
      if (IsRelationProperty())
        return typeof (ObjectID);

      return Nullable.GetUnderlyingType (PropertyInfo.PropertyType) ?? PropertyInfo.PropertyType;
    }

    private bool IsTypeSupportedByStorageProvider (Type type)
    {
      StorageProviderDefinition storageProviderDefinition =
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions.GetMandatory (_classDefinition.StorageProviderID);
      return storageProviderDefinition.TypeProvider.IsTypeSupported (type);
    }

    //TODO: Move adding of "ID" to RdbmsPropertyReflector
    private string GetStorageSpecificIdentifier()
    {
      IStorageSpecificIdentifierAttribute attribute = AttributeUtility.GetCustomAttribute<IStorageSpecificIdentifierAttribute> (PropertyInfo, true);
      if (attribute != null)
        return attribute.Identifier;
      if (IsRelationProperty())
        return PropertyInfo.Name + "ID";
      return PropertyInfo.Name;
    }

    private bool? IsNullable()
    {
      if (PropertyInfo.PropertyType.IsValueType)
        return null;

      if (typeof (DomainObject).IsAssignableFrom (PropertyInfo.PropertyType))
        return true;

      return IsNullableFromAttribute();
    }

    private int? GetMaxLength()
    {
      ILengthConstrainedPropertyAttribute attribute = AttributeUtility.GetCustomAttribute<ILengthConstrainedPropertyAttribute> (PropertyInfo, true);
      if (attribute != null)
        return attribute.MaximumLength;
      return null;
    }

    private bool IsRelationProperty ()
    {
      return (typeof (DomainObject).IsAssignableFrom (PropertyInfo.PropertyType));
    }
  }
}
