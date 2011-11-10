// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class CLientTransactionGetRelationsTest : ClientTransactionBaseTest
  {
    private ClientTransactionEventReceiver _eventReceiver;

    public override void SetUp ()
    {
      base.SetUp ();

      _eventReceiver = new ClientTransactionEventReceiver (ClientTransactionMock);
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
      DomainObject order = ClientTransactionMock.GetRelatedObject (RelationEndPointID.Create (orderTicket.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"));

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
          RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));

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
      var clientTransaction =
          ClientTransactionObjectMother.CreateTransactionWithObjectLoaderDecorator<ClientTransactionMock> (
              loader => decorator ?? (decorator = new CountingObjectLoaderDecorator (loader)));

      DomainObject classWithValidRelation = clientTransaction.GetObject (id, false);
      Assert.That (decorator.NumberOfCallsToLoadObject, Is.EqualTo (1));
      Assert.That (decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo (0));

      Assert.That (
          clientTransaction.GetRelatedObject (
              RelationEndPointID.Create (
                  classWithValidRelation.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional")),
          Is.Null);

      Assert.That (decorator.NumberOfCallsToLoadObject, Is.EqualTo (1));
      Assert.That (decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo (0));

      clientTransaction.GetRelatedObject (
          RelationEndPointID.Create (classWithValidRelation.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional"));

      Assert.That (decorator.NumberOfCallsToLoadObject, Is.EqualTo (1));
      Assert.That (decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo (0));
    }

    [Test]
    public void GetOptionalRelatedObjectOverVirtualEndPointTwice ()
    {
      var id = new ObjectID ("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      CountingObjectLoaderDecorator decorator = null;
      var clientTransactionMock = ClientTransactionObjectMother.CreateTransactionWithObjectLoaderDecorator<ClientTransactionMock> (
        loader => decorator ?? (decorator = new CountingObjectLoaderDecorator (loader)));

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
          RelationEndPointID.Create (classWithGuidKey.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional"));

      Assert.That (decorator.NumberOfCallsToLoadObject, Is.EqualTo (1));
      Assert.That (decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo (1));
    }

    [Test]
    public void GetRelatedObjectWithInheritance ()
    {
      DomainObject expectedCeo = ClientTransactionMock.GetObject (DomainObjectIDs.Ceo6, false);
      DomainObject partner = ClientTransactionMock.GetObject (DomainObjectIDs.Partner1, false);

      DomainObject actualCeo = ClientTransactionMock.GetRelatedObject (RelationEndPointID.Create (partner.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Ceo"));
      Assert.That (actualCeo, Is.SameAs (expectedCeo));
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      _eventReceiver.Clear ();

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          RelationEndPointID.Create (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.That (orders, Is.Not.Null);
      Assert.That (orders.GetType (), Is.EqualTo (typeof (OrderCollection)), "Type of collection");
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
          RelationEndPointID.Create (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      DomainObjectCollection orders2 = ClientTransactionMock.GetRelatedObjects (
          RelationEndPointID.Create (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

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
          RelationEndPointID.Create (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.That (orders[DomainObjectIDs.Order1], Is.SameAs (order));
      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
    }

    [Test]
    public void LoadedEventDoesNotFireWithEmptyDomainObjectCollection ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer2);
      _eventReceiver.Clear ();

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (RelationEndPointID.Create (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Assert.That (orders, Is.Not.Null);
      Assert.IsEmpty (orders);
      Assert.That (_eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          RelationEndPointID.Create (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Assert.That (ReferenceEquals (order, orders[DomainObjectIDs.Order1]), Is.True);
    }

    [Test]
    public void GetRelatedObjectsAndNavigateBack ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          RelationEndPointID.Create (customer.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));

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
          RelationEndPointID.Create (industrialSector.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Companies"));

      Assert.That (companies[DomainObjectIDs.Partner2], Is.SameAs (expectedPartner));
    }
  }
}