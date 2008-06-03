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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;
using Mock_Is = Rhino.Mocks.Constraints.Is;
using Mock_Property = Rhino.Mocks.Constraints.Property;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Transaction
{
  [TestFixture]
  public class ClientTransactionListenerTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IClientTransactionListener _listener;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository ();
      
      _listener = _mockRepository.CreateMock<IClientTransactionListener> ();

      ClientTransactionMock.AddListener (_listener);
    }

    private void Expect (Proc expectation, Proc triggeringCode)
    {
      _mockRepository.BackToRecordAll ();

      using (_mockRepository.Ordered ())
      {
        expectation();
      }

      _mockRepository.ReplayAll ();

      triggeringCode ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NewObjectCreating ()
    {
      Expect (delegate
        {
          _listener.NewObjectCreating (typeof (ClassWithAllDataTypes), null);
          LastCall.Constraints (
              Mock_Is.Equal (typeof (ClassWithAllDataTypes)), Mock_Is.NotNull() & Mock_Property.ValueConstraint ("ID", Mock_Is.Null()));

          _listener.DataContainerMapRegistering (null);
          LastCall.IgnoreArguments ();

          _listener.ObjectInitializedFromDataContainer (null, null);
          LastCall.IgnoreArguments ();
        },
        delegate { ClassWithAllDataTypes.NewObject (); });
    }

    [Test]
    public void ObjectsLoadingInitializedObjectsLoaded ()
    {
      Expect (delegate
        {
          _listener.ObjectLoading (DomainObjectIDs.ClassWithAllDataTypes1);

          _listener.DataContainerMapRegistering (null);
          LastCall.IgnoreArguments ();

          _listener.ObjectInitializedFromDataContainer (null, null);
          LastCall.Constraints (
              Mock_Is.Equal (DomainObjectIDs.ClassWithAllDataTypes1),
              Mock_Is.NotNull() & Mock_Property.ValueConstraint ("ID", Mock_Is.NotNull()));

          _listener.ObjectsLoaded (null);
          LastCall.Constraints (Mock_Property.Value ("Count", 1));
        },
        delegate { ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1); });
    }

    [Test]
    public void ObjectsObjectDeletingObjectsDeletedRelationEndPointMapPerformingDelete2 ()
    {
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      Expect (delegate
        {
          _listener.ObjectDeleting (cwadt);

          _listener.RelationEndPointMapPerformingDelete (null);
          LastCall.Constraints (Mock_Property.Value ("Length", 0));

          _listener.ObjectDeleted (cwadt);
        },
        delegate { cwadt.Delete (); });
    }

    [Test]
    public void PropertyValueReadingPropertyValueRead ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int orderNumber = order.OrderNumber;

      Expect (delegate
        {
          _listener.PropertyValueReading (order.InternalDataContainer,
              order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"], ValueAccess.Current);
          _listener.PropertyValueRead (order.InternalDataContainer,
              order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"], orderNumber, ValueAccess.Current);
        },
        delegate { int i = order.OrderNumber; });
    }

    [Test]
    public void PropertyValueChangingPropertyValueChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int orderNumber = order.OrderNumber;

      Expect (delegate
        {
          _listener.PropertyValueChanging (order.InternalDataContainer,
              order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"], orderNumber, 43);
          _listener.PropertyValueChanged (order.InternalDataContainer,
              order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"], orderNumber, 43);
        },
        delegate { order.OrderNumber = 43; });
    }

    [Test]
    public void RelationReadingRelationRead ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Customer customer = order.Customer;
      ObjectList<OrderItem> orderItems = order.OrderItems;

      Expect (delegate
        {
          _listener.RelationReading (order, typeof (Order).FullName + ".Customer", ValueAccess.Current);
          _listener.RelationRead (order, typeof (Order).FullName + ".Customer", customer, ValueAccess.Current);

          _listener.RelationReading (order, typeof (Order).FullName + ".OrderItems", ValueAccess.Current);
          _listener.RelationRead (order, typeof (Order).FullName + ".OrderItems", orderItems, ValueAccess.Current);
          LastCall.Constraints (Mock_Is.Equal (order), Mock_Is.Equal (typeof (Order).FullName + ".OrderItems"),
              Mock_Property.Value ("Count", orderItems.Count), Mock_Is.Equal (ValueAccess.Current));
        },
        delegate
        {
          Customer c = order.Customer;
          ObjectList<OrderItem> i = order.OrderItems;
        });
    }

    [Test]
    public void RelationChangingRelationChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Customer customer = order.Customer;
      Customer newCustomer = Customer.NewObject();

      Expect (delegate
        {
          _listener.ObjectLoading (null);
          LastCall.IgnoreArguments().Repeat.Any ();
          _listener.ObjectsLoaded (null);
          LastCall.IgnoreArguments ().Repeat.Any ();

          _listener.DataContainerMapRegistering (null);
          LastCall.IgnoreArguments ();

          _listener.ObjectInitializedFromDataContainer (null, null);
          LastCall.IgnoreArguments ();

          _listener.RelationEndPointMapRegistering (null);
          LastCall.IgnoreArguments ().Repeat.Any();

          _listener.RelationChanging (order, typeof (Order).FullName + ".Customer", customer, newCustomer);
          _listener.RelationChanging (newCustomer, typeof (Customer).FullName + ".Orders", null, order);
          _listener.RelationChanging (customer, typeof (Customer).FullName + ".Orders", order, null);
          _listener.PropertyValueChanging (
              order.InternalDataContainer,
              order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".Customer"],
              customer.ID,
              newCustomer.ID);
          _listener.PropertyValueChanged (
              order.InternalDataContainer,
              order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".Customer"],
              customer.ID,
              newCustomer.ID);
          _listener.RelationChanged (order, typeof (Order).FullName + ".Customer");
          _listener.RelationChanged (newCustomer, typeof (Customer).FullName + ".Orders");
          _listener.RelationChanged (customer, typeof (Customer).FullName + ".Orders");
        },
        delegate
        {
          order.Customer = newCustomer;
        });
    }

    [Test]
    public void FilterQueryResult ()
    {
      Query query = new Query ("StoredProcedureQuery");
      OrderCollection orders = (OrderCollection) ClientTransactionMock.QueryManager.GetCollection (query);

      Expect (delegate
        {
          _listener.ObjectLoading (null);
          LastCall.IgnoreArguments ().Repeat.Any ();
          _listener.ObjectsLoaded (null);
          LastCall.IgnoreArguments ().Repeat.Any ();

          _listener.FilterQueryResult (null, null);
          LastCall.Constraints (Mock_Property.Value ("Count", orders.Count), Mock_Is.Equal (query));
        },
        delegate
        {
          ClientTransactionMock.QueryManager.GetCollection (query);
        });
    }


    [Test]
    public void TransactionCommittingTransactionCommitted ()
    {
      SetDatabaseModifyable ();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;

      Expect (delegate
        {
          _listener.TransactionCommitting (null);
          LastCall.Constraints (Mock_Property.Value ("Count", 1));
          _listener.TransactionCommitted (null);
          LastCall.Constraints (Mock_Property.Value ("Count", 1));
        },
        delegate
        {
          ClientTransactionMock.Commit ();
        });
    }

    [Test]
    public void TransactionRollingBackTransactionRolledBack ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;

      Expect (delegate
        {
          _listener.TransactionRollingBack (null);
          LastCall.Constraints (Mock_Property.Value ("Count", 1));
          _listener.TransactionRolledBack (null);
          LastCall.Constraints (Mock_Property.Value ("Count", 1));
        },
        delegate
        {
          ClientTransactionMock.Rollback ();
        });
    }

    [Test]
    public void RelationEndPointMapRegistering ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Expect (delegate
        {
          _listener.RelationReading (null, null, ValueAccess.Current);
          LastCall.IgnoreArguments ();
          _listener.ObjectLoading (null);
          LastCall.IgnoreArguments ();
          _listener.DataContainerMapRegistering (null);
          LastCall.IgnoreArguments ();

          _listener.RelationEndPointMapRegistering (null);
          LastCall.Constraints (Mock_Property.Value ("ObjectID", DomainObjectIDs.Customer1));

          _listener.ObjectInitializedFromDataContainer (null, null);
          LastCall.IgnoreArguments ();

          _listener.ObjectsLoaded (null);
          LastCall.IgnoreArguments ();
          _listener.RelationRead (null, null, (DomainObject) null, ValueAccess.Current);
          LastCall.IgnoreArguments ();
        },
        delegate
        {
          Customer customer = order.Customer;
        });
    }

    [Test]
    public void RelationEndPointMapUnregisteringDataManagerMarkingObjectDiscardedDataContainerMapUnregistering ()
    {
      Order order = Order.NewObject ();

      Expect (delegate
        {
          _listener.ObjectDeleting (null);
          LastCall.IgnoreArguments ();
          _listener.RelationEndPointMapPerformingDelete (null);
          LastCall.IgnoreArguments ();

          _listener.RelationEndPointMapUnregistering (null);
          LastCall.Constraints (Mock_Property.Value ("ObjectID", order.ID)).Repeat.Times (4); // four related objects/object collections in Order

          _listener.DataContainerMapUnregistering (order.InternalDataContainer);

          _listener.DataManagerMarkingObjectDiscarded (order.ID);

          _listener.ObjectDeleted (null);
          LastCall.IgnoreArguments ();
        },
        delegate
        {
          order.Delete ();
        });
    }

    [Test]
    public void DataContainerMapRegistering ()
    {
      Expect (delegate
        {
          _listener.ObjectLoading (null);
          LastCall.IgnoreArguments ();

          _listener.DataContainerMapRegistering (null);
          LastCall.Constraints (Mock_Property.Value ("ID", DomainObjectIDs.ClassWithAllDataTypes1));

          _listener.ObjectInitializedFromDataContainer (null, null);
          LastCall.IgnoreArguments();

          _listener.ObjectsLoaded (null);
          LastCall.IgnoreArguments ();
        },
        delegate
        {
          ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        });
    }

    [Test]
    public void DataManagerCopyingFromDataManagerCopyingTo ()
    {
      ClientTransactionMock transactionOne = new ClientTransactionMock ();
      ClientTransactionMock transactionTwo = new ClientTransactionMock ();
      DataManager managerOne = transactionOne.DataManager;
      DataManager managerTwo = transactionTwo.DataManager;

      transactionOne.AddListener (_listener);
      transactionTwo.AddListener (_listener);

      Expect (delegate
        {
          _listener.DataManagerCopyingFrom (managerOne);
          _listener.DataManagerCopyingTo (managerTwo);
          _listener.DataContainerMapCopyingFrom (managerOne.DataContainerMap);
          _listener.DataContainerMapCopyingTo (managerTwo.DataContainerMap);
          _listener.RelationEndPointMapCopyingFrom (managerOne.RelationEndPointMap);
          _listener.RelationEndPointMapCopyingTo (managerTwo.RelationEndPointMap);
        },
        delegate
        {
          managerTwo.CopyFrom (managerOne);
        });
    }

    [Test]
    public void SubTransactionCreatingSubTransactionCreated ()
    {
      Expect(delegate
      {
        _listener.SubTransactionCreating ();

        _listener.SubTransactionCreated (null);
        LastCall.Constraints (Mock_Is.NotNull () && Mock_Is.NotSame (ClientTransactionMock)
          && Mock_Property.Value ("ParentTransaction", ClientTransactionMock));
      },
      delegate
      {
        ClientTransactionMock.CreateSubTransaction();
      });
    }
  }
}
