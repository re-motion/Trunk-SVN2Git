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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
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

    private Order _originatingOrder1;
    private Order _originatingOrder2;

    private OrderTicket _fetchedOrderTicket1;
    private OrderTicket _fetchedOrderTicket2;
    private OrderItem _fetchedOrderItem1;
    private OrderItem _fetchedOrderItem2;
    private OrderItem _fetchedOrderItem3;
    private DataContainer _fetchedOrderItemDataContainer1;
    private DataContainer _fetchedOrderItemDataContainer2;
    private DataContainer _fetchedOrderItemDataContainer3;


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

      _originatingOrder1 = DomainObjectMother.CreateFakeObject<Order>();
      _originatingOrder2 = DomainObjectMother.CreateFakeObject<Order>();

      _fetchedOrderTicket1 = DomainObjectMother.CreateFakeObject<OrderTicket>();
      _fetchedOrderTicket2 = DomainObjectMother.CreateFakeObject<OrderTicket>();

      _fetchedOrderItem1 = DomainObjectMother.CreateFakeObject<OrderItem>();
      _fetchedOrderItem2 = DomainObjectMother.CreateFakeObject<OrderItem> ();
      _fetchedOrderItem3 = DomainObjectMother.CreateFakeObject<OrderItem> ();

      _fetchedOrderItemDataContainer1 = CreateFetchedOrderItemDataContainer (_fetchedOrderItem1, _originatingOrder1.ID);
      _fetchedOrderItemDataContainer2 = CreateFetchedOrderItemDataContainer (_fetchedOrderItem2, _originatingOrder2.ID);
      _fetchedOrderItemDataContainer3 = CreateFetchedOrderItemDataContainer (_fetchedOrderItem3, _originatingOrder1.ID);
    }

    [Test]
    public void PerformEagerFetching_EndPointCardinalityOne ()
    {
      _objectLoaderMock
          .Expect (mock => mock.LoadCollectionQueryResult<DomainObject> (_queryStub, _dataManagerMock))
          .Return (new[] { _fetchedOrderTicket1, _fetchedOrderTicket2 });
      _objectLoaderMock.Replay();
      _dataManagerMock.Replay();

      _eagerFetcher.PerformEagerFetching (
          new[] { _originatingOrder1, _originatingOrder2 }, _endPointDefinitionWithCardinalityOne, _queryStub, _objectLoaderMock, _dataManagerMock);

      _objectLoaderMock.VerifyAllExpectations();
      _dataManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void PerformEagerFetching_EndPointCardinalityMany ()
    {
      _objectLoaderMock
          .Expect (mock => mock.LoadCollectionQueryResult<DomainObject> (_queryStub, _dataManagerMock))
          .Return (new[] { _fetchedOrderItem1, _fetchedOrderItem2, _fetchedOrderItem3 });
      _objectLoaderMock.Replay();

      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem1.ID)).Return (_fetchedOrderItemDataContainer1);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem2.ID)).Return (_fetchedOrderItemDataContainer2);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem3.ID)).Return (_fetchedOrderItemDataContainer3);

      _dataManagerMock.Expect (
          mock => mock.MarkCollectionEndPointComplete (
              RelationEndPointID.Create(_originatingOrder1.ID, _endPointDefinitionWithCardinalityMany),
              new[] { _fetchedOrderItem1, _fetchedOrderItem3 }));
      _dataManagerMock.Expect (
          mock => mock.MarkCollectionEndPointComplete (
              RelationEndPointID.Create (_originatingOrder2.ID, _endPointDefinitionWithCardinalityMany),
              new[] { _fetchedOrderItem2 }));
      _dataManagerMock.Replay();

      _eagerFetcher.PerformEagerFetching (
          new[] { _originatingOrder1, _originatingOrder2 },
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
          mock => mock.MarkCollectionEndPointComplete (
              RelationEndPointID.Create (_originatingOrder1.ID, _endPointDefinitionWithCardinalityMany), 
              new DomainObject[0]));
      _dataManagerMock.Replay();

      _eagerFetcher.PerformEagerFetching (
          new[] { _originatingOrder1 }, _endPointDefinitionWithCardinalityMany, _queryStub, _objectLoaderMock, _dataManagerMock);

      _objectLoaderMock.VerifyAllExpectations();
      _dataManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void PerformEagerFetching_WithRelatedObjectPointingToNull ()
    {
      _objectLoaderMock
          .Expect (mock => mock.LoadCollectionQueryResult<DomainObject> (_queryStub, _dataManagerMock))
          .Return (new[] { _fetchedOrderItem1 });
      _objectLoaderMock.Replay ();

      var dataContainerPointingToNull = CreateFetchedOrderItemDataContainer (_fetchedOrderItem1, null);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem1.ID)).Return (dataContainerPointingToNull);

      _dataManagerMock.Expect (
          mock => mock.MarkCollectionEndPointComplete (
              RelationEndPointID.Create (_originatingOrder1.ID, _endPointDefinitionWithCardinalityMany),
              new DomainObject[0]));
      _dataManagerMock.Replay ();

      _eagerFetcher.PerformEagerFetching (
          new[] { _originatingOrder1 },
          _endPointDefinitionWithCardinalityMany,
          _queryStub,
          _objectLoaderMock,
          _dataManagerMock);

      _objectLoaderMock.VerifyAllExpectations ();
      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void PerformEagerFetching_EndPointCardinalityMany_EndPointAlreadyComplete ()
    {
      _objectLoaderMock
          .Expect (mock => mock.LoadCollectionQueryResult<DomainObject> (_queryStub, _dataManagerMock))
          .Return (new[] { _fetchedOrderItem1, _fetchedOrderItem2, _fetchedOrderItem3 });
      _objectLoaderMock.Replay ();

      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem1.ID)).Return (_fetchedOrderItemDataContainer1);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem2.ID)).Return (_fetchedOrderItemDataContainer2);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem3.ID)).Return (_fetchedOrderItemDataContainer3);

      _dataManagerMock
          .Expect (
              mock => mock.MarkCollectionEndPointComplete (
                  RelationEndPointID.Create (_originatingOrder1.ID, _endPointDefinitionWithCardinalityMany),
                  new[] { _fetchedOrderItem1, _fetchedOrderItem3 }))
          .Throw (new InvalidOperationException ("Already complete."));
      _dataManagerMock.Expect (
          mock => mock.MarkCollectionEndPointComplete (
              RelationEndPointID.Create (_originatingOrder2.ID, _endPointDefinitionWithCardinalityMany),
              new[] { _fetchedOrderItem2 }));
      _dataManagerMock.Replay ();

      _eagerFetcher.PerformEagerFetching (
          new[] { _originatingOrder1, _originatingOrder2 },
          _endPointDefinitionWithCardinalityMany,
          _queryStub,
          _objectLoaderMock,
          _dataManagerMock);

      _objectLoaderMock.VerifyAllExpectations ();
      _dataManagerMock.VerifyAllExpectations ();
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
          new[] { _originatingOrder1, _originatingOrder2 }, _endPointDefinitionWithCardinalityOne, _queryStub, _objectLoaderMock, _dataManagerMock);
    }

    private DataContainer CreateFetchedOrderItemDataContainer (OrderItem fetchedOrderItem, ObjectID originatingOrderID)
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (fetchedOrderItem.ID, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, originatingOrderID);
      return dataContainer;
    }
  }
}