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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement
{
  [TestFixture]
  public class CollectionEndPointDataKeeperTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private ICollectionEndPointChangeDetectionStrategy _changeDetectionStrategyMock;
    private IRelationEndPointProvider _endPointProviderStub;
    
    private DomainObject _domainObject1;
    private DomainObject _domainObject2;
    private DomainObject _domainObject3;

    private IObjectEndPoint _domainObjectEndPoint1;
    private IObjectEndPoint _domainObjectEndPoint2;
    private IObjectEndPoint _domainObjectEndPoint3;

    private ClientTransaction _clientTransaction;
    
    private CollectionEndPointDataKeeper _dataKeeper;

    private DelegateBasedComparer<DomainObject> _comparer123;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _changeDetectionStrategyMock = MockRepository.GenerateStrictMock<ICollectionEndPointChangeDetectionStrategy> ();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();

      _clientTransaction = ClientTransaction.CreateRootTransaction();

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject3 = DomainObjectMother.CreateFakeObject<Order> ();

      _domainObjectEndPoint1 = MockRepository.GenerateStub<IObjectEndPoint> ();
      _domainObjectEndPoint2 = MockRepository.GenerateStub<IObjectEndPoint> ();
      _domainObjectEndPoint3 = MockRepository.GenerateStub<IObjectEndPoint> ();

      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (RelationEndPointID.Create (_domainObject1.ID, typeof (Order), "Customer")))
          .Return (_domainObjectEndPoint1);
      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (RelationEndPointID.Create (_domainObject2.ID, typeof (Order), "Customer")))
          .Return (_domainObjectEndPoint2);
      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (RelationEndPointID.Create (_domainObject3.ID, typeof (Order), "Customer")))
          .Return (_domainObjectEndPoint3);

      _dataKeeper = new CollectionEndPointDataKeeper (_clientTransaction, _endPointID, null, _endPointProviderStub);

      _comparer123 = new DelegateBasedComparer<DomainObject> (Compare123);
    }

    [Test]
    public void Initialization ()
    {
      var dataKeeper = new CollectionEndPointDataKeeper (_clientTransaction, _endPointID, null, _endPointProviderStub);
      Assert.That (dataKeeper.CollectionData.ToArray(), Is.Empty);
      Assert.That (dataKeeper.OriginalOppositeEndPoints, Is.Empty);

      var endPointTracker = CollectionEndPointDataKeeperTestHelper.GetEndPointTracker (dataKeeper);
      Assert.That (endPointTracker.GetOppositeEndPoints (), Is.Empty);
      Assert.That (endPointTracker.EndPointProvider, Is.SameAs (_endPointProviderStub));
      Assert.That (
          endPointTracker.ObjectEndPointDefinition, 
          Is.SameAs (Configuration.ClassDefinitions[typeof (Order)].GetRelationEndPointDefinition (typeof (Order).FullName + ".Customer")));
    }

    [Test]
    public void CollectionData_RepresentsDataAndEndPoints ()
    {
      _dataKeeper.CollectionData.Insert (0, _domainObject1);

      Assert.That (_dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1 }));
      Assert.That (_dataKeeper.OppositeEndPoints, Is.EquivalentTo (new[] { _domainObjectEndPoint1 }));
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

      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();
      endPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject2);

      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Not.Contains (_domainObject2));

      _dataKeeper.RegisterOriginalOppositeEndPoint (endPointStub);

      Assert.That (_dataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.False);
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray(), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalOppositeEndPoints.ToArray(), Is.EqualTo (new[] { endPointStub }));
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
    public void UnregisterOriginalOppositeEndPoint ()
    {
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();
      endPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject2);
      endPointStub.Stub (stub => stub.ObjectID).Return (_domainObject2.ID);
      endPointStub.Stub (stub => stub.MarkSynchronized());

      _dataKeeper.RegisterOriginalOppositeEndPoint (endPointStub);

      Assert.That (_dataKeeper.OriginalOppositeEndPoints.Length, Is.EqualTo (1));
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Contains (_domainObject2));
      
      _dataKeeper.UnregisterOriginalOppositeEndPoint (endPointStub);

      Assert.That (_dataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.False);
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalOppositeEndPoints.ToArray(), Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has not been registered.")]
    public void UnregisterOriginalOppositeEndPoint_NotRegistered ()
    {
      var oppositeEndPoint = CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1);
      _dataKeeper.UnregisterOriginalOppositeEndPoint (oppositeEndPoint);
    }

    [Test]
    public void HasDataChanged ()
    {
      var changeCachingCollectionData =
          DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<ChangeCachingCollectionDataDecorator> (
            (EndPointTrackingCollectionDataDecorator) _dataKeeper.CollectionData);
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (
              Arg.Is (changeCachingCollectionData), 
              Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
          .Return (true);
      _changeDetectionStrategyMock.Replay ();

      _dataKeeper.CollectionData.Add (_domainObject2); // require use of strategy

      var result = _dataKeeper.HasDataChanged (_changeDetectionStrategyMock);

      _changeDetectionStrategyMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (true));
    }

    [Test]
    public void HasDataChanged_Cached ()
    {
      var changeCachingCollectionData =
          DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<ChangeCachingCollectionDataDecorator> (
            (EndPointTrackingCollectionDataDecorator) _dataKeeper.CollectionData);
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (
              Arg.Is (changeCachingCollectionData), 
              Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
          .Return (true)
          .Repeat.Once();
      _changeDetectionStrategyMock.Replay ();

      _dataKeeper.CollectionData.Add (_domainObject2); // require use of strategy
 
      var result1 = _dataKeeper.HasDataChanged (_changeDetectionStrategyMock);
      var result2 = _dataKeeper.HasDataChanged (_changeDetectionStrategyMock);

      _changeDetectionStrategyMock.VerifyAllExpectations ();

      Assert.That (result1, Is.EqualTo (true));
      Assert.That (result2, Is.EqualTo (true));
    }

    [Test]
    public void HasDataChanged_Cache_InvalidatedOnModifyingOperations ()
    {
      var changeCachingCollectionData =
          DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<ChangeCachingCollectionDataDecorator> (
            (EndPointTrackingCollectionDataDecorator) _dataKeeper.CollectionData);

      using (_changeDetectionStrategyMock.GetMockRepository ().Ordered ())
      {
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (
                Arg.Is (changeCachingCollectionData), 
                Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
            .Return (true);
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (
                Arg.Is (changeCachingCollectionData), 
                Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
            .Return (false);
      }
      _changeDetectionStrategyMock.Replay ();

      _dataKeeper.CollectionData.Add (_domainObject2); // require use of strategy

      var result1 = _dataKeeper.HasDataChanged (_changeDetectionStrategyMock);

      _dataKeeper.CollectionData.Clear ();

      var result2 = _dataKeeper.HasDataChanged (_changeDetectionStrategyMock);

      _changeDetectionStrategyMock.VerifyAllExpectations ();
      Assert.That (result1, Is.EqualTo (true));
      Assert.That (result2, Is.EqualTo (false));
    }

    [Test]
    public void SortCurrentAndOriginalData_WithoutComparer ()
    {
      var dataKeeper = new CollectionEndPointDataKeeper (_clientTransaction, _endPointID, null, _endPointProviderStub);

      dataKeeper.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject3));
      dataKeeper.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1));
      dataKeeper.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject2));

      dataKeeper.SortCurrentAndOriginalData ();

      Assert.That (dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That (dataKeeper.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject3, _domainObject1, _domainObject2 }));
    }

    [Test]
    public void SortCurrentAndOriginalData_WithComparer ()
    {
      var dataKeeper = new CollectionEndPointDataKeeper (_clientTransaction, _endPointID, _comparer123, _endPointProviderStub);

      dataKeeper.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject3));
      dataKeeper.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1));
      dataKeeper.RegisterOriginalOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject2));
      
      dataKeeper.SortCurrentAndOriginalData();

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
      _dataKeeper.CollectionData.Insert (0, _domainObject1);
      Assert.That (_dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1 }));
      Assert.That (_dataKeeper.OppositeEndPoints, Is.EquivalentTo (new[] { _domainObjectEndPoint1 }));
      
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), Is.Empty);
      Assert.That (_dataKeeper.OriginalOppositeEndPoints, Is.Empty);

      _dataKeeper.Commit ();

      Assert.That (_dataKeeper.OriginalCollectionData.ToArray(), Is.EqualTo (new[] { _domainObject1 }));
      Assert.That (_dataKeeper.OriginalOppositeEndPoints, Is.EquivalentTo (new[] { _domainObjectEndPoint1 }));
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

      Assert.That (_dataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.True);

      _dataKeeper.Commit ();

      Assert.That (_dataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.False);
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
      var comparer = Comparer<DomainObject>.Default;
      var endPointProvider = new SerializableEndPointProvider();
      var data = new CollectionEndPointDataKeeper (ClientTransaction.CreateRootTransaction (), _endPointID, comparer, endPointProvider);
      var endPointFake = new SerializableObjectEndPointFake (null, _domainObject1);
      data.RegisterOriginalOppositeEndPoint (endPointFake);

      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (data);

      Assert.That (deserializedInstance.SortExpressionBasedComparer, Is.Not.Null);
      Assert.That (deserializedInstance.CollectionData.Count, Is.EqualTo (1));
      Assert.That (deserializedInstance.OriginalCollectionData.Count, Is.EqualTo (1));
      Assert.That (deserializedInstance.OriginalOppositeEndPoints.Length, Is.EqualTo (1));
      Assert.That (CollectionEndPointDataKeeperTestHelper.GetEndPointTracker (deserializedInstance), Is.Not.Null);
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