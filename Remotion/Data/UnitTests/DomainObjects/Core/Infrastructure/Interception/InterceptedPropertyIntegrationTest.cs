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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using System.Linq;
using Remotion.Reflection;
using Throws = Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.Throws;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception
{
  [TestFixture]
  public class InterceptedPropertyIntegrationTest : ClientTransactionBaseTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      Assert.That (CurrentPropertyManager.CurrentPropertyName, Is.Null);
    }

    public override void TearDown ()
    {
      base.TearDown ();
      Assert.That (CurrentPropertyManager.CurrentPropertyName, Is.Null);
    }

    [Test]
    public void LoadOfSimpleObjectWorks ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      Assert.That (WasCreatedByFactory (order), Is.True);
    }

    [Test]
    public void ConstructionOfSimpleObjectWorks ()
    {
      Order order = Order.NewObject();
      Assert.That (WasCreatedByFactory (order), Is.True);

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      Assert.That (classWithAllDataTypes, Is.Not.Null);
      Assert.That (WasCreatedByFactory (classWithAllDataTypes), Is.True);
    }

    [Test]
    public void ConstructedObjectIsDerived ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      Assert.That (((object) classWithAllDataTypes).GetType().Equals (typeof (ClassWithAllDataTypes)), Is.False);
    }

    [Test]
    public void GetPropertyValueWorks ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      Assert.That (order.OrderNumber, Is.EqualTo (1));
      Assert.That (order.DeliveryDate, Is.EqualTo (new DateTime (2005, 01, 01)));
      Assert.That (order.OrderNumber, Is.EqualTo (1));
    }

    [Test]
    public void GetPropertyValue_WithNullAndAbstractProperty ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      Assert.That (classWithAllDataTypes.StringWithNullValueProperty, Is.Null);
    }

    [Test]
    public void SetPropertyValueWorks ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();

      order.OrderNumber = 15;
      Assert.That (order.OrderNumber, Is.EqualTo (15));

      order.DeliveryDate = new DateTime (2007, 02, 03);
      Assert.That (order.DeliveryDate, Is.EqualTo (new DateTime (2007, 02, 03)));

      Assert.That (order.OrderNumber, Is.EqualTo (15));
    }

    [Test]
    public void SetPropertyValue_WithNullAndAbstractProperty ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      classWithAllDataTypes.StringWithNullValueProperty = null;
      Assert.That (classWithAllDataTypes.StringWithNullValueProperty, Is.Null);
    }

    [Test]
    public void SetPropertyValue_WithCollectionSet ()
    {
      ClassWithAbstractRelatedCollectionSetter instance = ClassWithAbstractRelatedCollectionSetter.NewObject ();
      var newRelatedObjects = new ObjectList<ClassWithAbstractRelatedCollectionSetter> ();
      instance.RelatedObjects = newRelatedObjects;
      Assert.That (instance.RelatedObjects, Is.SameAs (newRelatedObjects));
    }


    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.NonInstantiableAbstractClass "
        + "as its member Foo (on type NonInstantiableAbstractClass) is abstract (and not an "
        + "automatic property).")]
    public void AbstractWithMethodCannotBeInstantiated ()
    {
      NonInstantiableAbstractClass.NewObject();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.NonInstantiableAbstractClassWithProps "
        + "as its member get_Foo (on type NonInstantiableAbstractClassWithProps) is abstract (and not an automatic property).")]
    public void AbstractWithNonAutoPropertiesCannotBeInstantiated ()
    {
      NonInstantiableAbstractClassWithProps.NewObject();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
        "Cannot instantiate type "
        + "'Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.NonInstantiableClassWithMixinWithPersistentAutoProperties' "
        + "because the mixin member 'MixinWithAutoProperties.PersistentAutoProperty' is an automatic property. Mixins must implement their persistent "
        + "members by using 'Properties' to get and set property values.")]
    public void ClassWithMixinWithAutoPropertiesCannotBeInstantiated ()
    {
      NonInstantiableClassWithMixinWithPersistentAutoProperties.NewObject ();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
        "Cannot instantiate type 'Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.NonInstantiableSealedClass' as it is sealed.")]
    public void SealedCannotBeInstantiated ()
    {
      NonInstantiableSealedClass.NewObject();
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void NonDomainCannotBeInstantiated ()
    {
      InterceptedDomainObjectCreator.Instance.Factory.GetConcreteDomainObjectType (typeof (NonInstantiableNonDomainClass));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage =
        "Type 'Remotion.Data.UnitTests.DomainObjects.TestDomain."
        + "Order' does not contain a constructor with the following signature: (String, String, String, Object).")]
    public void WrongConstructorCannotBeInstantiated ()
    {
      LifetimeService.NewObject (TestableClientTransaction, typeof (Order), ParamList.Create ("foo", "bar", "foobar", (object) null));
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "Thrown in ThrowException()")]
    public void ConstructorThrowIsPropagated ()
    {
      Throws.NewObject();
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage =
        "Type 'Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.ClassWithWrongConstructor' does not contain a "
        + "constructor with the following signature: ().")]
    public void ConstructorMismatch1 ()
    {
      ClassWithWrongConstructor.NewObject();
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage =
        "Type 'Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.ClassWithWrongConstructor' does not contain a "
        + "constructor with the following signature: (Double).")]
    public void ConstructorMismatch2 ()
    {
      ClassWithWrongConstructor.NewObject (3.0);
    }

    [Test]
    public void GetSetRelatedObjectAndOriginal ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      Customer customer = order.Customer;
      Assert.That (customer, Is.Not.Null);
      Assert.That (customer, Is.SameAs (DomainObjectIDs.Customer1.GetObject<Customer> ()));

      Customer newCustomer = Customer.NewObject();
      Assert.That (newCustomer, Is.Not.Null);
      order.Customer = newCustomer;
      Assert.That (order.Customer, Is.SameAs (newCustomer));

      Assert.That (order.OriginalCustomer, Is.SameAs (customer));
    }

    [Test]
    public void GetSetRelatedObjectAndOriginal_WithNullAndAutomaticProperty ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      Assert.IsNotEmpty (order.OrderItems);
      OrderItem orderItem = order.OrderItems[0];

      orderItem.Order = null;
      Assert.That (orderItem.Order, Is.Null);

      Assert.That (orderItem.OriginalOrder, Is.SameAs (order));
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      DomainObjectCollection orderItems = order.OrderItems;
      Assert.That (orderItems, Is.Not.Null);
      Assert.That (orderItems.Count, Is.EqualTo (2));

      Assert.That (orderItems.Contains (DomainObjectIDs.OrderItem1), Is.True);
      Assert.That (orderItems.Contains (DomainObjectIDs.OrderItem2), Is.True);

      OrderItem newItem = OrderItem.NewObject();
      order.OrderItems.Add (newItem);

      Assert.That (order.OrderItems.ContainsObject (newItem), Is.True);
    }

    [Test]
    public void GetRelatedObjects_WithAutomaticProperties ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      Assert.That (order.OrderItems, Is.Not.Null);
      Assert.That (order.OrderItems.Count, Is.EqualTo (2));

      Assert.That (order.OrderItems.Contains (DomainObjectIDs.OrderItem1), Is.True);
      Assert.That (order.OrderItems.Contains (DomainObjectIDs.OrderItem2), Is.True);

      OrderItem newItem = OrderItem.NewObject();
      order.OrderItems.Add (newItem);

      Assert.That (order.OrderItems.ContainsObject (newItem), Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?")]
    public void PropertyAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      Dev.Null = order.NotInMapping;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?")]
    public void RelatedAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      Dev.Null = order.NotInMappingRelated;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?")]
    public void RelatedObjectsAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      Dev.Null = order.NotInMappingRelatedObjects;
    }


    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?")]
    public void PropertySetAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      order.NotInMapping = 0;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?")]
    public void RelatedSetAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      order.NotInMappingRelated = null;
    }

    [Test]
    public void DefaultRelatedObject ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      OrderItem item = order.OrderItems[0];
      Assert.That (item.Order, Is.SameAs (order));

      Order newOrder = Order.NewObject();
      Assert.That (newOrder, Is.Not.Null);
      item.Order = newOrder;
      Assert.That (item.Order, Is.Not.SameAs (order));
      Assert.That (item.Order, Is.SameAs (newOrder));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot instantiate type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.AbstractClass' because it is abstract. " 
        + "For classes with automatic properties, InstantiableAttribute must be used.")]
    public void CannotInstantiateReallyAbstractClass ()
    {
      AbstractClass.NewObject();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?")]
    public void ExplicitInterfaceProperty ()
    {
      IPropertyInterface domainObject = ClassWithExplicitInterfaceProperty.NewObject();
      domainObject.Property = 5;
      Assert.That (domainObject.Property, Is.EqualTo (5));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?")]
    public void CurrentPropertyThrowsWhenNotInitializes ()
    {
      Order order = Order.NewObject();
      Dev.Null = order.CurrentProperty;
      Assert.Fail ("Expected exception");
    }

    [Test]
    public void PreparePropertyAccessCorrectlySetsCurrentProperty ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      order.PreparePropertyAccess ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber");
      int orderNumber;
      try
      {
        orderNumber = order.CurrentProperty.GetValue<int>();
      }
      finally
      {
        order.PropertyAccessFinished();
      }
      Assert.That (orderNumber, Is.EqualTo (order.OrderNumber));
    }

    [Test]
    public void PreparePropertyAccess_DoesNotThrowsOnInvalidPropertyName ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      order.PreparePropertyAccess ("Bla");
      order.PropertyAccessFinished ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The domain object type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' does not have a mapping property named 'Bla'.")]
    public void CurrentProperty_ThrowsOnInvalidPropertyName ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      order.PreparePropertyAccess ("Bla");
      try
      {
        Dev.Null = order.CurrentProperty;
      }
      finally
      {
        order.PropertyAccessFinished();
      }
    }

    [Test]
    public void AccessingInterceptedProperties_ViaReflection_GetProperty ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      var propertyInfo = ((object) order).GetType ().GetProperty ("OrderNumber");
      Assert.That (propertyInfo, Is.Not.Null);
      Assert.That (propertyInfo.GetValue (order, null), Is.EqualTo (1));
    }

    [Test]
    public void AccessingInterceptedProperties_ViaReflection_GetProperties ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      var propertyInfos = ((object) order).GetType ().GetProperties ();
      var orderNumberProperty = propertyInfos.Where (pi => pi.Name == "OrderNumber") .SingleOrDefault();
      Assert.That (orderNumberProperty, Is.Not.Null);
    }

    private bool WasCreatedByFactory (object o)
    {
      return InterceptedDomainObjectCreator.Instance.Factory.WasCreatedByFactory (o.GetType ());
    }
  }
}
