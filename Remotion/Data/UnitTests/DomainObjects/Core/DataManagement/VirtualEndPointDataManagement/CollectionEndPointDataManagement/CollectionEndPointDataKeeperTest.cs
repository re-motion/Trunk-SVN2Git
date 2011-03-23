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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPointDataManagement.CollectionEndPointDataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPointDataManagement.CollectionEndPointDataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPointDataManagement.CollectionEndPointDataManagement
{
  [TestFixture]
  public class CollectionEndPointDataKeeperTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private ICollectionEndPointChangeDetectionStrategy _changeDetectionStrategyMock;
    
    private DomainObject _domainObject1;
    private DomainObject _domainObject2;
    private DomainObject _domainObject3;
    private DomainObject _domainObject4;

    private IRealObjectEndPoint _domainObjectEndPoint1;
    private IRealObjectEndPoint _domainObjectEndPoint2;
    private IRealObjectEndPoint _domainObjectEndPoint3;

    private ClientTransaction _clientTransaction;
    
    private CollectionEndPointDataKeeper _dataKeeper;

    private DelegateBasedComparer<DomainObject> _comparer123;
    private VirtualEndPointStateUpdateListener _stateUpdateListener;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _changeDetectionStrategyMock = MockRepository.GenerateStrictMock<ICollectionEndPointChangeDetectionStrategy> ();

      _clientTransaction = ClientTransaction.CreateRootTransaction();

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order2);
      _domainObject3 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order3);
      _domainObject4 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order4);

      _domainObjectEndPoint1 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _domainObjectEndPoint1.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject1);
      _domainObjectEndPoint1.Stub (stub => stub.ObjectID).Return (_domainObject1.ID);

      _domainObjectEndPoint2 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _domainObjectEndPoint2.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject2);
      _domainObjectEndPoint2.Stub (stub => stub.ObjectID).Return (_domainObject2.ID);

      _domainObjectEndPoint3 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _domainObjectEndPoint3.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject3);
      _domainObjectEndPoint3.Stub (stub => stub.ObjectID).Return (_domainObject3.ID);

      _stateUpdateListener = new VirtualEndPointStateUpdateListener (_clientTransaction, _endPointID);
      _dataKeeper = new CollectionEndPointDataKeeper (_endPointID, _changeDetectionStrategyMock, _stateUpdateListener);

      _comparer123 = new DelegateBasedComparer<DomainObject> (Compare123);
    }

    [Test]
    public void Initialization ()
    {
      var dataKeeper = new CollectionEndPointDataKeeper (_endPointID, _changeDetectionStrategyMock, _stateUpdateListener);

      Assert.That (dataKeeper.CollectionData, Is.TypeOf (typeof (ChangeCachingCollectionDataDecorator)));
      Assert.That (dataKeeper.CollectionData.ToArray (), Is.Empty);
      Assert.That (dataKeeper.OriginalOppositeEndPoints, Is.Empty);
   }

    [Test]
    public void CollectionData ()
    {
      _dataKeeper.CollectionData.Insert (0, _domainObject1);

      Assert.That (_dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1 }));
    }

    [Test]
    public void ContainsOriginalOppositeEndPoint ()
    {
      var oppositeEndPoint = CollectionEndPointTestHelper.GetFakeOppositeEndPoint(_domainObject1);

      Assert.That (_dataKeeper.ContainsOriginalOppositeEndPoint (oppositeEndPoint), Is.False);

      _dataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);

      Assert.That (_dataKeeper.ContainsOriginalOppositeEndPoint (oppositeEndPoint), Is.True);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      Assert.That (_dataKeeper.OriginalOppositeEndPoints.ToArray(), Is.Empty);

      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject2);
      endPointStub.Stub (stub => stub.ObjectID).Return (_domainObject2.ID);
      
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalItemsWithoutEndPoints, List.Not.Contains (_domainObject2));

      _dataKeeper.RegisterOriginalOppositeEndPoint (endPointStub);

      Assert.That (_dataKeeper.HasDataChanged (), Is.False);
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray(), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalOppositeEndPoints.ToArray(), Is.EqualTo (new[] { endPointStub }));
      Assert.That (_dataKeeper.CurrentOppositeEndPoints.ToArray (), Is.EqualTo (new[] { endPointStub }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has already been registered.")]
    public void RegisterOriginalOppositeEndPoint_AlreadyRegistered ()
    {
      var oppositeEndPoint = CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1);
      _dataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      _dataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_AlreadyRegisteredWithoutEndPoint ()
    {
      Assert.That (_dataKeeper.OriginalOppositeEndPoints.ToArray (), Is.Empty);

      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject2);
      endPointStub.Stub (stub => stub.ObjectID).Return (_domainObject2.ID);

      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_domainObject2);

      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalOppositeEndPoints, Is.Empty);
      Assert.That (_dataKeeper.CurrentOppositeEndPoints, Is.Empty);
      Assert.That (_dataKeeper.OriginalItemsWithoutEndPoints, List.Contains (_domainObject2));

      _dataKeeper.RegisterOriginalOppositeEndPoint (endPointStub);

      Assert.That (_dataKeeper.HasDataChanged (), Is.False);
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalOppositeEndPoints.ToArray (), Is.EqualTo (new[] { endPointStub }));
      Assert.That (_dataKeeper.OriginalItemsWithoutEndPoints, List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.CurrentOppositeEndPoints.ToArray(), Is.EqualTo(new[]{endPointStub}));
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject2);
      endPointStub.Stub (stub => stub.ObjectID).Return (_domainObject2.ID);
      endPointStub.Stub (stub => stub.MarkSynchronized());

      _dataKeeper.RegisterOriginalOppositeEndPoint (endPointStub);

      Assert.That (_dataKeeper.OriginalOppositeEndPoints.Length, Is.EqualTo (1));
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.CurrentOppositeEndPoints.ToArray (), Is.EqualTo (new[] { endPointStub }));

      _dataKeeper.UnregisterOriginalOppositeEndPoint (endPointStub);

      Assert.That (_dataKeeper.HasDataChanged (), Is.False);
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalOppositeEndPoints.ToArray(), Is.Empty);
      Assert.That (_dataKeeper.CurrentOppositeEndPoints, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has not been registered.")]
    public void UnregisterOriginalOppositeEndPoint_NotRegistered ()
    {
      var oppositeEndPoint = CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1);
      _dataKeeper.UnregisterOriginalOppositeEndPoint (oppositeEndPoint);
    }

    [Test]
    public void ContainsCurrentOppositeEndPoint ()
    {
      Assert.That (_dataKeeper.ContainsCurrentOppositeEndPoint (_domainObjectEndPoint1), Is.False);

      _dataKeeper.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint1);

      Assert.That (_dataKeeper.ContainsCurrentOppositeEndPoint (_domainObjectEndPoint1), Is.True);
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      Assert.That (_dataKeeper.CurrentOppositeEndPoints.ToArray (), Is.Empty);

      _dataKeeper.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint1);

      Assert.That (_dataKeeper.CurrentOppositeEndPoints.ToArray (), Is.EqualTo (new[] { _domainObjectEndPoint1 }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has already been registered.")]
    public void RegisterCurrentOppositeEndPoint_AlreadyRegistered ()
    {
      _dataKeeper.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint1);
      _dataKeeper.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint1);
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _dataKeeper.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint1);

      Assert.That (_dataKeeper.CurrentOppositeEndPoints.ToArray (), List.Contains (_domainObjectEndPoint1));

      _dataKeeper.UnregisterCurrentOppositeEndPoint (_domainObjectEndPoint1);

      Assert.That (_dataKeeper.CurrentOppositeEndPoints.ToArray(), List.Not.Contains(_domainObjectEndPoint1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has not been registered.")]
    public void UnregisterCurrentOppositeEndPoint_NotRegistered ()
    {
      _dataKeeper.UnregisterCurrentOppositeEndPoint (_domainObjectEndPoint1);
    }
    
    [Test]
    public void ContainsOriginalItemWithoutEndPoint_True ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_domainObject2);
      Assert.That (_dataKeeper.ContainsOriginalItemWithoutEndPoint (_domainObject2), Is.True);
    }

    [Test]
    public void ContainsOriginalItemWithoutEndPoint_False ()
    {
      Assert.That (_dataKeeper.ContainsOriginalItemWithoutEndPoint (_domainObject2), Is.False);
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint ()
    {
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalItemsWithoutEndPoints.ToArray (), List.Not.Contains (_domainObject2));

      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_domainObject2);

      Assert.That (_dataKeeper.OriginalItemsWithoutEndPoints, Is.EqualTo (new[] { _domainObject2 }));
      Assert.That (_dataKeeper.HasDataChanged (), Is.False);
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalOppositeEndPoints.ToArray (), Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The original collection already contains a domain object with ID 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid'.")]
    public void RegisterOriginalItemWithoutEndPoint_AlreadyRegisteredWithoutEndPoint ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_domainObject2);
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_domainObject2);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The original collection already contains a domain object with ID 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid'.")]
    public void RegisterOriginalItemWithoutEndPoint_AlreadyRegisteredWithEndPoint ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_domainObjectEndPoint2);
      try
      {
        _dataKeeper.RegisterOriginalItemWithoutEndPoint (_domainObject2);
      }
      catch
      {
        Assert.That (_dataKeeper.OriginalItemsWithoutEndPoints, List.Not.Contains (_domainObject2));
        throw;
      }
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_domainObject2);
      Assert.That (_dataKeeper.OriginalItemsWithoutEndPoints, Is.EqualTo (new[] { _domainObject2 }));
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Contains (_domainObject2));

      _dataKeeper.UnregisterOriginalItemWithoutEndPoint (_domainObject2);

      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalItemsWithoutEndPoints.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.HasDataChanged (), Is.False);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' has not been registered as an item without end-point.")]
    public void UnregisterOriginalItemWithoutEndPoint_ItemNotRegisteredWithoutEndPoint ()
    {
      _dataKeeper.UnregisterOriginalItemWithoutEndPoint (_domainObject2);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The domain object with ID 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' has not been registered as an item without end-point.")]
    public void UnregisterOriginalItemWithoutEndPoint_RegisteredWithEndPoint ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_domainObjectEndPoint2);

      try
      {
        _dataKeeper.UnregisterOriginalItemWithoutEndPoint (_domainObject2);
      }
      catch
      {
        Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Contains (_domainObject2));
        throw;
      }
    }

    [Test]
    public void HasDataChanged ()
    {
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (
              Arg.Is (_dataKeeper.CollectionData), 
              Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
          .Return (true);
      _changeDetectionStrategyMock.Replay ();

      _dataKeeper.CollectionData.Add (_domainObject2); // require use of strategy

      var result = _dataKeeper.HasDataChanged ();

      _changeDetectionStrategyMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (true));
    }

    [Test]
    public void HasDataChanged_Cached ()
    {
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (
              Arg.Is (_dataKeeper.CollectionData), 
              Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
          .Return (true)
          .Repeat.Once();
      _changeDetectionStrategyMock.Replay ();

      _dataKeeper.CollectionData.Add (_domainObject2); // require use of strategy
 
      var result1 = _dataKeeper.HasDataChanged ();
      var result2 = _dataKeeper.HasDataChanged ();

      _changeDetectionStrategyMock.VerifyAllExpectations ();

      Assert.That (result1, Is.EqualTo (true));
      Assert.That (result2, Is.EqualTo (true));
    }

    [Test]
    public void HasDataChanged_Cache_InvalidatedOnModifyingOperations ()
    {
      using (_changeDetectionStrategyMock.GetMockRepository ().Ordered ())
      {
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (
                Arg.Is (_dataKeeper.CollectionData), 
                Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
            .Return (true);
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (
                Arg.Is (_dataKeeper.CollectionData), 
                Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
            .Return (false);
      }
      _changeDetectionStrategyMock.Replay ();

      _dataKeeper.CollectionData.Add (_domainObject2); // require use of strategy

      var result1 = _dataKeeper.HasDataChanged ();

      _dataKeeper.CollectionData.Clear ();

      var result2 = _dataKeeper.HasDataChanged ();

      _changeDetectionStrategyMock.VerifyAllExpectations ();
      Assert.That (result1, Is.EqualTo (true));
      Assert.That (result2, Is.EqualTo (false));
    }

    [Test]
    public void SortCurrentAndOriginalData ()
    {
      var dataKeeper = new CollectionEndPointDataKeeper (_endPointID, _changeDetectionStrategyMock, _stateUpdateListener);

      dataKeeper.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject3));
      dataKeeper.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1));
      dataKeeper.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject2));
      
      dataKeeper.SortCurrentAndOriginalData(_comparer123);

      Assert.That (dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
      Assert.That (dataKeeper.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
    }

    [Test]
    public void OriginalCollectionData ()
    {
      var originalData = _dataKeeper.OriginalCollectionData;

      Assert.That (originalData.ToArray (), Is.Empty);
      Assert.That (originalData.IsReadOnly, Is.True);
    }

    [Test]
    public void Commit_UpdatesOriginalContentsAndEndPoints ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_domainObjectEndPoint1);
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_domainObject2);

      _dataKeeper.CollectionData.Insert (0, _domainObject3);
      _dataKeeper.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint3);

      _dataKeeper.CollectionData.Insert (0, _domainObject4);

      Assert.That (_dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject4, _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That (_dataKeeper.CurrentOppositeEndPoints, Is.EquivalentTo(new[] { _domainObjectEndPoint1, _domainObjectEndPoint3 }));

      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_dataKeeper.OriginalOppositeEndPoints, Is.EquivalentTo (new[] { _domainObjectEndPoint1 }));
      Assert.That (_dataKeeper.OriginalItemsWithoutEndPoints, Is.EquivalentTo (new[] { _domainObject2 }));

      _dataKeeper.Commit();

      Assert.That (_dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject4, _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That (_dataKeeper.CurrentOppositeEndPoints, Is.EquivalentTo (new[] { _domainObjectEndPoint1, _domainObjectEndPoint3 }));

      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject4, _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That (_dataKeeper.OriginalOppositeEndPoints, Is.EquivalentTo (new[] { _domainObjectEndPoint1, _domainObjectEndPoint3 }));
      Assert.That (_dataKeeper.OriginalItemsWithoutEndPoints, Is.EquivalentTo (new[] { _domainObject2, _domainObject4 }));
    }

    [Test]
    public void Commit_InvalidatesHasChangedCache ()
    {
      using (_changeDetectionStrategyMock.GetMockRepository ().Ordered ())
      {
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (
                Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.CollectionData),
                Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
            .Return (true);
      }
      _changeDetectionStrategyMock.Replay ();

      _dataKeeper.CollectionData.Add (_domainObject2); // require use of strategy

      Assert.That (_dataKeeper.HasDataChanged (), Is.True);

      _dataKeeper.Commit ();

      Assert.That (_dataKeeper.HasDataChanged (), Is.False);
    }

    [Test]
    public void Rollback_UpdatesCurrentContentsAndEndPoints ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_domainObjectEndPoint1);
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_domainObject2);

      _dataKeeper.CollectionData.Insert (0, _domainObject3);
      _dataKeeper.RegisterCurrentOppositeEndPoint (_domainObjectEndPoint3);

      Assert.That (_dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That (_dataKeeper.CurrentOppositeEndPoints.ToArray (), Is.EquivalentTo (new[] { _domainObjectEndPoint1, _domainObjectEndPoint3 }));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_dataKeeper.OriginalOppositeEndPoints.ToArray (), Is.EquivalentTo (new[] { _domainObjectEndPoint1 }));
      Assert.That (_dataKeeper.OriginalItemsWithoutEndPoints.ToArray (), Is.EquivalentTo (new[] { _domainObject2 }));

      _dataKeeper.Rollback();

      Assert.That (_dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_dataKeeper.CurrentOppositeEndPoints.ToArray (), Is.EquivalentTo (new[] { _domainObjectEndPoint1 }));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_dataKeeper.OriginalOppositeEndPoints.ToArray (), Is.EquivalentTo (new[] { _domainObjectEndPoint1 }));
      Assert.That (_dataKeeper.OriginalItemsWithoutEndPoints.ToArray (), Is.EquivalentTo (new[] { _domainObject2 }));
    }

    [Test]
    public void Rollback_InvalidatesHasChangedCache ()
    {
      using (_changeDetectionStrategyMock.GetMockRepository ().Ordered ())
      {
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (
                Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.CollectionData),
                Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
            .Return (true);
      }
      _changeDetectionStrategyMock.Replay ();

      _dataKeeper.CollectionData.Add (_domainObject2); // require use of strategy

      Assert.That (_dataKeeper.HasDataChanged (), Is.True);

      _dataKeeper.Rollback();

      Assert.That (_dataKeeper.HasDataChanged (), Is.False);
    }

    [Test]
    public void StateUpdates_RoutedToTransactionEventSink ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_clientTransaction);
      _dataKeeper.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1));

      _dataKeeper.CollectionData.Clear();

      listenerMock.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_clientTransaction, _endPointID, null));
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var changeDetectionStrategy = new SerializableCollectionEndPointChangeDetectionStrategyFake();
      var updateListener = new VirtualEndPointStateUpdateListener (ClientTransaction.CreateRootTransaction(), _endPointID);
      var data = new CollectionEndPointDataKeeper (_endPointID, changeDetectionStrategy, updateListener);

      var endPointFake = new SerializableRealObjectEndPointFake (null, _domainObject1);
      data.RegisterOriginalOppositeEndPoint (endPointFake);
      data.RegisterOriginalItemWithoutEndPoint (_domainObject2);

      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (data);

      Assert.That (deserializedInstance.EndPointID, Is.Not.Null);
      Assert.That (deserializedInstance.ChangeDetectionStrategy, Is.Not.Null);
      Assert.That (deserializedInstance.CollectionData.Count, Is.EqualTo (2));
      Assert.That (deserializedInstance.OriginalCollectionData.Count, Is.EqualTo (2));
      Assert.That (deserializedInstance.OriginalOppositeEndPoints.Length, Is.EqualTo (1));
      Assert.That (deserializedInstance.OriginalItemsWithoutEndPoints.Length, Is.EqualTo (1));
      Assert.That (deserializedInstance.CurrentOppositeEndPoints.Length, Is.EqualTo (1));
    }

    private int Compare123 (DomainObject x, DomainObject y)
    {
      if (x == y)
        return 0;

      if (x == _domainObject1)
        return -1;

      if (x == _domainObject3)
        return 1;

      if (y == _domainObject1)
        return 1;

      return -1;
    }
  }
}