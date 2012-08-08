// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Remotion.Data.UnitTests.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions
{
  [TestFixture]
  public class RelationReadEventsTest : StandardMappingTest
  {
    private ClientTransaction _rootTransaction;
    private ClientTransaction _inactiveSubTransaction;
    private ClientTransaction _activeSubTransaction;

    private IClientTransactionListener _listenerDynamicMock;
    private IClientTransactionExtension _extensionStrictMock;

    private Order _loadedOrder;
    private Customer _relatedCustomer;
    private OrderTicket _relatedOrderTicket;
    private OrderItem _relatedOrderItem1;
    private OrderItem _relatedOrderItem2;

    public override void SetUp ()
    {
      base.SetUp();

      _rootTransaction = ClientTransaction.CreateRootTransaction();
      _inactiveSubTransaction = _rootTransaction.CreateSubTransaction();
      _activeSubTransaction = _inactiveSubTransaction.CreateSubTransaction();

      _loadedOrder = _activeSubTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      _relatedCustomer = _activeSubTransaction.Execute (() => Customer.GetObject (DomainObjectIDs.Customer1));
      _relatedOrderTicket = _activeSubTransaction.Execute (() => OrderTicket.GetObject (DomainObjectIDs.OrderTicket1));
      _relatedOrderItem1 = _activeSubTransaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem1));
      _relatedOrderItem2 = _activeSubTransaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem2));

      _listenerDynamicMock = MockRepository.GenerateMock<IClientTransactionListener>();
      _extensionStrictMock = MockRepository.GenerateStrictMock<IClientTransactionExtension>();

      ClientTransactionTestHelper.AddListener (_rootTransaction, _listenerDynamicMock);
      ClientTransactionTestHelper.AddListener (_inactiveSubTransaction, _listenerDynamicMock);
      ClientTransactionTestHelper.AddListener (_activeSubTransaction, _listenerDynamicMock);

      _extensionStrictMock.Stub (stub => stub.Key).Return ("test");
      _rootTransaction.Extensions.Add (_extensionStrictMock);
      _inactiveSubTransaction.Extensions.Add (_extensionStrictMock);
      _activeSubTransaction.Extensions.Add (_extensionStrictMock);
    }

    [Test]
    public void RelationReadEvents_OnlyRaisedInActiveSub_NonVirtualEndPoint ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "Customer");
      ExpectRelationReadEvents (_activeSubTransaction, _loadedOrder, endPointDefinition, _relatedCustomer);

      Dev.Null = _activeSubTransaction.Execute (() => _loadedOrder.Customer);

      AssertNoRelationReadEvents (_inactiveSubTransaction);
      AssertNoRelationReadEvents (_rootTransaction);

      _listenerDynamicMock.VerifyAllExpectations();
      _extensionStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void RelationReadEvents_OnlyRaisedInActiveSub_VirtualEndPoint_One ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");
      ExpectRelationReadEvents (_activeSubTransaction, _loadedOrder, endPointDefinition, _relatedOrderTicket);

      Dev.Null = _activeSubTransaction.Execute (() => _loadedOrder.OrderTicket);

      AssertNoRelationReadEvents (_inactiveSubTransaction);
      AssertNoRelationReadEvents (_rootTransaction);

      _listenerDynamicMock.VerifyAllExpectations ();
      _extensionStrictMock.VerifyAllExpectations ();
    }

    [Test]
    public void RelationReadEvents_OnlyRaisedInActiveSub_VirtualEndPoint_Many ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");
      ExpectRelationReadEvents (_activeSubTransaction, _loadedOrder, endPointDefinition, new[] { _relatedOrderItem1, _relatedOrderItem2 });

      _activeSubTransaction.Execute (() => _loadedOrder.OrderItems.EnsureDataComplete());

      AssertNoRelationReadEvents (_inactiveSubTransaction);
      AssertNoRelationReadEvents (_rootTransaction);

      _listenerDynamicMock.VerifyAllExpectations ();
      _extensionStrictMock.VerifyAllExpectations ();
    }

    private void ExpectRelationReadEvents (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition endPointDefinition,
        DomainObject relatedDomainObject)
    {
      using (_listenerDynamicMock.GetMockRepository().Ordered())
      {
        _listenerDynamicMock.Expect (mock => mock.RelationReading (clientTransaction, domainObject, endPointDefinition, ValueAccess.Current));
        _listenerDynamicMock.Expect (
            mock => mock.RelationRead (clientTransaction, domainObject, endPointDefinition, relatedDomainObject, ValueAccess.Current));
      }

      using (_extensionStrictMock.GetMockRepository().Ordered())
      {
        _extensionStrictMock.Expect (mock => mock.RelationReading (clientTransaction, domainObject, endPointDefinition, ValueAccess.Current));
        _extensionStrictMock.Expect (
            mock => mock.RelationRead (clientTransaction, domainObject, endPointDefinition, relatedDomainObject, ValueAccess.Current));
      }
    }

    private void ExpectRelationReadEvents (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition endPointDefinition,
        DomainObject[] relatedDomainObjects)
    {
      using (_listenerDynamicMock.GetMockRepository ().Ordered ())
      {
        _listenerDynamicMock.Expect (mock => mock.RelationReading (clientTransaction, domainObject, endPointDefinition, ValueAccess.Current));
        _listenerDynamicMock.Expect (
            mock => mock.RelationRead (
                Arg.Is (clientTransaction),
                Arg.Is (domainObject),
                Arg.Is (endPointDefinition),
                Arg<ReadOnlyDomainObjectCollectionAdapter<DomainObject>>.List.Equivalent (relatedDomainObjects),
                Arg.Is (ValueAccess.Current)));
      }

      using (_extensionStrictMock.GetMockRepository ().Ordered ())
      {
        _extensionStrictMock.Expect (mock => mock.RelationReading (clientTransaction, domainObject, endPointDefinition, ValueAccess.Current));
        _extensionStrictMock.Expect (
            mock => mock.RelationRead (
                Arg.Is (clientTransaction),
                Arg.Is (domainObject),
                Arg.Is (endPointDefinition),
                Arg<ReadOnlyDomainObjectCollectionAdapter<DomainObject>>.List.Equivalent (relatedDomainObjects),
                Arg.Is (ValueAccess.Current)));
      }
    }

    private void AssertNoRelationReadEvents (ClientTransaction clientTransaction)
    {
      _listenerDynamicMock.AssertWasNotCalled (
          mock => mock.RelationReading (
              Arg.Is (clientTransaction),
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything,
              Arg<ValueAccess>.Is.Anything));
      _listenerDynamicMock.AssertWasNotCalled (
          mock => mock.RelationRead (
              Arg.Is (clientTransaction),
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<ValueAccess>.Is.Anything));
      _listenerDynamicMock.AssertWasNotCalled (
          mock => mock.RelationRead (
              Arg.Is (clientTransaction),
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything,
              Arg<ReadOnlyDomainObjectCollectionAdapter<DomainObject>>.Is.Anything,
              Arg<ValueAccess>.Is.Anything));

      _extensionStrictMock.AssertWasNotCalled (
          mock => mock.RelationReading (
              Arg.Is (clientTransaction),
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything,
              Arg<ValueAccess>.Is.Anything));
      _extensionStrictMock.AssertWasNotCalled (
          mock => mock.RelationRead (
              Arg.Is (clientTransaction),
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<ValueAccess>.Is.Anything));
      _extensionStrictMock.AssertWasNotCalled (
          mock => mock.RelationRead (
              Arg.Is (clientTransaction),
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything,
              Arg<ReadOnlyDomainObjectCollectionAdapter<DomainObject>>.Is.Anything,
              Arg<ValueAccess>.Is.Anything));
    }
  }
}