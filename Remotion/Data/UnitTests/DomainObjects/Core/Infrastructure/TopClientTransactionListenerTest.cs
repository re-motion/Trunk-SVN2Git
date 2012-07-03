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
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
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
    private Order _invalidObject;

    private TopClientTransactionListener _listener;

    private MockRepository _mockRepository;

    private DomainObjectMockEventReceiver _order1EventReceiverMock;
    private DomainObjectMockEventReceiver _order2EventReceiverMock;
    private DomainObjectMockEventReceiver _invalidObjectEventReceiverMock;
    
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
      _invalidObject = _clientTransaction.Execute (
          () =>
          {
            var order = Order.NewObject();
            order.Delete();
            return order;
          });

      _listener = new TopClientTransactionListener ();

      _mockRepository = new MockRepository();
      _order1EventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      _order2EventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order2);
      _invalidObjectEventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_invalidObject);

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
      CheckEventWithListenersLast (
          l => l.ObjectsLoaded (_clientTransaction, domainObjects),
          () =>
          {
            _loadEventReceiverMock.Expect (mock => mock.OnLoaded (_order1)).WithCurrentTransaction (_clientTransaction);
            _loadEventReceiverMock.Expect (mock => mock.OnLoaded (_order2)).WithCurrentTransaction (_clientTransaction);
            _transactionEventReceiverMock
                .Expect (
                    mock => mock.Loaded (
                        Arg.Is (_clientTransaction),
                        Arg<ClientTransactionEventArgs>.Matches (args => args.DomainObjects.SequenceEqual (domainObjects))))
                .WithCurrentTransaction (_clientTransaction);
          });
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
    public void PropertyValueChanging ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      object oldValue = "old";
      object newValue = "new";

      CheckEventWithListenersFirst (
          l => l.PropertyValueChanging (_clientTransaction, _order1, propertyDefinition, oldValue, newValue),
          () => _order1EventReceiverMock
              .Expect (mock => mock.PropertyChanging (_order1, propertyDefinition, oldValue, newValue))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void PropertyValueChanging_WithObjectIDProperty ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID();
      object oldValue = "old";
      object newValue = "new";

      CheckEventWithListenersFirst (
          l => l.PropertyValueChanging (_clientTransaction, _order1, propertyDefinition, oldValue, newValue),
          () => 
              _order1EventReceiverMock
                  .Expect (mock => mock.PropertyChanging (Arg<object>.Is.Anything, Arg<PropertyChangeEventArgs>.Is.Anything))
                  .Repeat.Never()
                  .Message ("No DomainObject event for foreign key properties."));
    }

    [Test]
    public void PropertyValueChanging_WithNulls ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      object oldValue = null;
      object newValue = null;

      CheckEventWithListenersFirst (
          l => l.PropertyValueChanging (_clientTransaction, _order1, propertyDefinition, oldValue, newValue),
          () => _order1EventReceiverMock
                    .Expect (mock => mock.PropertyChanging (_order1, propertyDefinition, oldValue, newValue))
                    .WithCurrentTransaction (_clientTransaction));
    }
    
    [Test]
    public void PropertyValueChanged ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      object oldValue = "old";
      object newValue = "new";

      CheckEventWithListenersLast (
          l => l.PropertyValueChanged (_clientTransaction, _order1, propertyDefinition, oldValue, newValue),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.PropertyChanged (_order1, propertyDefinition, oldValue, newValue))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void PropertyValueChanged_WithObjectIDProperty ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID();
      object oldValue = "old";
      object newValue = "new";

      CheckEventWithListenersLast (
          l => l.PropertyValueChanged (_clientTransaction, _order1, propertyDefinition, oldValue, newValue),
          () =>
              _order1EventReceiverMock
                  .Expect (mock => mock.PropertyChanged (Arg<object>.Is.Anything, Arg<PropertyChangeEventArgs>.Is.Anything))
                  .Repeat.Never ()
                  .Message ("No DomainObject event for foreign key properties."));
    }

    [Test]
    public void PropertyValueChanged_WithNulls ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      object oldValue = null;
      object newValue = null;

      CheckEventWithListenersLast (
          l => l.PropertyValueChanged (_clientTransaction, _order1, propertyDefinition, oldValue, newValue),
          () => _order1EventReceiverMock
                    .Expect (mock => mock.PropertyChanged (_order1, propertyDefinition, oldValue, newValue))
                    .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RelationChanging ()
    {
      var endPointDefinition = GetSomeEndPointDefinition();
      var oldValue = DomainObjectMother.CreateFakeObject();
      var newValue = DomainObjectMother.CreateFakeObject ();

      CheckEventWithListenersFirst (
          l => l.RelationChanging (_clientTransaction, _order1, endPointDefinition, oldValue, newValue),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.RelationChanging (_order1, endPointDefinition, oldValue, newValue))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RelationChanging_WithNulls ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();
      DomainObject oldValue = null;
      DomainObject newValue = null;

      CheckEventWithListenersFirst (
          l => l.RelationChanging (_clientTransaction, _order1, endPointDefinition, oldValue, newValue),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.RelationChanging (_order1, endPointDefinition, oldValue, newValue))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RelationChanged ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();
      var oldValue = DomainObjectMother.CreateFakeObject ();
      var newValue = DomainObjectMother.CreateFakeObject ();

      CheckEventWithListenersLast (
          l => l.RelationChanged (_clientTransaction, _order1, endPointDefinition, oldValue, newValue),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.RelationChanged (_order1, endPointDefinition, oldValue, newValue))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RelationChanged_WithNulls ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();
      DomainObject oldValue = null;
      DomainObject newValue = null;

      CheckEventWithListenersLast (
          l => l.RelationChanged (_clientTransaction, _order1, endPointDefinition, oldValue, newValue),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.RelationChanged (_order1, endPointDefinition, oldValue, newValue))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void TransactionCommitting ()
    {
      CheckEventWithListenersFirst (
          l => l.TransactionCommitting (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _order1, _order2 })),
          () =>
          {
            _transactionEventReceiverMock
                .Expect (mock => mock.Committing (_clientTransaction, _order1, _order2))
                .WithCurrentTransaction (_clientTransaction);
            _order1EventReceiverMock
                .Expect (mock => mock.Committing (_order1, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
            _order2EventReceiverMock
                .Expect (mock => mock.Committing (_order2, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
          });
    }

    [Test]
    public void TransactionCommitting_InvalidObject ()
    {
      CheckEventWithListenersFirst (
          l => l.TransactionCommitting (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _invalidObject })),
          () =>
          {
            _transactionEventReceiverMock
                .Expect (mock => mock.Committing (_clientTransaction, _invalidObject))
                .WithCurrentTransaction (_clientTransaction);
            _invalidObjectEventReceiverMock
                .Expect (mock => mock.Committing (Arg<object>.Is.Anything, Arg<EventArgs>.Is.Anything))
                .Repeat.Never()
                .Message ("DomainObject event should not be raised if object is made invalid.");
          });
    }

    [Test]
    public void TransactionCommitted ()
    {
      CheckEventWithListenersLast (
          l => l.TransactionCommitted (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _order1, _order2 })),
          () =>
          {
            _order2EventReceiverMock
                .Expect (mock => mock.Committed (_order2, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
            _order1EventReceiverMock
                .Expect (mock => mock.Committed (_order1, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
            _transactionEventReceiverMock
                .Expect (mock => mock.Committed (_clientTransaction, _order1, _order2))
                .WithCurrentTransaction (_clientTransaction);
          });
    }

    [Test]
    public void TransactionRollingBack ()
    {
      CheckEventWithListenersFirst (
          l => l.TransactionRollingBack (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _order1, _order2 })),
          () =>
          {
            _transactionEventReceiverMock
                .Expect (mock => mock.RollingBack (_clientTransaction, _order1, _order2))
                .WithCurrentTransaction (_clientTransaction);
            _order1EventReceiverMock
                .Expect (mock => mock.RollingBack (_order1, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
            _order2EventReceiverMock
                .Expect (mock => mock.RollingBack (_order2, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
          });
    }

    [Test]
    public void TransactionRollingBack_InvalidObjects ()
    {
      CheckEventWithListenersFirst (
          l => l.TransactionRollingBack (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _invalidObject })),
          () =>
          {
            _transactionEventReceiverMock
                .Expect (mock => mock.RollingBack (_clientTransaction, _invalidObject))
                .WithCurrentTransaction (_clientTransaction);
            _invalidObjectEventReceiverMock
                .Expect (mock => mock.RollingBack (Arg<DomainObject>.Is.Anything, Arg<EventArgs>.Is.Anything))
                .Repeat.Never ()
                .Message ("DomainObject event should not be raised if object is made invalid.");
          });
    }

    [Test]
    public void TransactionRolledBack ()
    {
      CheckEventWithListenersLast (
          l => l.TransactionRolledBack (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _order1, _order2 })),
          () =>
          {
            _order2EventReceiverMock
                .Expect (mock => mock.RolledBack (_order2, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
            _order1EventReceiverMock
                .Expect (mock => mock.RolledBack (_order1, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
            _transactionEventReceiverMock
                .Expect (mock => mock.RolledBack (_clientTransaction, _order1, _order2))
                .WithCurrentTransaction (_clientTransaction);
          });
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