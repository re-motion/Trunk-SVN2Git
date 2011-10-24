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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Linq;
using Remotion.Data.UnitTests.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class ObjectLoaderTest : StandardMappingTest
  {
    private MockRepository _mockRepository;
    
    private IPersistenceStrategy _persistenceStrategyMock;
    private IEagerFetcher _fetcherMock;
    private ILoadedObjectRegistrationAgent _loadedObjectRegistrationAgentMock;
    private ILoadedObjectProvider _loadedObjectProviderMock;
    private IDataManager _dataManagerStub;

    private ObjectLoader _objectLoader;

    private DomainObject _domainObject1;
    private DomainObject _domainObject2;

    private DataContainer _dataContainer1;
    private DataContainer _dataContainer2;

    private IQuery _fakeQuery;
    private ILoadedObject _fakeExistingObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();

      _persistenceStrategyMock = _mockRepository.StrictMock<IPersistenceStrategy> ();
      _fetcherMock = _mockRepository.StrictMock<IEagerFetcher> ();
      _loadedObjectRegistrationAgentMock = _mockRepository.StrictMock<ILoadedObjectRegistrationAgent>();
      _loadedObjectProviderMock = _mockRepository.StrictMock<ILoadedObjectProvider>();
      _dataManagerStub = _mockRepository.Stub<IDataManager>();
      
      _objectLoader = new ObjectLoader (_persistenceStrategyMock, _fetcherMock, _loadedObjectRegistrationAgentMock);

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order2);

      _dataContainer1 = DataContainer.CreateForExisting (_domainObject1.ID, null, pd => pd.DefaultValue);
      _dataContainer2 = DataContainer.CreateForExisting (_domainObject2.ID, null, pd => pd.DefaultValue);

      _fakeQuery = CreateFakeQuery();

      _fakeExistingObject = MockRepository.GenerateStub<ILoadedObject> ();
    }

    [Test]
    public void LoadObject ()
    {
      _persistenceStrategyMock.Expect (mock => mock.LoadDataContainer (_domainObject1.ID)).Return (_dataContainer1);
      _loadedObjectRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<ILoadedObject>.Is.Anything, Arg.Is (_dataManagerStub)))
          .WhenCalled (mi => CheckFreshlyLoadedObject ((ILoadedObject) mi.Arguments[0], _dataContainer1))
          .Return (_domainObject1);

      _mockRepository.ReplayAll();

      var result = _objectLoader.LoadObject (_domainObject1.ID, _dataManagerStub);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void LoadObjects ()
    {
      _persistenceStrategyMock
          .Expect (mock => mock.LoadDataContainers (
              Arg<ICollection<ObjectID>>.List.Equal (new[] { _domainObject1.ID, _domainObject2.ID }), 
              Arg.Is (false)))
          .Return (new DataContainerCollection { _dataContainer1, _dataContainer2 });
      _loadedObjectRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<IEnumerable<ILoadedObject>>.Is.Anything, Arg.Is (_dataManagerStub)))
          .WhenCalled (
              mi => CheckRegisteredObjects (
                  mi,
                  obj => CheckFreshlyLoadedObject (obj, _dataContainer1),
                  obj => CheckFreshlyLoadedObject (obj, _dataContainer2)))
          .Return (new[] { _domainObject1, _domainObject2 });

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObjects (new[] { _domainObject1.ID, _domainObject2.ID }, false, _dataManagerStub);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void LoadObjects_WithNotFoundObjects ()
    {
      _persistenceStrategyMock
          .Expect (mock => mock.LoadDataContainers (
              Arg<ICollection<ObjectID>>.List.Equal (new[] { _domainObject1.ID, _domainObject2.ID, DomainObjectIDs.Order3 }),
              Arg.Is (false)))
          .Return (new DataContainerCollection { _dataContainer1 });
      _loadedObjectRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<IEnumerable<ILoadedObject>>.Is.Anything, Arg.Is (_dataManagerStub)))
          .WhenCalled (
              mi => CheckRegisteredObjects (
                  mi,
                  obj => CheckFreshlyLoadedObject (obj, _dataContainer1)))
          .Return (new[] { _domainObject1 });

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObjects (new[] { _domainObject1.ID, _domainObject2.ID, DomainObjectIDs.Order3 }, false, _dataManagerStub);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (new[] { _domainObject1, null, null }));
    }

    [Test]
    public void LoadObjects_WithUnorderedPersistenceResult ()
    {
      _persistenceStrategyMock
          .Expect (mock => mock.LoadDataContainers (
              Arg<ICollection<ObjectID>>.List.Equal (new[] { _domainObject1.ID, _domainObject2.ID }),
              Arg.Is (false)))
          .Return (new DataContainerCollection { _dataContainer2, _dataContainer1 });
      _loadedObjectRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<IEnumerable<ILoadedObject>>.Is.Anything, Arg.Is (_dataManagerStub)))
          .WhenCalled (
              mi => CheckRegisteredObjects (
                  mi,
                  obj => CheckFreshlyLoadedObject (obj, _dataContainer2),
                  obj => CheckFreshlyLoadedObject (obj, _dataContainer1)))
          .Return (new[] { _domainObject2, _domainObject1 });

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObjects (new[] { _domainObject1.ID, _domainObject2.ID }, false, _dataManagerStub);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void GetOrLoadRelatedObject_WithAlreadyExistingObject ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order3, "OrderTicket");
      var fakeOriginatingDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order3);

      _dataManagerStub.Stub (stub => stub.GetDataContainerWithLazyLoad (endPointID.ObjectID)).Return (fakeOriginatingDataContainer);
      _persistenceStrategyMock
          .Expect (mock => mock.LoadRelatedDataContainer (fakeOriginatingDataContainer, endPointID))
          .Return (_dataContainer1);
      _loadedObjectProviderMock.Expect (mock => mock.GetLoadedObject (_dataContainer1.ID)).Return (_fakeExistingObject);
      _loadedObjectRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (_fakeExistingObject, _dataManagerStub))
          .Return (_domainObject1);
      _mockRepository.ReplayAll();      

      var result = _objectLoader.GetOrLoadRelatedObject (endPointID, _dataManagerStub, _loadedObjectProviderMock);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void GetOrLoadRelatedObject_WithFreshlyLoadedObject ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order3, "OrderTicket");
      var fakeOriginatingDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order3);

      _dataManagerStub.Stub (stub => stub.GetDataContainerWithLazyLoad (endPointID.ObjectID)).Return (fakeOriginatingDataContainer);
      _persistenceStrategyMock
          .Expect (mock => mock.LoadRelatedDataContainer (fakeOriginatingDataContainer, endPointID))
          .Return (_dataContainer1);
      _loadedObjectProviderMock.Expect (mock => mock.GetLoadedObject (_dataContainer1.ID)).Return (null);
      _loadedObjectRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<ILoadedObject>.Is.Anything, Arg.Is (_dataManagerStub)))
          .WhenCalled (mi => CheckFreshlyLoadedObject ((ILoadedObject) mi.Arguments[0], _dataContainer1))
          .Return (_domainObject1);
      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadRelatedObject (endPointID, _dataManagerStub, _loadedObjectProviderMock);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void GetOrLoadRelatedObject_WithNull ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order3, "OrderTicket");
      var fakeOriginatingDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order3);

      _dataManagerStub.Stub (stub => stub.GetDataContainerWithLazyLoad (endPointID.ObjectID)).Return (fakeOriginatingDataContainer);
      _persistenceStrategyMock
          .Expect (mock => mock.LoadRelatedDataContainer (fakeOriginatingDataContainer, endPointID))
          .Return (null);
      _loadedObjectRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<ILoadedObject>.Is.Anything, Arg.Is (_dataManagerStub)))
          .WhenCalled (mi => CheckNullLoadedObject ((ILoadedObject) mi.Arguments[0]))
          .Return (null);
      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadRelatedObject (endPointID, _dataManagerStub, _loadedObjectProviderMock);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "GetOrLoadRelatedObject can only be used with virtual end points.\r\nParameter name: relationEndPointID")]
    public void GetOrLoadRelatedObject_NonVirtualID ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      _objectLoader.GetOrLoadRelatedObject (endPointID, _dataManagerStub, _loadedObjectProviderMock);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "GetOrLoadRelatedObject can only be used with one-valued end points.\r\nParameter name: relationEndPointID")]
    public void GetOrLoadRelatedObject_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_domainObject1.ID, "OrderItems");
      _objectLoader.GetOrLoadRelatedObject (endPointID, _dataManagerStub, _loadedObjectProviderMock);
    }

    [Test]
    public void GetOrLoadRelatedObjects ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");

      _persistenceStrategyMock
          .Expect (mock => mock.LoadRelatedDataContainers (endPointID))
          .Return (new DataContainerCollection { _dataContainer1, _dataContainer2 });
      _loadedObjectProviderMock.Expect (mock => mock.GetLoadedObject (_dataContainer1.ID)).Return (null);
      _loadedObjectProviderMock.Expect (mock => mock.GetLoadedObject (_dataContainer2.ID)).Return (_fakeExistingObject);
      _loadedObjectRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<IEnumerable<ILoadedObject>>.Is.Anything, Arg.Is (_dataManagerStub)))
          .WhenCalled (
              mi => CheckRegisteredObjects (
                  mi,
                  obj => CheckFreshlyLoadedObject (obj, _dataContainer1),
                  obj => CheckAlreadyExistingLoadedObject (obj, _fakeExistingObject)))
          .Return (new[] { _domainObject1, _domainObject2 });

      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadRelatedObjects (endPointID, _dataManagerStub, _loadedObjectProviderMock);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "GetOrLoadRelatedObjects can only be used with many-valued end points.\r\nParameter name: relationEndPointID")]
    public void GetOrLoadRelatedObjects_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _objectLoader.GetOrLoadRelatedObjects (endPointID, _dataManagerStub, _loadedObjectProviderMock);
    }

    [Test]
    public void GetOrLoadCollectionQueryResult ()
    {
      _persistenceStrategyMock
          .Expect (mock => mock.LoadDataContainersForQuery (_fakeQuery))
          .Return (new[] { _dataContainer1, _dataContainer2 });
      _loadedObjectProviderMock.Expect (mock => mock.GetLoadedObject (_dataContainer1.ID)).Return (null);
      _loadedObjectProviderMock.Expect (mock => mock.GetLoadedObject (_dataContainer2.ID)).Return (_fakeExistingObject);
      _loadedObjectRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<IEnumerable<ILoadedObject>>.Is.Anything, Arg.Is (_dataManagerStub)))
          .WhenCalled (
              mi => CheckRegisteredObjects (
                  mi,
                  obj => CheckFreshlyLoadedObject (obj, _dataContainer1),
                  obj => CheckAlreadyExistingLoadedObject (obj, _fakeExistingObject)))
          .Return (new[] { _domainObject1, _domainObject2 });

      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadCollectionQueryResult<Order> (_fakeQuery, _dataManagerStub, _loadedObjectProviderMock);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_WithNulls ()
    {
      _persistenceStrategyMock
          .Expect (mock => mock.LoadDataContainersForQuery (_fakeQuery))
          .Return (new[] { _dataContainer1, null });
      _loadedObjectProviderMock.Expect (mock => mock.GetLoadedObject (_dataContainer1.ID)).Return (null);
      _loadedObjectRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<IEnumerable<ILoadedObject>>.Is.Anything, Arg.Is (_dataManagerStub)))
          .WhenCalled (
              mi => CheckRegisteredObjects (
                  mi,
                  obj => CheckFreshlyLoadedObject (obj, _dataContainer1),
                  CheckNullLoadedObject))
          .Return (new[] { _domainObject1, null });

      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadCollectionQueryResult<Order> (_fakeQuery, _dataManagerStub, _loadedObjectProviderMock);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _domainObject1, null }));
    }

    [Test]
    [ExpectedException (typeof (UnexpectedQueryResultException), ExpectedMessage =
        "The query returned an object of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order', but a query result of type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer' was expected.")]
    public void GetOrLoadCollectionQueryResult_CastError ()
    {
      _persistenceStrategyMock
          .Expect (mock => mock.LoadDataContainersForQuery (_fakeQuery))
          .Return (new[] { _dataContainer1 });
      _loadedObjectProviderMock.Expect (mock => mock.GetLoadedObject (_dataContainer1.ID)).Return (null);
      _loadedObjectRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<IEnumerable<ILoadedObject>>.Is.Anything, Arg.Is (_dataManagerStub)))
          .WhenCalled (
              mi => CheckRegisteredObjects (
                  mi,
                  obj => CheckFreshlyLoadedObject (obj, _dataContainer1)))
          .Return (new[] { _domainObject1 });

      _mockRepository.ReplayAll ();

      _objectLoader.GetOrLoadCollectionQueryResult<Customer> (_fakeQuery, _dataManagerStub, _loadedObjectProviderMock);
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_WithFetching ()
    {
      var fetchQueryStub = CreateFakeQuery ();
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");
      _fakeQuery.EagerFetchQueries.Add (endPointDefinition, fetchQueryStub);

      _persistenceStrategyMock
          .Expect (mock => mock.LoadDataContainersForQuery (_fakeQuery))
          .Return (new[] { _dataContainer1 });
      _loadedObjectProviderMock.Expect (mock => mock.GetLoadedObject (_dataContainer1.ID)).Return (null);
      _loadedObjectRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<IEnumerable<ILoadedObject>>.Is.Anything, Arg.Is (_dataManagerStub)))
          .WhenCalled (
              mi => CheckRegisteredObjects (
                  mi,
                  obj => CheckFreshlyLoadedObject (obj, _dataContainer1)))
          .Return (new[] { _domainObject1 });
      _fetcherMock
          .Expect (mock => mock.PerformEagerFetching (
              Arg<DomainObject[]>.List.Equal(new[] { _domainObject1 }),
              Arg.Is (endPointDefinition),
              Arg.Is (fetchQueryStub),
              Arg.Is (_objectLoader),
              Arg.Is (_dataManagerStub), 
              Arg.Is (_loadedObjectProviderMock)));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadCollectionQueryResult<Order> (_fakeQuery, _dataManagerStub, _loadedObjectProviderMock);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _domainObject1 }));
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_WithFetching_NoOriginalObjects ()
    {
      var fetchQueryStub = CreateFakeQuery ();
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");
      _fakeQuery.EagerFetchQueries.Add (endPointDefinition, fetchQueryStub);

      _persistenceStrategyMock
          .Expect (mock => mock.LoadDataContainersForQuery (_fakeQuery))
          .Return (new DataContainer[0]);
      _loadedObjectRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<IEnumerable<ILoadedObject>>.List.Equal (new ILoadedObject[0]), Arg.Is (_dataManagerStub)))
          .Return (new DomainObject[0]);

      _mockRepository.ReplayAll ();

      _objectLoader.GetOrLoadCollectionQueryResult<Order> (_fakeQuery, _dataManagerStub, _loadedObjectProviderMock);

      _fetcherMock.AssertWasNotCalled (mock => mock.PerformEagerFetching (
          Arg<DomainObject[]>.Is.Anything,
          Arg<IRelationEndPointDefinition>.Is.Anything,
          Arg<IQuery>.Is.Anything,
          Arg<IObjectLoader>.Is.Anything,
          Arg<IDataManager>.Is.Anything, 
          Arg<ILoadedObjectProvider>.Is.Anything));
    }

    private IQuery CreateFakeQuery ()
    {
      return QueryFactory.CreateCollectionQuery (
          "test",
          _domainObject1.ID.StorageProviderDefinition,
          "TEST",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));
    }

    private void CheckAlreadyExistingLoadedObject (ILoadedObject loadedObject, ILoadedObject expected)
    {
      Assert.That (loadedObject, Is.SameAs (expected));
    }

    private void CheckFreshlyLoadedObject (ILoadedObject loadedObject, DataContainer dataContainer)
    {
      Assert.That (
          loadedObject,
          Is.TypeOf<FreshlyLoadedObject> ()
              .With.Property ((FreshlyLoadedObject obj) => obj.FreshlyLoadedDataContainer).SameAs (dataContainer));
    }

    private void CheckNullLoadedObject (ILoadedObject loadedObject)
    {
      Assert.That (loadedObject, Is.TypeOf<NullLoadedObject> ());
    }

    private void CheckRegisteredObjects (MethodInvocation mi, params Action<ILoadedObject>[] checks)
    {
      var loadedObjects = ((IEnumerable<ILoadedObject>) mi.Arguments[0]).ToArray ();
      Assert.That (loadedObjects.Length, Is.EqualTo (checks.Length));

      for (int i = 0; i < loadedObjects.Length; i++)
        checks[i] (loadedObjects[i]);
    }
  }
}