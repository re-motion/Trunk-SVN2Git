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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries.EagerFetching
{
  [TestFixture]
  public class EagerFetchingObjectLoaderDecoratorTest : StandardMappingTest
  {
    private IFetchEnabledObjectLoader _decoratedObjectLoaderMock;
    private IEagerFetcher _eagerFetcherMock;

    private EagerFetchingObjectLoaderDecorator _decorator;

    private IQuery _queryStub;
    private IQuery _fetchQueryStub1;
    private IQuery _fetchQueryStub2;

    private IRelationEndPointDefinition _orderTicketEndPointDefinition;
    private IRelationEndPointDefinition _customerEndPointDefinition;

    private Order _originatingOrder1;
    private Order _originatingOrder2;
    private ILoadedObjectData _originatingOrderData1;
    private ILoadedObjectData _originatingOrderData2;


    public override void SetUp ()
    {
      base.SetUp();

      _decoratedObjectLoaderMock = MockRepository.GenerateStrictMock<IFetchEnabledObjectLoader>();
      _eagerFetcherMock = MockRepository.GenerateStrictMock<IEagerFetcher>();

      _decorator = new EagerFetchingObjectLoaderDecorator (_decoratedObjectLoaderMock, _eagerFetcherMock);

      _queryStub = MockRepository.GenerateStub<IQuery>();
      _fetchQueryStub1 = MockRepository.GenerateStub<IQuery> ();
      _fetchQueryStub2 = MockRepository.GenerateStub<IQuery> ();

      _orderTicketEndPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");
      _customerEndPointDefinition = GetEndPointDefinition (typeof (Order), "Customer");

      _originatingOrder1 = DomainObjectMother.CreateFakeObject<Order>();
      _originatingOrder2 = DomainObjectMother.CreateFakeObject<Order>();

      _originatingOrderData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (_originatingOrder1);
      _originatingOrderData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (_originatingOrder2);
    }
    
    [Test]
    public void GetOrLoadCollectionQueryResult_PerformsEagerFetching ()
    {
      var fetchQueries = new EagerFetchQueryCollection
                         {
                             { _orderTicketEndPointDefinition, _fetchQueryStub1 },
                             { _customerEndPointDefinition, _fetchQueryStub2 }
                         };
      _queryStub
          .Stub (stub => stub.EagerFetchQueries)
          .Return (fetchQueries);

      var originatingObjectsData = new[] { _originatingOrderData1, _originatingOrderData2 };

      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_queryStub))
          .Return (originatingObjectsData);
      _decoratedObjectLoaderMock.Replay();

      _eagerFetcherMock.Expect (mock => mock.PerformEagerFetching (originatingObjectsData, fetchQueries, _decorator));
      _eagerFetcherMock.Replay();

      var result = _decorator.GetOrLoadCollectionQueryResult (_queryStub);

      _decoratedObjectLoaderMock.VerifyAllExpectations();
      _eagerFetcherMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (originatingObjectsData));
    }

    [Test]
    public void GetOrLoadFetchQueryResult_PerformsEagerFetching ()
    {
      var fetchQueries = new EagerFetchQueryCollection
                         {
                             { _orderTicketEndPointDefinition, _fetchQueryStub1 },
                             { _customerEndPointDefinition, _fetchQueryStub2 }
                         };
      _queryStub
          .Stub (stub => stub.EagerFetchQueries)
          .Return (fetchQueries);

      var originatingObjectsData =
          new[]
          {
              LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (_originatingOrderData1),
              LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (_originatingOrderData2)
          };

      _decoratedObjectLoaderMock
          .Expect (mock => mock.GetOrLoadFetchQueryResult (_queryStub))
          .Return (originatingObjectsData);
      _decoratedObjectLoaderMock.Replay ();

      _eagerFetcherMock
          .Expect (mock => mock.PerformEagerFetching (
              Arg<ICollection<ILoadedObjectData>>.List.Equal (new[] { _originatingOrderData1, _originatingOrderData2 }), 
              Arg.Is (fetchQueries),
              Arg.Is (_decorator)));
      _eagerFetcherMock.Replay ();

      var result = _decorator.GetOrLoadFetchQueryResult (_queryStub);

      _decoratedObjectLoaderMock.VerifyAllExpectations ();
      _eagerFetcherMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (originatingObjectsData));
    }

    [Test]
    public void Serialization ()
    {
      var instance = new EagerFetchingObjectLoaderDecorator (
          new SerializableFetchEnabledObjectLoaderFake(), new SerializableEagerFetcherFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.EagerFetcher, Is.Not.Null);
      Assert.That (deserializedInstance.DecoratedObjectLoader, Is.Not.Null);
    }
  }
}