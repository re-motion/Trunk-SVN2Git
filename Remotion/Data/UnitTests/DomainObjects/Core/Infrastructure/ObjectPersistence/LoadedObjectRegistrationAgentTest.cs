// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class LoadedObjectRegistrationAgentTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;

    private MockRepository _mockRepository;
    private IClientTransactionListener _eventSinkMock;
    private IDataManager _dataManagerMock;
    private ClientTransactionMockEventReceiver _transactionEventReceiverMock;

    private LoadedObjectRegistrationAgent _agent;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientTransaction = ClientTransaction.CreateRootTransaction();

      _mockRepository = new MockRepository();
      _eventSinkMock = _mockRepository.StrictMock<IClientTransactionListener> ();
      _dataManagerMock = _mockRepository.StrictMock<IDataManager> ();
      _transactionEventReceiverMock = _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_clientTransaction);

      _agent = new LoadedObjectRegistrationAgent (_clientTransaction, _eventSinkMock);
    }

    [Test]
    public void RegisterIfRequired_Single_AlreadyExistingLoadedObject ()
    {
      var alreadyExistingLoadedObject = GetAlreadyExistingLoadedObject();

      _mockRepository.ReplayAll();

      var result = _agent.RegisterIfRequired (alreadyExistingLoadedObject, _dataManagerMock);

      _eventSinkMock.AssertWasNotCalled (
          mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _dataManagerMock.AssertWasNotCalled (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything));
      _transactionEventReceiverMock.AssertWasNotCalled (mock => mock.Loaded (Arg<object>.Is.Anything, Arg<ClientTransactionEventArgs>.Is.Anything));

      Assert.That (result, Is.SameAs (alreadyExistingLoadedObject.ExistingDataContainer.DomainObject));
    }

    [Test]
    public void RegisterIfRequired_Single_FreshlyLoadedObject ()
    {
      var freshlyLoadedObject = GetFreshlyLoadedObject();
      var dataContainer = freshlyLoadedObject.FreshlyLoadedDataContainer;
      Assert.That (dataContainer.HasDomainObject, Is.False);

      using (_mockRepository.Ordered ())
      {
        _eventSinkMock.Expect (
            mock => mock.ObjectsLoading (Arg.Is (_clientTransaction), Arg<ReadOnlyCollection<ObjectID>>.Is.Equal (new[] { dataContainer.ID })));
        _dataManagerMock
            .Expect (mock => mock.RegisterDataContainer (dataContainer))
            .WhenCalled (mi => CheckHasEnlistedDomainObject (dataContainer));
        _eventSinkMock
            .Expect (mock => mock.ObjectsLoaded (
                    Arg.Is (_clientTransaction),
                    Arg<ReadOnlyCollection<DomainObject>>.Matches (c => c.SequenceEqual (new[] { dataContainer.DomainObject }))))
            .WhenCalled (mi => CheckOnLoadedCalled(((ReadOnlyCollection<DomainObject>) mi.Arguments[1])));
        _transactionEventReceiverMock
            .Expect (
                mock => mock.Loaded (
                    Arg.Is (_clientTransaction),
                    Arg<ClientTransactionEventArgs>.Matches (args => args.DomainObjects.SequenceEqual (new[] { dataContainer.DomainObject }))))
            .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_clientTransaction)));
      }
      _mockRepository.ReplayAll();

      var result = _agent.RegisterIfRequired (freshlyLoadedObject, _dataManagerMock);

      _mockRepository.VerifyAll();

      Assert.That (result, Is.SameAs (dataContainer.DomainObject));
      Assert.That (_clientTransaction.IsDiscarded, Is.False);
    }

    [Test]
    public void RegisterIfRequired_Single_NullLoadedObject ()
    {
      var nullLoadedObject = GetNullLoadedObject ();

      _mockRepository.ReplayAll ();

      var result = _agent.RegisterIfRequired (nullLoadedObject, _dataManagerMock);

      _eventSinkMock.AssertWasNotCalled (
          mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _dataManagerMock.AssertWasNotCalled (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything));
      _transactionEventReceiverMock.AssertWasNotCalled (mock => mock.Loaded (Arg<object>.Is.Anything, Arg<ClientTransactionEventArgs>.Is.Anything));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void RegisterIfRequired_Single_InvalidLoadedObject ()
    {
      var alreadyExistingLoadedObject = GetInvalidLoadedObject ();

      _mockRepository.ReplayAll ();

      var result = _agent.RegisterIfRequired (alreadyExistingLoadedObject, _dataManagerMock);

      _eventSinkMock.AssertWasNotCalled (
          mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _dataManagerMock.AssertWasNotCalled (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything));
      _transactionEventReceiverMock.AssertWasNotCalled (mock => mock.Loaded (Arg<object>.Is.Anything, Arg<ClientTransactionEventArgs>.Is.Anything));

      Assert.That (result, Is.SameAs (alreadyExistingLoadedObject.InvalidObjectReference));
    }

    [Test]
    public void RegisterIfRequired_Many_MultipleObjects ()
    {
      var freshlyLoadedObject1 = GetFreshlyLoadedObject ();
      var registerableDataContainer1 = freshlyLoadedObject1.FreshlyLoadedDataContainer;
      Assert.That (registerableDataContainer1.HasDomainObject, Is.False);

      var freshlyLoadedObject2 = GetFreshlyLoadedObject ();
      var registerableDataContainer2 = freshlyLoadedObject2.FreshlyLoadedDataContainer;
      Assert.That (registerableDataContainer2.HasDomainObject, Is.False);

      var alreadyExistingLoadedObject = GetAlreadyExistingLoadedObject ();
      var nullLoadedObject = GetNullLoadedObject ();
      var invalidLoadedObject = GetInvalidLoadedObject();

      using (_mockRepository.Ordered ())
      {
        _eventSinkMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_clientTransaction), 
            Arg<ReadOnlyCollection<ObjectID>>.Is.Equal (new[] { registerableDataContainer1.ID, registerableDataContainer2.ID })));
        _dataManagerMock
            .Expect (mock => mock.RegisterDataContainer (registerableDataContainer1))
            .WhenCalled (mi => CheckHasEnlistedDomainObject (registerableDataContainer1));
        _dataManagerMock
            .Expect (mock => mock.RegisterDataContainer (registerableDataContainer2))
            .WhenCalled (mi => CheckHasEnlistedDomainObject (registerableDataContainer2));
        _eventSinkMock
            .Expect (
                mock => mock.ObjectsLoaded (
                    Arg.Is (_clientTransaction),
                    Arg<ReadOnlyCollection<DomainObject>>.Matches (
                        c => c.SequenceEqual (new[] { registerableDataContainer1.DomainObject, registerableDataContainer2.DomainObject }))))
            .WhenCalled (mi => CheckOnLoadedCalled ((ReadOnlyCollection<DomainObject>) mi.Arguments[1]));
        _transactionEventReceiverMock
            .Expect (
                mock => mock.Loaded (
                    Arg.Is (_clientTransaction),
                    Arg<ClientTransactionEventArgs>.Matches (
                        args =>
                        args.DomainObjects.SequenceEqual (new[] { registerableDataContainer1.DomainObject, registerableDataContainer2.DomainObject }))))
            .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_clientTransaction)));
      }
      _mockRepository.ReplayAll ();

      var result =
          _agent.RegisterIfRequired (
              new ILoadedObject[] { freshlyLoadedObject1, alreadyExistingLoadedObject, freshlyLoadedObject2, nullLoadedObject, invalidLoadedObject }, 
              _dataManagerMock);

      _mockRepository.VerifyAll ();

      var expectedResult =
          new[]
          {
              registerableDataContainer1.DomainObject, 
              alreadyExistingLoadedObject.ExistingDataContainer.DomainObject,
              registerableDataContainer2.DomainObject, 
              null,
              invalidLoadedObject.InvalidObjectReference
          };
      Assert.That (result, Is.EqualTo (expectedResult));
      Assert.That (_clientTransaction.IsDiscarded, Is.False);
    }

    [Test]
    public void RegisterIfRequired_Many_ExceptionWhenRegisteringObject ()
    {
      var exception = new Exception ("Test");
      
      var freshlyLoadedObject1 = GetFreshlyLoadedObject ();
      var registerableDataContainer1 = freshlyLoadedObject1.FreshlyLoadedDataContainer;
      Assert.That (registerableDataContainer1.HasDomainObject, Is.False);

      var freshlyLoadedObject2 = GetFreshlyLoadedObject ();
      var registerableDataContainer2 = freshlyLoadedObject2.FreshlyLoadedDataContainer;
      Assert.That (registerableDataContainer2.HasDomainObject, Is.False);

      using (_mockRepository.Ordered ())
      {
        _eventSinkMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_clientTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Is.Equal (new[] { registerableDataContainer1.ID, registerableDataContainer2.ID })));
        _dataManagerMock.Expect (mock => mock.RegisterDataContainer (registerableDataContainer1));
        _dataManagerMock
            .Expect (mock => mock.RegisterDataContainer (registerableDataContainer2))
            .Throw (exception);
        _eventSinkMock
            .Expect (
                mock => mock.ObjectsLoaded (
                    Arg.Is (_clientTransaction),
                    Arg<ReadOnlyCollection<DomainObject>>.Matches (
                        c => c.SequenceEqual (new[] { registerableDataContainer1.DomainObject }))))
            .WhenCalled (mi => CheckOnLoadedCalled ((ReadOnlyCollection<DomainObject>) mi.Arguments[1]));
        _transactionEventReceiverMock
            .Expect (
                mock => mock.Loaded (
                    Arg.Is (_clientTransaction),
                    Arg<ClientTransactionEventArgs>.Matches (
                        args =>
                        args.DomainObjects.SequenceEqual (new[] { registerableDataContainer1.DomainObject }))));
      }
      _mockRepository.ReplayAll ();

      Assert.That (
          () => _agent.RegisterIfRequired (new ILoadedObject[] { freshlyLoadedObject1, freshlyLoadedObject2 }, _dataManagerMock), 
          Throws.Exception.SameAs (exception));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Serializability ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();
      var agent = new LoadedObjectRegistrationAgent (clientTransaction, ClientTransactionTestHelper.GetTransactionEventSink (clientTransaction));

      var deserializedInstance = Serializer.SerializeAndDeserialize (agent);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.TransactionEventSink, Is.Not.Null);
    }

    private FreshlyLoadedObject GetFreshlyLoadedObject ()
    {
      var id = new ObjectID (typeof (Order), Guid.NewGuid());
      var dataContainer = DataContainer.CreateForExisting (id, null, pd => pd.DefaultValue);
      return new FreshlyLoadedObject (dataContainer);
    }

    private AlreadyExistingLoadedObject GetAlreadyExistingLoadedObject ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      var dataContainer = DataContainer.CreateForExisting (domainObject.ID, null, pd => pd.DefaultValue);
      dataContainer.SetDomainObject (domainObject);
      DataContainerTestHelper.SetClientTransaction (dataContainer, _clientTransaction);
      return new AlreadyExistingLoadedObject (dataContainer);
    }

    private NullLoadedObject GetNullLoadedObject ()
    {
      return new NullLoadedObject();
    }

    private InvalidLoadedObject GetInvalidLoadedObject ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      return new InvalidLoadedObject (domainObject);
    }

    private void CheckHasEnlistedDomainObject (DataContainer dataContainer)
    {
      Assert.That (dataContainer.HasDomainObject, Is.True);
      Assert.That (dataContainer.DomainObject, Is.SameAs (_clientTransaction.GetEnlistedDomainObject (dataContainer.ID)));
    }

    private void CheckOnLoadedCalled (IEnumerable<DomainObject> domainObjects)
    {
      Assert.That (
          domainObjects.All (item => ((TestDomainBase) item).OnLoadedCalled),
          "OnLoaded must be called for all registered DataContainers.");
      Assert.That (
          domainObjects.All (item => ((TestDomainBase) item).OnLoadedTx == _clientTransaction),
          "OnLoaded must be called within a transaction scope.");
    }

  }
}