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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.SortExpressions
{
  public static class SortExpressionDefinitionObjectMother
  {
    public static SortedPropertySpecification CreateSortedPropertyAscending (PropertyDefinition propertyDefinition)
    {
      return new SortedPropertySpecification (propertyDefinition, SortOrder.Ascending);
    }

    public static SortedPropertySpecification CreateSortedPropertyDescending (PropertyDefinition propertyDefinition)
    {
      return new SortedPropertySpecification (propertyDefinition, SortOrder.Descending);
    }

    public static SortExpressionDefinition CreateOrderItemSortExpressionPositionAscProductDesc ()
    {
      var orderItemClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      var positionPropertyDefinition = orderItemClassDefinition.GetMandatoryPropertyDefinition (typeof (OrderItem).FullName + ".Position");
      var productPropertyDefinition = orderItemClassDefinition.GetMandatoryPropertyDefinition (typeof (OrderItem).FullName + ".Product");

      return new SortExpressionDefinition (
          new[]
          {
              CreateSortedPropertyAscending (positionPropertyDefinition), 
              CreateSortedPropertyDescending (productPropertyDefinition)
          });
    }

    public static SortExpressionDefinition CreateEmptySortExpression ()
    {
      return new SortExpressionDefinition (new SortedPropertySpecification[0]);
    }
  }
}