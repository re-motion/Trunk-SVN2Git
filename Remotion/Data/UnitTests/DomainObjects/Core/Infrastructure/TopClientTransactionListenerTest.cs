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
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class TopClientTransactionListenerTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;
    private Order _order1;
    private Order _order2;

    private TopClientTransactionListener _listener;

    private MockRepository _mockRepository;
    private DomainObjectMockEventReceiver _order1EventReceiverMock;
    private DomainObjectMockEventReceiver _order2EventReceiverMock;
    private IUnloadEventReceiver _unloadEventReceiverMock;
    private ILoadEventReceiver _loadEventReceiverMock;

    private ClientTransactionMockEventReceiver _transactionEventReceiverMock;

    private IClientTransactionListener _innerListenerMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _order1 = _clientTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      _order2 = _clientTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order2));

      _listener = new TopClientTransactionListener ();

      _mockRepository = new MockRepository();
      _order1EventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      _order2EventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order2);

      _unloadEventReceiverMock = _mockRepository.StrictMock<IUnloadEventReceiver>();
      _order1.SetUnloadEventReceiver (_unloadEventReceiverMock);
      _order2.SetUnloadEventReceiver (_unloadEventReceiverMock);

      _transactionEventReceiverMock = _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_clientTransaction);

      _loadEventReceiverMock = _mockRepository.StrictMock<ILoadEventReceiver> ();
      _order1.SetLoadEventReceiver (_loadEventReceiverMock);
      _order2.SetLoadEventReceiver (_loadEventReceiverMock);

      _innerListenerMock = _mockRepository.StrictMock<IClientTransactionListener>();
      _listener.AddListener (_innerListenerMock);
    }

    [Test]
    public void SubTransactionCreated ()
    {
      var subTransaction = ClientTransactionObjectMother.Create();

      CheckEventWithListenersLast (
          l => l.SubTransactionCreated (_clientTransaction, subTransaction),
          () => 
            _transactionEventReceiverMock
                    .Expect (
                        mock => mock.SubTransactionCreated (
                            Arg.Is (_clientTransaction),
                            Arg<SubTransactionCreatedEventArgs>.Matches (args => args.SubTransaction == subTransaction)))
                    .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void ObjectsLoaded ()
    {
      var domainObjects = Array.AsReadOnly (new DomainObject[] { _order1, _order2 });
      CheckEventWithListenersInTheMiddle (
          l => l.ObjectsLoaded (_clientTransaction, domainObjects),
          () =>
          {
            _loadEventReceiverMock.Expect (mock => mock.OnLoaded (_order1)).WithCurrentTransaction (_clientTransaction);
            _loadEventReceiverMock.Expect (mock => mock.OnLoaded (_order2)).WithCurrentTransaction (_clientTransaction);
          },
          () =>
            _transactionEventReceiverMock
                    .Expect (
                        mock => mock.Loaded (
                            Arg.Is (_clientTransaction),
                            Arg<ClientTransactionEventArgs>.Matches (args => args.DomainObjects.SequenceEqual (domainObjects))))
                    .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void ObjectsUnloading ()
    {
      var unloadedDomainObjects = Array.AsReadOnly (new DomainObject[] { _order1, _order2 });

      CheckEventWithListenersFirst (
          l => l.ObjectsUnloading (_clientTransaction, unloadedDomainObjects),
          () =>
          {
            _unloadEventReceiverMock
                .Expect (mock => mock.OnUnloading (_order1))
                .WithCurrentTransaction (_clientTransaction);
            _unloadEventReceiverMock
                .Expect (mock => mock.OnUnloading (_order2))
                .WithCurrentTransaction (_clientTransaction);
          });
    }

    [Test]
    public void ObjectsUnloaded ()
    {
      var unloadedDomainObjects = Array.AsReadOnly (new DomainObject[] { _order1, _order2 });

      CheckEventWithListenersLast (
          l => l.ObjectsUnloaded (_clientTransaction, unloadedDomainObjects),
          () =>
          {
            _unloadEventReceiverMock
                .Expect (mock => mock.OnUnloaded (_order2))
                .WithCurrentTransaction (_clientTransaction);
            _unloadEventReceiverMock
                .Expect (mock => mock.OnUnloaded (_order1))
                .WithCurrentTransaction (_clientTransaction);
          });
    }

    [Test]
    public void ObjectDeleting ()
    {
      CheckEventWithListenersFirst (
          l => l.ObjectDeleting (_clientTransaction, _order1),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.Deleting (_order1, EventArgs.Empty))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void ObjectDeleted ()
    {
      CheckEventWithListenersLast (
          l => l.ObjectDeleted (_clientTransaction, _order1),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.Deleted (_order1, EventArgs.Empty))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void Serializable ()
    {
      var instance = new TopClientTransactionListener();
      instance.AddListener (new SerializableClientTransactionListenerFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.Listeners, Is.Not.Empty);
    }

    private void CheckEventWithListenersLast (Action<IClientTransactionListener> triggeringEvent, Action orderedPreListenerExpectations)
    {
      CheckEventWithListenersInTheMiddle (triggeringEvent, orderedPreListenerExpectations, () => { });
    }

    private void CheckEventWithListenersFirst (Action<IClientTransactionListener> triggeringEvent, Action orderedPostListenerExpectations)
    {
      CheckEventWithListenersInTheMiddle (triggeringEvent, () => { }, orderedPostListenerExpectations);
    }

    private void CheckEventWithListenersInTheMiddle (Action<IClientTransactionListener> triggeringEvent, Action orderedPreListenerExpectations, Action orderedPostListenerExpectations)
    {
      using (_mockRepository.Ordered ())
      {
        orderedPreListenerExpectations ();
        _innerListenerMock.Expect (triggeringEvent);
        orderedPostListenerExpectations ();
      }
      _mockRepository.ReplayAll ();

      triggeringEvent (_listener);

      _mockRepository.VerifyAll ();
    }
  }

  static class TestHelper
  {
    public static IMethodOptions<T> WithCurrentTransaction<T> (this IMethodOptions<T> options, ClientTransaction expectedTransaction)
    {
      return options.WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (expectedTransaction)));
    }
  }
}