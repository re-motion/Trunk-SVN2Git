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

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  /// <summary>
  /// Provides simple factory methods to manually create <see cref="PropertyDefinition"/> objects for testing. The definition objects will have
  /// dummy (invalid) PropertyInfo properties.
  /// </summary>
  public static class PropertyDefinitionFactory
  {
    public static string FakeProperty
    {
      get { return null; }
    }

    public static PropertyInfo GetFakePropertyInfo ()
    {
      return typeof (PropertyDefinitionFactory).GetProperty ("FakeProperty", BindingFlags.Static | BindingFlags.Public);
    }

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
          propertyType,
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
          classDefinition, declaringClassType, propertyName, columnName, propertyType, IsNullable (propertyType), null, StorageClass.Persistent);
    }

    public static PropertyDefinition Create (
        ClassDefinition classDefinition,
        Type declaringClassType,
        string propertyName,
        string columnName,
        Type propertyType,
        bool isNullable)
    {
      return Create (classDefinition, declaringClassType, propertyName, columnName, propertyType, isNullable, null, StorageClass.Persistent);
    }

    public static PropertyDefinition Create (
        ClassDefinition classDefinition,
        Type declaringClassType,
        string propertyName,
        string columnName,
        Type propertyType,
        bool isNullable,
        int maxLength)
    {
      return Create (classDefinition, declaringClassType, propertyName, columnName, propertyType, isNullable, maxLength, StorageClass.Persistent);
    }

    public static PropertyDefinition Create (
        ClassDefinition classDefinition,
        Type declaringClassType,
        string propertyName,
        string columnName,
        Type propertyType,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

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
          propertyType,
          IsObjectID (propertyType),
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
        Type propertyType,
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
          propertyType,
          IsObjectID (propertyType),
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
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      return Create (
          classDefinition,
          propertyInfo.Name,
          propertyInfo.PropertyType,
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
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      return Create (
          classDefinition,
          propertyInfo.Name,
          propertyInfo.PropertyType,
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
        bool isNullable
        )
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      return Create (
          classDefinition,
          propertyInfo.Name,
          propertyInfo.PropertyType,
          IsObjectID(propertyInfo.PropertyType),
          isNullable,
          null,
          storageClass,
          propertyInfo,
          GetFakeStorageProperty (propertyInfo.Name));
    }

    public static PropertyDefinition Create (ClassDefinition classDefinition, string propertyName, Type propertyType, bool isObjectID, bool isNullable, int? maxLength, StorageClass storageClass, PropertyInfo propertyInfo, IStoragePropertyDefinition columnDefinition)
    {
      var propertyDefinition = new PropertyDefinition (
          classDefinition,
          PropertyInfoAdapter.Create(propertyInfo),
          propertyName,
          propertyType,
          isObjectID,
          isNullable,
          maxLength,
          storageClass);
      if (columnDefinition != null)
        propertyDefinition.SetStorageProperty (columnDefinition);
      return propertyDefinition;
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition, string propertyName, string columnName, StorageClass storageClass)
    {
      var fakePropertyInfo = GetFakePropertyInfo();
      return Create (
          classDefinition,
          propertyName,
          typeof (string), 
          false,
          IsNullable (fakePropertyInfo.PropertyType),
          null,
          storageClass,
          fakePropertyInfo,
          GetFakeStorageProperty (columnName));
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, StorageClass storageClass)
    {
      var fakePropertyInfo = GetFakePropertyInfo();
      return Create (
          classDefinition,
          propertyName,
          propertyType,
          IsObjectID (propertyType),
          IsNullable (fakePropertyInfo.PropertyType),
          null,
          storageClass,
          fakePropertyInfo,
          GetFakeStorageProperty (columnName));
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition,
        string propertyName,
        string columnName,
        Type propertyType,
        bool isNullable,
        StorageClass storageClass)
    {
      return Create (
          classDefinition,
          propertyName,
          propertyType,
          IsObjectID (propertyType),
          isNullable,
          null,
          storageClass,
          GetFakePropertyInfo(),
          GetFakeStorageProperty (columnName));
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
          propertyType,
          IsObjectID (propertyType),
          isNullable,
          maxLength,
          storageClass,
          GetFakePropertyInfo(),
          GetFakeStorageProperty (columnName));
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition, string propertyName, string columnName)
    {
      var fakePropertyInfo = GetFakePropertyInfo();
      return Create (
          classDefinition,
          propertyName,
          typeof (string), 
          false,
          IsNullable(fakePropertyInfo.PropertyType),
          null,
          StorageClass.Persistent,
          fakePropertyInfo,
          GetFakeStorageProperty (columnName));
    }

    private static bool IsObjectID (Type propertyType)
    {
      return propertyType == typeof (ObjectID);
    }

    private static bool IsNullable (Type propertyType)
    {
      if (propertyType.IsValueType)
        return Nullable.GetUnderlyingType (propertyType) != null;

      if (typeof (DomainObject).IsAssignableFrom (propertyType))
        return true;

      return false;
    }
  }
}