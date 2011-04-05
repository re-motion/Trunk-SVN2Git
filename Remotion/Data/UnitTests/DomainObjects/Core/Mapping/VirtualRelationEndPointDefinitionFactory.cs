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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  public static class VirtualRelationEndPointDefinitionFactory
  {
    private static readonly PropertyInfo s_dummyPropertyInfo = typeof (Order).GetProperty ("OrderNumber");

    public static VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinition (ClassDefinition classDefinition, string propertyName, bool isMandatory, CardinalityType cardinality, string propertyTypeName, string sortExpressionString)
    {
      return new VirtualRelationEndPointDefinition (classDefinition, propertyName, isMandatory, cardinality, propertyTypeName, sortExpressionString, s_dummyPropertyInfo);
    }

    public static VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinition (ClassDefinition classDefinition, string propertyName, bool isMandatory, CardinalityType cardinality, Type propertyType, string sortExpressionString)
    {
      return new VirtualRelationEndPointDefinition (classDefinition, propertyName, isMandatory, cardinality, propertyType, sortExpressionString, s_dummyPropertyInfo);
    }

    public static VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinition (ClassDefinition classDefinition, string propertyName, bool isMandatory, CardinalityType cardinality, Type propertyType)
    {
      return CreateVirtualRelationEndPointDefinition (classDefinition, propertyName, isMandatory, cardinality, propertyType, null);
    }
  }
}
