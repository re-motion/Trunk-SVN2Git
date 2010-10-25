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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.SortExpressions
{
  [TestFixture]
  public class SortedPropertySpecificationTest : StandardMappingTest
  {
    private ClassDefinition _orderItemClassDefinition;
    private PropertyDefinition _productPropertyDefinition;
    private PropertyDefinition _positionPropertyDefinition;

    private ClassDefinition _customerClassDefinition;
    private PropertyDefinition _customerSincePropertyDefinition;
    private PropertyDefinition _customerTypePropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp ();
      _orderItemClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      _productPropertyDefinition = _orderItemClassDefinition.GetMandatoryPropertyDefinition (typeof (OrderItem).FullName + ".Product");
      _positionPropertyDefinition = _orderItemClassDefinition.GetMandatoryPropertyDefinition (typeof (OrderItem).FullName + ".Position");

      _customerClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Customer));
      _customerSincePropertyDefinition = _customerClassDefinition.GetMandatoryPropertyDefinition (typeof (Customer).FullName + ".CustomerSince");
      _customerTypePropertyDefinition = _customerClassDefinition.GetMandatoryPropertyDefinition (typeof (Customer).FullName + ".Type");
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Cannot sort by property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty' - its property type "
        + "('Byte[]') does not implement IComparable.")]
    public void Initialization_NoIComparableType ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));
      var propertyDefinition = classDefinition.GetPropertyDefinition (typeof (ClassWithAllDataTypes).FullName + ".BinaryProperty");
      
      new SortedPropertySpecification (propertyDefinition, SortOrder.Ascending);
    }

    [Test]
    public void Initialization_IComparableType_Nullable ()
    {
      Assert.That (Nullable.GetUnderlyingType (_customerSincePropertyDefinition.PropertyType), Is.Not.Null);
      new SortedPropertySpecification (_customerSincePropertyDefinition, SortOrder.Ascending);
    }

    [Test]
    public void Initialization_NotResolvedType ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));
      var fakePropertyDefinition = MockRepository.GenerateStub<PropertyDefinition> (classDefinition, "BinaryProperty", null, StorageClass.Persistent, new FakeColumnDefinition ("T"));
      Assert.That (fakePropertyDefinition.IsPropertyTypeResolved, Is.False);

      var result = new SortedPropertySpecification (fakePropertyDefinition, SortOrder.Ascending);

      Assert.That (result.PropertyDefinition, Is.SameAs (fakePropertyDefinition));
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

      Assert.That (specification1, Is.Not.EqualTo (null));
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

    [Test]
    public void GetComparer_NonExistingProperty_Nullable ()
    {
      var specification = new SortedPropertySpecification (_customerSincePropertyDefinition, SortOrder.Ascending);

      var comparer = specification.GetComparer<DataContainer> ((dc, property) => 
          dc.ClassDefinition == property.ClassDefinition 
          ? dc[property.PropertyName] 
          : null);

      var dataContainer1 = CreateCustomerDataContainer (new DateTime(2010, 01, 02), Customer.CustomerType.Gold);
      var dataContainer2 = CreatePartnerDataContainer ();

      Assert.That (comparer.Compare (dataContainer1, dataContainer2), Is.EqualTo (1));
      Assert.That (comparer.Compare (dataContainer2, dataContainer1), Is.EqualTo (-1));
    }

    [Test]
    public void GetComparer_NonExistingProperty_NonNullable ()
    {
      var specification = new SortedPropertySpecification (_customerTypePropertyDefinition, SortOrder.Ascending);

      var comparer = specification.GetComparer<DataContainer> ((dc, property) =>
          dc.ClassDefinition == property.ClassDefinition
          ? dc[property.PropertyName]
          : null);

      var dataContainer1 = CreateCustomerDataContainer (new DateTime (2010, 01, 02), Customer.CustomerType.Gold);
      var dataContainer2 = CreatePartnerDataContainer ();

      Assert.That (comparer.Compare (dataContainer1, dataContainer2), Is.EqualTo (1));
      Assert.That (comparer.Compare (dataContainer2, dataContainer1), Is.EqualTo (-1));
    }

    private DataContainer CreateOrderItemDataContainer (string product)
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderItem1);
      dataContainer.PropertyValues[_productPropertyDefinition.PropertyName].Value = product;
      return dataContainer;
    }

    private DataContainer CreateCustomerDataContainer (DateTime customerSince, Customer.CustomerType customerType)
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Customer1);
      dataContainer.PropertyValues[_customerSincePropertyDefinition.PropertyName].Value = customerSince;
      dataContainer.PropertyValues[_customerTypePropertyDefinition.PropertyName].Value = customerType;
      return dataContainer;
    }

    private DataContainer CreatePartnerDataContainer ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Partner1);
      return dataContainer;
    }
  }
}