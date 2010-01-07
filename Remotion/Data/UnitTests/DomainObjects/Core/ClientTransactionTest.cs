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
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class ClientTransactionTest : ClientTransactionBaseTest
  {
    [Test]
    public void GetObjectForDataContainer_EnlistedObject ()
    {
      var creator = (InterceptedDomainObjectCreator) MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)).GetDomainObjectCreator ();
      var orderType = creator.Factory.GetConcreteDomainObjectType (typeof (Order));

      var enlisted = (DomainObject) FormatterServices.GetSafeUninitializedObject (orderType);
      enlisted.Initialize (DomainObjectIDs.Order1, ClientTransactionMock);
      ClientTransactionMock.EnlistDomainObject (enlisted);

      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      dataContainer.RegisterWithTransaction (ClientTransactionMock);

      var retrieved = ClientTransactionMock.GetObjectForDataContainer (dataContainer);
      Assert.That (retrieved, Is.SameAs (enlisted));
    }

    [Test]
    public void GetObjectForDataContainer_NoEnlistedObject_CreatesNew ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      dataContainer.RegisterWithTransaction (ClientTransactionMock);

      var retrieved = ClientTransactionMock.GetObjectForDataContainer (dataContainer);

      Assert.That (retrieved, Is.InstanceOfType (typeof (Order)));
      Assert.That (retrieved.ID, Is.EqualTo (dataContainer.ID));
    }

    [Test]
    public void GetObjectForDataContainer_NoEnlistedObject_UsesCreator ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      dataContainer.RegisterWithTransaction (ClientTransactionMock);

      var retrieved = ClientTransactionMock.GetObjectForDataContainer (dataContainer);

      var expectedCreator = (InterceptedDomainObjectCreator) MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)).GetDomainObjectCreator ();
      expectedCreator.Factory.WasCreatedByFactory (((object) retrieved).GetType ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The data container must be registered with the ClientTransaction before an object is retrieved for it.")]
    public void GetObjectForDataContainer_NonRegisteredObject ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      ClientTransactionMock.GetObjectForDataContainer (dataContainer);
    }

    [Test]
    public void EnsureDataAvailable_AlreadyLoaded ()
    {
      var domainObject = Order.GetObject (DomainObjectIDs.Order1);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener>();
      ClientTransactionMock.AddListener (listenerMock);

      ClientTransactionMock.EnsureDataAvailable (domainObject);

      listenerMock.AssertWasNotCalled (mock => mock.ObjectLoading (Arg<ObjectID>.Is.Anything));
    }

    [Test]
    public void EnsureDataAvailable_NotLoadedYet ()
    {
      var domainObject = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);

      ClientTransactionMock.EnlistDomainObject (domainObject);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[domainObject.ID], Is.Null);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener>();
      ClientTransactionMock.AddListener (listenerMock);

      ClientTransactionMock.EnsureDataAvailable (domainObject);

      listenerMock.AssertWasCalled (mock => mock.ObjectLoading (DomainObjectIDs.Order1));
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[domainObject.ID], Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void EnsureDataAvailable_Discarded ()
    {
      Order domainObject = Order.NewObject ();
      domainObject.Delete ();

      ClientTransactionMock.EnsureDataAvailable (domainObject);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException))]
    public void EnsureDataAvailable_NotFound ()
    {
      var domainObject = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();

      ClientTransactionMock.EnlistDomainObject (domainObject);
      ClientTransactionMock.EnsureDataAvailable (domainObject);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void EnsureDataAvailable_NotEnlisted ()
    {
      var domainObject = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);

      ClientTransactionMock.EnsureDataAvailable (domainObject);
    }

    [Test]
    public void EnsureDataAvailable_Many_AlreadyLoaded ()
    {
      var domainObject1 = Order.GetObject (DomainObjectIDs.Order1);
      var domainObject2 = Order.GetObject (DomainObjectIDs.Order2);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      ClientTransactionMock.EnsureDataAvailable (new[] { domainObject1, domainObject2 });

      listenerMock.AssertWasNotCalled (mock => mock.ObjectLoading (Arg<ObjectID>.Is.Anything));
    }

    [Test]
    public void EnsureDataAvailable_Many_NotLoadedYet ()
    {
      var domainObject1 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      var domainObject2 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order2);

      ClientTransactionMock.EnlistDomainObject (domainObject1);
      ClientTransactionMock.EnlistDomainObject (domainObject2);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      ClientTransactionMock.EnsureDataAvailable (new[] { domainObject1, domainObject2 });

      listenerMock.AssertWasCalled (mock => mock.ObjectLoading (DomainObjectIDs.Order1));
      listenerMock.AssertWasCalled (mock => mock.ObjectLoading (DomainObjectIDs.Order2));
    }

    [Test]
    public void EnsureDataAvailable_Many_SomeLoadedSomeNot ()
    {
      var domainObject1 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      var domainObject2 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order2);
      var domainObject3 = Order.GetObject (DomainObjectIDs.Order3);

      ClientTransactionMock.EnlistDomainObject (domainObject1);
      ClientTransactionMock.EnlistDomainObject (domainObject2);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      ClientTransactionMock.EnsureDataAvailable (new[] { domainObject1, domainObject2, domainObject3 });

      listenerMock.AssertWasCalled (mock => mock.ObjectLoading (DomainObjectIDs.Order1));
      listenerMock.AssertWasCalled (mock => mock.ObjectLoading (DomainObjectIDs.Order2));
      listenerMock.AssertWasNotCalled (mock => mock.ObjectLoading (DomainObjectIDs.Order3));
    }

    [Test]
    public void EnsureDataAvailable_Many_SomeLoadedSomeNot_PerformsBulkLoad ()
    {
      var domainObject1 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      var domainObject2 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order2);
      var domainObject3 = Order.GetObject (DomainObjectIDs.Order3);

      ClientTransactionMock.EnlistDomainObject (domainObject1);
      ClientTransactionMock.EnlistDomainObject (domainObject2);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      ClientTransactionMock.EnsureDataAvailable (new[] { domainObject1, domainObject2, domainObject3 });

      listenerMock.AssertWasCalled (mock => mock.ObjectsLoaded (Arg<ReadOnlyCollection<DomainObject>>
          .Matches (loadedObjects => loadedObjects.SetEquals (new[] { domainObject1, domainObject2 }))));
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void EnsureDataAvailable_Many_Discarded ()
    {
      Order domainObject = Order.NewObject ();
      domainObject.Delete ();

      ClientTransactionMock.EnsureDataAvailable (new[] { domainObject });
    }

    [Test]
    [ExpectedException (typeof (BulkLoadException))]
    public void EnsureDataAvailable_Many_NotFound ()
    {
      var domainObject = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();

      ClientTransactionMock.EnlistDomainObject (domainObject);
      ClientTransactionMock.EnsureDataAvailable (new[] { domainObject });
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void EnsureDataAvailable_Many_NotEnlisted ()
    {
      var domainObject = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);

      ClientTransactionMock.EnsureDataAvailable (new[] { domainObject });
    }

    [Test]
    public void IsEnlisted ()
    {
      var order = Order.NewObject ();
      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.True);
      Assert.That (ClientTransaction.CreateRootTransaction ().IsEnlisted (order), Is.False);
    }

    [Test]
    public void Enlist ()
    {
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.False);

      ClientTransactionMock.EnlistDomainObject (order);

      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.True);
    }

    [Test]
    public void Enlist_DoesntLoad ()
    {
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.False);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      ClientTransactionMock.EnlistDomainObject (order);

      listenerMock.AssertWasNotCalled (mock => mock.ObjectLoading (Arg<ObjectID>.Is.Anything));
    }

    [Test]
    public void Enlist_Multiple ()
    {
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);

      ClientTransactionMock.EnlistDomainObject (order);
      ClientTransactionMock.EnlistDomainObject (order);

      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A domain object instance for object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' already exists in this transaction.")]
    public void Enlist_IDAlreadyEnlisted ()
    {
      Order.GetObject (DomainObjectIDs.Order1);

      var orderToBeEnlisted = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      ClientTransactionMock.EnlistDomainObject (orderToBeEnlisted);
    }

    [Test]
    public void Enlist_DiscardedObjects ()
    {
      Order discardedObject = Order.NewObject ();
      discardedObject.Delete ();
      Assert.That (discardedObject.IsDiscarded, Is.True);

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();

      newTransaction.EnlistDomainObject (discardedObject);

      Assert.That (newTransaction.IsEnlisted (discardedObject), Is.True);
    }

    [Test]
    public void EnlistSameDomainObjects ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = order.OrderItems[0];
      Order newOrder = Order.NewObject ();

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);

      Assert.That (newTransaction.IsEnlisted (order), Is.True);
      Assert.That (newTransaction.IsEnlisted (orderItem), Is.True);
      Assert.That (newTransaction.IsEnlisted (newOrder), Is.True);
    }

    [Test]
    public void EnlistSameDomainObjects_WithDiscardedSource ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = order.OrderItems[0];

      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.True);
      Assert.That (ClientTransactionMock.IsEnlisted (orderItem), Is.True);

      ClientTransactionMock.Discard ();
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();

      Assert.That (newTransaction.IsEnlisted (order), Is.False);
      Assert.That (newTransaction.IsEnlisted (orderItem), Is.False);

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);

      Assert.That (newTransaction.IsEnlisted (order), Is.True);
      Assert.That (newTransaction.IsEnlisted (orderItem), Is.True);
    }

    [Test]
    public void EnlistSameDomainObjects_CopyEventHandlers_False ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      bool orderItemAdded = false;
      order.OrderItems.Added += delegate { orderItemAdded = true; };
      
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistSameDomainObjects (ClientTransactionMock, false);

        Assert.That (orderItemAdded, Is.False);
        order.OrderItems.Add (OrderItem.NewObject ());
        Assert.That (orderItemAdded, Is.False);
      }
    }

    [Test]
    public void EnlistSameDomainObjects_CopyEventHandlers_True ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      
      bool orderItemAdded = false;
      order.OrderItems.Added += delegate { orderItemAdded = true; };

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistSameDomainObjects (ClientTransactionMock, true);

        Assert.That (orderItemAdded, Is.False);
        order.OrderItems.Add (OrderItem.NewObject ());
        Assert.That (orderItemAdded, Is.True);
      }
    }

    [Test]
    public void EnlistSameDomainObjects_CopyEventHandlers_True_DiscardedSource ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      bool orderItemAdded = false;
      order.OrderItems.Added += delegate { orderItemAdded = true; };
      
      ClientTransactionMock.Discard ();

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistSameDomainObjects (ClientTransactionMock, true);
        
        Assert.That (orderItemAdded, Is.False);
        order.OrderItems.Add (OrderItem.NewObject ());
        Assert.That (orderItemAdded, Is.True);
      }
    }
    
    [Test]
    public void CopyCollectionEventHandlers ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      
      bool orderItemAdded = false;
      order.OrderItems.Added += delegate { orderItemAdded = true; };
      
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        Assert.That (orderItemAdded, Is.False);

        ClientTransaction.Current.EnlistDomainObject (order);
        ClientTransaction.Current.CopyCollectionEventHandlers (order, ClientTransactionMock);

        order.OrderItems.Add (OrderItem.NewObject ());
        Assert.That (orderItemAdded, Is.True);
      }
    }

  }
}