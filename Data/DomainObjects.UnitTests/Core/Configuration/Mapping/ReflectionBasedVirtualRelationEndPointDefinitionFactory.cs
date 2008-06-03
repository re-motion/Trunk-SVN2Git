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

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  public static class ReflectionBasedVirtualRelationEndPointDefinitionFactory
  {
    private static PropertyInfo s_dummyPropertyInfo = typeof (Order).GetProperty ("OrderNumber");

    public static ReflectionBasedVirtualRelationEndPointDefinition CreateReflectionBasedVirtualRelationEndPointDefinition (ClassDefinition classDefinition, string propertyName, bool isMandatory, CardinalityType cardinality, string propertyTypeName, string sortExpression)
    {
      return new ReflectionBasedVirtualRelationEndPointDefinition (classDefinition, propertyName, isMandatory, cardinality, propertyTypeName, sortExpression, s_dummyPropertyInfo);
    }

    public static ReflectionBasedVirtualRelationEndPointDefinition CreateReflectionBasedVirtualRelationEndPointDefinition (ClassDefinition classDefinition, string propertyName, bool isMandatory, CardinalityType cardinality, Type propertyType, string sortExpression)
    {
      return new ReflectionBasedVirtualRelationEndPointDefinition (classDefinition, propertyName, isMandatory, cardinality, propertyType, sortExpression, s_dummyPropertyInfo);
    }

    public static ReflectionBasedVirtualRelationEndPointDefinition CreateReflectionBasedVirtualRelationEndPointDefinition (ClassDefinition classDefinition, string propertyName, bool isMandatory, CardinalityType cardinality, Type propertyType)
    {
      return CreateReflectionBasedVirtualRelationEndPointDefinition (classDefinition, propertyName, isMandatory, cardinality, propertyType, null);
    }
  }
}
