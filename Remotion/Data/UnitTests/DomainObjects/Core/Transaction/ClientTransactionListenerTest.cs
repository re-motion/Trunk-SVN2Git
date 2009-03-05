// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Mock_Is = Rhino.Mocks.Constraints.Is;
using Mock_Property = Rhino.Mocks.Constraints.Property;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class ClientTransactionListenerTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IClientTransactionListener _stricktListenerMock;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository ();
      _stricktListenerMock = _mockRepository.StrictMock<IClientTransactionListener> ();
    }

    [Test]
    public void NewObjectCreating ()
    {
      Expect (_stricktListenerMock, delegate
      {
        _stricktListenerMock.NewObjectCreating (typeof (ClassWithAllDataTypes), null);
        LastCall.Constraints (
            Mock_Is.Equal (typeof (ClassWithAllDataTypes)), Mock_Is.NotNull() & Mock_Property.ValueConstraint ("ID", Mock_Is.Null()));

        _stricktListenerMock.DataContainerMapRegistering (null);
        LastCall.IgnoreArguments ();

        _stricktListenerMock.ObjectGotID (null, null);
        LastCall.IgnoreArguments ();
      }, delegate { ClassWithAllDataTypes.NewObject (); });
    }

    [Test]
    public void ObjectsLoadingInitializedObjectsLoaded ()
    {
      Expect (_stricktListenerMock, delegate
      {
        _stricktListenerMock.ObjectLoading (DomainObjectIDs.ClassWithAllDataTypes1);

        _stricktListenerMock.ObjectGotID (null, null);
        LastCall.Constraints (
            Mock_Is.NotNull() & Mock_Property.ValueConstraint ("ID", Mock_Is.NotNull()),
            Mock_Is.Equal (DomainObjectIDs.ClassWithAllDataTypes1));

        _stricktListenerMock.DataContainerMapRegistering (null);
        LastCall.IgnoreArguments ();

        _stricktListenerMock.ObjectsLoaded (null);
        LastCall.Constraints (Mock_Property.Value ("Count", 1));
      }, delegate { ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1); });
    }

    [Test]
    public void ObjectsObjectDeletingObjectsDeletedRelationEndPointMapPerformingDelete2 ()
    {
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      Expect (_stricktListenerMock, delegate
      {
        _stricktListenerMock.ObjectDeleting (cwadt);

        _stricktListenerMock.RelationEndPointMapPerformingDelete (null);
        LastCall.Constraints (Mock_Property.Value ("Length", 0));

        _stricktListenerMock.ObjectDeleted (cwadt);
      }, delegate { cwadt.Delete (); });
    }

    [Test]
    public void PropertyValueReadingPropertyValueRead ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int orderNumber = order.OrderNumber;

      Expect (_stricktListenerMock, delegate
      {
        _stricktListenerMock.PropertyValueReading (order.InternalDataContainer,
                                                   order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"], ValueAccess.Current);
        _stricktListenerMock.PropertyValueRead (order.InternalDataContainer,
                                                order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"], orderNumber, ValueAccess.Current);
      }, delegate { int i = order.OrderNumber; });
    }

    [Test]
    public void PropertyValueChangingPropertyValueChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int orderNumber = order.OrderNumber;

      Expect (_stricktListenerMock, delegate
      {
        _stricktListenerMock.PropertyValueChanging (order.InternalDataContainer,
                                                    order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"], orderNumber, 43);
        _stricktListenerMock.PropertyValueChanged (order.InternalDataContainer,
                                                   order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"], orderNumber, 43);
      }, delegate { order.OrderNumber = 43; });
    }

    [Test]
    public void RelationReadingRelationRead ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Customer customer = order.Customer;
      ObjectList<OrderItem> orderItems = order.OrderItems;

      Expect (_stricktListenerMock, delegate
      {
        _stricktListenerMock.RelationReading (order, typeof (Order).FullName + ".Customer", ValueAccess.Current);
        _stricktListenerMock.RelationRead (order, typeof (Order).FullName + ".Customer", customer, ValueAccess.Current);

        _stricktListenerMock.RelationReading (order, typeof (Order).FullName + ".OrderItems", ValueAccess.Current);
        _stricktListenerMock.RelationRead (order, typeof (Order).FullName + ".OrderItems", orderItems, ValueAccess.Current);
        LastCall.Constraints (Mock_Is.Equal (order), Mock_Is.Equal (typeof (Order).FullName + ".OrderItems"),
                              Mock_Property.Value ("Count", orderItems.Count), Mock_Is.Equal (ValueAccess.Current));
      }, delegate
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

      Expect (_stricktListenerMock, delegate
      {
        _stricktListenerMock.ObjectLoading (null);
        LastCall.IgnoreArguments().Repeat.Any ();
        _stricktListenerMock.ObjectsLoaded (null);
        LastCall.IgnoreArguments ().Repeat.Any ();

        _stricktListenerMock.ObjectGotID (null, null);
        LastCall.IgnoreArguments ();

        _stricktListenerMock.DataContainerMapRegistering (null);
        LastCall.IgnoreArguments ();

        _stricktListenerMock.RelationEndPointMapRegistering (null);
        LastCall.IgnoreArguments ().Repeat.Any();

        _stricktListenerMock.RelationChanging (order, typeof (Order).FullName + ".Customer", customer, newCustomer);
        _stricktListenerMock.RelationChanging (newCustomer, typeof (Customer).FullName + ".Orders", null, order);
        _stricktListenerMock.RelationChanging (customer, typeof (Customer).FullName + ".Orders", order, null);
        _stricktListenerMock.PropertyValueChanging (
            order.InternalDataContainer,
            order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".Customer"],
            customer.ID,
            newCustomer.ID);
        _stricktListenerMock.PropertyValueChanged (
            order.InternalDataContainer,
            order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".Customer"],
            customer.ID,
            newCustomer.ID);
        _stricktListenerMock.RelationChanged (order, typeof (Order).FullName + ".Customer");
        _stricktListenerMock.RelationChanged (newCustomer, typeof (Customer).FullName + ".Orders");
        _stricktListenerMock.RelationChanged (customer, typeof (Customer).FullName + ".Orders");
      }, delegate
      {
        order.Customer = newCustomer;
      });
    }

    [Test]
    public void FilterQueryResult ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQuery");
      var orders = (OrderCollection) ClientTransactionMock.QueryManager.GetCollection (query).ToCustomCollection ();

      ClientTransactionMock.AddListener (_stricktListenerMock);

      var newQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();
      _stricktListenerMock
          .Expect (mock => mock.FilterQueryResult (Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == orders.Count)))
          .Return (newQueryResult);

      _stricktListenerMock.Replay ();

      var result = ClientTransactionMock.QueryManager.GetCollection (query);
      Assert.That (result, Is.SameAs (newQueryResult));

      _mockRepository.VerifyAll ();
    }


    [Test]
    public void TransactionCommittingTransactionCommitted ()
    {
      SetDatabaseModifyable ();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;

      Expect (_stricktListenerMock, delegate
      {
        _stricktListenerMock.TransactionCommitting (null);
        LastCall.Constraints (Mock_Property.Value ("Count", 1));
        _stricktListenerMock.TransactionCommitted (null);
        LastCall.Constraints (Mock_Property.Value ("Count", 1));
      }, delegate
      {
        ClientTransactionMock.Commit ();
      });
    }

    [Test]
    public void TransactionRollingBackTransactionRolledBack ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;

      Expect (_stricktListenerMock, delegate
      {
        _stricktListenerMock.TransactionRollingBack (null);
        LastCall.Constraints (Mock_Property.Value ("Count", 1));
        _stricktListenerMock.TransactionRolledBack (null);
        LastCall.Constraints (Mock_Property.Value ("Count", 1));
      }, delegate
      {
        ClientTransactionMock.Rollback ();
      });
    }

    [Test]
    public void RelationEndPointMapRegistering ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Expect (_stricktListenerMock, delegate
      {
        _stricktListenerMock.RelationReading (null, null, ValueAccess.Current);
        LastCall.IgnoreArguments ();
        _stricktListenerMock.ObjectLoading (null);
        LastCall.IgnoreArguments ();

        _stricktListenerMock.ObjectGotID (null, null);
        LastCall.IgnoreArguments ();
          
        _stricktListenerMock.DataContainerMapRegistering (null);
        LastCall.IgnoreArguments ();

        _stricktListenerMock.RelationEndPointMapRegistering (null);
        LastCall.Constraints (Mock_Property.Value ("ObjectID", DomainObjectIDs.Customer1));

        _stricktListenerMock.ObjectsLoaded (null);
        LastCall.IgnoreArguments ();
        _stricktListenerMock.RelationRead (null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.IgnoreArguments ();
      }, delegate
      {
        Dev.Null = order.Customer;
      });
    }

    [Test]
    public void RelationEndPointMapUnregisteringDataManagerMarkingObjectDiscardedDataContainerMapUnregistering ()
    {
      Order order = Order.NewObject ();

      Expect (_stricktListenerMock, delegate
      {
        _stricktListenerMock.ObjectDeleting (null);
        LastCall.IgnoreArguments ();
        _stricktListenerMock.RelationEndPointMapPerformingDelete (null);
        LastCall.IgnoreArguments ();

        _stricktListenerMock.RelationEndPointMapUnregistering (null);
        LastCall.Constraints (Mock_Property.Value ("ObjectID", order.ID)).Repeat.Times (4); // four related objects/object collections in Order

        _stricktListenerMock.DataContainerMapUnregistering (order.InternalDataContainer);

        _stricktListenerMock.DataManagerMarkingObjectDiscarded (order.ID);

        _stricktListenerMock.ObjectDeleted (null);
        LastCall.IgnoreArguments ();
      }, delegate
      {
        order.Delete ();
      });
    }

    [Test]
    public void DataContainerMapRegistering ()
    {
      Expect (_stricktListenerMock, delegate
      {
        _stricktListenerMock.ObjectLoading (null);
        LastCall.IgnoreArguments ();

        _stricktListenerMock.ObjectGotID (null, null);
        LastCall.IgnoreArguments ();

        _stricktListenerMock.DataContainerMapRegistering (null);
        LastCall.Constraints (Mock_Property.Value ("ID", DomainObjectIDs.ClassWithAllDataTypes1));

        _stricktListenerMock.ObjectsLoaded (null);
        LastCall.IgnoreArguments ();
      }, delegate
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

      transactionOne.AddListener (_stricktListenerMock);
      transactionTwo.AddListener (_stricktListenerMock);

      Expect (_stricktListenerMock, delegate
      {
        _stricktListenerMock.DataManagerCopyingFrom (managerOne);
        _stricktListenerMock.DataManagerCopyingTo (managerTwo);
        _stricktListenerMock.DataContainerMapCopyingFrom (managerOne.DataContainerMap);
        _stricktListenerMock.DataContainerMapCopyingTo (managerTwo.DataContainerMap);
        _stricktListenerMock.RelationEndPointMapCopyingFrom (managerOne.RelationEndPointMap);
        _stricktListenerMock.RelationEndPointMapCopyingTo (managerTwo.RelationEndPointMap);
      }, delegate
      {
        managerTwo.CopyFrom (managerOne);
      });
    }

    [Test]
    public void SubTransactionCreatingSubTransactionCreated ()
    {
      Expect(_stricktListenerMock, delegate
      {
        _stricktListenerMock.SubTransactionCreating ();

        _stricktListenerMock.SubTransactionCreated (null);
        LastCall.Constraints (Mock_Is.NotNull () && Mock_Is.NotSame (ClientTransactionMock)
                              && Mock_Property.Value ("ParentTransaction", ClientTransactionMock));
      }, delegate
      {
        ClientTransactionMock.CreateSubTransaction();
      });
    }

    private void Expect (IClientTransactionListener listenerMock, Action expectation, Action triggeringCode)
    {
      ClientTransactionMock.AddListener (listenerMock);

      _mockRepository.BackToRecordAll ();

      using (_mockRepository.Ordered ())
      {
        expectation ();
      }

      _mockRepository.ReplayAll ();

      triggeringCode ();

      _mockRepository.VerifyAll ();
    }

  }
}
