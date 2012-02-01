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
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands
{
  [TestFixture]
  public class UnloadAllCommandTest : StandardMappingTest
  {
    private IRelationEndPointManager _endPointManagerMock;
    private ClientTransaction _clientTransaction;
    private DataContainerMap _dataContainerMap;
    private IClientTransactionListener _transactionEventSinkMock;
    private IInvalidDomainObjectManager _invalidDomainObjectManagerMock;

    private DataContainer _existingDataContainer;
    private TestDomainBase _existingDomainObject;

    private DataContainer _newDataContainer;
    private TestDomainBase _newDomainObject;

    private UnloadAllCommand _unloadCommand;

    public override void SetUp ()
    {
      base.SetUp ();
      _endPointManagerMock = MockRepository.GenerateStrictMock<IRelationEndPointManager>();
      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _dataContainerMap = new DataContainerMap (_clientTransaction);
      _transactionEventSinkMock = MockRepository.GenerateStrictMock<IClientTransactionListener>();
      _invalidDomainObjectManagerMock = MockRepository.GenerateStrictMock<IInvalidDomainObjectManager>();

      _existingDataContainer = CreateExistingDataContainer ();
      _existingDomainObject = (TestDomainBase) _existingDataContainer.DomainObject;

      _newDataContainer = CreateNewDataContainer ();
      _newDomainObject = (TestDomainBase) _newDataContainer.DomainObject;

      _unloadCommand = new UnloadAllCommand (
          _endPointManagerMock, _dataContainerMap, _invalidDomainObjectManagerMock, _clientTransaction, _transactionEventSinkMock);
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      _dataContainerMap.Register (_existingDataContainer);
      _dataContainerMap.Register (_newDataContainer);

      // Order of registration
      _transactionEventSinkMock
          .Expect (
              mock => mock.ObjectsUnloading (
                  Arg.Is (_clientTransaction),
                  Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _existingDomainObject, _newDomainObject })));
      _transactionEventSinkMock.Replay ();

      _unloadCommand.NotifyClientTransactionOfBegin ();

      _transactionEventSinkMock.VerifyAllExpectations();
    }

    [Test]
    public void NotifyClientTransactionOfBegin_ReexecutedForNewlyRegisteredObjects ()
    {
      _dataContainerMap.Register (_existingDataContainer);

      _transactionEventSinkMock
          .Expect (
              mock => mock.ObjectsUnloading (
                  Arg.Is (_clientTransaction),
                  Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _existingDomainObject })))
          .WhenCalled (mi => _dataContainerMap.Register (_newDataContainer));
      _transactionEventSinkMock
          .Expect (
              mock => mock.ObjectsUnloading (
                  Arg.Is (_clientTransaction),
                  Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _newDomainObject })));
      _transactionEventSinkMock.Replay ();

      _unloadCommand.NotifyClientTransactionOfBegin ();

      _transactionEventSinkMock.VerifyAllExpectations ();
    }

    [Test]
    public void Begin ()
    {
      _dataContainerMap.Register (_existingDataContainer);
      _dataContainerMap.Register (_newDataContainer);

      var unloadEventReceiverMock = MockRepository.GenerateStrictMock<IUnloadEventReceiver>();
      _existingDomainObject.SetUnloadEventReceiver (unloadEventReceiverMock);
      _newDomainObject.SetUnloadEventReceiver (unloadEventReceiverMock);

      using (unloadEventReceiverMock.GetMockRepository ().Ordered ())
      {
        // Order of registration
        unloadEventReceiverMock
            .Expect (mock => mock.OnUnloading (_existingDomainObject))
            .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_clientTransaction)));
        unloadEventReceiverMock
            .Expect (mock => mock.OnUnloading (_newDomainObject))
            .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_clientTransaction)));
      }
      unloadEventReceiverMock.Replay ();

      _unloadCommand.Begin ();

      unloadEventReceiverMock.VerifyAllExpectations();
    }

    [Test]
    public void Begin_ReexecutedForNewlyRegisteredObjects ()
    {
      _dataContainerMap.Register (_existingDataContainer);

      var unloadEventReceiverMock = MockRepository.GenerateStrictMock<IUnloadEventReceiver> ();
      _existingDomainObject.SetUnloadEventReceiver (unloadEventReceiverMock);
      _newDomainObject.SetUnloadEventReceiver (unloadEventReceiverMock);

      unloadEventReceiverMock
          .Expect (mock => mock.OnUnloading (_existingDomainObject))
          .WhenCalled (mi => _dataContainerMap.Register (_newDataContainer));
      unloadEventReceiverMock
          .Expect (mock => mock.OnUnloading (_newDomainObject));
      unloadEventReceiverMock.Replay ();

      _unloadCommand.Begin ();

      unloadEventReceiverMock.VerifyAllExpectations ();
    }

    [Test]
    public void Perform_ClearsAndDiscardsDataContainers_AndResetsEndPoints ()
    {
      _dataContainerMap.Register (_existingDataContainer);
      Assert.That (_dataContainerMap, Is.Not.Empty.And.Member (_existingDataContainer));

      _endPointManagerMock
          .Expect (mock => mock.Reset ())
          .WhenCalled (mi => Assert.That (_dataContainerMap, Is.Not.Empty));
      _endPointManagerMock.Replay ();

      _unloadCommand.Perform();

      Assert.That (_dataContainerMap, Is.Empty);
      _endPointManagerMock.VerifyAllExpectations ();
      Assert.That (_existingDataContainer.IsDiscarded, Is.True);
    }

    [Test]
    public void Perform_RaisesDataContainerUnregisteringEvents ()
    {
      _dataContainerMap.Register (_existingDataContainer);
      Assert.That (_dataContainerMap, Is.Not.Empty.And.Member (_existingDataContainer));
      _endPointManagerMock.Stub (mock => mock.Reset ());

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_dataContainerMap.ClientTransaction);

      _unloadCommand.Perform();

      listenerMock.AssertWasCalled (mock => mock.DataContainerMapUnregistering (_dataContainerMap.ClientTransaction, _existingDataContainer));
    }

    [Test]
    public void Perform_InvalidatesAndDiscardsNewDataContainers ()
    {
      _dataContainerMap.Register (_newDataContainer);
      _endPointManagerMock.Stub (mock => mock.Reset ());

      _invalidDomainObjectManagerMock.Expect (mock => mock.MarkInvalid (_newDataContainer.DomainObject)).Return (true);
      _invalidDomainObjectManagerMock.Replay ();

      _unloadCommand.Perform ();

      _invalidDomainObjectManagerMock.VerifyAllExpectations ();
      Assert.That (_newDataContainer.IsDiscarded, Is.True);
    }

    [Test]
    public void End_WithoutPerform ()
    {
      _dataContainerMap.Register (_existingDataContainer);
      _dataContainerMap.Register (_newDataContainer);
      
      var unloadEventReceiverMock = MockRepository.GenerateStrictMock<IUnloadEventReceiver> ();
      _existingDomainObject.SetUnloadEventReceiver (unloadEventReceiverMock);
      _newDomainObject.SetUnloadEventReceiver (unloadEventReceiverMock);

      unloadEventReceiverMock.Replay ();

      _unloadCommand.End ();

      unloadEventReceiverMock.AssertWasNotCalled (mock => mock.OnUnloaded (Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void End_WithPerform ()
    {
      _dataContainerMap.Register (_existingDataContainer);
      _dataContainerMap.Register (_newDataContainer);

      _invalidDomainObjectManagerMock.Stub (mock => mock.MarkInvalid (Arg<DomainObject>.Is.Anything)).Return (true);
      _endPointManagerMock.Stub (mock => mock.Reset());
      _unloadCommand.Perform ();
      
      var unloadEventReceiverMock = MockRepository.GenerateStrictMock<IUnloadEventReceiver> ();
      _existingDomainObject.SetUnloadEventReceiver (unloadEventReceiverMock);
      _newDomainObject.SetUnloadEventReceiver (unloadEventReceiverMock);
      
      using (unloadEventReceiverMock.GetMockRepository ().Ordered ())
      {
        // Reverse order of registration
        unloadEventReceiverMock
            .Expect (mock => mock.OnUnloaded (_newDataContainer.DomainObject))
            .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_clientTransaction)));
        unloadEventReceiverMock
            .Expect (mock => mock.OnUnloaded (_existingDataContainer.DomainObject))
            .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_clientTransaction)));
      }
      unloadEventReceiverMock.Replay ();

      _unloadCommand.End ();

      unloadEventReceiverMock.VerifyAllExpectations();
    }

    [Test]
    public void NotifyClientTransactionOfEnd_WithoutPerform ()
    {
      _dataContainerMap.Register (_existingDataContainer);
      _dataContainerMap.Register (_newDataContainer);

      _unloadCommand.NotifyClientTransactionOfEnd();

      _transactionEventSinkMock.AssertWasNotCalled (
          mock => mock.ObjectsUnloaded (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
    }

    [Test]
    public void NotifyClientTransactionOfEnd_WithPerform ()
    {
      _dataContainerMap.Register (_existingDataContainer);
      _dataContainerMap.Register (_newDataContainer);

      _invalidDomainObjectManagerMock.Stub (mock => mock.MarkInvalid (Arg<DomainObject>.Is.Anything)).Return (true);
      _endPointManagerMock.Stub (mock => mock.Reset ());
      _unloadCommand.Perform ();

      // Order of registration
      _transactionEventSinkMock.Expect (
          mock => mock.ObjectsUnloaded (
              Arg.Is (_clientTransaction),
              Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _existingDataContainer.DomainObject, _newDataContainer.DomainObject })));

      _unloadCommand.NotifyClientTransactionOfEnd ();

      _transactionEventSinkMock.VerifyAllExpectations();
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var result = _unloadCommand.ExpandToAllRelatedObjects();

      Assert.That (result.GetNestedCommands(), Is.EqualTo (new[] { _unloadCommand }));
    }

    private DataContainer CreateExistingDataContainer ()
    {
      var dataContainer = DataContainer.CreateForExisting (new ObjectID (typeof (Order), Guid.NewGuid ()), null, pd => pd.DefaultValue);
      dataContainer.SetDomainObject (LifetimeService.GetObjectReference (_clientTransaction, dataContainer.ID));
      return dataContainer;
    }

    private DataContainer CreateNewDataContainer ()
    {
      var dataContainer = DataContainer.CreateNew (new ObjectID (typeof (Order), Guid.NewGuid ()));
      dataContainer.SetDomainObject (LifetimeService.GetObjectReference(_clientTransaction, dataContainer.ID));
      return dataContainer;
    }
  }
}