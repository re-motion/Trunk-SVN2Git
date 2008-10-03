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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class ClientTransactionTest : ClientTransactionBaseTest
  {
    private enum ApplicationDataKey
    {
      Key1 = 0
    }

    private ClientTransactionEventReceiver _eventReceiver;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _eventReceiver = new ClientTransactionEventReceiver (ClientTransactionMock);
    }

    public override void TearDown ()
    {
      base.TearDown ();

      _eventReceiver.Unregister ();
    }

    [Test]
    public void DataContainerMapLookUp ()
    {
      DomainObject domainObject1 = ClientTransactionMock.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);

      DomainObjectCollection domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (domainObject1, domainObjects[0]);
      _eventReceiver.Clear ();

      DomainObject domainObject2 = ClientTransactionMock.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);

      Assert.AreSame (domainObject1, domainObject2);
    }

    [Test]
    public void LoadingOfMultipleSimpleObjects ()
    {
      ObjectID id1 = DomainObjectIDs.ClassWithAllDataTypes1;
      ObjectID id2 = DomainObjectIDs.ClassWithAllDataTypes2;

      DomainObject domainObject1 = ClientTransactionMock.GetObject (id1);
      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);

      DomainObjectCollection domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (domainObject1, domainObjects[0]);
      _eventReceiver.Clear ();

      DomainObject domainObject2 = ClientTransactionMock.GetObject (id2);
      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);

      domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (domainObject2, domainObjects[0]);

      Assert.IsFalse (ReferenceEquals (domainObject1, domainObject2));
    }

    [Test]
    public void GetRelatedObjectForAlreadyLoadedObjects ()
    {
      DomainObject order = ClientTransactionMock.GetObject (DomainObjectIDs.Order1);
      DomainObject orderTicket = ClientTransactionMock.GetObject (DomainObjectIDs.OrderTicket1);

      _eventReceiver.Clear ();

      Assert.AreSame (orderTicket, ClientTransactionMock.GetRelatedObject (
          new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket")));

      Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);

      Assert.AreSame (order, ClientTransactionMock.GetRelatedObject (new RelationEndPointID (orderTicket.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order")));
      Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      DomainObject orderTicket = ClientTransactionMock.GetObject (DomainObjectIDs.OrderTicket1);
      _eventReceiver.Clear ();
      DomainObject order = ClientTransactionMock.GetRelatedObject (new RelationEndPointID (orderTicket.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"));

      Assert.IsNotNull (order);
      Assert.AreEqual (DomainObjectIDs.Order1, order.ID);
      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);

      DomainObjectCollection domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (order, domainObjects[0]);
    }

    [Test]
    public void GetRelatedObjectOverVirtualEndPoint ()
    {
      DomainObject order = ClientTransactionMock.GetObject (DomainObjectIDs.Order1);
      _eventReceiver.Clear ();

      DomainObject orderTicket = ClientTransactionMock.GetRelatedObject (
          new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));

      Assert.IsNotNull (orderTicket);
      Assert.AreEqual (DomainObjectIDs.OrderTicket1, orderTicket.ID);
      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);

      DomainObjectCollection domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (orderTicket, domainObjects[0]);
    }

    [Test]
    public void GetOptionalRelatedObject ()
    {
      ObjectID id = new ObjectID ("ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      DomainObject classWithValidRelation = ClientTransactionMock.GetObject (id);
      _eventReceiver.Clear ();

      Assert.IsNull (ClientTransactionMock.GetRelatedObject (
          new RelationEndPointID (classWithValidRelation.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional")));

      Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);
    }

    [Test]
    public void GetOptionalRelatedObjectOverVirtualEndPoint ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      DomainObject classWithGuidKey = ClientTransactionMock.GetObject (id);
      _eventReceiver.Clear ();

      Assert.IsNull (ClientTransactionMock.GetRelatedObject (
          new RelationEndPointID (classWithGuidKey.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional")));

      Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);
    }

    [Test]
    public void GetOptionalRelatedObjectTwice ()
    {
      ObjectID id = new ObjectID ("ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      ClientTransactionMock clientTransactionMock = new ClientTransactionMock ();

      DomainObject classWithValidRelation = clientTransactionMock.GetObject (id);
      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
      Assert.AreEqual (0, clientTransactionMock.NumberOfCallsToLoadRelatedObject);

      Assert.IsNull (clientTransactionMock.GetRelatedObject (
          new RelationEndPointID (classWithValidRelation.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional")));

      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
      Assert.AreEqual (0, clientTransactionMock.NumberOfCallsToLoadRelatedObject);

      clientTransactionMock.GetRelatedObject (
          new RelationEndPointID (classWithValidRelation.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional"));

      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
      Assert.AreEqual (0, clientTransactionMock.NumberOfCallsToLoadRelatedObject);
    }

    [Test]
    public void GetOptionalRelatedObjectOverVirtualEndPointTwice ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      ClientTransactionMock clientTransactionMock = new ClientTransactionMock ();

      DomainObject classWithGuidKey = clientTransactionMock.GetObject (id);
      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
      Assert.AreEqual (0, clientTransactionMock.NumberOfCallsToLoadRelatedObject);

      Assert.IsNull (clientTransactionMock.GetRelatedObject (
          new RelationEndPointID (classWithGuidKey.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional")));

      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadRelatedObject);

      clientTransactionMock.GetRelatedObject (
          new RelationEndPointID (classWithGuidKey.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional"));

      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadRelatedObject);
    }

    [Test]
    public void GetRelatedObjectWithInheritance ()
    {
      DomainObject expectedCeo = ClientTransactionMock.GetObject (DomainObjectIDs.Ceo6);
      DomainObject partner = ClientTransactionMock.GetObject (DomainObjectIDs.Partner1);

      DomainObject actualCeo = ClientTransactionMock.GetRelatedObject (new RelationEndPointID (partner.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Ceo"));
      Assert.AreSame (expectedCeo, actualCeo);
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      _eventReceiver.Clear ();

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.IsNotNull (orders);
      Assert.AreEqual (typeof (OrderCollection), orders.GetType (), "Type of collection");
      Assert.AreEqual (2, orders.Count);

      DomainObjectCollection domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (2, domainObjects.Count);
    }

    [Test]
    public void GetRelatedObjectsTwice ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      _eventReceiver.Clear ();

      DomainObjectCollection orders1 = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      DomainObjectCollection orders2 = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.IsTrue (ReferenceEquals (orders1, orders2));

      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);
      DomainObjectCollection domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (2, domainObjects.Count);
    }

    [Test]
    public void GetRelatedObjectsWithAlreadyLoadedObject ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _eventReceiver.Clear ();

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.AreSame (order, orders[DomainObjectIDs.Order1]);
      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);
    }

    [Test]
    public void LoadedEventDoesNotFireWithEmptyDomainObjectCollection ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer2);
      _eventReceiver.Clear ();

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (new RelationEndPointID (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.IsNotNull (orders);
      Assert.IsEmpty (orders);
      Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Assert.IsTrue (ReferenceEquals (order, orders[DomainObjectIDs.Order1]));
    }

    [Test]
    public void GetRelatedObjectsAndNavigateBack ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.AreSame (customer, ClientTransactionMock.GetRelatedObject (
          new RelationEndPointID (orders[0].ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer")));
    }

    [Test]
    public void GetRelatedObjectsWithInheritance ()
    {
      DomainObject industrialSector = ClientTransactionMock.GetObject (DomainObjectIDs.IndustrialSector2);
      DomainObject expectedPartner = ClientTransactionMock.GetObject (DomainObjectIDs.Partner2);

      DomainObjectCollection companies = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (industrialSector.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Companies"));

      Assert.AreSame (expectedPartner, companies[DomainObjectIDs.Partner2]);
    }

    [Test]
    [ExpectedException (typeof (DataManagementException))]
    public void SetRelatedObjectWithInvalidType ()
    {
      DomainObject order = ClientTransactionMock.GetObject (DomainObjectIDs.Order1);
      DomainObject customer = ClientTransactionMock.GetObject (DomainObjectIDs.Customer1);

      ClientTransactionMock.SetRelatedObject (new RelationEndPointID (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"), customer);
    }

    [Test]
    [ExpectedException (typeof (DataManagementException))]
    public void SetRelatedObjectWithBaseType ()
    {
      DomainObject person = ClientTransactionMock.GetObject (DomainObjectIDs.Person1);
      DomainObject company = ClientTransactionMock.GetObject (DomainObjectIDs.Company1);

      ClientTransactionMock.SetRelatedObject (new RelationEndPointID (person.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Person.AssociatedPartnerCompany"), company);
    }

    [Test]
    public void GetObjects_UnloadedObjects ()
    {
      ObjectList<DomainObject> objects = ClientTransactionMock.GetObjects<DomainObject> (DomainObjectIDs.Order1, DomainObjectIDs.Order2,
          DomainObjectIDs.OrderItem1);
      object[] expectedObjects = new object[] {Order.GetObject (DomainObjectIDs.Order1), Order.GetObject (DomainObjectIDs.Order2),
          OrderItem.GetObject (DomainObjectIDs.OrderItem1)};
      Assert.That (objects, Is.EqualTo (expectedObjects));
    }

    [Test]
    public void GetObjects_UnloadedObjects_Events ()
    {
      ObjectList<DomainObject> objects = ClientTransactionMock.GetObjects<DomainObject> (DomainObjectIDs.Order1, DomainObjectIDs.Order2,
          DomainObjectIDs.OrderItem1);
      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);
      Assert.That (_eventReceiver.LoadedDomainObjects[0], Is.EqualTo (objects));
    }

    [Test]
    public void GetObjects_LoadedObjects ()
    {
      object[] expectedObjects = new object[] {Order.GetObject (DomainObjectIDs.Order1), Order.GetObject (DomainObjectIDs.Order2),
          OrderItem.GetObject (DomainObjectIDs.OrderItem1)};
      ObjectList<DomainObject> objects = ClientTransactionMock.GetObjects<DomainObject> (DomainObjectIDs.Order1, DomainObjectIDs.Order2,
          DomainObjectIDs.OrderItem1);
      Assert.That (objects, Is.EqualTo (expectedObjects));
    }

    [Test]
    public void GetObjects_LoadedObjects_Events ()
    {
      Order.GetObject (DomainObjectIDs.Order1);
      Order.GetObject (DomainObjectIDs.Order2);
      OrderItem.GetObject (DomainObjectIDs.OrderItem1);

      _eventReceiver.Clear ();

      ClientTransactionMock.GetObjects<DomainObject> (DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1);
      Assert.That (_eventReceiver.LoadedDomainObjects, Is.Empty);
    }

    [Test]
    public void GetObjects_NewObjects ()
    {
      DomainObject[] expectedObjects = new DomainObject[] { Order.NewObject (), OrderItem.NewObject () };
      ObjectList<DomainObject> objects = ClientTransactionMock.GetObjects<DomainObject> (expectedObjects[0].ID, expectedObjects[1].ID);
      Assert.That (objects, Is.EqualTo (expectedObjects));
    }

    [Test]
    public void GetObjects_NewObjects_Events ()
    {
      DomainObject[] expectedObjects = new DomainObject[] { Order.NewObject (), OrderItem.NewObject () };
      _eventReceiver.Clear ();

      ClientTransactionMock.GetObjects<DomainObject> (expectedObjects[0].ID, expectedObjects[1].ID);
      Assert.That (_eventReceiver.LoadedDomainObjects, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (BulkLoadException), ExpectedMessage = "There were errors when loading a bulk of DomainObjects:\r\n"
        + "Object 'Order|33333333-3333-3333-3333-333333333333|System.Guid' could not be found.\r\n")]
    public void GetObjects_NotFound ()
    {
      Guid guid = new Guid("33333333333333333333333333333333");
      ClientTransactionMock.GetObjects<DomainObject> (new ObjectID(typeof (Order), guid));
    }

    [Test]
    public void TryGetObjects_NotFound ()
    {
      Order newObject = Order.NewObject();
      Guid guid = new Guid ("33333333333333333333333333333333");
      ObjectList<Order> objects = ClientTransactionMock.TryGetObjects<Order> (
          DomainObjectIDs.Order1,
          newObject.ID,
          new ObjectID (typeof (Order), guid),
          DomainObjectIDs.Order2);
      DomainObject[] expectedObjects =
          new DomainObject[] { Order.GetObject (DomainObjectIDs.Order1), newObject, Order.GetObject (DomainObjectIDs.Order2) };
      Assert.That (objects, Is.EqualTo (expectedObjects));
    }
    
    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Values of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' "
      + "cannot be added to this collection. Values must be of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem' or derived from "
      + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem'.\r\nParameter name: domainObject")]
    public void GetObjects_InvalidType ()
    {
      ClientTransactionMock.GetObjects<OrderItem> (DomainObjectIDs.Order1);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException),
        ExpectedMessage = "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is already deleted.")]
    public void GetObjects_Deleted ()
    {
      Order.GetObject (DomainObjectIDs.Order1).Delete ();
      ClientTransactionMock.GetObjects<OrderItem> (DomainObjectIDs.Order1);
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException),
        ExpectedMessage = "Object 'ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid' is already discarded.")]
    public void GetObjects_Discarded ()
    {
      SetDatabaseModifyable();
      ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1).Delete ();
      ClientTransactionMock.Commit ();
      ClientTransactionMock.GetObjects<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException),
       ExpectedMessage = "Mandatory relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' of domain object"
        + " 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' cannot be null.")]
    public void CommitWithMandatoryOneToOneRelationNotSet ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      order.OrderTicket = newOrderTicket;

      ClientTransactionMock.Commit ();
    }

    [Test]
    public void CommitWithOptionalOneToOneRelationNotSet ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      employee.Computer = null;

      ClientTransactionMock.Commit ();

      // expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException),
      ExpectedMessage = "Mandatory relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Companies' of domain object"
       + " 'IndustrialSector|8565a077-ea01-4b5d-beaa-293dc484bddc|System.Guid' contains no items.")]
    public void CommitWithMandatoryOneToManyRelationNotSet ()
    {
      IndustrialSector industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector2);
      industrialSector.Companies.Clear ();

      ClientTransactionMock.Commit ();
    }

    [Test]
    public void CommitTwice ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket oldOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      oldOrderTicket.Order = newOrderTicket.Order;
      order.OrderTicket = newOrderTicket;

      ClientTransactionMock.Commit ();

      Assert.IsFalse (ClientTransactionMock.IsDiscarded);

			object orderTimestamp = order.InternalDataContainer.Timestamp;
			object oldOrderTicketTimestamp = oldOrderTicket.InternalDataContainer.Timestamp;
			object newOrderTicketTimestamp = newOrderTicket.InternalDataContainer.Timestamp;

      ClientTransactionMock.Commit ();

      Assert.IsFalse (ClientTransactionMock.IsDiscarded);

			Assert.AreEqual (orderTimestamp, order.InternalDataContainer.Timestamp);
			Assert.AreEqual (oldOrderTicketTimestamp, oldOrderTicket.InternalDataContainer.Timestamp);
			Assert.AreEqual (newOrderTicketTimestamp, newOrderTicket.InternalDataContainer.Timestamp);
    }

    [Test]
    public void CommitTwiceWithChange ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket oldOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
      Order oldOrderOfNewOrderTicket = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

      oldOrderTicket.Order = newOrderTicket.Order;
      order.OrderTicket = newOrderTicket;

      ClientTransactionMock.Commit ();

			object orderTimestamp = order.InternalDataContainer.Timestamp;
			object oldOrderTicketTimestamp = oldOrderTicket.InternalDataContainer.Timestamp;
			object newOrderTicketTimestamp = newOrderTicket.InternalDataContainer.Timestamp;
			object oldOrderOfNewOrderTicketTimestamp = oldOrderOfNewOrderTicket.InternalDataContainer.Timestamp;

      order.OrderTicket = oldOrderTicket;
      oldOrderOfNewOrderTicket.OrderTicket = newOrderTicket;

      ClientTransactionMock.Commit ();

			Assert.AreEqual (orderTimestamp, order.InternalDataContainer.Timestamp);
			Assert.IsFalse (oldOrderTicketTimestamp.Equals (oldOrderTicket.InternalDataContainer.Timestamp));
			Assert.IsFalse (newOrderTicketTimestamp.Equals (newOrderTicket.InternalDataContainer.Timestamp));
			Assert.AreEqual (oldOrderOfNewOrderTicketTimestamp, oldOrderOfNewOrderTicket.InternalDataContainer.Timestamp);
    }

    [Test]
    public void OppositeDomainObjectsTypeAfterCommit ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      customer.Orders.Add (Order.GetObject (DomainObjectIDs.Order2));
      ClientTransactionMock.Commit ();

      DomainObjectCollection originalOrders = customer.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      Assert.AreEqual (typeof (OrderCollection), originalOrders.GetType ());
      Assert.IsTrue (originalOrders.IsReadOnly);

      Assert.AreEqual (customer.Orders.RequiredItemType, originalOrders.RequiredItemType);
    }

    [Test]
    public void RollbackReadOnlyOppositeDomainObjects ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      customer.Orders.Add (Order.GetObject (DomainObjectIDs.Order2));

      customer.Orders.SetIsReadOnly (true);
      ClientTransactionMock.Rollback ();

      Assert.IsTrue (customer.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders").IsReadOnly);
      Assert.IsTrue (customer.Orders.IsReadOnly);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException))]
    public void CommitDeletedObject ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.Delete ();
      ClientTransactionMock.Commit ();

      Computer.GetObject (DomainObjectIDs.Computer1);
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void AccessDeletedObjectAfterCommit ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.Delete ();
      ClientTransactionMock.Commit ();

      string serialNumber = computer.SerialNumber;
    }

    [Test]
    public void GetObjectByNewIndependentTransaction ()
    {
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction();
      using (clientTransaction.EnterDiscardingScope())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);

        Assert.AreSame (clientTransaction, order.InternalDataContainer.ClientTransaction);
        Assert.IsTrue (order.CanBeUsedInTransaction (clientTransaction));
      }
    }

    [Test]
    public void GetDeletedObjectByNewIndependentTransaction ()
    {
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction();
      using (clientTransaction.EnterDiscardingScope())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);

        order.Delete();

        order = Order.GetObject (DomainObjectIDs.Order1, true);
        Assert.AreEqual (StateType.Deleted, order.State);
        Assert.AreSame (clientTransaction, order.InternalDataContainer.ClientTransaction);
        Assert.IsTrue (order.CanBeUsedInTransaction (clientTransaction));
      }
    }

    [Test]
    public void CommitIndependentTransactions ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction();

      Order order1;
      using (clientTransaction1.EnterNonDiscardingScope())
      {
        order1 = Order.GetObject (DomainObjectIDs.Order1);
        order1.OrderNumber = 50;
      }

      Order order2;
      using (clientTransaction2.EnterNonDiscardingScope ())
      {
        order2 = Order.GetObject (DomainObjectIDs.Order2);
        order2.OrderNumber = 60;
      }

      clientTransaction1.Commit ();
      clientTransaction2.Commit ();

      ClientTransaction clientTransaction3 = ClientTransaction.CreateRootTransaction();
      using (clientTransaction3.EnterNonDiscardingScope ())
      {
        Order changedOrder1 = Order.GetObject (DomainObjectIDs.Order1);
        Order changedOrder2 = Order.GetObject (DomainObjectIDs.Order2);

        Assert.IsFalse (ReferenceEquals (order1, changedOrder1));
        Assert.IsFalse (ReferenceEquals (order2, changedOrder2));

        Assert.AreEqual (50, changedOrder1.OrderNumber);
        Assert.AreEqual (60, changedOrder2.OrderNumber);
      }
    }

    [Test]
    public void QueryManager ()
    {
      Assert.IsNotNull (ClientTransactionMock.QueryManager);
      Assert.AreSame (ClientTransactionMock, ClientTransactionMock.QueryManager.ClientTransaction);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void NoAutoInitializationOfCurrent ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
        ClientTransaction transaction = ClientTransactionScope.CurrentTransaction;
      }
    }

    [Test]
    public void HasCurrentTrue ()
    {
      Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
    }

    [Test]
    public void HasCurrentFalseViaNullTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
      }
    }

    [Test]
    public void HasCurrentFalseViaNullScope ()
    {
      ClientTransactionScope.ResetActiveScope ();
      Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
    }

    [Test]
    public void ClientTransactionCurrentIdenticalToScopeCurrentButNullOnEmptyScope ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction ();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction ();

      Assert.AreSame (ClientTransactionScope.CurrentTransaction, ClientTransaction.Current);

      using (clientTransaction1.EnterDiscardingScope ())
      {
        Assert.AreSame (ClientTransactionScope.CurrentTransaction, ClientTransaction.Current);
        using (clientTransaction2.EnterDiscardingScope ())
        {
          Assert.AreSame (ClientTransactionScope.CurrentTransaction, ClientTransaction.Current);
        }
        Assert.AreSame (ClientTransactionScope.CurrentTransaction, ClientTransaction.Current);
      }
      Assert.AreSame (ClientTransactionScope.CurrentTransaction, ClientTransaction.Current);

      using (ClientTransactionScope.EnterNullScope ())
      {
        Assert.IsNull (ClientTransaction.Current);
      }
    }

    [Test]
    public void MandatoryRelationNotSetExceptionForOneToOneRelation ()
    {
      OrderTicket newOrderTicket = OrderTicket.NewObject ();

      try
      {
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.Fail ("MandatoryRelationNotSetException was expected");
      }
      catch (MandatoryRelationNotSetException ex)
      {
        string expectedMessage = string.Format ("Mandatory relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' of domain object '{0}' cannot be null.", newOrderTicket.ID);
        Assert.AreEqual (expectedMessage, ex.Message);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", ex.PropertyName);
        Assert.AreSame (newOrderTicket, ex.DomainObject);
      }
    }

    [Test]
    public void MandatoryRelationNotSetExceptionForOneToManyRelation ()
    {
      IndustrialSector newIndustrialSector = IndustrialSector.NewObject ();

      try
      {
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.Fail ("MandatoryRelationNotSetException was expected");
      }
      catch (MandatoryRelationNotSetException ex)
      {
        string expectedMessage = string.Format ("Mandatory relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Companies' of domain object '{0}' contains no items.", newIndustrialSector.ID);
        Assert.AreEqual (expectedMessage, ex.Message);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Companies", ex.PropertyName);
        Assert.AreSame (newIndustrialSector, ex.DomainObject);
      }
    }

    [Test]
    public void HasChanged ()
    {
      Assert.IsFalse (ClientTransactionScope.CurrentTransaction.HasChanged ());
      Order order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.OrderNumber = order1.OrderNumber + 1;
      Assert.IsTrue (ClientTransactionScope.CurrentTransaction.HasChanged ());
    }

    [Test]
    public void ApplicationData ()
    {
      Assert.IsNotNull (ClientTransactionScope.CurrentTransaction.ApplicationData);
      Assert.IsAssignableFrom (typeof (Dictionary<Enum, object>), ClientTransactionScope.CurrentTransaction.ApplicationData);

      Assert.IsFalse (ClientTransactionScope.CurrentTransaction.ApplicationData.ContainsKey (ApplicationDataKey.Key1));
      ClientTransactionScope.CurrentTransaction.ApplicationData[ApplicationDataKey.Key1] = "TestData";
      Assert.AreEqual ("TestData", ClientTransactionScope.CurrentTransaction.ApplicationData[ApplicationDataKey.Key1]);
      ClientTransactionScope.CurrentTransaction.ApplicationData.Remove (ApplicationDataKey.Key1);
      Assert.IsFalse (ClientTransactionScope.CurrentTransaction.ApplicationData.ContainsKey (ApplicationDataKey.Key1));
    }

    [Test]
    public void ClientTransactionGetObjectIsIndependentOfCurrentTransaction ()
    {
      ClientTransactionMock clientTransactionMock = new ClientTransactionMock ();
      Order order = (Order) clientTransactionMock.GetObject (DomainObjectIDs.Order1);
      Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      Assert.IsTrue (order.CanBeUsedInTransaction (clientTransactionMock));

      using (clientTransactionMock.EnterDiscardingScope ())
      {
        Assert.IsTrue (order.OrderTicket.CanBeUsedInTransaction (clientTransactionMock));
        Assert.IsTrue (order.Official.CanBeUsedInTransaction (clientTransactionMock));
        Assert.IsTrue (order.OrderItems[0].CanBeUsedInTransaction (clientTransactionMock));
      }
    }

    [Test]
    public void ClientTransactionEventsTriggeredInRightTransaction ()
    {
      ClientTransactionMock mock = new ClientTransactionMock();
      int events = 0;
      mock.Committed += delegate { ++events; Assert.AreSame (mock, ClientTransactionScope.CurrentTransaction); };
      mock.Committing += delegate { ++events; Assert.AreSame (mock, ClientTransactionScope.CurrentTransaction); };
      mock.Loaded += delegate { ++events; Assert.AreSame (mock, ClientTransactionScope.CurrentTransaction); };
      mock.RolledBack += delegate { ++events; Assert.AreSame (mock, ClientTransactionScope.CurrentTransaction); };
      mock.RollingBack += delegate { ++events; Assert.AreSame (mock, ClientTransactionScope.CurrentTransaction); };

      Assert.AreEqual (0, events);
      mock.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (1, events); // loaded

      events = 0;
      mock.Commit ();
      Assert.AreEqual (2, events); // committing, committed

      events = 0;
      mock.Rollback ();
      Assert.AreEqual (2, events); // rollingback, rolledback
    }

    [Test]
    public void ReadOnly ()
    {
      ClientTransactionMock clientTransaction = new ClientTransactionMock ();
      Assert.IsFalse (clientTransaction.IsReadOnly);
      clientTransaction.IsReadOnly = true;
      Assert.IsTrue (clientTransaction.IsReadOnly);
    }

    [Test]
    public void DiscardReturnsFalse ()
    {
      Assert.IsFalse (ClientTransactionMock.Discard ());
    }

    [Test]
    public void IsDiscardedReturnsFalse ()
    {
      Assert.IsFalse (ClientTransactionMock.IsDiscarded);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The transaction can no longer be used because it has been discarded.")]
    public void DiscardRendersTransactionUnusable ()
    {
      ClientTransactionMock.Discard ();
      Assert.IsTrue (ClientTransactionMock.IsDiscarded);
      ClientTransactionMock.GetObject (DomainObjectIDs.Order1);
    }

    [Test]
    public void DefaultEnterScope ()
    {
      ClientTransactionScope outerScope = ClientTransactionScope.ActiveScope;
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      using (newTransaction.EnterDiscardingScope ())
      {
        Assert.AreNotSame (outerScope, ClientTransactionScope.ActiveScope);
        Assert.AreSame (newTransaction, ClientTransactionScope.CurrentTransaction);
        Assert.AreEqual (AutoRollbackBehavior.Discard, ClientTransactionScope.ActiveScope.AutoRollbackBehavior);
      }
    }

    [Test]
    public void EnterScopeWithRollbackBehavior ()
    {
      ClientTransactionScope outerScope = ClientTransactionScope.ActiveScope;
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      using (newTransaction.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Assert.AreNotSame (outerScope, ClientTransactionScope.ActiveScope);
        Assert.AreSame (newTransaction, ClientTransactionScope.CurrentTransaction);
        Assert.AreEqual (AutoRollbackBehavior.Rollback, ClientTransactionScope.ActiveScope.AutoRollbackBehavior);
      }

      using (newTransaction.EnterScope (AutoRollbackBehavior.None))
      {
        Assert.AreNotSame (outerScope, ClientTransactionScope.ActiveScope);
        Assert.AreSame (newTransaction, ClientTransactionScope.CurrentTransaction);
        Assert.AreEqual (AutoRollbackBehavior.None, ClientTransactionScope.ActiveScope.AutoRollbackBehavior);
      }
    }

    [Test]
    public void EnterNonDiscardingScope ()
    {
      ClientTransactionScope outerScope = ClientTransactionScope.ActiveScope;
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      using (newTransaction.EnterNonDiscardingScope ())
      {
        Assert.AreNotSame (outerScope, ClientTransactionScope.ActiveScope);
        Assert.AreSame (newTransaction, ClientTransactionScope.CurrentTransaction);
        Assert.AreEqual (AutoRollbackBehavior.None, ClientTransactionScope.ActiveScope.AutoRollbackBehavior);
      }
    }

    [Test]
    public void CopyCollectionEventHandlers ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DomainObjectCollectionChangeEventHandler addedEventHandler = delegate { };
      DomainObjectCollectionChangeEventHandler addingEventHandler = delegate { };
      DomainObjectCollectionChangeEventHandler removedEventHandler = delegate { };
      DomainObjectCollectionChangeEventHandler removingEventHandler = delegate { };

      order.OrderItems.Added += addedEventHandler;
      order.OrderItems.Adding += addingEventHandler;
      order.OrderItems.Removed += removedEventHandler;
      order.OrderItems.Removing += removingEventHandler;

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (order);
        ClientTransaction.Current.CopyCollectionEventHandlers (order, ClientTransactionMock);

        Assert.IsTrue (HasEventHandler (order.OrderItems, "Added", addedEventHandler));
        Assert.IsTrue (HasEventHandler (order.OrderItems, "Adding", addingEventHandler));
        Assert.IsTrue (HasEventHandler (order.OrderItems, "Removed", removedEventHandler));
        Assert.IsTrue (HasEventHandler (order.OrderItems, "Removing", removingEventHandler));
      }
    }

    [Test]
    public void CopyCollectionEventHandlers_DoesNotLoadRelatedObjectsInOriginalTransaction ()
    {
      MockRepository mockRepository = new MockRepository ();
      IClientTransactionListener listenerMock = mockRepository.StrictMock<IClientTransactionListener> ();
      // no calls are expected
      mockRepository.ReplayAll ();

      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int loadedObjectsBefore = ClientTransactionMock.DataManager.DataContainerMap.Count;
      ClientTransactionMock.AddListener (listenerMock);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (order);
        ClientTransaction.Current.CopyCollectionEventHandlers (order, ClientTransactionMock);
      }

      int loadedObjectsAfter = ClientTransactionMock.DataManager.DataContainerMap.Count;
      Assert.That (loadedObjectsAfter, Is.EqualTo (loadedObjectsBefore));

      mockRepository.VerifyAll ();
    }

    [Test]
    [Ignore ("TODO: Optimize CopyCollectionEventHandlers")]
    public void CopyCollectionEventHandlers_DoesNotLoadRelatedObjectsInDestinationTransaction_IfNotRequiredTo ()
    {
      MockRepository mockRepository = new MockRepository ();
      IClientTransactionListener listenerMock = mockRepository.StrictMock<IClientTransactionListener> ();
      // no calls are expected
      mockRepository.ReplayAll ();

      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Dev.Null = order.OrderItems; // load relation in source transaction, but do not attach event handlers

      ClientTransactionMock innerTransaction = new ClientTransactionMock();
      using (innerTransaction.EnterDiscardingScope ())
      {
        innerTransaction.EnlistDomainObject (order);
        innerTransaction.AddListener (listenerMock);
        int loadedObjectsBefore = innerTransaction.DataManager.DataContainerMap.Count;
        innerTransaction.CopyCollectionEventHandlers (order, ClientTransactionMock);
        int loadedObjectsAfter = innerTransaction.DataManager.DataContainerMap.Count;
        Assert.That (loadedObjectsAfter, Is.EqualTo (loadedObjectsBefore));
      }

      mockRepository.VerifyAll ();
    }

    [Test]
    public void CopyCollectionEventHandlers_DoesLoadRelatedObjectsInDestinationTransaction_IfRequiredTo ()
    {
      MockRepository mockRepository = new MockRepository ();
      IClientTransactionListener listenerMock = mockRepository.StrictMock<IClientTransactionListener> ();

      listenerMock.ObjectLoading (DomainObjectIDs.OrderItem1);
      listenerMock.ObjectLoading (DomainObjectIDs.OrderItem2);
      listenerMock.DataContainerMapRegistering (null);
      LastCall.IgnoreArguments ().Repeat.Any();
      listenerMock.RelationEndPointMapRegistering (null);
      LastCall.IgnoreArguments ().Repeat.Any ();
      listenerMock.ObjectsLoaded (null);
      LastCall.IgnoreArguments ();
      listenerMock.ObjectInitializedFromDataContainer (null, null);
      LastCall.IgnoreArguments ().Repeat.Any();

      mockRepository.ReplayAll ();

      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.OrderItems.Added += delegate { };

      ClientTransactionMock innerTransaction = new ClientTransactionMock ();
      using (innerTransaction.EnterDiscardingScope ())
      {
        innerTransaction.EnlistDomainObject (order);
        innerTransaction.AddListener (listenerMock);
        int loadedObjectsBefore = innerTransaction.DataManager.DataContainerMap.Count;
        innerTransaction.CopyCollectionEventHandlers (order, ClientTransactionMock);
        int loadedObjectsAfter = innerTransaction.DataManager.DataContainerMap.Count;
        Assert.That (loadedObjectsAfter, Is.Not.EqualTo (loadedObjectsBefore));
      }

      mockRepository.VerifyAll ();
    }

    [Test]
    public void CopyTransactionEventHandlers ()
    {
      ClientTransactionEventHandler committedHandler = delegate { };
      ClientTransactionEventHandler committingHandler = delegate { };
      ClientTransactionEventHandler loadedHandler = delegate { };
      ClientTransactionEventHandler rolledBackHandler = delegate { };
      ClientTransactionEventHandler rollingBackHandler = delegate { };
      SubTransactionCreatedEventHandler subTransactionCreatedHandler1 = delegate { };
      SubTransactionCreatedEventHandler subTransactionCreatedHandler2 = delegate { };

      ClientTransaction.Current.Committed += committedHandler;
      ClientTransaction.Current.Committing += committingHandler;
      ClientTransaction.Current.Loaded += loadedHandler;
      ClientTransaction.Current.RolledBack += rolledBackHandler;
      ClientTransaction.Current.RollingBack += rollingBackHandler;
      ClientTransaction.Current.SubTransactionCreated += subTransactionCreatedHandler1;
      ClientTransaction.Current.SubTransactionCreated += subTransactionCreatedHandler2;

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.CopyTransactionEventHandlers (ClientTransactionMock);
        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "Committed", committedHandler));
        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "Committing", committingHandler));
        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "Loaded", loadedHandler));
        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "RolledBack", rolledBackHandler));
        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "RollingBack", rollingBackHandler));
        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "SubTransactionCreated", subTransactionCreatedHandler1));
        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "SubTransactionCreated", subTransactionCreatedHandler2));
      }
    }

    [Test]
    public void CopyTransactionEventHandlers_WithNoEventsDoesNotOverwriteOldHandlers ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransactionEventHandler committedHandler = delegate { };
        ClientTransactionEventHandler committingHandler = delegate { };
        ClientTransactionEventHandler loadedHandler = delegate { };
        ClientTransactionEventHandler rolledBackHandler = delegate { };
        ClientTransactionEventHandler rollingBackHandler = delegate { };
        SubTransactionCreatedEventHandler subTransactionCreatedHandler1 = delegate { };
        SubTransactionCreatedEventHandler subTransactionCreatedHandler2 = delegate { };

        ClientTransaction.Current.Committed += committedHandler;
        ClientTransaction.Current.Committing += committingHandler;
        ClientTransaction.Current.Loaded += loadedHandler;
        ClientTransaction.Current.RolledBack += rolledBackHandler;
        ClientTransaction.Current.RollingBack += rollingBackHandler;
        ClientTransaction.Current.SubTransactionCreated += subTransactionCreatedHandler1;
        ClientTransaction.Current.SubTransactionCreated += subTransactionCreatedHandler2;

        ClientTransaction.Current.CopyTransactionEventHandlers (ClientTransactionMock);

        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "Committed", committedHandler));
        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "Committing", committingHandler));
        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "Loaded", loadedHandler));
        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "RolledBack", rolledBackHandler));
        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "RollingBack", rollingBackHandler));
        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "SubTransactionCreated", subTransactionCreatedHandler1));
        Assert.IsTrue (HasEventHandler (ClientTransaction.Current, "SubTransactionCreated", subTransactionCreatedHandler2));
      }
    }

    [Test]
    public void LinqToClientTransaction ()
    {
      Order o1 = Order.GetObject (DomainObjectIDs.Order1);
      Order o2 = Order.GetObject (DomainObjectIDs.Order2);
      Order o3 = Order.GetObject (DomainObjectIDs.Order3);

      var loadedOrders = from o in ClientTransactionMock.GetEnlistedObjects<Order>()
                         select o;
      Assert.That (loadedOrders.ToArray(), Is.EquivalentTo(new[] {o1, o2, o3}));
    }

    [Test]
    public void ToITransaction ()
    {
      ITransaction transaction = ClientTransactionMock.ToITransation();

      Assert.That (((ClientTransactionWrapper) transaction).WrappedInstance, Is.SameAs (ClientTransactionMock));
    }

    private bool HasEventHandler (object instance, string eventName, Delegate handler)
    {
      Delegate eventField = (Delegate) PrivateInvoke.GetNonPublicField (instance, eventName);
      return eventField != null && Array.IndexOf (eventField.GetInvocationList (), handler) != -1;
    }
  }
}
