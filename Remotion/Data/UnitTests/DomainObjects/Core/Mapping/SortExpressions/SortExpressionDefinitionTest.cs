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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.SortExpressions
{
  [TestFixture]
  public class SortExpressionDefinitionTest : StandardMappingTest
  {
    private ClassDefinition _orderItemClassDefinition;
    private PropertyDefinition _productPropertyDefinition;
    private PropertyDefinition _positionPropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _orderItemClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      _productPropertyDefinition = _orderItemClassDefinition.GetMandatoryPropertyDefinition (typeof (OrderItem).FullName + ".Product");
      _positionPropertyDefinition = _orderItemClassDefinition.GetMandatoryPropertyDefinition (typeof (OrderItem).FullName + ".Position");
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException), ExpectedMessage = 
        "A SortExpressionDefinition must contain at least one sorted property.\r\nParameter name: sortedProperties")]
    public void Initialization_Empty ()
    {
      new SortExpressionDefinition (new SortedPropertySpecification[0]);
    }

    [Test]
    public new void ToString ()
    {
      var sortExpressionDefinition =
          new SortExpressionDefinition (
              new[]
              {
                  SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending (_productPropertyDefinition),
                  SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending (_positionPropertyDefinition)
              });

      var result = sortExpressionDefinition.ToString();

      var expected =
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product ASC, "
          + "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Position DESC";
      Assert.That (result, Is.EqualTo (expected));
    }

    [Test]
    public void GetComparer_SingleSortedProperty ()
    {
      var sortExpressionDefinition =
          new SortExpressionDefinition (
              new[]
              {
                  SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending (_productPropertyDefinition)
              });

      IComparer<DataContainer> comparer = sortExpressionDefinition.GetComparer<DataContainer> (
          (dc, property) => dc.PropertyValues[property.PropertyName].GetValueWithoutEvents (ValueAccess.Current));

      var dataContainer1 = CreateOrderItemDataContainer ("aaa");
      var dataContainer2 = CreateOrderItemDataContainer ("bbb");
      var dataContainer3 = CreateOrderItemDataContainer ("aaa");

      Assert.That (comparer.Compare (dataContainer1, dataContainer2), Is.EqualTo (1));
      Assert.That (comparer.Compare (dataContainer2, dataContainer1), Is.EqualTo (-1));
      Assert.That (comparer.Compare (dataContainer3, dataContainer1), Is.EqualTo (0));
    }

    [Test]
    public void GetComparer_MultipleSortedProperties ()
    {
      var sortExpressionDefinition =
          new SortExpressionDefinition (
              new[]
              {
                  SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending (_productPropertyDefinition),
                  SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending (_positionPropertyDefinition)
              });

      IComparer<DataContainer> comparer = sortExpressionDefinition.GetComparer<DataContainer> (
          (dc, property) => dc.PropertyValues[property.PropertyName].GetValueWithoutEvents (ValueAccess.Current));

      var dataContainer1 = CreateOrderItemDataContainer ("aaa", 1);
      var dataContainer2 = CreateOrderItemDataContainer ("bbb", 2);
      var dataContainer3 = CreateOrderItemDataContainer ("aaa", 1);
      var dataContainer4 = CreateOrderItemDataContainer ("aaa", 2);

      Assert.That (comparer.Compare (dataContainer1, dataContainer2), Is.EqualTo (1));
      Assert.That (comparer.Compare (dataContainer2, dataContainer1), Is.EqualTo (-1));
      Assert.That (comparer.Compare (dataContainer1, dataContainer3), Is.EqualTo (0));
      Assert.That (comparer.Compare (dataContainer1, dataContainer4), Is.EqualTo (-1));
    }

    private DataContainer CreateOrderItemDataContainer (string product)
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderItem1);
      dataContainer.PropertyValues[_productPropertyDefinition.PropertyName].Value = product;
      return dataContainer;
    }

    private DataContainer CreateOrderItemDataContainer (string product, int position)
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderItem1);
      dataContainer.PropertyValues[_productPropertyDefinition.PropertyName].Value = product;
      dataContainer.PropertyValues[_positionPropertyDefinition.PropertyName].Value = position;
      return dataContainer;
    }
  }
}