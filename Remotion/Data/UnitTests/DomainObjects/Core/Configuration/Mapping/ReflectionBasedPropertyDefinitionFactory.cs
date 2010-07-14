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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
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
      ColumnDefinition columnDefinition = null;
      if (storageClass == StorageClass.Persistent)
        columnDefinition = new ColumnDefinition (columnName, propertyInfo);
      return new ReflectionBasedPropertyDefinition (
          classDefinition,
          propertyInfo,
          propertyName,
          propertyType,
          isNullable,
          maxLength,
          storageClass,
          columnDefinition);
    }
  }
}
