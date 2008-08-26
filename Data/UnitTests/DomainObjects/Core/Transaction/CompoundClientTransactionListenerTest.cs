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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
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
      CheckNotification (typeof (IClientTransactionListener).GetMethod ("ObjectsLoaded"), new object[] {new DomainObjectCollection()});

      CheckNotification (typeof (IClientTransactionListener).GetMethod ("ObjectInitializedFromDataContainer"), new object[] { order.ID, order });

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
              new Type[]
                  {
                      typeof (DomainObject), typeof (string),
                      typeof (DomainObject), typeof (ValueAccess)
                  }),
          new object[] {order, "Foo", order, ValueAccess.Original});
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod (
              "RelationRead",
              new Type[]
                  {
                      typeof (DomainObject), typeof (string),
                      typeof (DomainObjectCollection), typeof (ValueAccess)
                  }),
          new object[]
              {
                  order, "FooBar",
                  new DomainObjectCollection(), ValueAccess.Original
              });
      CheckNotification (typeof (IClientTransactionListener).GetMethod ("RelationReading"), new object[] {order, "Whatever", ValueAccess.Current});

      CheckNotification (typeof (IClientTransactionListener).GetMethod ("RelationChanging"), new object[] {order, "Fred?", order, order2});
      CheckNotification (typeof (IClientTransactionListener).GetMethod ("RelationChanged"), new object[] {order, "Baz"});

      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("FilterQueryResult"),
          new object[]
              {
                  new DomainObjectCollection(),
                  new Query (QueryFactory.CreateOrderQueryWithCustomCollectionType())
              });

      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("TransactionCommitting"),
          new object[] {new DomainObjectCollection()});
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("TransactionCommitted"),
          new object[] {new DomainObjectCollection()});
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("TransactionRollingBack"),
          new object[] {new DomainObjectCollection()});
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("TransactionRolledBack"),
          new object[] {new DomainObjectCollection()});

      RelationEndPoint endPoint = RelationEndPoint.CreateNullRelationEndPoint (
          new RelationEndPointDefinition (order.ID.ClassDefinition, typeof(Order).FullName + ".Customer", true));

      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("RelationEndPointMapRegistering"),
          new object[] {endPoint});
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("RelationEndPointMapUnregistering"),
          new object[] {endPoint.ID});
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("RelationEndPointMapPerformingDelete"),
          new object[] {new RelationEndPointID[0]});
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("RelationEndPointMapCopyingFrom"),
          new object[] { ClientTransactionMock.DataManager.RelationEndPointMap });
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("RelationEndPointMapCopyingTo"),
          new object[] { ClientTransactionMock.DataManager.RelationEndPointMap });

      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("DataManagerMarkingObjectDiscarded"),
          new object[] {order.ID});
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("DataManagerCopyingFrom"),
          new object[] { ClientTransactionMock.DataManager });
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("DataManagerCopyingTo"),
          new object[] { ClientTransactionMock.DataManager });

      CheckNotification (typeof (IClientTransactionListener).GetMethod ("DataContainerMapRegistering"), new object[] {order.InternalDataContainer});
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("DataContainerMapUnregistering"),
          new object[] {order.InternalDataContainer});
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("DataContainerMapCopyingFrom"),
          new object[] { ClientTransactionMock.DataManager.DataContainerMap });
      CheckNotification (
          typeof (IClientTransactionListener).GetMethod ("DataContainerMapCopyingTo"),
          new object[] { ClientTransactionMock.DataManager.DataContainerMap });
    }
  }
}
