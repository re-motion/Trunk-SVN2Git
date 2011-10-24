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

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class ObjectLoaderTest : StandardMappingTest
  {
    private MockRepository _mockRepository;
    
    private IPersistenceStrategy _persistenceStrategyMock;
    private IEagerFetcher _fetcherMock;
    private ILoadedObjectDataRegistrationAgent _loadedObjectDataRegistrationAgentMock;
    private ILoadedObjectDataProvider _loadedObjectDataProviderStub;
    private IDataManager _dataManagerStub;

    private ObjectLoader _objectLoader;

    private DomainObject _domainObject1;
    private DomainObject _domainObject2;

    private IQuery _fakeQuery;
    private ILoadedObjectData _fakeLoadedObjectData1;
    private ILoadedObjectData _fakeLoadedObjectData2;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();

      _persistenceStrategyMock = _mockRepository.StrictMock<IPersistenceStrategy> ();
      _fetcherMock = _mockRepository.StrictMock<IEagerFetcher> ();
      _loadedObjectDataRegistrationAgentMock = _mockRepository.StrictMock<ILoadedObjectDataRegistrationAgent>();
      _loadedObjectDataProviderStub = _mockRepository.Stub<ILoadedObjectDataProvider>();
      _dataManagerStub = _mockRepository.Stub<IDataManager>();
      
      _objectLoader = new ObjectLoader (_persistenceStrategyMock, _fetcherMock, _loadedObjectDataRegistrationAgentMock);

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order2);

      _fakeQuery = CreateFakeQuery();

      _fakeLoadedObjectData1 = MockRepository.GenerateStub<ILoadedObjectData> ();
      _fakeLoadedObjectData2 = MockRepository.GenerateStub<ILoadedObjectData> ();
    }

    [Test]
    public void LoadObject ()
    {
      _persistenceStrategyMock.Expect (mock => mock.LoadObjectData (_domainObject1.ID)).Return (_fakeLoadedObjectData1);
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg.Is (_fakeLoadedObjectData1), Arg.Is (_dataManagerStub)))
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
          .Expect (mock => mock.LoadObjectData (
              Arg<ICollection<ObjectID>>.List.Equal (new[] { _domainObject1.ID, _domainObject2.ID }), 
              Arg.Is (true)))
          .Return (new[] { _fakeLoadedObjectData1, _fakeLoadedObjectData2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (
              Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _fakeLoadedObjectData1, _fakeLoadedObjectData2 }), 
              Arg.Is (_dataManagerStub)))
          .Return (new[] { _domainObject1, _domainObject2 });

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObjects (new[] { _domainObject1.ID, _domainObject2.ID }, true, _dataManagerStub);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void LoadObjects_WithNotFoundObjects ()
    {
      _persistenceStrategyMock
          .Expect (mock => mock.LoadObjectData (
              Arg<ICollection<ObjectID>>.List.Equal (new[] { _domainObject1.ID, _domainObject2.ID, DomainObjectIDs.Order3 }),
              Arg.Is (false)))
          .Return (new[] { _fakeLoadedObjectData1 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (
              mock =>
              mock.RegisterIfRequired (Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _fakeLoadedObjectData1 }), Arg.Is (_dataManagerStub)))
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
          .Expect (mock => mock.LoadObjectData (
              Arg<ICollection<ObjectID>>.List.Equal (new[] { _domainObject1.ID, _domainObject2.ID }),
              Arg.Is (false)))
          .Return (new[] { _fakeLoadedObjectData2, _fakeLoadedObjectData1 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (
              mock =>
              mock.RegisterIfRequired (
                  Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _fakeLoadedObjectData2, _fakeLoadedObjectData1 }), Arg.Is (_dataManagerStub)))
          .Return (new[] { _domainObject2, _domainObject1 });

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObjects (new[] { _domainObject1.ID, _domainObject2.ID }, false, _dataManagerStub);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void GetOrLoadRelatedObject ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order3, "OrderTicket");
      var fakeOriginatingDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order3);

      _dataManagerStub.Stub (stub => stub.GetDataContainerWithLazyLoad (endPointID.ObjectID)).Return (fakeOriginatingDataContainer);
      _persistenceStrategyMock
          .Expect (mock => mock.ResolveObjectRelationData (fakeOriginatingDataContainer, endPointID, _loadedObjectDataProviderStub))
          .Return (_fakeLoadedObjectData1);
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (_fakeLoadedObjectData1, _dataManagerStub))
          .Return (_domainObject1);
      _mockRepository.ReplayAll();

      var result = _objectLoader.GetOrLoadRelatedObject (endPointID, _dataManagerStub, _loadedObjectDataProviderStub);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void GetOrLoadRelatedObject_WithNull ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order3, "OrderTicket");
      var fakeOriginatingDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order3);

      _dataManagerStub.Stub (stub => stub.GetDataContainerWithLazyLoad (endPointID.ObjectID)).Return (fakeOriginatingDataContainer);
      _persistenceStrategyMock
          .Expect (mock => mock.ResolveObjectRelationData (fakeOriginatingDataContainer, endPointID, _loadedObjectDataProviderStub))
          .Return (_fakeLoadedObjectData1);
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg.Is (_fakeLoadedObjectData1), Arg.Is (_dataManagerStub)))
          .Return (null);
      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadRelatedObject (endPointID, _dataManagerStub, _loadedObjectDataProviderStub);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "GetOrLoadRelatedObject can only be used with virtual end points.\r\nParameter name: relationEndPointID")]
    public void GetOrLoadRelatedObject_NonVirtualID ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      _objectLoader.GetOrLoadRelatedObject (endPointID, _dataManagerStub, _loadedObjectDataProviderStub);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "GetOrLoadRelatedObject can only be used with one-valued end points.\r\nParameter name: relationEndPointID")]
    public void GetOrLoadRelatedObject_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_domainObject1.ID, "OrderItems");
      _objectLoader.GetOrLoadRelatedObject (endPointID, _dataManagerStub, _loadedObjectDataProviderStub);
    }

    [Test]
    public void GetOrLoadRelatedObjects ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");

      _persistenceStrategyMock
          .Expect (mock => mock.ResolveCollectionRelationData (endPointID, _loadedObjectDataProviderStub))
          .Return (new[] { _fakeLoadedObjectData1, _fakeLoadedObjectData2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (
              mock =>
              mock.RegisterIfRequired (
                  Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _fakeLoadedObjectData1, _fakeLoadedObjectData2 }), Arg.Is (_dataManagerStub)))
          .Return (new[] { _domainObject1, _domainObject2 });

      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadRelatedObjects (endPointID, _dataManagerStub, _loadedObjectDataProviderStub);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "GetOrLoadRelatedObjects can only be used with many-valued end points.\r\nParameter name: relationEndPointID")]
    public void GetOrLoadRelatedObjects_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _objectLoader.GetOrLoadRelatedObjects (endPointID, _dataManagerStub, _loadedObjectDataProviderStub);
    }

    [Test]
    public void GetOrLoadCollectionQueryResult ()
    {
      _persistenceStrategyMock
          .Expect (mock => mock.ExecuteCollectionQuery (_fakeQuery, _loadedObjectDataProviderStub))
          .Return (new[] { _fakeLoadedObjectData1, _fakeLoadedObjectData2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (
              Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _fakeLoadedObjectData1, _fakeLoadedObjectData2 }), 
              Arg.Is (_dataManagerStub)))
          .Return (new[] { _domainObject1, _domainObject2 });

      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadCollectionQueryResult<Order> (_fakeQuery, _dataManagerStub, _loadedObjectDataProviderStub);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_WithNulls ()
    {
      _persistenceStrategyMock
          .Expect (mock => mock.ExecuteCollectionQuery (_fakeQuery, _loadedObjectDataProviderStub))
          .Return (new[] { _fakeLoadedObjectData1, _fakeLoadedObjectData2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (
              mock =>
              mock.RegisterIfRequired (
                  Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _fakeLoadedObjectData1, _fakeLoadedObjectData2 }), Arg.Is (_dataManagerStub)))
          .Return (new[] { _domainObject1, null });

      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadCollectionQueryResult<Order> (_fakeQuery, _dataManagerStub, _loadedObjectDataProviderStub);

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
          .Expect (mock => mock.ExecuteCollectionQuery (_fakeQuery, _loadedObjectDataProviderStub))
          .Return (new[] { _fakeLoadedObjectData1 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (
              mock =>
              mock.RegisterIfRequired (Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _fakeLoadedObjectData1 }), Arg.Is (_dataManagerStub)))
          .Return (new[] { _domainObject1 });

      _mockRepository.ReplayAll ();

      _objectLoader.GetOrLoadCollectionQueryResult<Customer> (_fakeQuery, _dataManagerStub, _loadedObjectDataProviderStub);
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_WithFetching ()
    {
      var fetchQueryStub = CreateFakeQuery ();
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");
      _fakeQuery.EagerFetchQueries.Add (endPointDefinition, fetchQueryStub);

      _persistenceStrategyMock
          .Expect (mock => mock.ExecuteCollectionQuery (_fakeQuery, _loadedObjectDataProviderStub))
          .Return (new[] { _fakeLoadedObjectData1 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _fakeLoadedObjectData1 }), Arg.Is (_dataManagerStub)))
          .Return (new[] { _domainObject1 });
      _fetcherMock
          .Expect (mock => mock.PerformEagerFetching (
              Arg<DomainObject[]>.List.Equal(new[] { _domainObject1 }),
              Arg.Is (endPointDefinition),
              Arg.Is (fetchQueryStub),
              Arg.Is (_objectLoader),
              Arg.Is (_dataManagerStub), 
              Arg.Is (_loadedObjectDataProviderStub)));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadCollectionQueryResult<Order> (_fakeQuery, _dataManagerStub, _loadedObjectDataProviderStub);

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
          .Expect (mock => mock.ExecuteCollectionQuery (_fakeQuery, _loadedObjectDataProviderStub))
          .Return (new ILoadedObjectData[0]);
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new ILoadedObjectData[0]), Arg.Is (_dataManagerStub)))
          .Return (new DomainObject[0]);

      _mockRepository.ReplayAll ();

      _objectLoader.GetOrLoadCollectionQueryResult<Order> (_fakeQuery, _dataManagerStub, _loadedObjectDataProviderStub);

      _fetcherMock.AssertWasNotCalled (mock => mock.PerformEagerFetching (
          Arg<DomainObject[]>.Is.Anything,
          Arg<IRelationEndPointDefinition>.Is.Anything,
          Arg<IQuery>.Is.Anything,
          Arg<IObjectLoader>.Is.Anything,
          Arg<IDataManager>.Is.Anything, 
          Arg<ILoadedObjectDataProvider>.Is.Anything));
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
  }
}