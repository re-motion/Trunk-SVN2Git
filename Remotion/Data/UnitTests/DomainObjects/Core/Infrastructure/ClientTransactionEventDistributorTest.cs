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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class ClientTransactionEventDistributorTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;
    private Order _order1;
    private Order _order2;
    private Order _invalidObject;

    private ClientTransactionEventDistributor _distributor;

    private MockRepository _mockRepository;

    private DomainObjectMockEventReceiver _order1EventReceiverMock;
    private DomainObjectMockEventReceiver _order2EventReceiverMock;
    private DomainObjectMockEventReceiver _invalidObjectEventReceiverMock;
    
    private IUnloadEventReceiver _unloadEventReceiverMock;
    private ILoadEventReceiver _loadEventReceiverMock;

    private ClientTransactionMockEventReceiver _transactionEventReceiverMock;

    private IClientTransactionExtension _extensionMock;
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

      _distributor = new ClientTransactionEventDistributor ();

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

      _extensionMock = _mockRepository.StrictMock<IClientTransactionExtension>();
      _extensionMock.Stub (stub => stub.Key).Return ("extension");
      _extensionMock.Replay();
      _distributor.Extensions.Add (_extensionMock);
      _extensionMock.BackToRecord ();

      _innerListenerMock = _mockRepository.StrictMock<IClientTransactionListener> ();
      _distributor.AddListener (_innerListenerMock);
    }

    [Test]
    public void TransactionInitialize ()
    {
      CheckEventWithListenersFirst (
          l => l.TransactionInitialize (_clientTransaction), 
          x => x.TransactionInitialize (_clientTransaction));
    }

    [Test]
    public void TransactionDiscard ()
    {
      CheckEventWithListenersFirst (
          l => l.TransactionDiscard (_clientTransaction), 
          x => x.TransactionDiscard (_clientTransaction));
    }

    [Test]
    public void SubTransactionCreating ()
    {
      CheckEventWithListenersFirst (
          l => l.SubTransactionCreating (_clientTransaction), 
          x => x.SubTransactionCreating (_clientTransaction));
    }

    [Test]
    public void SubTransactionInitialize ()
    {
      var subTransaction = ClientTransactionObjectMother.Create ();
      CheckEventWithListenersFirst (
          l => l.SubTransactionInitialize (_clientTransaction, subTransaction), 
          x => x.SubTransactionInitialize (_clientTransaction, subTransaction));
    }

    [Test]
    public void SubTransactionCreated ()
    {
      var subTransaction = ClientTransactionObjectMother.Create();

      CheckEventWithListenersLast (
          l => l.SubTransactionCreated (_clientTransaction, subTransaction),
          x => x.SubTransactionCreated (_clientTransaction, subTransaction),
          () => 
            _transactionEventReceiverMock
                    .Expect (
                        mock => mock.SubTransactionCreated (
                            Arg.Is (_clientTransaction),
                            Arg<SubTransactionCreatedEventArgs>.Matches (args => args.SubTransaction == subTransaction)))
                    .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void NewObjectCreating ()
    {
      CheckEventWithListenersFirst (
          l => l.NewObjectCreating (_clientTransaction, typeof (Order)),
          x => x.NewObjectCreating (_clientTransaction, typeof (Order)));
    }

    [Test]
    public void ObjectsLoading ()
    {
      var objectIDs = Array.AsReadOnly (new[] { DomainObjectIDs.Order1 });
      CheckEventWithListenersFirst (
          l => l.ObjectsLoading (_clientTransaction, objectIDs),
          x => x.ObjectsLoading (_clientTransaction, objectIDs));
    }

    [Test]
    public void ObjectsLoaded ()
    {
      var domainObjects = Array.AsReadOnly (new DomainObject[] { _order1, _order2 });
      CheckEventWithListenersLast (
          l => l.ObjectsLoaded (_clientTransaction, domainObjects),
          x => x.ObjectsLoaded (_clientTransaction, domainObjects),
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
          x => x.ObjectsUnloading (_clientTransaction, unloadedDomainObjects),
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
          x => x.ObjectsUnloaded (_clientTransaction, unloadedDomainObjects),
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
          x => x.ObjectDeleting (_clientTransaction, _order1),
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
          x => x.ObjectDeleted (_clientTransaction, _order1),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.Deleted (_order1, EventArgs.Empty))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void PropertyValueReading ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      CheckEventWithListenersFirst (
          l => l.PropertyValueReading (_clientTransaction, _order1, propertyDefinition, ValueAccess.Current),
          x => x.PropertyValueReading (_clientTransaction, _order1, propertyDefinition, ValueAccess.Current));
    }

    [Test]
    public void PropertyValueRead ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      CheckEventWithListenersLast (
          l => l.PropertyValueRead (_clientTransaction, _order1, propertyDefinition, 17,  ValueAccess.Current),
          x => x.PropertyValueRead (_clientTransaction, _order1, propertyDefinition, 17, ValueAccess.Current));
    }

    [Test]
    public void PropertyValueRead_WithNull ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      CheckEventWithListenersLast (
          l => l.PropertyValueRead (_clientTransaction, _order1, propertyDefinition, null, ValueAccess.Current),
          x => x.PropertyValueRead (_clientTransaction, _order1, propertyDefinition, null, ValueAccess.Current));
    }

    [Test]
    public void PropertyValueChanging ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      object oldValue = "old";
      object newValue = "new";

      CheckEventWithListenersFirst (
          l => l.PropertyValueChanging (_clientTransaction, _order1, propertyDefinition, oldValue, newValue),
          x => x.PropertyValueChanging (_clientTransaction, _order1, propertyDefinition, oldValue, newValue),
          () => _order1EventReceiverMock
              .Expect (mock => mock.PropertyChanging (propertyDefinition, oldValue, newValue))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void PropertyValueChanging_WithNulls ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();

      CheckEventWithListenersFirst (
          l => l.PropertyValueChanging (_clientTransaction, _order1, propertyDefinition, null, null),
          x => x.PropertyValueChanging (_clientTransaction, _order1, propertyDefinition, null, null),
          () => _order1EventReceiverMock
                    .Expect (mock => mock.PropertyChanging (propertyDefinition, null, null))
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
          x => x.PropertyValueChanged (_clientTransaction, _order1, propertyDefinition, oldValue, newValue),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.PropertyChanged (propertyDefinition, oldValue, newValue))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void PropertyValueChanged_WithNulls ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();

      CheckEventWithListenersLast (
          l => l.PropertyValueChanged (_clientTransaction, _order1, propertyDefinition, null, null),
          x => x.PropertyValueChanged (_clientTransaction, _order1, propertyDefinition, null, null),
          () => _order1EventReceiverMock
                    .Expect (mock => mock.PropertyChanged (propertyDefinition, null, null))
                    .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RelationReading ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();

      CheckEventWithListenersFirst (
          l => l.RelationReading (_clientTransaction, _order1, endPointDefinition, ValueAccess.Current),
          x => x.RelationReading (_clientTransaction, _order1, endPointDefinition, ValueAccess.Current));
    }

    [Test]
    public void RelationRead_Object ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();

      CheckEventWithListenersLast (
          l => l.RelationRead (_clientTransaction, _order1, endPointDefinition, _order2, ValueAccess.Current),
          x => x.RelationRead (_clientTransaction, _order1, endPointDefinition, _order2, ValueAccess.Current));
    }

    [Test]
    public void RelationRead_Object_WithNull ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();

      CheckEventWithListenersLast (
          l => l.RelationRead (_clientTransaction, _order1, endPointDefinition, (DomainObject) null, ValueAccess.Current),
          x => x.RelationRead (_clientTransaction, _order1, endPointDefinition, (DomainObject) null, ValueAccess.Current));
    }

    [Test]
    public void RelationRead_Collection ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();

      var relatedObjects = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (new DomainObjectCollection());
      CheckEventWithListenersLast (
          l => l.RelationRead (_clientTransaction, _order1, endPointDefinition, relatedObjects, ValueAccess.Current),
          x => x.RelationRead (_clientTransaction, _order1, endPointDefinition, relatedObjects, ValueAccess.Current));
    }

    [Test]
    public void RelationChanging ()
    {
      var endPointDefinition = GetSomeEndPointDefinition();
      var oldValue = DomainObjectMother.CreateFakeObject();
      var newValue = DomainObjectMother.CreateFakeObject ();

      CheckEventWithListenersFirst (
          l => l.RelationChanging (_clientTransaction, _order1, endPointDefinition, oldValue, newValue),
          x => x.RelationChanging (_clientTransaction, _order1, endPointDefinition, oldValue, newValue),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.RelationChanging (endPointDefinition, oldValue, newValue))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RelationChanging_WithNulls ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();

      CheckEventWithListenersFirst (
          l => l.RelationChanging (_clientTransaction, _order1, endPointDefinition, null, null),
          x => x.RelationChanging (_clientTransaction, _order1, endPointDefinition, null, null),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.RelationChanging (endPointDefinition, null, null))
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
          x => x.RelationChanged (_clientTransaction, _order1, endPointDefinition, oldValue, newValue),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.RelationChanged (endPointDefinition, oldValue, newValue))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RelationChanged_WithNulls ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();

      CheckEventWithListenersLast (
          l => l.RelationChanged (_clientTransaction, _order1, endPointDefinition, null, null),
          x => x.RelationChanged (_clientTransaction, _order1, endPointDefinition, null, null),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.RelationChanged (endPointDefinition, null, null))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void FilterQueryResult ()
    {
      var queryResult1 = QueryResultObjectMother.CreateQueryResult<Order> ();
      var queryResult2 = QueryResultObjectMother.CreateQueryResult<Order> ();
      var queryResult3 = QueryResultObjectMother.CreateQueryResult<Order> ();
      using (_mockRepository.Ordered ())
      {
        _innerListenerMock.Expect (l => l.FilterQueryResult (_clientTransaction, queryResult1)).Return (queryResult2);
        _extensionMock.Expect (x => x.FilterQueryResult (_clientTransaction, queryResult2)).Return (queryResult3);
      }
      _mockRepository.ReplayAll ();

      var result = _distributor.FilterQueryResult (_clientTransaction, queryResult1);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.SameAs (queryResult3));
    }

    [Test]
    public void TransactionCommitting ()
    {
      var eventRegistrar = MockRepository.GenerateStub<ICommittingEventRegistrar>();
      CheckEventWithListenersFirst (
          l => l.TransactionCommitting (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _order1, _order2 }), eventRegistrar),
          x => x.Committing (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _order1, _order2 }), eventRegistrar),
          () =>
          {
            _transactionEventReceiverMock
                .Expect (
                    mock =>
                    mock.Committing (
                        Arg.Is (_clientTransaction),
                        Arg<ClientTransactionCommittingEventArgs>.Matches (
                            args => args.DomainObjects.SetEquals (new[] { _order1, _order2 }) && args.EventRegistrar == eventRegistrar)))
                .WithCurrentTransaction (_clientTransaction);
            _order1EventReceiverMock
                .Expect (mock => mock.Committing (
                    Arg.Is (_order1), 
                    Arg<DomainObjectCommittingEventArgs>.Matches (args => args.EventRegistrar == eventRegistrar)))
                .WithCurrentTransaction (_clientTransaction);
            _order2EventReceiverMock
                .Expect (mock => mock.Committing (
                    Arg.Is (_order2),
                    Arg<DomainObjectCommittingEventArgs>.Matches (args => args.EventRegistrar == eventRegistrar)))
                .WithCurrentTransaction (_clientTransaction);
          });
    }

   [Test]
    public void TransactionCommitting_InvalidObject ()
    {
      var eventRegistrar = MockRepository.GenerateStub<ICommittingEventRegistrar> ();
      CheckEventWithListenersFirst (
          l => l.TransactionCommitting (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _invalidObject }), eventRegistrar),
          x => x.Committing (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _invalidObject }), eventRegistrar),
          () =>
          {
            _transactionEventReceiverMock
                .Expect (mock => mock.Committing (new[] { _invalidObject }))
                .WithCurrentTransaction (_clientTransaction);
            _invalidObjectEventReceiverMock
                .Expect (mock => mock.Committing (Arg<object>.Is.Anything, Arg<DomainObjectCommittingEventArgs>.Is.Anything))
                .Repeat.Never()
                .Message ("DomainObject event should not be raised if object is made invalid.");
          });
    }

   [Test]
   public void TransactionCommitValidate ()
   {
     var data1 = PersistableDataObjectMother.Create ();
     var data2 = PersistableDataObjectMother.Create ();
     CheckEventWithListenersFirst (
         l => l.TransactionCommitValidate (_clientTransaction, Array.AsReadOnly (new[] { data1, data2 })),
         x => x.CommitValidate (_clientTransaction, Array.AsReadOnly (new[] { data1, data2 })));
   }

    [Test]
    public void TransactionCommitted ()
    {
      CheckEventWithListenersLast (
          l => l.TransactionCommitted (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _order1, _order2 })),
          x => x.Committed (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _order1, _order2 })),
          () =>
          {
            _order2EventReceiverMock
                .Expect (mock => mock.Committed (_order2, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
            _order1EventReceiverMock
                .Expect (mock => mock.Committed (_order1, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
            _transactionEventReceiverMock
                .Expect (mock => mock.Committed (_order1, _order2))
                .WithCurrentTransaction (_clientTransaction);
          });
    }

    [Test]
    public void TransactionRollingBack ()
    {
      CheckEventWithListenersFirst (
          l => l.TransactionRollingBack (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _order1, _order2 })),
          x => x.RollingBack (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _order1, _order2 })),
          () =>
          {
            _transactionEventReceiverMock
                .Expect (mock => mock.RollingBack (_order1, _order2))
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
          x => x.RollingBack (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _invalidObject })),
          () =>
          {
            _transactionEventReceiverMock
                .Expect (mock => mock.RollingBack (new[] { _invalidObject }))
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
          x => x.RolledBack (_clientTransaction, Array.AsReadOnly (new DomainObject[] { _order1, _order2 })),
          () =>
          {
            _order2EventReceiverMock
                .Expect (mock => mock.RolledBack (_order2, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
            _order1EventReceiverMock
                .Expect (mock => mock.RolledBack (_order1, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
            _transactionEventReceiverMock
                .Expect (mock => mock.RolledBack (_order1, _order2))
                .WithCurrentTransaction (_clientTransaction);
          });
    }

    [Test]
    public void Serializable ()
    {
      var instance = new ClientTransactionEventDistributor();
      instance.AddListener (new SerializableClientTransactionListenerFake ());
      instance.Extensions.Add (new SerializableClientTransactionExtensionFake ("bla"));

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.Listeners, Is.Not.Empty);
      Assert.That (deserializedInstance.Extensions, Is.Not.Empty);
    }

    private void CheckEventWithListenersLast (
        Action<IClientTransactionListener> triggeringEvent, Action<IClientTransactionExtension> extensionEvent, Action orderedPreListenerExpectations = null)
    {
      using (_mockRepository.Ordered ())
      {
        if (orderedPreListenerExpectations != null)
          orderedPreListenerExpectations();
        _extensionMock.Expect (extensionEvent);
        _innerListenerMock.Expect (triggeringEvent);
      }
      _mockRepository.ReplayAll ();

      triggeringEvent (_distributor);

      _mockRepository.VerifyAll ();
    }

    private void CheckEventWithListenersFirst (
        Action<IClientTransactionListener> triggeringEvent, Action<IClientTransactionExtension> extensionEvent, Action orderedPostListenerExpectations = null)
    {
      using (_mockRepository.Ordered ())
      {
        _innerListenerMock.Expect (triggeringEvent);
        _extensionMock.Expect (extensionEvent);
        if (orderedPostListenerExpectations != null)
          orderedPostListenerExpectations();
      }
      _mockRepository.ReplayAll ();

      triggeringEvent (_distributor);

      _mockRepository.VerifyAll ();
    }
  }
}