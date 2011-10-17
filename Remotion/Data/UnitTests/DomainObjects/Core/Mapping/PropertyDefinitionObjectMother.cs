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
using Remotion.Reflection;
using Rhino.Mocks;
using ReflectionUtility = Remotion.Data.DomainObjects.ReflectionUtility;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  /// <summary>
  /// Provides simple factory methods to manually create <see cref="PropertyDefinition"/> objects for testing.
  /// </summary>
  public static class PropertyDefinitionObjectMother
  {
    public static PropertyDefinition CreateForFakePropertyInfo ()
    {
      return CreateForFakePropertyInfo (ClassDefinitionObjectMother.CreateClassDefinition ());
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition)
    {
      return CreateForFakePropertyInfo (classDefinition, "Test");
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition, string propertyName)
    {
      return CreateForFakePropertyInfo (classDefinition, propertyName, typeof (string), true, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition, string propertyName, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo (
          classDefinition, propertyName, typeof (string), true, null, storageClass);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition, string propertyName, Type propertyType)
    {
      return CreateForFakePropertyInfo (classDefinition, propertyName, false, propertyType, true, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition, string propertyName, Type propertyType, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo (
          classDefinition, propertyName, propertyType, IsNullable (propertyType), null, storageClass);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition, string propertyName, Type propertyType, bool isNullable, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo (
          classDefinition, propertyName, propertyType, isNullable, null, storageClass);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition,
        string propertyName,
        Type propertyType,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass)
    {
      return CreateForFakePropertyInfo (
          classDefinition,
          propertyName,
          IsObjectID (propertyType),
          propertyType,
          isNullable,
          maxLength,
          storageClass);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition,
        string propertyName,
        bool isObjectID,
        Type propertyType,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass)
    {
      return CreateForPropertyInformation (
          classDefinition,
          propertyName,
          isObjectID,
          isNullable,
          maxLength,
          storageClass,
          CreatePropertyInformationStub (propertyName + "FakeProperty", propertyType, classDefinition.ClassType));
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID (ClassDefinition classDefinition)
    {
      return CreateForFakePropertyInfo_ObjectID (classDefinition, "Test");
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID (ClassDefinition classDefinition, string propertyName)
    {
      return CreateForFakePropertyInfo (classDefinition, propertyName, true, typeof (ObjectID), true, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateAndFindPropertyInfo (ClassDefinition classDefinition, Type declaringClassType, string propertyName)
    {
      var propertyInfo = declaringClassType.GetProperty (
          propertyName, 
          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
      Assert.IsNotNull (propertyInfo, "Property '" + propertyName + "' on type '" + declaringClassType + "'.");

      var fullPropertyName = declaringClassType.FullName + "." + propertyName;

      return CreateForPropertyInfo (
          classDefinition,
          fullPropertyName,
          IsObjectID (propertyInfo.PropertyType),
          IsNullable (propertyInfo.PropertyType),
          null,
          StorageClass.Persistent,
          propertyInfo);
    }

    public static PropertyDefinition CreateForPropertyInformation (
        ClassDefinition classDefinition,
        StorageClass storageClass,
        IPropertyInformation propertyInformation)
    {
      return CreateForPropertyInformation (
          classDefinition,
          "Test",
          false,
          false,
          null,
          storageClass,
          propertyInformation);
    }

    public static PropertyDefinition CreateForPropertyInfo (
        ClassDefinition classDefinition,
        StorageClass storageClass,
        PropertyInfo propertyInfo)
    {
      return CreateForPropertyInfo (
          classDefinition,
          propertyInfo.Name,
          IsObjectID (propertyInfo.PropertyType),
          IsNullable (propertyInfo.PropertyType),
          null,
          storageClass,
          propertyInfo);
    }

    public static PropertyDefinition CreateForPropertyInfo (
        ClassDefinition classDefinition,
        StorageClass storageClass,
        PropertyInfo propertyInfo,
        bool isNullable)
    {
      return CreateForPropertyInfo (
          classDefinition,
          propertyInfo.Name,
          IsObjectID (propertyInfo.PropertyType),
          isNullable,
          null,
          storageClass,
          propertyInfo);
    }

    public static PropertyDefinition CreateForPropertyInfo (
        ClassDefinition classDefinition, 
        string propertyName, 
        bool isObjectID, 
        bool isNullable, 
        int? maxLength,
        StorageClass storageClass, 
        PropertyInfo propertyInfo)
    {
      return CreateForPropertyInformation (
          classDefinition,
          propertyName,
          isObjectID,
          isNullable,
          maxLength,
          storageClass,
          PropertyInfoAdapter.Create (propertyInfo));
    }

    private static PropertyDefinition CreateForPropertyInformation (
        ClassDefinition classDefinition, 
        string propertyName,
        bool isObjectID,
        bool isNullable, 
        int? maxLength, 
        StorageClass storageClass,
        IPropertyInformation propertyInformation)
    {
      var propertyDefinition = new PropertyDefinition (
          classDefinition,
          propertyInformation,
          propertyName,
          isObjectID,
          isNullable,
          maxLength,
          storageClass);
      return propertyDefinition;
    }

    public static bool IsObjectID (Type propertyType)
    {
      Assert.IsFalse (propertyType == typeof (ObjectID));
      return ReflectionUtility.IsDomainObject (propertyType);
    }

    public static bool IsNullable (Type propertyType)
    {
      if (propertyType.IsValueType)
        return Nullable.GetUnderlyingType (propertyType) != null;

      if (typeof (DomainObject).IsAssignableFrom (propertyType))
        return true;

      return false;
    }

    private static IPropertyInformation CreatePropertyInformationStub (string propertyName, Type propertyType, Type declaringType)
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub.Stub (stub => stub.Name).Return (propertyName);
      propertyInformationStub.Stub (stub => stub.PropertyType).Return (propertyType);
      propertyInformationStub.Stub (stub => stub.DeclaringType).Return (TypeAdapter.Create (declaringType));
      propertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType()).Return (TypeAdapter.Create (declaringType));
      return propertyInformationStub;
    }
  }
}