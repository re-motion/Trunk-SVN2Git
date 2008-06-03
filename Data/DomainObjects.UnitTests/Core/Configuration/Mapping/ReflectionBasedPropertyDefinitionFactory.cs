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

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  /// <summary>
  /// Provides simple factory methods to manually create ReflectionBasedPropertyDefinition objects for testing. The definition objects will have
  /// dummy (invalid) PropertyInfo properties.
  /// </summary>
  public static class ReflectionBasedPropertyDefinitionFactory
  {
    public static ReflectionBasedPropertyDefinition CreateReflectionBasedPropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType)
    {
      return CreateReflectionBasedPropertyDefinition(classDefinition, propertyName, columnName, propertyType, null, null, true);
    }

    public static ReflectionBasedPropertyDefinition CreateReflectionBasedPropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, bool isNullable)
    {
      return CreateReflectionBasedPropertyDefinition(classDefinition, propertyName, columnName, propertyType, isNullable, null, true);
    }

    public static ReflectionBasedPropertyDefinition CreateReflectionBasedPropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, int maxLength)
    {
      return CreateReflectionBasedPropertyDefinition(classDefinition, propertyName, columnName, propertyType, null, maxLength, true);
    }

    public static ReflectionBasedPropertyDefinition CreateReflectionBasedPropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, bool isNullable, int maxLength)
    {
      return CreateReflectionBasedPropertyDefinition(classDefinition, propertyName, columnName, propertyType, isNullable, maxLength, true);
    }

    public static ReflectionBasedPropertyDefinition CreateReflectionBasedPropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName, Type propertyType, bool? isNullable, int? maxLength, bool isPersistent)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("columnName", columnName);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

      //int lastDot = propertyName.LastIndexOf ('.');
      //if (lastDot == -1)
      //  throw new ArgumentException (string.Format ("Property name {0} is not a well-formed long property name.", propertyName), "propertyName");

      //string declaringTypeName = propertyName.Substring (0, lastDot);
      //string shortPropertyName = propertyName.Substring (lastDot + 1);
      
      //Type declaringType = Type.GetType (declaringTypeName, true);
      //PropertyInfo propertyInfo = declaringType.GetProperty (shortPropertyName,
      //    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);

      //if (propertyInfo == null)
      //  throw new ArgumentException (string.Format ("Property name {0} does not denote a property {1} declared on type {2} .", propertyName,
      //      shortPropertyName, declaringTypeName), "propertyName");

      PropertyInfo propertyInfo = typeof (Order).GetProperty ("OrderNumber");

      return new ReflectionBasedPropertyDefinition(classDefinition, propertyInfo, propertyName, columnName, propertyType, isNullable, maxLength,
          isPersistent);
    }
  }
}
