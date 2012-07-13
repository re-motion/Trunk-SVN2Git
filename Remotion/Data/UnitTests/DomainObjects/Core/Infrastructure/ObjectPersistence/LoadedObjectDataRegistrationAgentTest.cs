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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class LoadedObjectDataRegistrationAgentTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;

    private MockRepository _mockRepository;
    private ClientTransactionEventSinkWithMock _eventSinkWithMock;
    private IDataManager _dataManagerMock;

    private LoadedObjectDataRegistrationAgent _agent;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientTransaction = ClientTransaction.CreateRootTransaction();

      _mockRepository = new MockRepository();
      _eventSinkWithMock = ClientTransactionEventSinkWithMock.CreateWithStrictMock(_clientTransaction);
      _dataManagerMock = _mockRepository.StrictMock<IDataManager> ();

      _agent = new LoadedObjectDataRegistrationAgent (_clientTransaction, _dataManagerMock, _eventSinkWithMock);
    }

    [Test]
    public void RegisterIfRequired_Single_AlreadyExistingLoadedObject ()
    {
      var alreadyExistingLoadedObject = GetAlreadyExistingLoadedObject();

      _mockRepository.ReplayAll();

      _agent.RegisterIfRequired (alreadyExistingLoadedObject, true);

      _eventSinkWithMock.AssertWasNotCalledMock (
          mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _dataManagerMock.AssertWasNotCalled (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything));
    }

    [Test]
    public void RegisterIfRequired_Single_FreshlyLoadedObject ()
    {
      var freshlyLoadedObject = GetFreshlyLoadedObject();
      var dataContainer = freshlyLoadedObject.FreshlyLoadedDataContainer;
      Assert.That (dataContainer.HasDomainObject, Is.False);

      using (_mockRepository.Ordered ())
      {
        _eventSinkWithMock.ExpectMock (
            mock => mock.ObjectsLoading (Arg.Is (_clientTransaction), Arg<ReadOnlyCollection<ObjectID>>.Is.Equal (new[] { dataContainer.ID })));
        _dataManagerMock
            .Expect (mock => mock.RegisterDataContainer (dataContainer))
            .WhenCalled (mi => CheckHasEnlistedDomainObject (dataContainer));
        _eventSinkWithMock
            .ExpectMock (mock => mock.ObjectsLoaded (
                    Arg.Is (_clientTransaction),
                    Arg<ReadOnlyCollection<DomainObject>>.Matches (c => c.SequenceEqual (new[] { dataContainer.DomainObject }))));
      }
      _mockRepository.ReplayAll();

      _agent.RegisterIfRequired (freshlyLoadedObject, true);

      _mockRepository.VerifyAll();
      Assert.That (_clientTransaction.IsDiscarded, Is.False);
    }

    [Test]
    public void RegisterIfRequired_Single_NullLoadedObject ()
    {
      var nullLoadedObject = GetNullLoadedObject ();

      _mockRepository.ReplayAll ();

      _agent.RegisterIfRequired (nullLoadedObject, true);

      _eventSinkWithMock.AssertWasNotCalledMock (
          mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _dataManagerMock.AssertWasNotCalled (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything));
    }

    [Test]
    public void RegisterIfRequired_Single_InvalidLoadedObject ()
    {
      var alreadyExistingLoadedObject = GetInvalidLoadedObject ();

      _mockRepository.ReplayAll ();

      _agent.RegisterIfRequired (alreadyExistingLoadedObject, true);

      _eventSinkWithMock.AssertWasNotCalledMock (
          mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _dataManagerMock.AssertWasNotCalled (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything));
    }

    [Test]
    public void RegisterIfRequired_Single_NotFoundLoadedObject_ThrowOnNotFoundFalse ()
    {
      var notFoundLoadedObject = GetNotFoundLoadedObject ();

      _mockRepository.ReplayAll ();

      Assert.That (() => _agent.RegisterIfRequired (notFoundLoadedObject, false), Throws.Nothing);

      _eventSinkWithMock.AssertWasNotCalledMock (
          mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _dataManagerMock.AssertWasNotCalled (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything));
    }

    [Test]
    public void RegisterIfRequired_Single_NotFoundLoadedObject_ThrowOnNotFoundTrue ()
    {
      var notFoundLoadedObject = GetNotFoundLoadedObject ();

      _mockRepository.ReplayAll ();

      Assert.That (
          () => _agent.RegisterIfRequired (notFoundLoadedObject, true),
          Throws.TypeOf<ObjectsNotFoundException> ().With.Message.EqualTo (
              string.Format ("Object(s) could not be found: '{0}'.", notFoundLoadedObject.ObjectID)));

      _eventSinkWithMock.AssertWasNotCalledMock (
          mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _dataManagerMock.AssertWasNotCalled (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything));
    }

    [Test]
    public void RegisterIfRequired_Many_MultipleObjects_ThrowOnNotFoundFalse ()
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
      var notFoundLoadedObject1 = GetNotFoundLoadedObject();
      var notFoundLoadedObject2 = GetNotFoundLoadedObject ();

      using (_mockRepository.Ordered ())
      {
        _eventSinkWithMock.ExpectMock (mock => mock.ObjectsLoading (
            Arg.Is (_clientTransaction), 
            Arg<ReadOnlyCollection<ObjectID>>.Is.Equal (new[] { registerableDataContainer1.ID, registerableDataContainer2.ID })));
        _dataManagerMock
            .Expect (mock => mock.RegisterDataContainer (registerableDataContainer1))
            .WhenCalled (mi => CheckHasEnlistedDomainObject (registerableDataContainer1));
        _dataManagerMock
            .Expect (mock => mock.RegisterDataContainer (registerableDataContainer2))
            .WhenCalled (mi => CheckHasEnlistedDomainObject (registerableDataContainer2));
        _eventSinkWithMock
            .ExpectMock (
                mock => mock.ObjectsLoaded (
                    Arg.Is (_clientTransaction),
                    Arg<ReadOnlyCollection<DomainObject>>.Matches (
                        c => c.SequenceEqual (new[] { registerableDataContainer1.DomainObject, registerableDataContainer2.DomainObject }))));
      }
      _mockRepository.ReplayAll ();

      var allObjects =
          new ILoadedObjectData[]
          {
              freshlyLoadedObject1,
              alreadyExistingLoadedObject,
              freshlyLoadedObject2,
              nullLoadedObject,
              invalidLoadedObject,
              notFoundLoadedObject1,
              notFoundLoadedObject2
          };
      _agent.RegisterIfRequired (allObjects, false);

      _mockRepository.VerifyAll ();
      Assert.That (_clientTransaction.IsDiscarded, Is.False);
    }

    [Test]
    public void RegisterIfRequired_Many_MultipleObjects_ThrowOnNotFoundTrue ()
    {
      var freshlyLoadedObject1 = GetFreshlyLoadedObject ();
      var registerableDataContainer1 = freshlyLoadedObject1.FreshlyLoadedDataContainer;
      Assert.That (registerableDataContainer1.HasDomainObject, Is.False);

      var freshlyLoadedObject2 = GetFreshlyLoadedObject ();
      var registerableDataContainer2 = freshlyLoadedObject2.FreshlyLoadedDataContainer;
      Assert.That (registerableDataContainer2.HasDomainObject, Is.False);

      var alreadyExistingLoadedObject = GetAlreadyExistingLoadedObject ();
      var nullLoadedObject = GetNullLoadedObject ();
      var invalidLoadedObject = GetInvalidLoadedObject ();
      var notFoundLoadedObject1 = GetNotFoundLoadedObject ();
      var notFoundLoadedObject2 = GetNotFoundLoadedObject ();

      _mockRepository.ReplayAll ();

      var allObjects =
          new ILoadedObjectData[]
          {
              freshlyLoadedObject1,
              alreadyExistingLoadedObject,
              freshlyLoadedObject2,
              nullLoadedObject,
              invalidLoadedObject,
              notFoundLoadedObject1,
              notFoundLoadedObject2
          };
      Assert.That (
          () => _agent.RegisterIfRequired (allObjects, true),
          Throws.TypeOf<ObjectsNotFoundException>().With.Message.EqualTo (
              string.Format ("Object(s) could not be found: '{0}', '{1}'.", notFoundLoadedObject1.ObjectID, notFoundLoadedObject2.ObjectID)));

      _eventSinkWithMock.AssertWasNotCalledMock (
          mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _dataManagerMock.AssertWasNotCalled (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything));
      _eventSinkWithMock.AssertWasNotCalledMock (
          mock => mock.ObjectsLoaded (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
    }

    [Test]
    public void RegisterIfRequired_Many_NoObjects ()
    {
      _mockRepository.ReplayAll ();

      _agent.RegisterIfRequired (new ILoadedObjectData[0], true);

      _eventSinkWithMock.AssertWasNotCalledMock (
          mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _dataManagerMock.AssertWasNotCalled (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything));
      _eventSinkWithMock.AssertWasNotCalledMock (
          mock => mock.ObjectsLoaded (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
    }

    [Test]
    public void RegisterIfRequired_Many_ExceptionWhenRegisteringObject_EventsFiredIfOtherObjectsSucceeded ()
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
        _eventSinkWithMock.ExpectMock (mock => mock.ObjectsLoading (
            Arg.Is (_clientTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Is.Equal (new[] { registerableDataContainer1.ID, registerableDataContainer2.ID })));
        _dataManagerMock.Expect (mock => mock.RegisterDataContainer (registerableDataContainer1));
        _dataManagerMock
            .Expect (mock => mock.RegisterDataContainer (registerableDataContainer2))
            .Throw (exception);
        _eventSinkWithMock
            .ExpectMock (
                mock => mock.ObjectsLoaded (
                    Arg.Is (_clientTransaction),
                    Arg<ReadOnlyCollection<DomainObject>>.Matches (
                        c => c.SequenceEqual (new[] { registerableDataContainer1.DomainObject }))));
      }
      _mockRepository.ReplayAll ();

      Assert.That (
          () => _agent.RegisterIfRequired (new ILoadedObjectData[] { freshlyLoadedObject1, freshlyLoadedObject2 }, true), 
          Throws.Exception.SameAs (exception));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RegisterIfRequired_Many_ExceptionWhenRegisteringObject_EventsNotFiredWhenNoOtherObjectsSucceeded ()
    {
      var exception = new Exception ("Test");

      var freshlyLoadedObject1 = GetFreshlyLoadedObject();
      var registerableDataContainer1 = freshlyLoadedObject1.FreshlyLoadedDataContainer;

      using (_mockRepository.Ordered())
      {
        _eventSinkWithMock.ExpectMock (
            mock => mock.ObjectsLoading (
                Arg.Is (_clientTransaction),
                Arg<ReadOnlyCollection<ObjectID>>.Is.Equal (new[] { registerableDataContainer1.ID })));
        _dataManagerMock
            .Expect (mock => mock.RegisterDataContainer (registerableDataContainer1))
            .Throw (exception);
      }
      _mockRepository.ReplayAll();

      Assert.That (
          () => _agent.RegisterIfRequired (new ILoadedObjectData[] { freshlyLoadedObject1 }, true),
          Throws.Exception.SameAs (exception));

      _eventSinkWithMock.AssertWasNotCalledMock (
          mock => mock.ObjectsLoaded (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
    }

    [Test]
    public void Serializable ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();
      var agent = new LoadedObjectDataRegistrationAgent (
          clientTransaction, new SerializableDataManagerFake(), new SerializableClientTransactionEventSinkFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (agent);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.DataManager, Is.Not.Null);
      Assert.That (deserializedInstance.TransactionEventSink, Is.Not.Null);
    }

    private FreshlyLoadedObjectData GetFreshlyLoadedObject ()
    {
      var id = new ObjectID (typeof (Order), Guid.NewGuid());
      var dataContainer = DataContainer.CreateForExisting (id, null, pd => pd.DefaultValue);
      return new FreshlyLoadedObjectData (dataContainer);
    }

    private AlreadyExistingLoadedObjectData GetAlreadyExistingLoadedObject ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      var dataContainer = DataContainer.CreateForExisting (domainObject.ID, null, pd => pd.DefaultValue);
      dataContainer.SetDomainObject (domainObject);
      DataContainerTestHelper.SetClientTransaction (dataContainer, _clientTransaction);
      return new AlreadyExistingLoadedObjectData (dataContainer);
    }

    private NullLoadedObjectData GetNullLoadedObject ()
    {
      return new NullLoadedObjectData();
    }

    private InvalidLoadedObjectData GetInvalidLoadedObject ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      return new InvalidLoadedObjectData (domainObject);
    }

    private NotFoundLoadedObjectData GetNotFoundLoadedObject ()
    {
      return new NotFoundLoadedObjectData (new ObjectID (typeof (Order), Guid.NewGuid()));
    }

    private void CheckHasEnlistedDomainObject (DataContainer dataContainer)
    {
      Assert.That (dataContainer.HasDomainObject, Is.True);
      Assert.That (dataContainer.DomainObject, Is.SameAs (_clientTransaction.GetEnlistedDomainObject (dataContainer.ID)));
    }
  }
}