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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Reflection;
using Remotion.Utilities;
using Rhino.Mocks;
using ReflectionUtility = Remotion.Data.DomainObjects.ReflectionUtility;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  /// <summary>
  /// Provides simple factory methods to manually create <see cref="PropertyDefinition"/> objects for testing. The definition objects will have
  /// dummy (invalid) PropertyInfo properties.
  /// </summary>
  public static class PropertyDefinitionFactory
  {
    public static IStoragePropertyDefinition GetFakeStorageProperty (string name)
    {
      if (name == null)
        return null;
      else
        return new FakeStoragePropertyDefinition (name);
    }

    public static PropertyDefinition Create (ClassDefinition classDefinition, string propertyName, Type propertyType)
    {
      return Create (
          classDefinition,
          propertyName,
          IsObjectID (propertyType),
          IsNullable (propertyType),
          null,
          StorageClass.Persistent,
          classDefinition.ClassType.GetProperty (propertyName),
          GetFakeStorageProperty (propertyName));
    }

    public static PropertyDefinition Create (
        ClassDefinition classDefinition, Type declaringClassType, string propertyName, string columnName, Type propertyType)
    {
      return Create (
          classDefinition, declaringClassType, propertyName, columnName, IsNullable (propertyType), null, StorageClass.Persistent);
    }

    public static PropertyDefinition Create (
        ClassDefinition classDefinition,
        Type declaringClassType,
        string propertyName,
        string columnName,
        bool isNullable)
    {
      return Create (classDefinition, declaringClassType, propertyName, columnName, isNullable, null, StorageClass.Persistent);
    }

    public static PropertyDefinition Create (
        ClassDefinition classDefinition,
        Type declaringClassType,
        string propertyName,
        string columnName,
        bool isNullable,
        int maxLength)
    {
      return Create (classDefinition, declaringClassType, propertyName, columnName, isNullable, maxLength, StorageClass.Persistent);
    }

    public static PropertyDefinition Create (
        ClassDefinition classDefinition,
        Type declaringClassType,
        string propertyName,
        string columnName,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);

      var propertyInfo = declaringClassType.GetProperty (
          propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
      Assert.IsNotNull (propertyInfo);

      Type originalDeclaringType = declaringClassType;
      if (originalDeclaringType.IsGenericType && !originalDeclaringType.IsGenericTypeDefinition)
        originalDeclaringType = originalDeclaringType.GetGenericTypeDefinition();
      var fullPropertyName = originalDeclaringType.FullName + "." + propertyName;

      var propertyDefinition = new PropertyDefinition (
          classDefinition,
          PropertyInfoAdapter.Create(propertyInfo),
          fullPropertyName,
          ReflectionUtility.IsDomainObject (propertyInfo.PropertyType),
          isNullable,
          maxLength,
          storageClass);
      if (storageClass == StorageClass.Persistent)
      {
        propertyDefinition.SetStorageProperty (
            new SimpleColumnDefinition (columnName, propertyDefinition.PropertyType, "dummyType", propertyDefinition.IsNullable, false));
      }
      return propertyDefinition;
    }

    public static PropertyDefinition Create (
        ClassDefinition classDefinition,
        string propertyName,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass,
        IPropertyInformation propertyInfo,
        IStoragePropertyDefinition columnDefinition)
    {
      var propertyDefinition = new PropertyDefinition (
          classDefinition,
          propertyInfo,
          propertyName,
          IsObjectID (propertyInfo.PropertyType),
          isNullable,
          maxLength,
          storageClass);
      propertyDefinition.SetStorageProperty (columnDefinition);
      return propertyDefinition;
    }

    public static PropertyDefinition Create (
        ClassDefinition classDefinition,
        StorageClass storageClass,
        PropertyInfo propertyInfo)
    {
      return Create (
          classDefinition,
          propertyInfo.Name,
          IsObjectID (propertyInfo.PropertyType),
          IsNullable (propertyInfo.PropertyType),
          null,
          storageClass,
          propertyInfo,
          GetFakeStorageProperty (propertyInfo.Name));
    }

    public static PropertyDefinition Create (
        ClassDefinition classDefinition,
        StorageClass storageClass,
        PropertyInfo propertyInfo,
        IStoragePropertyDefinition storagePropertyDefinition)
    {
      return Create (
          classDefinition,
          propertyInfo.Name,
          IsObjectID (propertyInfo.PropertyType),
          IsNullable (propertyInfo.PropertyType),
          null,
          storageClass,
          propertyInfo,
          storagePropertyDefinition);
    }

    public static PropertyDefinition Create (
        ClassDefinition classDefinition,
        StorageClass storageClass,
        PropertyInfo propertyInfo,
        bool isNullable)
    {
      return Create (
          classDefinition,
          propertyInfo.Name,
          IsObjectID (propertyInfo.PropertyType),
          isNullable,
          null,
          storageClass,
          propertyInfo,
          GetFakeStorageProperty (propertyInfo.Name));
    }

    public static PropertyDefinition Create (
        ClassDefinition classDefinition, string propertyName, bool isObjectID, bool isNullable, int? maxLength, StorageClass storageClass, PropertyInfo propertyInfo, IStoragePropertyDefinition columnDefinition)
    {
      return Create (
          classDefinition,
          propertyName,
          isObjectID,
          isNullable,
          maxLength,
          storageClass,
          PropertyInfoAdapter.Create (propertyInfo),
          columnDefinition);
    }

    private static PropertyDefinition Create (
        ClassDefinition classDefinition, string propertyName, bool isObjectID, bool isNullable, int? maxLength, StorageClass storageClass, IPropertyInformation propertyInformation, IStoragePropertyDefinition columnDefinition)
    {
      var propertyDefinition = new PropertyDefinition (
          classDefinition,
          propertyInformation,
          propertyName,
          isObjectID,
          isNullable,
          maxLength,
          storageClass);
      if (columnDefinition != null)
        propertyDefinition.SetStorageProperty (columnDefinition);
      return propertyDefinition;
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition, string propertyName, string columnName)
    {
      return CreateForFakePropertyInfo (
          classDefinition, propertyName, columnName, typeof (string), true, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition, string propertyName, string columnName, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo (
          classDefinition, propertyName, columnName, typeof (string), true, null, storageClass);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo (
          classDefinition, propertyName, columnName, propertyType, IsNullable (propertyType), null, storageClass);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, bool isNullable, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo (
          classDefinition, propertyName, columnName, propertyType, isNullable, null, storageClass);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition,
        string propertyName,
        string columnName,
        Type propertyType,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass)
    {
      return Create (
          classDefinition,
          propertyName,
          IsObjectID (propertyType),
          isNullable,
          maxLength,
          storageClass,
          CreatePropertyInformationStub (propertyName, propertyType, classDefinition.ClassType),
          GetFakeStorageProperty (columnName));
    }

    private static bool IsObjectID (Type propertyType)
    {
      Assertion.IsFalse (propertyType == typeof (ObjectID));
      return ReflectionUtility.IsDomainObject (propertyType);
    }

    private static bool IsNullable (Type propertyType)
    {
      if (propertyType.IsValueType)
        return Nullable.GetUnderlyingType (propertyType) != null;

      if (typeof (DomainObject).IsAssignableFrom (propertyType))
        return true;

      return false;
    }

    private static IPropertyInformation CreatePropertyInformationStub (string propertyName, Type propertyType, Type declaringType)
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation> ();
      propertyInformationStub.Stub (stub => stub.Name).Return (propertyName);
      propertyInformationStub.Stub (stub => stub.PropertyType).Return (propertyType);
      propertyInformationStub.Stub (stub => stub.DeclaringType).Return (TypeAdapter.Create (declaringType));
      propertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType ()).Return (TypeAdapter.Create (declaringType));
      return propertyInformationStub;
    }
  }
}