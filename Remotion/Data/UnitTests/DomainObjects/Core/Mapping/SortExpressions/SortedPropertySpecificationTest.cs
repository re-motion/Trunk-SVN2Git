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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.SortExpressions
{
  [TestFixture]
  public class SortedPropertySpecificationTest : StandardMappingTest
  {
    private ClassDefinition _orderItemClassDefinition;
    private PropertyDefinition _productPropertyDefinition;
    private PropertyDefinition _positionPropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp ();
      _orderItemClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));

      _productPropertyDefinition = _orderItemClassDefinition.GetPropertyDefinition (typeof (OrderItem).FullName + ".Product");
      _positionPropertyDefinition = _orderItemClassDefinition.GetPropertyDefinition (typeof (OrderItem).FullName + ".Position");
    }

    [Test]
    public new void ToString ()
    {
      var specificationAsc = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Ascending);
      var specificationDesc = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Descending);

      Assert.That (specificationAsc.ToString (), Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product ASC"));
      Assert.That (specificationDesc.ToString (), Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product DESC"));
    }

    [Test]
    public void Equals ()
    {
      var specification1 = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Ascending);
      var specification2 = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Ascending);
      var specification3 = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Descending);
      var specification4 = new SortedPropertySpecification (_positionPropertyDefinition, SortOrder.Ascending);

      Assert.That (specification1, Is.EqualTo (specification2));
      Assert.That (specification1, Is.Not.EqualTo (specification3));
      Assert.That (specification1, Is.Not.EqualTo (specification4));
    }

    [Test]
    public new void GetHashCode ()
    {
      var specification1 = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Ascending);
      var specification2 = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Ascending);

      Assert.That (specification1.GetHashCode(), Is.EqualTo (specification2.GetHashCode()));
    }

    [Test]
    public void GetComparer_Ascending ()
    {
      var specification = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Ascending);

      var comparer = specification.GetComparer<DataContainer> ((dc, property) => dc[property.PropertyName]);

      var dataContainer1 = CreateOrderItemDataContainer ("aaa");
      var dataContainer2 = CreateOrderItemDataContainer ("bbb");
      var dataContainer3 = CreateOrderItemDataContainer ("aaa");

      Assert.That (comparer.Compare (dataContainer1, dataContainer2), Is.EqualTo (-1));
      Assert.That (comparer.Compare (dataContainer2, dataContainer1), Is.EqualTo (1));
      Assert.That (comparer.Compare (dataContainer1, dataContainer3), Is.EqualTo (0));
    }

    [Test]
    public void GetComparer_Descending ()
    {
      var specification = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Descending);

      var comparer = specification.GetComparer<DataContainer> ((dc, property) => dc[property.PropertyName]);

      var dataContainer1 = CreateOrderItemDataContainer ("aaa");
      var dataContainer2 = CreateOrderItemDataContainer ("bbb");
      var dataContainer3 = CreateOrderItemDataContainer ("aaa");

      Assert.That (comparer.Compare (dataContainer1, dataContainer2), Is.EqualTo (1));
      Assert.That (comparer.Compare (dataContainer2, dataContainer1), Is.EqualTo (-1));
      Assert.That (comparer.Compare (dataContainer1, dataContainer3), Is.EqualTo (0));
    }

    private DataContainer CreateOrderItemDataContainer (string product)
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderItem1);
      dataContainer.PropertyValues[_productPropertyDefinition.PropertyName].Value = product;
      return dataContainer;
    }
  }
}