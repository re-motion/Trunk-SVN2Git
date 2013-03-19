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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class FetchEnabledObjectLoaderTest : StandardMappingTest
  {
    private MockRepository _mockRepository;

    private IFetchEnabledPersistenceStrategy _persistenceStrategyMock;
    private ILoadedObjectDataRegistrationAgent _loadedObjectDataRegistrationAgentMock;
    private ILoadedObjectDataProvider _loadedObjectDataProviderStub;
    private IEagerFetcher _eagerFetcherMock;

    private FetchEnabledObjectLoader _fetchEnabledObjectLoader;
    
    private ILoadedObjectData _resultItem1;
    private ILoadedObjectData _resultItem2;
    private LoadedObjectDataWithDataSourceData _resultItemWithSourceData1;
    private LoadedObjectDataWithDataSourceData _resultItemWithSourceData2;
    private IRelationEndPointDefinition _orderTicketEndPointDefinition;
    private IRelationEndPointDefinition _customerEndPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();

      _persistenceStrategyMock = _mockRepository.StrictMock<IFetchEnabledPersistenceStrategy> ();
      _loadedObjectDataRegistrationAgentMock = _mockRepository.StrictMock<ILoadedObjectDataRegistrationAgent> ();
      _loadedObjectDataProviderStub = _mockRepository.Stub<ILoadedObjectDataProvider> ();
      _eagerFetcherMock = _mockRepository.StrictMock<IEagerFetcher> ();

      _fetchEnabledObjectLoader = new FetchEnabledObjectLoader (
          _persistenceStrategyMock,
          _loadedObjectDataRegistrationAgentMock,
          _loadedObjectDataProviderStub,
          _eagerFetcherMock);

      _resultItem1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.Order1);
      _resultItem2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.Order2);
      _resultItemWithSourceData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (DomainObjectIDs.Order1);
      _resultItemWithSourceData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (DomainObjectIDs.Order2);

      _orderTicketEndPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");
      _customerEndPointDefinition = GetEndPointDefinition (typeof (Order), "Customer");
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_PerformsEagerFetching ()
    {
      var fetchQuery1 = CreateFakeQuery ();
      var fetchQuery2 = CreateFakeQuery ();
      var fakeQuery = CreateFakeQuery (
          Tuple.Create (_orderTicketEndPointDefinition, fetchQuery1),
          Tuple.Create (_customerEndPointDefinition, fetchQuery2));

      _persistenceStrategyMock
          .Expect (mock => mock.ExecuteCollectionQuery (fakeQuery, _loadedObjectDataProviderStub))
          .Return (new[] { _resultItem1, _resultItem2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (
              mock => mock.RegisterIfRequired (
                  Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _resultItem1, _resultItem2 }), 
                  Arg.Is (true)));
      _eagerFetcherMock.Expect (
          mock => mock.PerformEagerFetching (new[] { _resultItem1, _resultItem2 }, fakeQuery.EagerFetchQueries, _fetchEnabledObjectLoader));
      _mockRepository.ReplayAll();

      var result = _fetchEnabledObjectLoader.GetOrLoadCollectionQueryResult (fakeQuery);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (new[] {_resultItem1, _resultItem2 }));
    }

    [Test]
    public void GetOrLoadFetchQueryResult ()
    {
      var fetchQuery1 = CreateFakeQuery();
      var fetchQuery2 = CreateFakeQuery();
      var fakeQuery = CreateFakeQuery (
          Tuple.Create (_orderTicketEndPointDefinition, fetchQuery1), 
          Tuple.Create (_customerEndPointDefinition, fetchQuery2));

      _persistenceStrategyMock
          .Expect (mock => mock.ExecuteFetchQuery (fakeQuery, _loadedObjectDataProviderStub))
          .Return (new[] { _resultItemWithSourceData1, _resultItemWithSourceData2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (
              Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _resultItemWithSourceData1.LoadedObjectData, _resultItemWithSourceData2.LoadedObjectData }),
              Arg.Is (true)));
      _eagerFetcherMock
          .Expect (mock => mock.PerformEagerFetching (
              Arg<ICollection<ILoadedObjectData>>.List.Equal (new[] { _resultItemWithSourceData1.LoadedObjectData, _resultItemWithSourceData2.LoadedObjectData }),
              Arg.Is (fakeQuery.EagerFetchQueries),
              Arg.Is (_fetchEnabledObjectLoader)));

      _mockRepository.ReplayAll ();

      var result = _fetchEnabledObjectLoader.GetOrLoadFetchQueryResult (fakeQuery);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (new[] { _resultItemWithSourceData1, _resultItemWithSourceData2 }));
    }

    [Test]
    public void Serializable ()
    {
      var instance = new FetchEnabledObjectLoader (
          new SerializableFetchEnabledPersistenceStrategyFake(),
          new SerializableLoadedObjectDataRegistrationAgentFake(),
          new SerializableLoadedObjectDataProviderFake(),
          new SerializableEagerFetcherFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.PersistenceStrategy, Is.Not.Null);
      Assert.That (deserializedInstance.LoadedObjectDataRegistrationAgent, Is.Not.Null);
      Assert.That (deserializedInstance.LoadedObjectDataProvider, Is.Not.Null);
      Assert.That (deserializedInstance.EagerFetcher, Is.Not.Null);
    }

    private IQuery CreateFakeQuery (params Tuple<IRelationEndPointDefinition, IQuery>[] fetchQueries)
    {
      var query = QueryFactory.CreateCollectionQuery (
          "test", TestDomainStorageProviderDefinition, "TEST", new QueryParameterCollection(), typeof (DomainObjectCollection));
      foreach (var fetchQuery in fetchQueries)
        query.EagerFetchQueries.Add (fetchQuery.Item1, fetchQuery.Item2);

      return query;
    }
  }
}