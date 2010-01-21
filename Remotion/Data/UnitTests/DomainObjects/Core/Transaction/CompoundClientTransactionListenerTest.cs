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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;
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

    private void CheckNotification (MethodInfo method, object[] arguments)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      _mockRepository.BackToRecordAll ();
      using (_mockRepository.Ordered ())
      {
        method.Invoke (_listener1, arguments);
        method.Invoke (_listener2, arguments);
      }

      _mockRepository.ReplayAll ();

      method.Invoke (_compoundListener, arguments);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AggregatedClientsAreNotified ()
    {
      Order order = Order.NewObject();
      Order order2 = Order.NewObject();

      CheckNotification (typeof (IClientTransactionListener).GetMethod ("SubTransactionCreating"), new object[0]);
      CheckNotification (typeof (IClientTransactionListener).GetMethod ("SubTransactionCreated"), new object[] { ClientTransactionMock });

      CheckNotification (typeof (IClientTransactionListener).GetMethod ("NewObjectCreating"), new object[] {typeof (string), null});

      CheckNotification (typeof (IClientTransactionListener).GetMethod ("ObjectLoading"), new object[] { order.ID });
      CheckNotification (typeof (IClientTransactionListener).GetMethod ("ObjectsLoaded"), new object[] { new ReadOnlyCollection<DomainObject>(new DomainObject[0]) });

      CheckNotification (typeof (IClientTransactionListener).GetMethod ("ObjectGotID"), new object[] { order, order.ID });

      CheckNotification (typeof (IClientTransactionListener).GetMethod ("ObjectDeleting"), new object[] {order});
      CheckNotification (typeof (IClientTransactionListener).GetMethod ("ObjectDeleted"), new object[] {order});

      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("PropertyValueReading"),
          new object[]
              {
                  order.InternalDataContainer,
                  order.InternalDataContainer.PropertyValues[0], ValueAccess.Original
              });
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("PropertyValueRead"),
          new object[]
              {
                  order.InternalDataContainer,
                  order.InternalDataContainer.PropertyValues[0], "Foo", ValueAccess.Original
              });

      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("PropertyValueChanging"),
          new object[]
              {
                  order.InternalDataContainer,
                  order.InternalDataContainer.PropertyValues[0], "Foo", "Bar"
              });
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("PropertyValueChanged"),
          new object[]
              {
                  order.InternalDataContainer,
                  order.InternalDataContainer.PropertyValues[0], "Foo", "Bar"
              });

      CheckNotification (
          typeof (IClientTransactionListener).GetMethod (
              "RelationRead",
              new[]
                  {
                      typeof (DomainObject), typeof (string),
                      typeof (DomainObject), typeof (ValueAccess)
                  }),
          new object[] {order, "Foo", order, ValueAccess.Original});
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod (
              "RelationRead",
              new[]
                  {
                      typeof (DomainObject), typeof (string),
                      typeof (ReadOnlyCollection<DomainObject>), typeof (ValueAccess)
                  }),
          new object[]
              {
                  order, "FooBar",
                  new ReadOnlyCollection<DomainObject>(new DomainObject[0]), ValueAccess.Original
              });
      CheckNotification (typeof (IClientTransactionListener).GetMethod ("RelationReading"), new object[] {order, "Whatever", ValueAccess.Current});

      CheckNotification (typeof (IClientTransactionListener).GetMethod ("RelationChanging"), new object[] {order, "Fred?", order, order2});
      CheckNotification (typeof (IClientTransactionListener).GetMethod ("RelationChanged"), new object[] {order, "Baz"});

      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("TransactionCommitting"),
          new object[] { new ReadOnlyCollection<DomainObject>(new DomainObject[0]) });
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("TransactionCommitted"),
          new object[] { new ReadOnlyCollection<DomainObject> (new DomainObject[0]) });
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("TransactionRollingBack"),
          new object[] { new ReadOnlyCollection<DomainObject> (new DomainObject[0]) });
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("TransactionRolledBack"),
          new object[] { new ReadOnlyCollection<DomainObject> (new DomainObject[0]) });

      var id = new RelationEndPointID (order.ID, typeof (Order).FullName + ".Customer");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (id, null);

      CheckNotification (typeof (IClientTransactionListener).GetMethod ("RelationEndPointMapRegistering"), new object[] {endPoint});

      CheckNotification (typeof (IClientTransactionListener).GetMethod ("RelationEndPointMapUnregistering"), new object[] {endPoint.ID});
      CheckNotification (typeof (IClientTransactionListener).GetMethod ("RelationEndPointUnloading"), new object[] { endPoint });

      CheckNotification (typeof (IClientTransactionListener).GetMethod ("DataManagerMarkingObjectDiscarded"), new object[] {order.ID});

      CheckNotification (typeof (IClientTransactionListener).GetMethod ("DataContainerMapRegistering"), new object[] {order.InternalDataContainer});
      CheckNotification (typeof (IClientTransactionListener).GetMethod ("DataContainerMapUnregistering"), new object[] {order.InternalDataContainer});
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
      var newResult1 = new QueryResult<Order> (QueryFactory.CreateQuery (TestQueryFactory.CreateOrderQueryWithCustomCollectionType ()), new[] { Order.GetObject (DomainObjectIDs.Order1) });
      var newResult2 = new QueryResult<Order> (QueryFactory.CreateQuery (TestQueryFactory.CreateOrderQueryWithCustomCollectionType ()), new[] { Order.GetObject (DomainObjectIDs.Order2) });

      listenerMock1.Expect (mock => mock.FilterQueryResult (originalResult)).Return (newResult1);
      listenerMock2.Expect (mock => mock.FilterQueryResult (newResult1)).Return (newResult2);

      var finalResult = compoundListener.FilterQueryResult (originalResult);
      Assert.That (finalResult, Is.SameAs (newResult2));
    }
  }
}
