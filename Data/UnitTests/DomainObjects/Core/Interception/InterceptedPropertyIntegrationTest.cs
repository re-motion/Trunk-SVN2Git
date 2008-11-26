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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception
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
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.IsTrue (WasCreatedByFactory (order));
    }

    [Test]
    public void ConstructionOfSimpleObjectWorks ()
    {
      Order order = Order.NewObject();
      Assert.IsTrue (WasCreatedByFactory (order));

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      Assert.IsNotNull (classWithAllDataTypes);
      Assert.IsTrue (WasCreatedByFactory (classWithAllDataTypes));
    }

    [Test]
    public void ConstructedObjectIsDerived ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      Assert.IsFalse (((object) classWithAllDataTypes).GetType().Equals (typeof (ClassWithAllDataTypes)));
    }

    [Test]
    public void GetPropertyValueWorks ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (1, order.OrderNumber);
      Assert.AreEqual (new DateTime (2005, 01, 01), order.DeliveryDate);
      Assert.AreEqual (1, order.OrderNumber);
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
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      order.OrderNumber = 15;
      Assert.AreEqual (15, order.OrderNumber);

      order.DeliveryDate = new DateTime (2007, 02, 03);
      Assert.AreEqual (new DateTime (2007, 02, 03), order.DeliveryDate);

      Assert.AreEqual (15, order.OrderNumber);
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
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain.NonInstantiableAbstractClass "
        + "as its member Foo (on type NonInstantiableAbstractClass) is abstract (and not an "
        + "automatic property).", MatchType = MessageMatch.Contains)]
    public void AbstractWithMethodCannotBeInstantiated ()
    {
      NonInstantiableAbstractClass.NewObject();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain.NonInstantiableAbstractClassWithProps "
        + "as its member get_Foo (on type NonInstantiableAbstractClassWithProps) is abstract (and not an automatic property).",
        MatchType = MessageMatch.Contains)]
    public void AbstractWithNonAutoPropertiesCannotBeInstantiated ()
    {
      NonInstantiableAbstractClassWithProps.NewObject();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain.NonInstantiableSealedClass as it is sealed.",
        MatchType = MessageMatch.Contains)]
    public void SealedCannotBeInstantiated ()
    {
      NonInstantiableSealedClass.NewObject();
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void NonDomainCannotBeInstantiated ()
    {
      DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.GetConcreteDomainObjectType (typeof (NonInstantiableNonDomainClass));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage =
        "Type Remotion.Data.UnitTests.DomainObjects.TestDomain."
        + "Order does not support the requested constructor with signature (System.String, System.String, System.String, System.Object).")]
    public void WrongConstructorCannotBeInstantiated ()
    {
      Type t = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.GetConcreteDomainObjectType (typeof (Order));
      DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.GetTypesafeConstructorInvoker<DomainObject> (t)
          .With ("foo", "bar", "foobar", (object) null);
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "Thrown in ThrowException()")]
    public void ConstructorThrowIsPropagated ()
    {
      Throws.NewObject();
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage =
        "Type Remotion.Data.UnitTests.DomainObjects.Core.Interception."
        + "TestDomain.ClassWithWrongConstructor does not support the requested constructor with signature ().")]
    public void ConstructorMismatch1 ()
    {
      ClassWithWrongConstructor.NewObject();
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage =
        "Type Remotion.Data.UnitTests.DomainObjects.Core.Interception."
        + "TestDomain.ClassWithWrongConstructor does not support the requested constructor with signature (System.Double).")]
    public void ConstructorMismatch2 ()
    {
      ClassWithWrongConstructor.NewObject (3.0);
    }

    [Test]
    public void GetSetRelatedObjectAndOriginal ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Customer customer = order.Customer;
      Assert.IsNotNull (customer);
      Assert.AreSame (Customer.GetObject (DomainObjectIDs.Customer1), customer);

      Customer newCustomer = Customer.NewObject();
      Assert.IsNotNull (newCustomer);
      order.Customer = newCustomer;
      Assert.AreSame (newCustomer, order.Customer);

      Assert.AreSame (customer, order.OriginalCustomer);
    }

    [Test]
    public void GetSetRelatedObjectAndOriginal_WithNullAndAutomaticProperty ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.IsNotEmpty (order.OrderItems);
      OrderItem orderItem = order.OrderItems[0];

      orderItem.Order = null;
      Assert.IsNull (orderItem.Order);

      Assert.AreSame (order, orderItem.OriginalOrder);
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DomainObjectCollection orderItems = order.OrderItems;
      Assert.IsNotNull (orderItems);
      Assert.AreEqual (2, orderItems.Count);

      Assert.IsTrue (orderItems.Contains (DomainObjectIDs.OrderItem1));
      Assert.IsTrue (orderItems.Contains (DomainObjectIDs.OrderItem2));

      OrderItem newItem = OrderItem.NewObject();
      order.OrderItems.Add (newItem);

      Assert.IsTrue (order.OrderItems.ContainsObject (newItem));
    }

    [Test]
    public void GetRelatedObjects_WithAutomaticProperties ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.IsNotNull (order.OrderItems);
      Assert.AreEqual (2, order.OrderItems.Count);

      Assert.IsTrue (order.OrderItems.Contains (DomainObjectIDs.OrderItem1));
      Assert.IsTrue (order.OrderItems.Contains (DomainObjectIDs.OrderItem2));

      OrderItem newItem = OrderItem.NewObject();
      order.OrderItems.Add (newItem);

      Assert.IsTrue (order.OrderItems.ContainsObject (newItem));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.",
        MatchType = MessageMatch.Contains)]
    public void PropertyAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      Dev.Null = order.NotInMapping;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.",
        MatchType = MessageMatch.Contains)]
    public void RelatedAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      Dev.Null = order.NotInMappingRelated;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.",
        MatchType = MessageMatch.Contains)]
    public void RelatedObjectsAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      Dev.Null = order.NotInMappingRelatedObjects;
    }


    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.",
        MatchType = MessageMatch.Contains)]
    public void PropertySetAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      order.NotInMapping = 0;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.",
        MatchType = MessageMatch.Contains)]
    public void RelatedSetAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      order.NotInMappingRelated = null;
    }

    [Test]
    public void DefaultRelatedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem item = order.OrderItems[0];
      Assert.AreSame (order, item.Order);

      Order newOrder = Order.NewObject();
      Assert.IsNotNull (newOrder);
      item.Order = newOrder;
      Assert.AreNotSame (order, item.Order);
      Assert.AreSame (newOrder, item.Order);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.TestDomain."
        + "AbstractClass as it is abstract; for classes with automatic properties, InstantiableAttribute must be used.\r\nParameter name: baseType")]
    public void CannotInstantiateReallyAbstractClass ()
    {
      AbstractClass.NewObject();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property or it hasn't been properly initialized.",
        MatchType = MessageMatch.Contains)]
    public void ExplicitInterfaceProperty ()
    {
      IPropertyInterface domainObject = ClassWithExplicitInterfaceProperty.NewObject();
      domainObject.Property = 5;
      Assert.AreEqual (5, domainObject.Property);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "no current property", MatchType = MessageMatch.Contains)]
    public void CurrentPropertyThrowsWhenNotInitializes ()
    {
      Order order = Order.NewObject();
      Dev.Null = order.CurrentProperty;
      Assert.Fail ("Expected exception");
    }

    [Test]
    public void PreparePropertyAccessCorrectlySetsCurrentProperty ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
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
      Assert.AreEqual (order.OrderNumber, orderNumber);
    }

    [Test]
    public void PreparePropertyAccess_DoesNotThrowsOnInvalidPropertyName ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.PreparePropertyAccess ("Bla");
      order.PropertyAccessFinished ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object type Remotion.Data.UnitTests.DomainObjects.TestDomain.Order " 
        + "does not have a mapping property named 'Bla'.\r\nParameter name: propertyName")]
    public void CurrentProperty_ThrowsOnInvalidPropertyName ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
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

    private bool WasCreatedByFactory (object o)
    {
      return DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.WasCreatedByFactory (o.GetType ());
    }
  }
}