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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  public class EagerFetcherTest : StandardMappingTest
  {
    private IDataManager _dataManagerMock;
    private IRelationEndPointMapReadOnlyView _relationEndPointMapStub;
    private EagerFetcher _eagerFetcher;
    private IObjectLoader _objectLoaderMock;

    private IQuery _queryStub;

    private IRelationEndPointDefinition _endPointDefinitionWithCardinalityOne;
    private IRelationEndPointDefinition _endPointDefinitionWithCardinalityMany;

    private DomainObject _fakeOriginalObject1;
    private DomainObject _fakeOriginalObject2;

    private DomainObject _fakeFetchedOneObject1;
    private DomainObject _fakeFetchedOneObject2;
    private DomainObject _fakeFetchedManyObject1;
    private DomainObject _fakeFetchedManyObject2;


    public override void SetUp ()
    {
      base.SetUp();

      _dataManagerMock = MockRepository.GenerateStrictMock<IDataManager>();
      _relationEndPointMapStub = MockRepository.GenerateStub<IRelationEndPointMapReadOnlyView>();
      _dataManagerMock.Stub ((stub => stub.RelationEndPointMap)).Return (_relationEndPointMapStub);
      _objectLoaderMock = MockRepository.GenerateStrictMock<IObjectLoader>();

      _eagerFetcher = new EagerFetcher();

      _queryStub = MockRepository.GenerateStub<IQuery>();

      var orderClassDefinition = DomainObjectIDs.Order1.ClassDefinition;
      _endPointDefinitionWithCardinalityOne = orderClassDefinition.GetRelationEndPointDefinition (typeof (Order).FullName + "." + "OrderTicket");
      _endPointDefinitionWithCardinalityMany = orderClassDefinition.GetRelationEndPointDefinition (typeof (Order).FullName + "." + "OrderItems");

      _fakeOriginalObject1 = DomainObjectMother.CreateFakeObject<Order>();
      _fakeOriginalObject2 = DomainObjectMother.CreateFakeObject<Order>();

      _fakeFetchedOneObject1 = DomainObjectMother.CreateFakeObject<OrderTicket>();
      _fakeFetchedOneObject2 = DomainObjectMother.CreateFakeObject<OrderTicket>();

      _fakeFetchedManyObject1 = DomainObjectMother.CreateFakeObject<OrderItem>();
      _fakeFetchedManyObject2 = DomainObjectMother.CreateFakeObject<OrderItem>();
    }

    [Test]
    public void PerformEagerFetching_EndPointCardinalityOne ()
    {
      _objectLoaderMock
          .Expect (mock => mock.LoadCollectionQueryResult<DomainObject> (_queryStub, _dataManagerMock))
          .Return (new[] { _fakeFetchedOneObject1, _fakeFetchedOneObject2 });
      _objectLoaderMock.Replay();
      _dataManagerMock.Replay();

      _eagerFetcher.PerformEagerFetching (
          new[] { _fakeOriginalObject1, _fakeOriginalObject2 }, _endPointDefinitionWithCardinalityOne, _queryStub, _objectLoaderMock, _dataManagerMock);

      _objectLoaderMock.VerifyAllExpectations();
      _dataManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void PerformEagerFetching_EndPointCardinalityMany ()
    {
      _objectLoaderMock
          .Expect (mock => mock.LoadCollectionQueryResult<DomainObject> (_queryStub, _dataManagerMock))
          .Return (new[] { _fakeFetchedManyObject1, _fakeFetchedManyObject2 });
      _objectLoaderMock.Replay();

      _dataManagerMock.Expect (
          mock => mock.MarkCollectionEndPointComplete (RelationEndPointID.Create(_fakeOriginalObject1.ID, _endPointDefinitionWithCardinalityMany)));
      _dataManagerMock.Expect (
          mock => mock.MarkCollectionEndPointComplete (RelationEndPointID.Create(_fakeOriginalObject2.ID, _endPointDefinitionWithCardinalityMany)));
      _dataManagerMock.Replay();

      _eagerFetcher.PerformEagerFetching (
          new[] { _fakeOriginalObject1, _fakeOriginalObject2 },
          _endPointDefinitionWithCardinalityMany,
          _queryStub,
          _objectLoaderMock,
          _dataManagerMock);

      _objectLoaderMock.VerifyAllExpectations();
      _dataManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void PerformEagerFetching_EndPointCardinalityMany_WithNullOriginalObject ()
    {
      _objectLoaderMock
          .Expect (mock => mock.LoadCollectionQueryResult<DomainObject> (_queryStub, _dataManagerMock))
          .Return (new DomainObject[0]);
      _objectLoaderMock.Replay();

      _dataManagerMock.Replay();

      _eagerFetcher.PerformEagerFetching (
          new DomainObject[] { null }, _endPointDefinitionWithCardinalityMany, _queryStub, _objectLoaderMock, _dataManagerMock);

      _objectLoaderMock.VerifyAllExpectations();
      _dataManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void PerformEagerFetching_WithNullRelatedObject ()
    {
      _objectLoaderMock
          .Expect (mock => mock.LoadCollectionQueryResult<DomainObject> (_queryStub, _dataManagerMock))
          .Return (new DomainObject[] { null });
      _objectLoaderMock.Replay();

      _dataManagerMock.Expect (
          mock => mock.MarkCollectionEndPointComplete (RelationEndPointID.Create(_fakeOriginalObject1.ID, _endPointDefinitionWithCardinalityMany)));
      _dataManagerMock.Replay();

      _eagerFetcher.PerformEagerFetching (
          new[] { _fakeOriginalObject1 }, _endPointDefinitionWithCardinalityMany, _queryStub, _objectLoaderMock, _dataManagerMock);

      _objectLoaderMock.VerifyAllExpectations();
      _dataManagerMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (UnexpectedQueryResultException), ExpectedMessage =
        "Eager fetching cannot be performed for query result object "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' and relation end point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket'. The end point belongs to an object of class 'Order' but the query "
        + "result has class 'OrderItem'.")]
    public void PerformEagerFetching_InvalidOriginalObject ()
    {
      var invalidOriginalObject = DomainObjectMother.CreateFakeObject<OrderItem> (DomainObjectIDs.OrderItem1);
      _eagerFetcher.PerformEagerFetching (
          new[] { invalidOriginalObject }, _endPointDefinitionWithCardinalityOne, _queryStub, _objectLoaderMock, _dataManagerMock);
    }

    [Test]
    [ExpectedException (typeof (UnexpectedQueryResultException), ExpectedMessage =
        "An eager fetch query returned an object of an unexpected type. For relation end point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket', an object of class 'OrderTicket' was expected, but an object of "
        + "class 'OrderItem' was returned.")]
    public void PerformEagerFetching_InvalidFetchedObject ()
    {
      var invalidFetchedObject = DomainObjectMother.CreateFakeObject<OrderItem> (DomainObjectIDs.OrderItem1);

      _objectLoaderMock
          .Stub (mock => mock.LoadCollectionQueryResult<DomainObject> (_queryStub, _dataManagerMock))
          .Return (new[] { invalidFetchedObject });
      _objectLoaderMock.Replay();

      _eagerFetcher.PerformEagerFetching (
          new[] { _fakeOriginalObject1, _fakeOriginalObject2 }, _endPointDefinitionWithCardinalityOne, _queryStub, _objectLoaderMock, _dataManagerMock);
    }
  }
}