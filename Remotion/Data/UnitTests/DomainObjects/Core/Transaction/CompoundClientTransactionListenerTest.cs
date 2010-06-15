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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class CompoundClientTransactionListenerTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IClientTransactionListener _listener1;
    private IClientTransactionListener _listener2;
    private CompoundClientTransactionListener _compoundListener;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      
      _listener1 = _mockRepository.StrictMock<IClientTransactionListener> ();
      _listener2 = _mockRepository.StrictMock<IClientTransactionListener> ();

      _compoundListener = new CompoundClientTransactionListener ();
      _compoundListener.AddListener (_listener1);
      _compoundListener.AddListener (_listener2);
    }

    private void CheckNotification (Action<IClientTransactionListener> notificationCall)
    {
      _mockRepository.BackToRecordAll ();
      using (_mockRepository.Ordered ())
      {
        _listener1.Expect (notificationCall);
        _listener2.Expect (notificationCall);
      }

      _mockRepository.ReplayAll ();

      notificationCall (_compoundListener);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AggregatedClientsAreNotified ()
    {
      var order = Order.NewObject();
      var order2 = Order.NewObject();
      var domainObjects = new ReadOnlyCollection<DomainObject> (new DomainObject[0]);
      var relatedObjects = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (new DomainObjectCollection ());

      var endPointMock = MockRepository.GenerateMock<IEndPoint>();

      CheckNotification (listener => listener.TransactionInitializing (ClientTransactionMock));
      CheckNotification (listener => listener.TransactionDiscarding (ClientTransactionMock));

      CheckNotification (listener => listener.SubTransactionCreating (ClientTransactionMock));
      CheckNotification (listener => listener.SubTransactionCreated (ClientTransactionMock, ClientTransactionMock));

      CheckNotification (listener => listener.NewObjectCreating (ClientTransactionMock, typeof (string), null));

      CheckNotification (listener => listener.ObjectsLoading (ClientTransactionMock, new ReadOnlyCollection<ObjectID> (new ObjectID[0])));
      CheckNotification (listener => listener.ObjectsLoaded (ClientTransactionMock, domainObjects));

      CheckNotification (listener => listener.ObjectsUnloading (ClientTransactionMock, domainObjects));
      CheckNotification (listener => listener.ObjectsUnloaded (ClientTransactionMock, domainObjects));

      CheckNotification (listener => listener.ObjectDeleting (ClientTransactionMock, order));
      CheckNotification (listener => listener.ObjectDeleted (ClientTransactionMock, order));

      CheckNotification (listener => listener.PropertyValueReading (
          ClientTransactionMock, 
          order.InternalDataContainer, 
          order.InternalDataContainer.PropertyValues[0], 
          ValueAccess.Original));
      CheckNotification (listener => listener.PropertyValueRead (
        ClientTransactionMock, 
        order.InternalDataContainer,
        order.InternalDataContainer.PropertyValues[0],
        "Foo", 
        ValueAccess.Original));

      CheckNotification (listener => listener.PropertyValueChanging (
          ClientTransactionMock, 
          order.InternalDataContainer,
          order.InternalDataContainer.PropertyValues[0], 
          "Foo", 
          "Bar"));
      CheckNotification (listener => listener.PropertyValueChanged (
          ClientTransactionMock, 
          order.InternalDataContainer,
          order.InternalDataContainer.PropertyValues[0], 
          "Foo", 
          "Bar"));

      CheckNotification (listener => listener.RelationRead (ClientTransactionMock, order, "Foo", order, ValueAccess.Original));
      CheckNotification (listener => listener.RelationRead (ClientTransactionMock, order, endPointMock, order, ValueAccess.Original));
      CheckNotification (listener => listener.RelationRead (ClientTransactionMock, order, "FooBar", relatedObjects, ValueAccess.Original));
      CheckNotification (listener => listener.RelationRead (ClientTransactionMock, order, endPointMock, relatedObjects, ValueAccess.Original));
      CheckNotification (listener => listener.RelationReading (ClientTransactionMock, order, "Whatever", ValueAccess.Current));
      CheckNotification (listener => listener.RelationReading (ClientTransactionMock, order, endPointMock, ValueAccess.Current));
      
      CheckNotification (listener => listener.RelationChanging (ClientTransactionMock, order, "Fred?", order, order2));
      CheckNotification (listener => listener.RelationChanging (ClientTransactionMock, order, endPointMock, order, order2));
      CheckNotification (listener => listener.RelationChanged (ClientTransactionMock, order, "Baz"));
      CheckNotification (listener => listener.RelationChanged (ClientTransactionMock, order, endPointMock));

      CheckNotification (listener => listener.TransactionCommitting (ClientTransactionMock, domainObjects));
      CheckNotification (listener => listener.TransactionCommitted (ClientTransactionMock, domainObjects));
      CheckNotification (listener => listener.TransactionRollingBack (ClientTransactionMock, domainObjects));
      CheckNotification (listener => listener.TransactionRolledBack (ClientTransactionMock, domainObjects));

      var id = new RelationEndPointID (order.ID, typeof (Order).FullName + ".Customer");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (id, null);

      CheckNotification (listener => listener.RelationEndPointMapRegistering (ClientTransactionMock, endPoint));

      CheckNotification (listener => listener.RelationEndPointMapUnregistering (ClientTransactionMock, endPoint.ID));
      CheckNotification (listener => listener.RelationEndPointUnloading (ClientTransactionMock, endPoint));

      CheckNotification (listener => listener.DataManagerMarkingObjectInvalid (ClientTransactionMock, order.ID));

      CheckNotification (listener => listener.DataContainerMapRegistering (ClientTransactionMock, order.InternalDataContainer));
      CheckNotification (listener => listener.DataContainerMapUnregistering (ClientTransactionMock, order.InternalDataContainer));

      CheckNotification (listener => listener.DataContainerStateUpdated (ClientTransactionMock, order.InternalDataContainer, StateType.Deleted));
      CheckNotification (listener => listener.VirtualRelationEndPointStateUpdated (ClientTransactionMock, endPoint.ID, true));
    }

    [Test]
    public void FilterQueryResult ()
    {
      var listenerMock1 = MockRepository.GenerateMock<IClientTransactionListener> ();
      var listenerMock2 = MockRepository.GenerateMock<IClientTransactionListener> ();

      var compoundListener = new CompoundClientTransactionListener ();
      compoundListener.AddListener (listenerMock1);
      compoundListener.AddListener (listenerMock2);

      var originalResult = new QueryResult<Order> (QueryFactory.CreateQuery(TestQueryFactory.CreateOrderQueryWithCustomCollectionType()), new Order[0]);
      var newResult1 = new QueryResult<Order> (QueryFactory.CreateQuery (TestQueryFactory.CreateOrderQueryWithCustomCollectionType ()), new[] { Order.GetObject (DomainObjectIDs.Order1)});
      var newResult2 = new QueryResult<Order> (QueryFactory.CreateQuery (TestQueryFactory.CreateOrderQueryWithCustomCollectionType ()), new[] { Order.GetObject (DomainObjectIDs.Order2)});

      listenerMock1.Expect (mock => mock.FilterQueryResult (ClientTransactionMock, originalResult)).Return (newResult1);
      listenerMock2.Expect (mock => mock.FilterQueryResult (ClientTransactionMock, newResult1)).Return (newResult2);

      var finalResult = compoundListener.FilterQueryResult (ClientTransactionMock, originalResult);
      Assert.That (finalResult, Is.SameAs (newResult2));
    }
  }
}
