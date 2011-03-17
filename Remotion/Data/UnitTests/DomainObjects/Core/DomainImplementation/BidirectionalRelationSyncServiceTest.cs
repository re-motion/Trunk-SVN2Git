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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects.Mapping;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainImplementation
{
  [TestFixture]
  public class BidirectionalRelationSyncServiceTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private RelationEndPointMap _map;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransaction.CreateRootTransaction();

      var dataManager = ClientTransactionTestHelper.GetDataManager (_transaction);
      _map = DataManagerTestHelper.GetRelationEndPointMap (dataManager);
    }
    
    [Test]
    public void IsSynchronized ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.OrderItem1, typeof (OrderItem), "Order");
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint>();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);
      endPointStub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointStub.Stub (stub => stub.IsDataComplete).Return (true);
      endPointStub.Stub (stub => stub.IsSynchronized).Return (true).Repeat.Once();
      endPointStub.Stub (stub => stub.IsSynchronized).Return (false).Repeat.Once ();
      RelationEndPointMapTestHelper.AddEndPoint (_map, endPointStub);

      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, endPointID), Is.True);
      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, endPointID), Is.False);
    }

    [Test]
    public void IsSynchronized_CalledFromSubTransaction_UsesRootTransaction ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.OrderItem1, typeof (OrderItem), "Order");
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);
      endPointStub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointStub.Stub (stub => stub.IsDataComplete).Return (true);
      endPointStub.Stub (stub => stub.IsSynchronized).Return (true).Repeat.Once ();
      endPointStub.Stub (stub => stub.IsSynchronized).Return (false).Repeat.Once ();
      RelationEndPointMapTestHelper.AddEndPoint (_map, endPointStub);

      var subTransaction = _transaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope())
      {
        Assert.That (BidirectionalRelationSyncService.IsSynchronized (subTransaction, endPointID), Is.True);
        Assert.That (BidirectionalRelationSyncService.IsSynchronized (subTransaction, endPointID), Is.False);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' of object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has not yet been fully loaded into the given ClientTransaction.")]
    public void IsSynchronized_EndPointNotRegistered ()
    {
      BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has not yet been fully loaded into the given ClientTransaction.")]
    public void IsSynchronized_EndPointIncomplete ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);
      endPointStub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointStub.Stub (stub => stub.IsDataComplete).Return (false);
      RelationEndPointMapTestHelper.AddEndPoint (_map, endPointStub);

      BidirectionalRelationSyncService.IsSynchronized (_transaction, endPointID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "BidirectionalRelationSyncService cannot be used with unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void IsSynchronized_UnidirectionalRelationEndPoint ()
    {
      BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "BidirectionalRelationSyncService cannot be used with unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void IsSynchronized_AnonymousRelationEndPoint ()
    {
      var locationClientEndPoint = RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client");
      var oppositeEndPoint = RelationEndPointID.Create (DomainObjectIDs.Client1, locationClientEndPoint.Definition.GetOppositeEndPointDefinition());
      BidirectionalRelationSyncService.IsSynchronized (_transaction, oppositeEndPoint);
    }

    [Test]
    public void Synchronize_WithObjectEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.OrderItem1, typeof (OrderItem), "Order");

      var oppositeEndPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      oppositeEndPointStub.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems"));

      var objectEndPointMock = MockRepository.GenerateStrictMock<IObjectEndPoint> ();
      objectEndPointMock.Stub (stub => stub.ID).Return (endPointID);
      objectEndPointMock.Stub (stub => stub.Definition).Return (endPointID.Definition);
      objectEndPointMock.Stub (stub => stub.IsDataComplete).Return (true);
      objectEndPointMock.Stub (stub => stub.GetOppositeRelationEndPointID ()).Return (oppositeEndPointStub.ID);
      objectEndPointMock.Expect (mock => mock.Synchronize (oppositeEndPointStub));
      objectEndPointMock.Replay ();

      RelationEndPointMapTestHelper.AddEndPoint (_map, objectEndPointMock);
      RelationEndPointMapTestHelper.AddEndPoint (_map, oppositeEndPointStub);

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);

      objectEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void Synchronize_WithCollectionEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      var collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      collectionEndPointMock.Stub (stub => stub.ID).Return (endPointID);
      collectionEndPointMock.Stub (stub => stub.Definition).Return (endPointID.Definition);
      collectionEndPointMock.Stub (stub => stub.IsDataComplete).Return (true);
      collectionEndPointMock.Expect (mock => mock.Synchronize ());
      collectionEndPointMock.Replay();
      RelationEndPointMapTestHelper.AddEndPoint (_map, collectionEndPointMock);

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);

      collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
      "BidirectionalRelationSyncService cannot be used with unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void Synchronize_UnidirectionalEndpoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client");

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "BidirectionalRelationSyncService cannot be used with unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void Synchronize_AnonymousEndPoint ()
    {
      var locationClientEndPoint = RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client");
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Client1, locationClientEndPoint.Definition.GetOppositeEndPointDefinition ());

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' of object "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' has not yet been fully loaded into the given ClientTransaction.")]
    public void Synchronize_EndPointNotRegistered ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.OrderItem1, typeof (OrderItem), "Order");
      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has not yet been fully loaded into the given ClientTransaction.")]
    public void Synchronize_EndPointIncomplete ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);
      endPointStub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointStub.Stub (stub => stub.IsDataComplete).Return (false);
      RelationEndPointMapTestHelper.AddEndPoint (_map, endPointStub);

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);
    }
  }
}