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
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class ClientTransactionListenerManagerTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;
    private IRootClientTransactionListener _rootListener;

    private ClientTransactionListenerManager _listenerManager;

    private IClientTransactionListener _fakeListener;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _rootListener = MockRepository.GenerateStrictMock<IRootClientTransactionListener>();

      _listenerManager = new ClientTransactionListenerManager (_clientTransaction, _rootListener);

      _fakeListener = MockRepository.GenerateStub<IClientTransactionListener>();
    }

    [Test]
    public void Listeners ()
    {
      _rootListener.Stub (stub => stub.Listeners).Return (new[] { _fakeListener });

      Assert.That (_listenerManager.Listeners, Is.EqualTo (new[] { _fakeListener }));
    }

    [Test]
    public void AddListener()
    {
      _rootListener.Expect (mock => mock.AddListener (_fakeListener));
      _rootListener.Replay();
      
      _listenerManager.AddListener (_fakeListener);

      _rootListener.VerifyAllExpectations();
    }

    [Test]
    public void RemoveListener ()
    {
      _rootListener.Expect (mock => mock.RemoveListener (_fakeListener));
      _rootListener.Replay ();

      _listenerManager.RemoveListener (_fakeListener);

      _rootListener.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseEvent ()
    {
      _rootListener.Expect (mock => mock.TransactionInitialize (_clientTransaction));
      _rootListener.Replay();

      _listenerManager.RaiseEvent ((tx, l) => l.TransactionInitialize (tx));

      _rootListener.VerifyAllExpectations();
    }

    [Test]
    public void Serializable ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();
      var rootListener = new SerializableRootClientTransactionListenerFake();
      var instance = new ClientTransactionListenerManager (clientTransaction, rootListener);

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.RootListener, Is.Not.Null);
    }
  }
}