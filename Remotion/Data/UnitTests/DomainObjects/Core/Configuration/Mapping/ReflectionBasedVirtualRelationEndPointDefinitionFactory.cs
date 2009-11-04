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

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
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
