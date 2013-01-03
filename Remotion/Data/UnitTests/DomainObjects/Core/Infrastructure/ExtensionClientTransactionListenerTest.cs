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
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class ExtensionClientTransactionListenerTest : StandardMappingTest
  {
    private IClientTransactionExtension _extensionMock;
    private ExtensionClientTransactionListener _listener;
    private ClientTransaction _clientTransaction;

    public override void SetUp ()
    {
      base.SetUp();

      _extensionMock = MockRepository.GenerateStrictMock<IClientTransactionExtension>();
      _listener = new ExtensionClientTransactionListener (_extensionMock);
    }

    [Test]
    public void TransactionInitialize_Delegated ()
    {
      _clientTransaction = ClientTransaction.CreateRootTransaction ();

      ExpectDelegation (l => l.TransactionInitialize (_clientTransaction), e => e.TransactionInitialize (_clientTransaction));
    }

    [Test]
    public void TransactionDiscard_Delegated ()
    {
      _clientTransaction = ClientTransaction.CreateRootTransaction ();

      ExpectDelegation (l => l.TransactionDiscard (_clientTransaction), e => e.TransactionDiscard (_clientTransaction));
    }

    [Test]
    public void SubTransactionEvents_Delegated ()
    {
      var tx2 = ClientTransaction.CreateRootTransaction();

      ExpectDelegation (l => l.SubTransactionCreating (_clientTransaction), e => e.SubTransactionCreating (_clientTransaction));
      ExpectDelegation (l => l.SubTransactionInitialize (_clientTransaction, tx2), e => e.SubTransactionInitialize (_clientTransaction, tx2));
      ExpectDelegation (l => l.SubTransactionCreated (_clientTransaction, tx2), e => e.SubTransactionCreated (_clientTransaction, tx2));
    }

    [Test]
    public void RelationChangeEvents_Delegated ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      var relationEndPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<OrderItem> ();
      var newRelatedObject = DomainObjectMother.CreateFakeObject<OrderItem> ();

      ExpectDelegation (
          l => l.RelationChanging (_clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject),
          e => e.RelationChanging (_clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject));
      ExpectDelegation (
          l => l.RelationChanged (_clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject),
          e => e.RelationChanged (_clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject));
    }

    [Test]
    public void TransactionCommitEvents_Delegated ()
    {
      var domainObjects = Array.AsReadOnly (new DomainObject[0]);
      var persistableData = Array.AsReadOnly (new PersistableData[0]);
      var eventRegistrar = MockRepository.GenerateStub<ICommittingEventRegistrar>();

      ExpectDelegation (
          l => l.TransactionCommitting (_clientTransaction, domainObjects, eventRegistrar), 
          e => e.Committing (_clientTransaction, domainObjects, eventRegistrar));
      ExpectDelegation (l => l.TransactionCommitValidate (_clientTransaction, persistableData), e => e.CommitValidate (_clientTransaction, persistableData));
      ExpectDelegation (l => l.TransactionCommitted (_clientTransaction, domainObjects), e => e.Committed (_clientTransaction, domainObjects));
    }

    [Test]
    public void FilterQueryResultEvents_Delegated ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery>();
      var queryResult = new QueryResult<Order> (queryStub, new Order[0]);
      var fakeQueryResult = new QueryResult<Order> (queryStub, new[] { DomainObjectMother.CreateFakeObject<Order>() });
      ExpectDelegation (l => l.FilterQueryResult (_clientTransaction, queryResult), e => e.FilterQueryResult (_clientTransaction, queryResult), fakeQueryResult);
    }

    private void ExpectDelegation (Action<IClientTransactionListener> listenerAction, Action<IClientTransactionExtension> expectedExtensionAction)
    {
      _extensionMock.Expect (expectedExtensionAction);
      _extensionMock.Replay();

      listenerAction (_listener);

      _extensionMock.VerifyAllExpectations();
    }

    private void ExpectDelegation<TR> (
        Function<IClientTransactionListener, TR> listenerFunc, Function<IClientTransactionExtension, TR> expectedExtensionFunc, TR fakeResult)
    {
      _extensionMock.Expect (expectedExtensionFunc).Return (fakeResult);
      _extensionMock.Replay ();

      var result = listenerFunc (_listener);

      _extensionMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }
  }
}