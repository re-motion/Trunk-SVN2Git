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
using System.Collections.ObjectModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class ObjectLoaderTest : StandardMappingTest
  {
    private MockRepository _mockRepository;

    private IDataSource _dataSourceMock;
    private IClientTransactionListener _eventSinkMock;
    private ClientTransactionMock _clientTransaction;

    private ObjectLoader _objectLoader;

    private DataContainer _dataContainer1;
    private DataContainer _dataContainer2;

    private IQuery _queryStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      
      _dataSourceMock = _mockRepository.StrictMock<IDataSource> ();
      _eventSinkMock = _mockRepository.DynamicMock<IClientTransactionListener> ();
      _clientTransaction = new ClientTransactionMock();

      _objectLoader = new ObjectLoader (_clientTransaction, _dataSourceMock, _eventSinkMock);

      _dataContainer1 = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      _dataContainer2 = DataContainer.CreateForExisting (DomainObjectIDs.Order2, null, pd => pd.DefaultValue);

      _queryStub = _mockRepository.Stub<IQuery> ();
      _queryStub.Stub (stub => stub.EagerFetchQueries).Return (new EagerFetchQueryCollection ());
    }

    [Test]
    public void LoadObject ()
    {
      _dataSourceMock.Stub (mock => mock.LoadDataContainer (DomainObjectIDs.Order1)).Return (_dataContainer1);

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadObject (DomainObjectIDs.Order1);

      CheckLoadedObject (result, _dataContainer1);
    }

    [Test]
    public void LoadObject_Events ()
    {
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _dataSourceMock.Expect (mock => mock.LoadDataContainer (DomainObjectIDs.Order1)).Return (_dataContainer1);
        
        ExpectObjectsLoading (DomainObjectIDs.Order1);
        ExpectObjectsLoaded (transactionEventReceiver, _dataContainer1);
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObject (DomainObjectIDs.Order1);

      _mockRepository.VerifyAll();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result }));
    }

    [Test]
    public void LoadObjects ()
    {
      _dataSourceMock
          .Stub (mock => mock.LoadDataContainers (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true))
          .Return (new DataContainerCollection (new[] { _dataContainer1, _dataContainer2 }, true));

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadObjects (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true);

      Assert.That (result.Length, Is.EqualTo (2));

      CheckLoadedObject (result[0], _dataContainer1);
      CheckLoadedObject (result[1], _dataContainer2);
    }

    [Test]
    public void LoadObjects_WithUnknownObjects ()
    {
      _dataSourceMock
          .Stub (mock => mock.LoadDataContainers (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order2 }, true))
          .Return (new DataContainerCollection (new[] { _dataContainer1, _dataContainer2 }, true));

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadObjects (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order2 }, true);

      Assert.That (result.Length, Is.EqualTo (3));

      CheckLoadedObject (result[0], _dataContainer1);
      Assert.That (result[1], Is.Null);
      CheckLoadedObject (result[2], _dataContainer2);
    }

    [Test]
    public void LoadObjects_Ordering ()
    {
      _dataSourceMock
          .Stub (mock => mock.LoadDataContainers (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true))
          .Return (new DataContainerCollection (new[] { _dataContainer2, _dataContainer1 }, true));

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadObjects (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true);

      Assert.That (result.Length, Is.EqualTo (2));

      CheckLoadedObject (result[0], _dataContainer1);
      CheckLoadedObject (result[1], _dataContainer2);
    }

    [Test]
    public void LoadObjects_Events ()
    {
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _dataSourceMock
            .Expect (mock => mock.LoadDataContainers (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true))
          .Return (new DataContainerCollection (new[] { _dataContainer1, _dataContainer2 }, true));

        ExpectObjectsLoading (DomainObjectIDs.Order1, DomainObjectIDs.Order2);
        ExpectObjectsLoaded (transactionEventReceiver, _dataContainer1, _dataContainer2);
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObjects (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true);

      _mockRepository.VerifyAll ();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result[0], result[1] }));
    }

    [Test]
    public void LoadObjects_EmptyResult_Events ()
    {
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      _dataSourceMock
          .Expect (mock => mock.LoadDataContainers (new ObjectID[0], true))
        .Return (new DataContainerCollection (new DataContainer[0], true));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObjects (new ObjectID[0], true);
      Assert.That (result, Is.Empty);

      _eventSinkMock.AssertWasNotCalled (mock => mock.ObjectsLoading (Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));

      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void LoadRelatedObject ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      _dataSourceMock
          .Stub (mock => mock.LoadRelatedDataContainer (endPointID))
          .Return (_dataContainer1);

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadRelatedObject (endPointID);

      CheckLoadedObject (result, _dataContainer1);
    }

    [Test]
    public void LoadRelatedObject_Null ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      _dataSourceMock
          .Stub (mock => mock.LoadRelatedDataContainer (endPointID))
          .Return (null);

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadRelatedObject (endPointID);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void LoadRelatedObject_Events ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _dataSourceMock.Expect (mock => mock.LoadRelatedDataContainer (endPointID)).Return (_dataContainer1);

        ExpectObjectsLoading (DomainObjectIDs.Order1);
        ExpectObjectsLoaded (transactionEventReceiver, _dataContainer1);
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadRelatedObject (endPointID);

      _mockRepository.VerifyAll ();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "LoadRelatedObject can only be used with virtual end points.\r\nParameter name: relationEndPointID")]
    public void LoadRelatedObject_NonVirtualID ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      _objectLoader.LoadRelatedObject (endPointID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "LoadRelatedObject can only be used with one-valued end points.\r\nParameter name: relationEndPointID")]
    public void LoadRelatedObject_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      _objectLoader.LoadRelatedObject (endPointID);
    }

    [Test]
    public void LoadRelatedObjects ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");

      _dataSourceMock
          .Stub (mock => mock.LoadRelatedDataContainers (endPointID))
          .Return (new DataContainerCollection (new[] { _dataContainer1, _dataContainer2 }, true));

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadRelatedObjects (endPointID);

      Assert.That (result.Length, Is.EqualTo (2));
      CheckLoadedObject (result[0], _dataContainer1);
      CheckLoadedObject (result[1], _dataContainer2);
    }

    [Test]
    public void LoadRelatedObjects_Events ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _dataSourceMock
            .Expect (mock => mock.LoadRelatedDataContainers (endPointID))
          .Return (new DataContainerCollection (new[] { _dataContainer1, _dataContainer2 }, true));

        ExpectObjectsLoading (DomainObjectIDs.Order1, DomainObjectIDs.Order2);
        ExpectObjectsLoaded (transactionEventReceiver, _dataContainer1, _dataContainer2);
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadRelatedObjects (endPointID);

      _mockRepository.VerifyAll ();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result[0], result[1] }));
    }

    [Test]
    public void LoadRelatedObjects_EmptyResult_Events ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      _dataSourceMock
          .Expect (mock => mock.LoadRelatedDataContainers (endPointID))
        .Return (new DataContainerCollection (new DataContainer[0], true));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadRelatedObjects (endPointID);
      Assert.That (result, Is.Empty);

      _eventSinkMock.AssertWasNotCalled (mock => mock.ObjectsLoading (Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));

      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "LoadRelatedObjects can only be used with many-valued end points.\r\nParameter name: relationEndPointID")]
    public void LoadRelatedObjects_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _objectLoader.LoadRelatedObjects (endPointID);
    }

    [Test]
    public void LoadRelatedObjects_MergedResult ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");

      var domainObject1 = PreregisterDataContainer(_dataContainer1);

      _dataSourceMock
          .Stub (mock => mock.LoadRelatedDataContainers (endPointID))
          .Return (new DataContainerCollection (new[] { _dataContainer1, _dataContainer2 }, true));

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadRelatedObjects (endPointID);

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0], Is.SameAs (domainObject1));
      CheckLoadedObject (result[1], _dataContainer2);
    }

    [Test]
    public void LoadRelatedObjects_MergedResult_Events ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      PreregisterDataContainer (_dataContainer1);

      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _dataSourceMock
            .Expect (mock => mock.LoadRelatedDataContainers (endPointID))
          .Return (new DataContainerCollection (new[] { _dataContainer1, _dataContainer2 }, true));

        ExpectObjectsLoading (DomainObjectIDs.Order2);
        ExpectObjectsLoaded (transactionEventReceiver, _dataContainer2);
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadRelatedObjects (endPointID);

      _mockRepository.VerifyAll ();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result[1] }));
    }

    [Test]
    public void LoadCollectionQueryResult ()
    {
      _dataSourceMock
          .Stub (mock => mock.LoadDataContainersForQuery (_queryStub))
          .Return (new[] { _dataContainer1, _dataContainer2 });

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadCollectionQueryResult<Order> (_queryStub);

      Assert.That (result.Length, Is.EqualTo (2));
      CheckLoadedObject (result[0], _dataContainer1);
      CheckLoadedObject (result[1], _dataContainer2);
    }

    [Test]
    public void LoadCollectionQueryResult_EmptyResult_Events ()
    {
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      _dataSourceMock
          .Stub (mock => mock.LoadDataContainersForQuery (_queryStub))
        .Return (new DataContainer[0]);

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadCollectionQueryResult<Order> (_queryStub);
      Assert.That (result, Is.Empty);

      _eventSinkMock.AssertWasNotCalled (mock => mock.ObjectsLoading (Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));

      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void LoadCollectionQueryResult_MergedResult ()
    {
      var domainObject1 = PreregisterDataContainer (_dataContainer1);

      _dataSourceMock
          .Stub (mock => mock.LoadDataContainersForQuery (_queryStub))
          .Return (new []{ _dataContainer1, _dataContainer2 });

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadCollectionQueryResult<Order> (_queryStub);

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result[0], Is.SameAs (domainObject1));
      CheckLoadedObject (result[1], _dataContainer2);
    }

    [Test]
    public void LoadCollectionQueryResult_MergedResult_Events ()
    {
      PreregisterDataContainer (_dataContainer1);

      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _dataSourceMock
            .Expect (mock => mock.LoadDataContainersForQuery (_queryStub))
          .Return (new[] { _dataContainer1, _dataContainer2 });

        ExpectObjectsLoading (DomainObjectIDs.Order2);
        ExpectObjectsLoaded (transactionEventReceiver, _dataContainer2);
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadCollectionQueryResult<Order> (_queryStub);

      _mockRepository.VerifyAll ();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result[1] }));
    }

    [Test]
    [ExpectedException (typeof (UnexpectedQueryResultException), ExpectedMessage = 
        "The query returned an object of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order', but a query result of type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer' was expected.")]
    public void LoadCollectionQueryResult_CastError ()
    {
      _dataSourceMock
          .Stub (mock => mock.LoadDataContainersForQuery (_queryStub))
          .Return (new[] { _dataContainer1, _dataContainer2 });

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadCollectionQueryResult<Customer> (_queryStub);

      Assert.That (result.Length, Is.EqualTo (2));
      CheckLoadedObject (result[0], _dataContainer1);
      CheckLoadedObject (result[1], _dataContainer2);
    }

    [Test]
    public void ClientTransactionLoadedEvent_Transaction ()
    {
      ClientTransaction loadTransaction = null;
      _clientTransaction.Loaded += delegate { loadTransaction = ClientTransaction.Current; };

      _dataSourceMock.Stub (mock => mock.LoadDataContainer (DomainObjectIDs.Order1)).Return (_dataContainer1);
      _mockRepository.ReplayAll ();

      _objectLoader.LoadObject (DomainObjectIDs.Order1);

      Assert.That (loadTransaction, Is.SameAs (_clientTransaction));
    }

    private void CheckLoadedObject (DomainObject loadedObject, DataContainer dataContainer)
    {
      Assert.That (loadedObject, Is.InstanceOfType (dataContainer.DomainObjectType));
      Assert.That (loadedObject.ID, Is.EqualTo (dataContainer.ID));
      Assert.That (_clientTransaction.IsEnlisted (loadedObject), Is.True);
      Assert.That (dataContainer.ClientTransaction, Is.SameAs (_clientTransaction));
      Assert.That (dataContainer.DomainObject, Is.SameAs (loadedObject));
    }

    private void ExpectObjectsLoading (params ObjectID[] expectedIDs)
    {
      _eventSinkMock
          .Expect (mock => mock.ObjectsLoading (Arg<ReadOnlyCollection<ObjectID>>.List.Equal (expectedIDs)))
          .WhenCalled (mi => Assert.That (
              expectedIDs.All (id => _clientTransaction.DataManager.DataContainerMap[id] == null), 
              "ObjectsLoading must be raised before IDs are registered"));
    }

    private void ExpectObjectsLoaded (ClientTransactionEventReceiver transactionEventReceiver, params DataContainer[] expectedDataContainers)
    {
      _eventSinkMock
          .Expect (mock => mock.ObjectsLoaded (
              Arg<ReadOnlyCollection<DomainObject>>.Matches (list =>
                  list.Select (item => item.ID).SequenceEqual (expectedDataContainers.Select (dc => dc.ID)))))
          .WhenCalled (mi =>
          {
            Assert.That (
                expectedDataContainers.All (dc => _clientTransaction.DataManager.DataContainerMap[dc.ID] == dc),
                "ObjectsLoaded must be raised after IDs are registered");
            Assert.That (
                ((ReadOnlyCollection<DomainObject>) mi.Arguments[0]).All (item => ((Order) item).OnLoadedCalled),
                "ObjectsLoaded must be raised after OnLoaded is called");
            Assert.That (
                ((ReadOnlyCollection<DomainObject>) mi.Arguments[0]).All (item => ((Order) item).LoadTransaction == _clientTransaction),
                "ObjectsLoaded must be raised after OnLoaded is called");
            Assert.That (
                transactionEventReceiver.LoadedDomainObjects, 
                Is.Empty, 
                "ObjectsLoaded must be raised before transaction OnLoaded is called");
          });
    }

    private DomainObject PreregisterDataContainer (DataContainer dataContainer)
    {
      var domainObject = ClientTransactionTestHelper.CallGetObjectReference (_clientTransaction, dataContainer.ID);
      dataContainer.SetDomainObject (domainObject);
      dataContainer.RegisterWithTransaction (_clientTransaction);
      return domainObject;
    }
  }
}