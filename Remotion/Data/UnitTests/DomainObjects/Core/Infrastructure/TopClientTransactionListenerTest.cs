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

      _innerListenerMock = _mockRepository.StrictMock<IClientTransactionListener>();
      _listener.AddListener (_innerListenerMock);
    }

    [Test]
    [Ignore ("TODO 4619")]
    public void ObjectsUnloading ()
    {
      var unloadedDomainObjects = Array.AsReadOnly (new DomainObject[] { _order1, _order2 });

      using (_mockRepository.Ordered ())
      {
        _innerListenerMock.Expect (mock => mock.ObjectsUnloading (_clientTransaction, unloadedDomainObjects));
        _unloadEventReceiverMock
            .Expect (mock => mock.OnUnloading (_order1))
            .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_clientTransaction)));
        _unloadEventReceiverMock
            .Expect (mock => mock.OnUnloading (_order2))
            .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_clientTransaction)));
      }
      _mockRepository.ReplayAll();

      _listener.ObjectsUnloading (_clientTransaction, unloadedDomainObjects);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Serializable ()
    {
      var instance = new TopClientTransactionListener();
      instance.AddListener (new SerializableClientTransactionListenerFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.Listeners, Is.Not.Empty);
    }
  }
}