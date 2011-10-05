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
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
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
    public void ParentTransaction ()
    {
      Assert.That (ClientTransaction.Current.ParentTransaction, Is.Null);
    }

    [Test]
    public void ActiveSubTransaction ()
    {
      Assert.That (ClientTransaction.Current.SubTransaction, Is.Null);
    }

    [Test]
    public void RootTransaction ()
    {
      Assert.That (ClientTransactionMock.RootTransaction, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    public void LeafTransaction ()
    {
      Assert.That (ClientTransactionMock.LeafTransaction, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    public void DataContainerMapLookUp ()
    {
      DomainObject domainObject1 = ClientTransactionMock.GetObject (DomainObjectIDs.ClassWithAllDataTypes1, false);
      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));

      var domainObjects = _eventReceiver.LoadedDomainObjects[0];
      Assert.That (domainObjects.Count, Is.EqualTo (1));
      Assert.That (domainObjects[0], Is.SameAs (domainObject1));
      _eventReceiver.Clear ();

      DomainObject domainObject2 = ClientTransactionMock.GetObject (DomainObjectIDs.ClassWithAllDataTypes1, false);
      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (0));

      Assert.That (domainObject2, Is.SameAs (domainObject1));
    }

    [Test]
    public void LoadingOfMultipleSimpleObjects ()
    {
      ObjectID id1 = DomainObjectIDs.ClassWithAllDataTypes1;
      ObjectID id2 = DomainObjectIDs.ClassWithAllDataTypes2;

      DomainObject domainObject1 = ClientTransactionMock.GetObject (id1, false);
      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));

      var domainObjects = _eventReceiver.LoadedDomainObjects[0];
      Assert.That (domainObjects.Count, Is.EqualTo (1));
      Assert.That (domainObjects[0], Is.SameAs (domainObject1));
      _eventReceiver.Clear ();

      DomainObject domainObject2 = ClientTransactionMock.GetObject (id2, false);
      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));

      domainObjects = _eventReceiver.LoadedDomainObjects[0];
      Assert.That (domainObjects.Count, Is.EqualTo (1));
      Assert.That (domainObjects[0], Is.SameAs (domainObject2));

      Assert.That (ReferenceEquals (domainObject1, domainObject2), Is.False);
    }

    [Test]
    public void GetRelatedObjectForAlreadyLoadedObjects ()
    {
      DomainObject order = ClientTransactionMock.GetObject (DomainObjectIDs.Order1, false);
      DomainObject orderTicket = ClientTransactionMock.GetObject (DomainObjectIDs.OrderTicket1, false);

      _eventReceiver.Clear ();

      Assert.That (
          ClientTransactionMock.GetRelatedObject (
              RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket")),
          Is.SameAs (orderTicket));

      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (0));

      Assert.That (
          ClientTransactionMock.GetRelatedObject (
              RelationEndPointID.Create (orderTicket.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order")),
          Is.SameAs (order));
      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      DomainObject orderTicket = ClientTransactionMock.GetObject (DomainObjectIDs.OrderTicket1, false);
      _eventReceiver.Clear ();
      DomainObject order = ClientTransactionMock.GetRelatedObject (RelationEndPointID.Create(orderTicket.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"));

      Assert.That (order, Is.Not.Null);
      Assert.That (order.ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));

      var domainObjects = _eventReceiver.LoadedDomainObjects[0];
      Assert.That (domainObjects.Count, Is.EqualTo (1));
      Assert.That (domainObjects[0], Is.SameAs (order));
    }

    [Test]
    public void GetRelatedObjectOverVirtualEndPoint ()
    {
      DomainObject order = ClientTransactionMock.GetObject (DomainObjectIDs.Order1, false);
      _eventReceiver.Clear ();

      DomainObject orderTicket = ClientTransactionMock.GetRelatedObject (
          RelationEndPointID.Create(order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));

      Assert.That (orderTicket, Is.Not.Null);
      Assert.That (orderTicket.ID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));

      var domainObjects = _eventReceiver.LoadedDomainObjects[0];
      Assert.That (domainObjects.Count, Is.EqualTo (1));
      Assert.That (domainObjects[0], Is.SameAs (orderTicket));
    }

    [Test]
    public void GetOptionalRelatedObject ()
    {
      var id = new ObjectID ("ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      DomainObject classWithValidRelation = ClientTransactionMock.GetObject (id, false);
      _eventReceiver.Clear ();

      Assert.That (
          ClientTransactionMock.GetRelatedObject (
              RelationEndPointID.Create (
                  classWithValidRelation.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional")),
          Is.Null);

      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetOptionalRelatedObjectOverVirtualEndPoint ()
    {
      var id = new ObjectID ("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      DomainObject classWithGuidKey = ClientTransactionMock.GetObject (id, false);
      _eventReceiver.Clear ();

      Assert.That (
          ClientTransactionMock.GetRelatedObject (
              RelationEndPointID.Create (
                  classWithGuidKey.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional")),
          Is.Null);

      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetOptionalRelatedObjectTwice ()
    {
      var id = new ObjectID ("ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      CountingObjectLoaderDecorator decorator = null;
      var clientTransactionMock = ClientTransactionObjectMother.CreateTransactionWithObjectLoader<ClientTransactionMock> (
          (tx, ps, es) => decorator = new CountingObjectLoaderDecorator (new ObjectLoader (tx, ps, es, MockRepository.GenerateStub<IEagerFetcher>())));

      DomainObject classWithValidRelation = clientTransactionMock.GetObject (id, false);
      Assert.That (decorator.NumberOfCallsToLoadObject, Is.EqualTo (1));
      Assert.That (decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo (0));

      Assert.That (
          clientTransactionMock.GetRelatedObject (
              RelationEndPointID.Create (
                  classWithValidRelation.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional")),
          Is.Null);

      Assert.That (decorator.NumberOfCallsToLoadObject, Is.EqualTo (1));
      Assert.That (decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo (0));

      clientTransactionMock.GetRelatedObject (
          RelationEndPointID.Create(classWithValidRelation.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional"));

      Assert.That (decorator.NumberOfCallsToLoadObject, Is.EqualTo (1));
      Assert.That (decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo (0));
    }

    [Test]
    public void GetOptionalRelatedObjectOverVirtualEndPointTwice ()
    {
      var id = new ObjectID ("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      CountingObjectLoaderDecorator decorator = null;
      var clientTransactionMock = ClientTransactionObjectMother.CreateTransactionWithObjectLoader<ClientTransactionMock> (
          (tx, ps, es) => decorator = new CountingObjectLoaderDecorator (new ObjectLoader (tx, ps, es, MockRepository.GenerateStub<IEagerFetcher> ())));
      
      DomainObject classWithGuidKey = clientTransactionMock.GetObject (id, false);
      Assert.That (decorator.NumberOfCallsToLoadObject, Is.EqualTo (1));
      Assert.That (decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo (0));

      Assert.That (
          clientTransactionMock.GetRelatedObject (
              RelationEndPointID.Create (
                  classWithGuidKey.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional")),
          Is.Null);

      Assert.That (decorator.NumberOfCallsToLoadObject, Is.EqualTo (1));
      Assert.That (decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo (1));

      clientTransactionMock.GetRelatedObject (
          RelationEndPointID.Create(classWithGuidKey.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional"));

      Assert.That (decorator.NumberOfCallsToLoadObject, Is.EqualTo (1));
      Assert.That (decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo (1));
    }

    [Test]
    public void GetRelatedObjectWithInheritance ()
    {
      DomainObject expectedCeo = ClientTransactionMock.GetObject (DomainObjectIDs.Ceo6, false);
      DomainObject partner = ClientTransactionMock.GetObject (DomainObjectIDs.Partner1, false);

      DomainObject actualCeo = ClientTransactionMock.GetRelatedObject (RelationEndPointID.Create(partner.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Ceo"));
      Assert.That (actualCeo, Is.SameAs (expectedCeo));
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      _eventReceiver.Clear ();

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          RelationEndPointID.Create(customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.That (orders, Is.Not.Null);
      Assert.That (orders.GetType(), Is.EqualTo (typeof (OrderCollection)), "Type of collection");
      Assert.That (orders.Count, Is.EqualTo (2));

      var domainObjects = _eventReceiver.LoadedDomainObjects[0];
      Assert.That (domainObjects.Count, Is.EqualTo (2));
    }

    [Test]
    public void GetRelatedObjectsTwice ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      _eventReceiver.Clear ();

      DomainObjectCollection orders1 = ClientTransactionMock.GetRelatedObjects (
          RelationEndPointID.Create(customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      DomainObjectCollection orders2 = ClientTransactionMock.GetRelatedObjects (
          RelationEndPointID.Create(customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.That (ReferenceEquals (orders1, orders2), Is.True);

      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      var domainObjects = _eventReceiver.LoadedDomainObjects[0];
      Assert.That (domainObjects.Count, Is.EqualTo (2));
    }

    [Test]
    public void GetRelatedObjectsWithAlreadyLoadedObject ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _eventReceiver.Clear ();

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          RelationEndPointID.Create(customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.That (orders[DomainObjectIDs.Order1], Is.SameAs (order));
      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
    }

    [Test]
    public void LoadedEventDoesNotFireWithEmptyDomainObjectCollection ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer2);
      _eventReceiver.Clear ();

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (RelationEndPointID.Create(customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.That (orders, Is.Not.Null);
      Assert.IsEmpty (orders);
      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          RelationEndPointID.Create(customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Assert.That (ReferenceEquals (order, orders[DomainObjectIDs.Order1]), Is.True);
    }

    [Test]
    public void GetRelatedObjectsAndNavigateBack ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          RelationEndPointID.Create(customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.That (
          ClientTransactionMock.GetRelatedObject (
              RelationEndPointID.Create (orders[0].ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer")),
          Is.SameAs (customer));
    }

    [Test]
    public void GetRelatedObjectsWithInheritance ()
    {
      DomainObject industrialSector = ClientTransactionMock.GetObject (DomainObjectIDs.IndustrialSector2, false);
      DomainObject expectedPartner = ClientTransactionMock.GetObject (DomainObjectIDs.Partner2, false);

      DomainObjectCollection companies = ClientTransactionMock.GetRelatedObjects (
          RelationEndPointID.Create(industrialSector.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Companies"));

      Assert.That (companies[DomainObjectIDs.Partner2], Is.SameAs (expectedPartner));
    }

    [Test]
    public void GetObjects_UnloadedObjects ()
    {
      DomainObject[] objects = ClientTransactionMock.GetObjects<DomainObject> (
          DomainObjectIDs.Order1, 
          DomainObjectIDs.Order2,
          DomainObjectIDs.OrderItem1);

      var expectedObjects = new object[] {
          Order.GetObject (DomainObjectIDs.Order1), 
          Order.GetObject (DomainObjectIDs.Order2),
          OrderItem.GetObject (DomainObjectIDs.OrderItem1)};
      Assert.That (objects, Is.EqualTo (expectedObjects));
    }

    [Test]
    public void GetObjects_UnloadedObjects_Events ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      DomainObject[] objects = ClientTransactionMock.GetObjects<DomainObject> (
          DomainObjectIDs.Order1, 
          DomainObjectIDs.Order2,
          DomainObjectIDs.OrderItem1);
      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (_eventReceiver.LoadedDomainObjects[0], Is.EqualTo (objects));

      listenerMock.AssertWasCalled (mock => mock.ObjectsLoading (
          Arg.Is (ClientTransactionMock), 
          Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1 })));

      listenerMock.AssertWasCalled (mock => mock.ObjectsLoaded (
          Arg.Is (ClientTransactionMock), 
          Arg<ReadOnlyCollection<DomainObject>>.List.Equal (objects)));
    }

    [Test]
    public void GetObjects_LoadedObjects ()
    {
      var expectedObjects = new object[] {Order.GetObject (DomainObjectIDs.Order1), Order.GetObject (DomainObjectIDs.Order2),
          OrderItem.GetObject (DomainObjectIDs.OrderItem1)};
      DomainObject[] objects = ClientTransactionMock.GetObjects<DomainObject> (DomainObjectIDs.Order1, DomainObjectIDs.Order2,
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

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      ClientTransactionMock.GetObjects<DomainObject> (DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1);
      Assert.That (_eventReceiver.LoadedDomainObjects, Is.Empty);

      listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
    }

    [Test]
    public void GetObjects_NewObjects ()
    {
      var expectedObjects = new DomainObject[] { Order.NewObject (), OrderItem.NewObject () };
      DomainObject[] objects = ClientTransactionMock.GetObjects<DomainObject> (expectedObjects[0].ID, expectedObjects[1].ID);
      Assert.That (objects, Is.EqualTo (expectedObjects));
    }

    [Test]
    public void GetObjects_NewObjects_Events ()
    {
      var expectedObjects = new DomainObject[] { Order.NewObject (), OrderItem.NewObject () };
      _eventReceiver.Clear ();

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      ClientTransactionMock.GetObjects<DomainObject> (expectedObjects[0].ID, expectedObjects[1].ID);
      Assert.That (_eventReceiver.LoadedDomainObjects, Is.Empty);

      listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
    }

    [Test]
    [ExpectedException (typeof (BulkLoadException), ExpectedMessage = "There were errors when loading a bulk of DomainObjects:\r\n"
        + "Object 'Order|33333333-3333-3333-3333-333333333333|System.Guid' could not be found.\r\n")]
    public void GetObjects_NotFound ()
    {
      var guid = new Guid("33333333333333333333333333333333");
      ClientTransactionMock.GetObjects<DomainObject> (new ObjectID(typeof (Order), guid));
    }

    [Test]
    public void TryGetObjects_NotFound ()
    {
      Order newObject = Order.NewObject();
      var guid = new Guid ("33333333333333333333333333333333");
      Order[] objects = ClientTransactionMock.TryGetObjects<Order> (
          DomainObjectIDs.Order1,
          newObject.ID,
          new ObjectID (typeof (Order), guid),
          DomainObjectIDs.Order2);
      var expectedObjects = new DomainObject[] { 
          Order.GetObject (DomainObjectIDs.Order1), 
          newObject, 
          null,
          Order.GetObject (DomainObjectIDs.Order2) };
      Assert.That (objects, Is.EqualTo (expectedObjects));
    }
    
    [Test]
    [ExpectedException (typeof (InvalidCastException))]
    public void GetObjects_InvalidType ()
    {
      ClientTransactionMock.GetObjects<OrderItem> (DomainObjectIDs.Order1);
    }

    [Test]
    public void GetObjects_Deleted ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      order.Delete ();

      var result = ClientTransactionMock.GetObjects<Order> (DomainObjectIDs.Order1);

      Assert.That (result[0], Is.SameAs (order));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException),
        ExpectedMessage = "Object 'ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid' is invalid in this transaction.")]
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

      Assert.That (ClientTransactionMock.IsDiscarded, Is.False);

      object orderTimestamp = order.InternalDataContainer.Timestamp;
      object oldOrderTicketTimestamp = oldOrderTicket.InternalDataContainer.Timestamp;
      object newOrderTicketTimestamp = newOrderTicket.InternalDataContainer.Timestamp;

      ClientTransactionMock.Commit ();

      Assert.That (ClientTransactionMock.IsDiscarded, Is.False);

      Assert.That (order.InternalDataContainer.Timestamp, Is.EqualTo (orderTimestamp));
      Assert.That (oldOrderTicket.InternalDataContainer.Timestamp, Is.EqualTo (oldOrderTicketTimestamp));
      Assert.That (newOrderTicket.InternalDataContainer.Timestamp, Is.EqualTo (newOrderTicketTimestamp));
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

      Assert.That (order.InternalDataContainer.Timestamp, Is.EqualTo (orderTimestamp));
      Assert.That (oldOrderTicketTimestamp.Equals (oldOrderTicket.InternalDataContainer.Timestamp), Is.False);
      Assert.That (newOrderTicketTimestamp.Equals (newOrderTicket.InternalDataContainer.Timestamp), Is.False);
      Assert.That (oldOrderOfNewOrderTicket.InternalDataContainer.Timestamp, Is.EqualTo (oldOrderOfNewOrderTicketTimestamp));
    }

    [Test]
    public void OppositeDomainObjectsTypeAfterCommit ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      customer.Orders.Add (Order.GetObject (DomainObjectIDs.Order2));
      ClientTransactionMock.Commit ();

      DomainObjectCollection originalOrders = customer.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      Assert.That (originalOrders.GetType(), Is.EqualTo (typeof (OrderCollection)));
      Assert.That (originalOrders.IsReadOnly, Is.True);

      Assert.That (originalOrders.RequiredItemType, Is.Null);
    }

    [Test]
    public void RollbackReadOnlyOppositeDomainObjects ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      customer.Orders.Add (Order.GetObject (DomainObjectIDs.Order2));

      DomainObjectCollectionDataTestHelper.MakeCollectionReadOnly (customer.Orders);
      ClientTransactionMock.Rollback ();

      Assert.That (customer.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders").IsReadOnly, Is.True);
      Assert.That (customer.Orders.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void CommitDeletedObject ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.Delete ();
      ClientTransactionMock.Commit ();

      Computer.GetObject (DomainObjectIDs.Computer1);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void AccessDeletedObjectAfterCommit ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.Delete ();
      ClientTransactionMock.Commit ();

      Dev.Null = computer.SerialNumber;
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void AccessDeleteObjectBeforeCommit ()
    {
      Client client = Client.GetObject (DomainObjectIDs.Client1);
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      Assert.That (location.Client, Is.SameAs (client));
      client.Delete ();
      Dev.Null = location.Client;
    }

    [Test]
    public void GetObjectByNewIndependentTransaction ()
    {
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction();
      using (clientTransaction.EnterDiscardingScope())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);

        Assert.That (order.InternalDataContainer.ClientTransaction, Is.SameAs (clientTransaction));
        Assert.That (clientTransaction.IsEnlisted (order), Is.True);
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
        Assert.That (order.State, Is.EqualTo (StateType.Deleted));
        Assert.That (order.InternalDataContainer.ClientTransaction, Is.SameAs (clientTransaction));
        Assert.That (clientTransaction.IsEnlisted (order), Is.True);
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

        Assert.That (ReferenceEquals (order1, changedOrder1), Is.False);
        Assert.That (ReferenceEquals (order2, changedOrder2), Is.False);

        Assert.That (changedOrder1.OrderNumber, Is.EqualTo (50));
        Assert.That (changedOrder2.OrderNumber, Is.EqualTo (60));
      }
    }

    [Test]
    public void QueryManager ()
    {
      Assert.That (ClientTransactionMock.QueryManager, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void NoAutoInitializationOfCurrent ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
        Dev.Null = ClientTransactionScope.CurrentTransaction;
      }
    }

    [Test]
    public void HasCurrentTrue ()
    {
      Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.True);
    }

    [Test]
    public void HasCurrentFalseViaNullTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
      }
    }

    [Test]
    public void HasCurrentFalseViaNullScope ()
    {
      ClientTransactionScope.ResetActiveScope ();
      Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
    }

    [Test]
    public void ClientTransactionCurrentIdenticalToScopeCurrentButNullOnEmptyScope ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction ();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction ();

      Assert.That (ClientTransaction.Current, Is.SameAs (ClientTransactionScope.CurrentTransaction));

      using (clientTransaction1.EnterDiscardingScope ())
      {
        Assert.That (ClientTransaction.Current, Is.SameAs (ClientTransactionScope.CurrentTransaction));
        using (clientTransaction2.EnterDiscardingScope ())
        {
          Assert.That (ClientTransaction.Current, Is.SameAs (ClientTransactionScope.CurrentTransaction));
        }
        Assert.That (ClientTransaction.Current, Is.SameAs (ClientTransactionScope.CurrentTransaction));
      }
      Assert.That (ClientTransaction.Current, Is.SameAs (ClientTransactionScope.CurrentTransaction));

      using (ClientTransactionScope.EnterNullScope ())
      {
        Assert.That (ClientTransaction.Current, Is.Null);
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
        Assert.That (ex.Message, Is.EqualTo (expectedMessage));
        Assert.That (ex.PropertyName, Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"));
        Assert.That (ex.DomainObject, Is.SameAs (newOrderTicket));
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
        Assert.That (ex.Message, Is.EqualTo (expectedMessage));
        Assert.That (ex.PropertyName, Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Companies"));
        Assert.That (ex.DomainObject, Is.SameAs (newIndustrialSector));
      }
    }

    [Test]
    public void HasChanged ()
    {
      Assert.That (ClientTransactionScope.CurrentTransaction.HasChanged(), Is.False);
      Order order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.OrderNumber = order1.OrderNumber + 1;
      Assert.That (ClientTransactionScope.CurrentTransaction.HasChanged(), Is.True);
    }

    [Test]
    public void ApplicationData ()
    {
      Assert.That (ClientTransactionScope.CurrentTransaction.ApplicationData, Is.Not.Null);
      Assert.IsAssignableFrom (typeof (Dictionary<Enum, object>), ClientTransactionScope.CurrentTransaction.ApplicationData);

      Assert.That (ClientTransactionScope.CurrentTransaction.ApplicationData.ContainsKey (ApplicationDataKey.Key1), Is.False);
      ClientTransactionScope.CurrentTransaction.ApplicationData[ApplicationDataKey.Key1] = "TestData";
      Assert.That (ClientTransactionScope.CurrentTransaction.ApplicationData[ApplicationDataKey.Key1], Is.EqualTo ("TestData"));
      ClientTransactionScope.CurrentTransaction.ApplicationData.Remove (ApplicationDataKey.Key1);
      Assert.That (ClientTransactionScope.CurrentTransaction.ApplicationData.ContainsKey (ApplicationDataKey.Key1), Is.False);
    }

    [Test]
    public void ClientTransactionGetObjectIsIndependentOfCurrentTransaction ()
    {
      var clientTransactionMock = new ClientTransactionMock ();
      var order = (Order) clientTransactionMock.GetObject (DomainObjectIDs.Order1, false);
      Assert.That (ClientTransactionScope.CurrentTransaction.IsEnlisted (order), Is.False);
      Assert.That (clientTransactionMock.IsEnlisted (order), Is.True);

      using (clientTransactionMock.EnterDiscardingScope ())
      {
        Assert.That (clientTransactionMock.IsEnlisted (order.OrderTicket), Is.True);
        Assert.That (clientTransactionMock.IsEnlisted (order.Official), Is.True);
        Assert.That (clientTransactionMock.IsEnlisted (order.OrderItems[0]), Is.True);
      }
    }

    [Test]
    public void ClientTransactionEventsTriggeredInRightTransaction ()
    {
      var mock = new ClientTransactionMock();
      int events = 0;
// ReSharper disable AccessToModifiedClosure
      mock.Committed += delegate { ++events;
                                   Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (mock));
      };
      mock.Committing += delegate { ++events;
                                    Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (mock));
      };
      mock.Loaded += delegate { ++events;
                                Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (mock));
      };
      mock.RolledBack += delegate { ++events;
                                    Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (mock));
      };
      mock.RollingBack += delegate { ++events;
                                     Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (mock));
      };
// ReSharper restore AccessToModifiedClosure

      Assert.That (events, Is.EqualTo (0));
      mock.GetObject (DomainObjectIDs.Order1, false);
      Assert.That (events, Is.EqualTo (1)); // loaded

      events = 0;
      mock.Commit ();
      Assert.That (events, Is.EqualTo (2)); // committing, committed

      events = 0;
      mock.Rollback ();
      Assert.That (events, Is.EqualTo (2)); // rollingback, rolledback
    }

    [Test]
    public void ReadOnly ()
    {
      var clientTransaction = new ClientTransactionMock ();
      Assert.That (clientTransaction.IsReadOnly, Is.False);
      clientTransaction.IsReadOnly = true;
      Assert.That (clientTransaction.IsReadOnly, Is.True);
    }

    [Test]
    public void IsDiscardedReturnsFalse ()
    {
      Assert.That (ClientTransactionMock.IsDiscarded, Is.False);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The transaction can no longer be used because it has been discarded.")]
    public void DiscardRendersTransactionUnusable ()
    {
      ClientTransactionMock.Discard ();
      Assert.That (ClientTransactionMock.IsDiscarded, Is.True);
      ClientTransactionMock.GetObject (DomainObjectIDs.Order1, false);
    }

    [Test]
    public void DefaultEnterScope ()
    {
      ClientTransactionScope outerScope = ClientTransactionScope.ActiveScope;
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      using (newTransaction.EnterDiscardingScope ())
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.Not.SameAs (outerScope));
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (newTransaction));
        Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.Discard));
      }
    }

    [Test]
    public void EnterScopeWithRollbackBehavior ()
    {
      ClientTransactionScope outerScope = ClientTransactionScope.ActiveScope;
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      using (newTransaction.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.Not.SameAs (outerScope));
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (newTransaction));
        Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.Rollback));
      }

      using (newTransaction.EnterScope (AutoRollbackBehavior.None))
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.Not.SameAs (outerScope));
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (newTransaction));
        Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.None));
      }
    }

    [Test]
    public void EnterNonDiscardingScope ()
    {
      ClientTransactionScope outerScope = ClientTransactionScope.ActiveScope;
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      using (newTransaction.EnterNonDiscardingScope ())
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.Not.SameAs (outerScope));
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (newTransaction));
        Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.None));
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

        Assert.That (HasEventHandler (order.OrderItems, "Added", addedEventHandler), Is.True);
        Assert.That (HasEventHandler (order.OrderItems, "Adding", addingEventHandler), Is.True);
        Assert.That (HasEventHandler (order.OrderItems, "Removed", removedEventHandler), Is.True);
        Assert.That (HasEventHandler (order.OrderItems, "Removing", removingEventHandler), Is.True);
      }
    }

    [Test]
    public void CopyCollectionEventHandlers_DoesNotLoadRelatedObjectsInOriginalTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int loadedObjectsBefore = ClientTransactionMock.DataManager.DataContainers.Count;

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (order);
        ClientTransaction.Current.CopyCollectionEventHandlers (order, ClientTransactionMock);
      }

      int loadedObjectsAfter = ClientTransactionMock.DataManager.DataContainers.Count;
      Assert.That (loadedObjectsAfter, Is.EqualTo (loadedObjectsBefore));
    }

    [Test]
    [Ignore ("TODO: Optimize CopyCollectionEventHandlers")]
    public void CopyCollectionEventHandlers_DoesNotLoadRelatedObjectsInDestinationTransaction_IfNotRequiredTo ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Dev.Null = order.OrderItems; // load relation in source transaction, but do not attach event handlers

      var innerTransaction = new ClientTransactionMock();
      using (innerTransaction.EnterDiscardingScope ())
      {
        innerTransaction.EnlistDomainObject (order);

        ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (innerTransaction);
        
        int loadedObjectsBefore = innerTransaction.DataManager.DataContainers.Count;
        innerTransaction.CopyCollectionEventHandlers (order, ClientTransactionMock);
        int loadedObjectsAfter = innerTransaction.DataManager.DataContainers.Count;
        Assert.That (loadedObjectsAfter, Is.EqualTo (loadedObjectsBefore));
      }
    }

    [Test]
    public void CopyCollectionEventHandlers_DoesLoadRelatedObjectsInDestinationTransaction_IfRequiredTo ()
    {
      var mockRepository = new MockRepository ();
      var listenerMock = mockRepository.StrictMock<IClientTransactionListener> ();

      var innerTransaction = new ClientTransactionMock ();

      listenerMock.Stub (stub => stub.TransactionDiscard (innerTransaction));

      listenerMock.ObjectsLoading (
          Arg.Is (innerTransaction), 
          Arg<ReadOnlyCollection<ObjectID>>.List.ContainsAll (new[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 }));
      listenerMock.DataContainerMapRegistering (null, null);
      LastCall.IgnoreArguments ().Repeat.Any();
      listenerMock.RelationEndPointMapRegistering (null, null);
      LastCall.IgnoreArguments ().Repeat.Any ();
      listenerMock.ObjectsLoaded (null, null);
      LastCall.IgnoreArguments ();
      listenerMock.VirtualRelationEndPointStateUpdated (null, null, null);
      LastCall.IgnoreArguments ().Repeat.Any ();

      mockRepository.ReplayAll ();

      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.OrderItems.Added += delegate { };

      using (innerTransaction.EnterDiscardingScope ())
      {
        innerTransaction.EnlistDomainObject (order);
        order.EnsureDataAvailable (); // preload order, but not orderItems

        innerTransaction.AddListener (listenerMock);
        int loadedObjectsBefore = innerTransaction.DataManager.DataContainers.Count;
        innerTransaction.CopyCollectionEventHandlers (order, ClientTransactionMock);
        int loadedObjectsAfter = innerTransaction.DataManager.DataContainers.Count;
        Assert.That (loadedObjectsAfter, Is.Not.EqualTo (loadedObjectsBefore));
      }

      mockRepository.VerifyAll ();
    }

    [Test]
    public void CopyTransactionEventHandlers ()
    {
      EventHandler<ClientTransactionEventArgs> committedHandler = delegate { };
      EventHandler<ClientTransactionEventArgs> committingHandler = delegate { };
      EventHandler<ClientTransactionEventArgs> loadedHandler = delegate { };
      EventHandler<ClientTransactionEventArgs> rolledBackHandler = delegate { };
      EventHandler<ClientTransactionEventArgs> rollingBackHandler = delegate { };
      EventHandler<SubTransactionCreatedEventArgs> subTransactionCreatedHandler1 = delegate { };
      EventHandler<SubTransactionCreatedEventArgs> subTransactionCreatedHandler2 = delegate { };

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
        Assert.That (HasEventHandler (ClientTransaction.Current, "Committed", committedHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "Committing", committingHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "Loaded", loadedHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "RolledBack", rolledBackHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "RollingBack", rollingBackHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "SubTransactionCreated", subTransactionCreatedHandler1), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "SubTransactionCreated", subTransactionCreatedHandler2), Is.True);
      }
    }

    [Test]
    public void CopyTransactionEventHandlers_WithNoEventsDoesNotOverwriteOldHandlers ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        EventHandler<ClientTransactionEventArgs> committedHandler = delegate { };
        EventHandler<ClientTransactionEventArgs> committingHandler = delegate { };
        EventHandler<ClientTransactionEventArgs> loadedHandler = delegate { };
        EventHandler<ClientTransactionEventArgs> rolledBackHandler = delegate { };
        EventHandler<ClientTransactionEventArgs> rollingBackHandler = delegate { };
        EventHandler<SubTransactionCreatedEventArgs> subTransactionCreatedHandler1 = delegate { };
        EventHandler<SubTransactionCreatedEventArgs> subTransactionCreatedHandler2 = delegate { };

        ClientTransaction.Current.Committed += committedHandler;
        ClientTransaction.Current.Committing += committingHandler;
        ClientTransaction.Current.Loaded += loadedHandler;
        ClientTransaction.Current.RolledBack += rolledBackHandler;
        ClientTransaction.Current.RollingBack += rollingBackHandler;
        ClientTransaction.Current.SubTransactionCreated += subTransactionCreatedHandler1;
        ClientTransaction.Current.SubTransactionCreated += subTransactionCreatedHandler2;

        ClientTransaction.Current.CopyTransactionEventHandlers (ClientTransactionMock);

        Assert.That (HasEventHandler (ClientTransaction.Current, "Committed", committedHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "Committing", committingHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "Loaded", loadedHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "RolledBack", rolledBackHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "RollingBack", rollingBackHandler), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "SubTransactionCreated", subTransactionCreatedHandler1), Is.True);
        Assert.That (HasEventHandler (ClientTransaction.Current, "SubTransactionCreated", subTransactionCreatedHandler2), Is.True);
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

    [Test]
    public void ToITransaction_Override ()
    {
      var wrapperStub = MockRepository.GenerateMock<ITransaction> ();
      var transaction = new ClientTransactionWithCustomITransaction (wrapperStub);
      Assert.That (transaction.ToITransation (), Is.SameAs (wrapperStub));
    }

    private bool HasEventHandler (object instance, string eventName, Delegate handler)
    {
      var eventField = (Delegate) PrivateInvoke.GetNonPublicField (instance, eventName);
      return eventField != null && Array.IndexOf (eventField.GetInvocationList (), handler) != -1;
    }
  }
}
