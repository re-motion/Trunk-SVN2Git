// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class PropertyAccessorDataCacheTest : StandardMappingTest
  {
    private PropertyAccessorDataCache _orderCache;
    private PropertyAccessorDataCache _distributorCache;
    private PropertyAccessorDataCache _closedGenericClassCache;
    private PropertyAccessorDataCache _derivedClassWithMixedPropertiesCache;

    public override void SetUp ()
    {
      base.SetUp();
      _orderCache = CreatePropertyAccessorDataCache (typeof (Order));
      _distributorCache = CreatePropertyAccessorDataCache (typeof (Distributor));
      _closedGenericClassCache = CreatePropertyAccessorDataCache (typeof (ClosedGenericClassWithManySideRelationProperties));
      _derivedClassWithMixedPropertiesCache = CreatePropertyAccessorDataCache (typeof (DerivedClassWithDifferentProperties));
    }

    [Test]
    public void GetPropertyAccessorData_ValueProperty ()
    {
      var data = _orderCache.GetPropertyAccessorData (typeof (Order) + ".OrderNumber");

      Assert.That (data, Is.Not.Null);
      Assert.That (data.PropertyIdentifier, Is.EqualTo (typeof (Order) + ".OrderNumber"));
      Assert.That (data.Kind, Is.EqualTo (PropertyKind.PropertyValue));
    }

    [Test]
    public void GetPropertyAccessorData_RelatedObjectProperty_RealSide ()
    {
      var data = _orderCache.GetPropertyAccessorData (typeof (Order) + ".Customer");

      Assert.That (data, Is.Not.Null);
      Assert.That (data.PropertyIdentifier, Is.EqualTo (typeof (Order) + ".Customer"));
      Assert.That (data.Kind, Is.EqualTo (PropertyKind.RelatedObject));
      Assert.That (data.RelationEndPointDefinition.IsVirtual, Is.False);
    }

    [Test]
    public void GetPropertyAccessorData_RelatedObjectProperty_VirtualSide ()
    {
      var data = _orderCache.GetPropertyAccessorData (typeof (Order) + ".OrderTicket");

      Assert.That (data, Is.Not.Null);
      Assert.That (data.PropertyIdentifier, Is.EqualTo (typeof (Order) + ".OrderTicket"));
      Assert.That (data.Kind, Is.EqualTo (PropertyKind.RelatedObject));
      Assert.That (data.RelationEndPointDefinition.IsVirtual, Is.True);
    }

    [Test]
    public void GetPropertyAccessorData_RelatedCollectionProperty ()
    {
      var data = _orderCache.GetPropertyAccessorData (typeof (Order) + ".OrderItems");

      Assert.That (data, Is.Not.Null);
      Assert.That (data.PropertyIdentifier, Is.EqualTo (typeof (Order) + ".OrderItems"));
      Assert.That (data.Kind, Is.EqualTo (PropertyKind.RelatedObjectCollection));
      Assert.That (data.RelationEndPointDefinition.IsVirtual, Is.True);
    }

    [Test]
    public void GetPropertyAccessorData_Unknown ()
    {
      var data = _orderCache.GetPropertyAccessorData (typeof (Order) + ".OrderSmell");

      Assert.That (data, Is.Null);
    }

    [Test]
    public void GetPropertyAccessorData_ItemsAreCached ()
    {
      var data1 = _orderCache.GetPropertyAccessorData (typeof (Order) + ".OrderNumber");
      var data2 = _orderCache.GetPropertyAccessorData (typeof (Order) + ".OrderNumber");

      Assert.That (data1, Is.Not.Null);
      Assert.That (data1, Is.SameAs (data2));
    }

    [Test]
    public void GetPropertyAccessorData_TypeAndShortName ()
    {
      var data = _orderCache.GetPropertyAccessorData (typeof (Order), "OrderNumber");
      
      Assert.That (data, Is.Not.Null);
      Assert.That (data, Is.EqualTo (_orderCache.GetPropertyAccessorData (typeof (Order).FullName + ".OrderNumber")));
    }

    [Test]
    public void GetPropertyAccessorData_TypeAndShortName_Unknown ()
    {
      var data = _orderCache.GetPropertyAccessorData (typeof (Order), "OrderSmell");

      Assert.That (data, Is.Null);
    }

    [Test]
    public void GetPropertyAccessorData_Expression ()
    {
      var data = _orderCache.GetPropertyAccessorData ((Order o) => o.OrderNumber);

      Assert.That (data, Is.Not.Null);
      Assert.That (data, Is.EqualTo (_orderCache.GetPropertyAccessorData (typeof (Order).FullName + ".OrderNumber")));
    }

    [Test]
    public void GetPropertyAccessorData_Expression_Mixin_ViaInterface ()
    {
      var cacheWithMixins = new PropertyAccessorDataCache (GetTypeDefinition (typeof (TargetClassForPersistentMixin)));
// ReSharper disable SuspiciousTypeConversion.Global
      var data = cacheWithMixins.GetPropertyAccessorData ((TargetClassForPersistentMixin t) => ((IMixinAddingPersistentProperties) t).PersistentProperty);
// ReSharper restore SuspiciousTypeConversion.Global

      Assert.That (data, Is.Not.Null);
      Assert.That (data, Is.EqualTo (cacheWithMixins.GetPropertyAccessorData (typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty")));
    }

    [Test]
    public void GetPropertyAccessorData_Expression_Mixin_ViaMixin ()
    {
      var cacheWithMixins = new PropertyAccessorDataCache (GetTypeDefinition (typeof (TargetClassForPersistentMixin)));
      var data = cacheWithMixins.GetPropertyAccessorData ((TargetClassForPersistentMixin t) => (Mixin.Get<MixinAddingPersistentProperties>(t).PersistentProperty));

      Assert.That (data, Is.Not.Null);
      Assert.That (data, Is.EqualTo (cacheWithMixins.GetPropertyAccessorData (typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty")));
    }

    [Test]
    public void GetPropertyAccessorData_Expression_Interface ()
    {
      var data = _orderCache.GetPropertyAccessorData ((Order o) => ((IOrder) o).OrderNumber);

      Assert.That (data, Is.Not.Null);
      Assert.That (data, Is.EqualTo (_orderCache.GetPropertyAccessorData (typeof (Order).FullName + ".OrderNumber")));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The memberAccessExpression must be a simple member access expression.\r\nParameter name: memberAccessExpression")]
    public void GetPropertyAccessorData_InvalidExpression_NoMember ()
    {
      _orderCache.GetPropertyAccessorData ((Order o) => o.OrderNumber + 5);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The expression must identify a property.\r\nParameter name: propertyAccessExpression")]
    public void GetPropertyAccessorData_InvalidExpression_Field ()
    {
      _orderCache.GetPropertyAccessorData ((Order o) => o.CtorCalled);
    }

    [Test]
    public void GetMandatoryPropertyAccessorData_FullPropertyName ()
    {
      var data = _orderCache.GetMandatoryPropertyAccessorData (typeof (Order).FullName + ".OrderNumber");

      Assert.That (data, Is.Not.Null);
      Assert.That (data, Is.EqualTo (_orderCache.GetPropertyAccessorData (typeof (Order).FullName + ".OrderNumber")));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The domain object type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' does not have a mapping property named "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderSmell'.")]
    public void GetMandatoryPropertyAccessorData_FullPropertyName_Unknown ()
    {
      _orderCache.GetMandatoryPropertyAccessorData (typeof (Order).FullName + ".OrderSmell");
    }

    [Test]
    public void GetMandatoryPropertyAccessorData_TypeAndShortName ()
    {
      var data = _orderCache.GetMandatoryPropertyAccessorData (typeof (Order), "OrderNumber");

      Assert.That (data, Is.Not.Null);
      Assert.That (data, Is.EqualTo (_orderCache.GetPropertyAccessorData (typeof (Order).FullName + ".OrderNumber")));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The domain object type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' does not have a mapping property named "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderSmell'.")]
    public void GetMandatoryPropertyAccessorData_TypeAndShortName_Unknown ()
    {
      _orderCache.GetMandatoryPropertyAccessorData (typeof (Order), "OrderSmell");
    }

    [Test]
    public void GetMandatoryPropertyAccessorData_Expression ()
    {
      var data = _orderCache.GetMandatoryPropertyAccessorData ((Order o) => o.OrderNumber);

      Assert.That (data, Is.Not.Null);
      Assert.That (data, Is.EqualTo (_orderCache.GetPropertyAccessorData (typeof (Order).FullName + ".OrderNumber")));
    }

    [Test]
    public void GetMandatoryPropertyAccessorData_Expression_Mixin_ViaInterface ()
    {
      var cacheWithMixins = new PropertyAccessorDataCache (GetTypeDefinition (typeof (TargetClassForPersistentMixin)));
      // ReSharper disable SuspiciousTypeConversion.Global
      var data = cacheWithMixins.GetMandatoryPropertyAccessorData ((TargetClassForPersistentMixin t) => ((IMixinAddingPersistentProperties) t).PersistentProperty);
      // ReSharper restore SuspiciousTypeConversion.Global

      Assert.That (data, Is.Not.Null);
      Assert.That (data, Is.EqualTo (cacheWithMixins.GetPropertyAccessorData (typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty")));
    }

    [Test]
    public void GetMandatoryPropertyAccessorData_Expression_Mixin_ViaMixin ()
    {
      var cacheWithMixins = new PropertyAccessorDataCache (GetTypeDefinition (typeof (TargetClassForPersistentMixin)));
      var data = cacheWithMixins.GetMandatoryPropertyAccessorData ((TargetClassForPersistentMixin t) => (Mixin.Get<MixinAddingPersistentProperties> (t).PersistentProperty));

      Assert.That (data, Is.Not.Null);
      Assert.That (data, Is.EqualTo (cacheWithMixins.GetPropertyAccessorData (typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty")));
    }

    [Test]
    public void GetMandatoryPropertyAccessorData_Expression_Interface ()
    {
      var data = _orderCache.GetMandatoryPropertyAccessorData ((Order o) => ((IOrder) o).OrderNumber);

      Assert.That (data, Is.Not.Null);
      Assert.That (data, Is.EqualTo (_orderCache.GetPropertyAccessorData (typeof (Order).FullName + ".OrderNumber")));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The domain object type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' does not have a mapping property identified by expression "
        + "'o => o.NotInMapping'.")]
    public void GetMandatoryPropertyAccessorData_Expression_Unknown ()
    {
      _orderCache.GetMandatoryPropertyAccessorData ((Order o) => o.NotInMapping);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The domain object type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' does not have a mapping property identified by expression "
        + "'o => Convert(Convert(o)).Product'.")]
    public void GetMandatoryPropertyAccessorData_Expression_UnknownOnThisObject ()
    {
      _orderCache.GetMandatoryPropertyAccessorData ((Order o) => ((OrderItem) (object) o).Product);
    }

    [Test]
    public void FindPropertyAccessorData_Property ()
    {
      var result = _orderCache.FindPropertyAccessorData (typeof (Order), "OrderNumber");

      var expected = _orderCache.GetMandatoryPropertyAccessorData (typeof (Order), "OrderNumber");
      Assert.That (result, Is.EqualTo (expected));
    }

    [Test]
    public void FindPropertyAccessorData_VirtualRelationEndPoint ()
    {
      var result = _orderCache.FindPropertyAccessorData (typeof (Order), "OrderItems");

      var expected = _orderCache.GetMandatoryPropertyAccessorData (typeof (Order), "OrderItems");
      Assert.That (result, Is.EqualTo (expected));
    }

    [Test]
    public void FindPropertyAccessorData_FromDerivedType ()
    {
      Assert.That (_distributorCache.GetPropertyAccessorData (typeof (Distributor), "ContactPerson"), Is.Null);

      var result = _distributorCache.FindPropertyAccessorData (typeof (Distributor), "ContactPerson");

      var expected = _distributorCache.GetMandatoryPropertyAccessorData (typeof (Partner), "ContactPerson");
      Assert.That (result, Is.EqualTo (expected));
    }

    [Test]
    public void FindPropertyAccessorData_FromGenericType ()
    {
      Assert.That (
          _closedGenericClassCache.GetPropertyAccessorData (typeof (ClosedGenericClassWithManySideRelationProperties), "BaseUnidirectional"),
          Is.Null);

      var result = _closedGenericClassCache.FindPropertyAccessorData (typeof (ClosedGenericClassWithManySideRelationProperties), "BaseUnidirectional");

      var expected = _closedGenericClassCache.GetMandatoryPropertyAccessorData (
          typeof (GenericClassWithManySideRelationPropertiesNotInMapping<>),
          "BaseUnidirectional");
      Assert.That (result, Is.EqualTo (expected));
    }

    [Test]
    public void FindPropertyAccessorData_WithShadowedProperty ()
    {
      var shadowingResult = _derivedClassWithMixedPropertiesCache.FindPropertyAccessorData (typeof (DerivedClassWithDifferentProperties), "String");
      var shadowingExpected =
          _derivedClassWithMixedPropertiesCache.GetMandatoryPropertyAccessorData (typeof (DerivedClassWithDifferentProperties), "String");
      Assert.That (shadowingResult, Is.EqualTo (shadowingExpected));

      var shadowedResult = _derivedClassWithMixedPropertiesCache.FindPropertyAccessorData (typeof (ClassWithDifferentProperties), "String");
      var shadowedExpected = _derivedClassWithMixedPropertiesCache.GetMandatoryPropertyAccessorData (typeof (ClassWithDifferentProperties), "String");
      Assert.That (shadowedResult, Is.EqualTo (shadowedExpected));
    }

    [Test]
    public void FindPropertyAccessorData_NonExistingProperty ()
    {
      var result = _distributorCache.FindPropertyAccessorData (typeof (Distributor), "Frobbers");
      Assert.That (result, Is.Null);
    }

    private PropertyAccessorDataCache CreatePropertyAccessorDataCache (Type classType)
    {
      return new PropertyAccessorDataCache (MappingConfiguration.Current.GetTypeDefinition (classType));
    }
  }
}