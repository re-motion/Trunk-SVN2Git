// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
    private IClientTransactionListener _strictListenerMock;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository ();
      _strictListenerMock = _mockRepository.StrictMock<IClientTransactionListener> ();
    }

    [Test]
    public void NewObjectCreating ()
    {
      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (
            mock => mock.NewObjectCreating (Arg.Is (typeof (ClassWithAllDataTypes)), Arg<DomainObject>.Matches (obj => obj != null && obj.ID == null)));
        _strictListenerMock.Expect (mock => mock.DataContainerMapRegistering (Arg<DataContainer>.Is.Anything));
        _strictListenerMock.Expect (mock => mock.ObjectGotID (Arg<DomainObject>.Is.Anything, Arg<ObjectID>.Is.Anything));
      }

      _mockRepository.ReplayAll ();

      ClassWithAllDataTypes.NewObject ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoadingInitializedObjectsLoaded ()
    {
      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.ObjectLoading (DomainObjectIDs.ClassWithAllDataTypes1));
        _strictListenerMock.Expect (
            mock => mock.ObjectGotID (Arg<DomainObject>.Matches (obj => obj.ID != null), Arg.Is (DomainObjectIDs.ClassWithAllDataTypes1)));
        _strictListenerMock.Expect (mock => mock.DataContainerMapRegistering (Arg<DataContainer>.Is.Anything));
        _strictListenerMock.Expect (mock => mock.ObjectsLoaded (Arg<DomainObjectCollection>.Matches (doc => doc.Count == 1)));
      }

      _mockRepository.ReplayAll ();

      ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsObjectDeletingObjectsDeletedRelationEndPointMapPerformingDelete2 ()
    {
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
          _strictListenerMock.Expect (mock => mock.ObjectDeleting (cwadt));
          _strictListenerMock.Expect (mock => mock.RelationEndPointMapPerformingDelete (Arg<RelationEndPointID[]>.Matches (ids => ids.Length == 0)));
          _strictListenerMock.Expect (mock => mock.ObjectDeleted (cwadt));
      }

      _mockRepository.ReplayAll ();

      cwadt.Delete ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyValueReadingPropertyValueRead ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int orderNumber = order.OrderNumber;

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (
            mock => mock.PropertyValueReading (
                        order.InternalDataContainer,
                        order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"],
                        ValueAccess.Current));
        _strictListenerMock.Expect (
            mock => mock.PropertyValueRead (
                        order.InternalDataContainer,
                        order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"],
                        orderNumber,
                        ValueAccess.Current));
      }

      _mockRepository.ReplayAll ();

      Dev.Null = order.OrderNumber;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyValueChangingPropertyValueChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int orderNumber = order.OrderNumber;

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (
            mock => mock.PropertyValueChanging (
                        order.InternalDataContainer,
                        order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"],
                        orderNumber,
                        43));
        _strictListenerMock.Expect (
            mock => mock.PropertyValueChanged (
                        order.InternalDataContainer,
                        order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"],
                        orderNumber,
                        43));
      }

      _mockRepository.ReplayAll ();

      order.OrderNumber = 43;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationReadingRelationRead ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Customer customer = order.Customer;
      ObjectList<OrderItem> orderItems = order.OrderItems;

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (
            mock => mock.RelationReading (order, typeof (Order).FullName + ".Customer", ValueAccess.Current));
        _strictListenerMock.Expect (
            mock => mock.RelationRead (order, typeof (Order).FullName + ".Customer", customer, ValueAccess.Current));
        _strictListenerMock.Expect (
            mock => mock.RelationReading (order, typeof (Order).FullName + ".OrderItems", ValueAccess.Current));
        _strictListenerMock.Expect (
            mock => mock.RelationRead (order, typeof (Order).FullName + ".OrderItems", orderItems, ValueAccess.Current));
        LastCall.Constraints (
            Mock_Is.Equal (order),
            Mock_Is.Equal (typeof (Order).FullName + ".OrderItems"),
            Mock_Property.Value ("Count", orderItems.Count),
            Mock_Is.Equal (ValueAccess.Current));
      }

      _mockRepository.ReplayAll ();

      Dev.Null = order.Customer;
      Dev.Null = order.OrderItems;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationChangingRelationChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Customer oldCustomer = order.Customer;
      Customer newCustomer = Customer.NewObject();
      
      // preload all related objects
      foreach (var otherOrder in oldCustomer.Orders)
        Dev.Null = otherOrder;

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.RelationChanging (order, typeof (Order).FullName + ".Customer", oldCustomer, newCustomer));
        _strictListenerMock.Expect (mock => mock.RelationChanging (newCustomer, typeof (Customer).FullName + ".Orders", null, order));
        _strictListenerMock.Expect (mock => mock.RelationChanging (oldCustomer, typeof (Customer).FullName + ".Orders", order, null));
        _strictListenerMock.Expect (
            mock => mock.PropertyValueChanging (
                        order.InternalDataContainer,
                        order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".Customer"],
                        oldCustomer.ID,
                        newCustomer.ID));
        _strictListenerMock.Expect (
            mock => mock.PropertyValueChanged (
                        order.InternalDataContainer,
                        order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".Customer"],
                        oldCustomer.ID,
                        newCustomer.ID));
        _strictListenerMock.Expect (mock => mock.RelationChanged (order, typeof (Order).FullName + ".Customer"));
        _strictListenerMock.Expect (mock => mock.RelationChanged (newCustomer, typeof (Customer).FullName + ".Orders"));
        _strictListenerMock.Expect (mock => mock.RelationChanged (oldCustomer, typeof (Customer).FullName + ".Orders"));
      }

      _mockRepository.ReplayAll ();

      order.Customer = newCustomer;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FilterQueryResult ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQuery");
      var orders = (OrderCollection) ClientTransactionMock.QueryManager.GetCollection (query).ToCustomCollection ();

      ClientTransactionMock.AddListener (_strictListenerMock);

      var newQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();
      _strictListenerMock
          .Expect (mock => mock.FilterQueryResult (Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == orders.Count)))
          .Return (newQueryResult);

      _mockRepository.ReplayAll ();

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

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.TransactionCommitting (Arg<DomainObjectCollection>.Matches (doc => doc.Count == 1)));
        _strictListenerMock.Expect (mock => mock.TransactionCommitted (Arg<DomainObjectCollection>.Matches (doc => doc.Count == 1)));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionMock.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TransactionRollingBackTransactionRolledBack ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.TransactionRollingBack (Arg<DomainObjectCollection>.Matches (doc => doc.Count == 1)));
        _strictListenerMock.Expect (mock => mock.TransactionRolledBack (Arg<DomainObjectCollection>.Matches (doc => doc.Count == 1)));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionMock.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationEndPointMapRegistering ()
    {
      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.ObjectLoading (Arg<ObjectID>.Is.Anything));
        _strictListenerMock.Expect (mock => mock.ObjectGotID (Arg<DomainObject>.Is.Anything, Arg<ObjectID>.Is.Anything));
        _strictListenerMock.Expect (mock => mock.DataContainerMapRegistering (Arg<DataContainer>.Is.Anything));

        _strictListenerMock.Expect (
            mock => mock.RelationEndPointMapRegistering (
                        Arg<RelationEndPoint>.Matches (
                            rep => rep.PropertyName == typeof (Company).FullName + ".IndustrialSector" && rep.ObjectID == DomainObjectIDs.Customer1)));

        _strictListenerMock.Expect (mock => mock.ObjectsLoaded (Arg<DomainObjectCollection>.Is.Anything));
      }

      _mockRepository.ReplayAll ();

      Customer.GetObject (DomainObjectIDs.Customer1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationEndPointMapUnregisteringDataManagerMarkingObjectDiscardedDataContainerMapUnregistering ()
    {
      Order order = Order.NewObject ();

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.ObjectDeleting (order));
        _strictListenerMock.Expect (mock => mock.RelationEndPointMapPerformingDelete (Arg<RelationEndPointID[]>.Is.Anything));

        _strictListenerMock
            .Expect (mock => mock.RelationEndPointMapUnregistering (Arg<RelationEndPointID>.Matches (id => id.ObjectID == order.ID)))
            .Repeat.Times (4); // four related objects/object collections in Order

        _strictListenerMock.Expect (mock => mock.DataContainerMapUnregistering (order.InternalDataContainer));
        _strictListenerMock.Expect (mock => mock.DataManagerMarkingObjectDiscarded (order.ID));
        _strictListenerMock.Expect (mock => mock.ObjectDeleted (order));
      }

      _mockRepository.ReplayAll ();

      order.Delete ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void DataContainerMapRegistering ()
    {
      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.ObjectLoading (DomainObjectIDs.ClassWithAllDataTypes1));
        _strictListenerMock.Expect (mock => mock.ObjectGotID (Arg<DomainObject>.Is.Anything, Arg.Is (DomainObjectIDs.ClassWithAllDataTypes1)));

        _strictListenerMock.Expect (
            mock => mock.DataContainerMapRegistering (Arg<DataContainer>.Matches (dc => dc.ID == DomainObjectIDs.ClassWithAllDataTypes1)));

        _strictListenerMock.Expect (mock => mock.ObjectsLoaded (Arg<DomainObjectCollection>.Is.Anything));
      }

      _mockRepository.ReplayAll ();

      ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void DataManagerCopyingFromDataManagerCopyingTo ()
    {
      var transactionOne = new ClientTransactionMock ();
      var transactionTwo = new ClientTransactionMock ();
      DataManager managerOne = transactionOne.DataManager;
      DataManager managerTwo = transactionTwo.DataManager;

      transactionOne.AddListener (_strictListenerMock);
      transactionTwo.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.DataManagerCopyingFrom (managerOne));
        _strictListenerMock.Expect (mock => mock.DataManagerCopyingTo (managerTwo));
        _strictListenerMock.Expect (mock => mock.DataContainerMapCopyingFrom (managerOne.DataContainerMap));
        _strictListenerMock.Expect (mock => mock.DataContainerMapCopyingTo (managerTwo.DataContainerMap));
        _strictListenerMock.Expect (mock => mock.RelationEndPointMapCopyingFrom (managerOne.RelationEndPointMap));
        _strictListenerMock.Expect (mock => mock.RelationEndPointMapCopyingTo (managerTwo.RelationEndPointMap));
      }

      _mockRepository.ReplayAll ();

      managerTwo.CopyFrom (managerOne);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void SubTransactionCreatingSubTransactionCreated ()
    {
      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.SubTransactionCreating ());
        _strictListenerMock.Expect (
            mock =>
            mock.SubTransactionCreated (
                Arg<ClientTransaction>.Matches (tx => tx != null && tx != ClientTransactionMock && tx.ParentTransaction == ClientTransactionMock)));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionMock.CreateSubTransaction();

      _mockRepository.VerifyAll ();
    }
  }
}
