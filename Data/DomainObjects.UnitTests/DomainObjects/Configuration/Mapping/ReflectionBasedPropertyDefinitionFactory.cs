/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping
{
  /// <summary>
  /// Provides simple factory methods to manually create ReflectionBasedPropertyDefinition objects for testing. The definition objects will have
  /// dummy (invalid) PropertyInfo properties.
  /// </summary>
  public static class ReflectionBasedPropertyDefinitionFactory
  {
    public static ReflectionBasedPropertyDefinition CreateReflectionBasedPropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType)
    {
      return CreateReflectionBasedPropertyDefinition (classDefinition, propertyName, columnName, propertyType, null, null, StorageClass.Persistent);
    }

    public static ReflectionBasedPropertyDefinition CreateReflectionBasedPropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, bool isNullable)
    {
      return CreateReflectionBasedPropertyDefinition (classDefinition, propertyName, columnName, propertyType, isNullable, null, StorageClass.Persistent);
    }

    public static ReflectionBasedPropertyDefinition CreateReflectionBasedPropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, int maxLength)
    {
      return CreateReflectionBasedPropertyDefinition (classDefinition, propertyName, columnName, propertyType, null, maxLength, StorageClass.Persistent);
    }

    public static ReflectionBasedPropertyDefinition CreateReflectionBasedPropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, bool isNullable, int maxLength)
    {
      return CreateReflectionBasedPropertyDefinition(classDefinition, propertyName, columnName, propertyType, isNullable, maxLength, StorageClass.Persistent);
    }

    public static ReflectionBasedPropertyDefinition CreateReflectionBasedPropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, bool? isNullable, int? maxLength, StorageClass storageClass)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

      PropertyInfo propertyInfo = typeof (Order).GetProperty ("OrderNumber");
      return new ReflectionBasedPropertyDefinition(classDefinition, propertyInfo, propertyName, columnName, propertyType, isNullable, maxLength,
          storageClass);
    }
  }
}
