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
    
    private DomainObject _domainObject1;
    private DomainObject _domainObject2;
    private DomainObject _domainObject3;

    private ClientTransaction _clientTransaction;
    
    private CollectionEndPointDataKeeper _dataKeeper;

    private DelegateBasedComparer<DomainObject> _comparer123;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _changeDetectionStrategyMock = MockRepository.GenerateStrictMock<ICollectionEndPointChangeDetectionStrategy> ();

      _clientTransaction = ClientTransaction.CreateRootTransaction();

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject3 = DomainObjectMother.CreateFakeObject<Order> ();

      _dataKeeper = new CollectionEndPointDataKeeper (_clientTransaction, _endPointID, null);

      _comparer123 = new DelegateBasedComparer<DomainObject> (Compare123);
    }

    public void Initialization ()
    {
      var data = new CollectionEndPointDataKeeper (_clientTransaction, _endPointID, null);
      Assert.That (data.CollectionData, Is.Empty);
    }

    [Test]
    public void ContainsOppositeEndPoint ()
    {
      var oppositeEndPoint = CollectionEndPointTestHelper.GetFakeOppositeEndPoint(_domainObject1);

      Assert.That (_dataKeeper.ContainsOppositeEndPoint (oppositeEndPoint), Is.False);

      _dataKeeper.RegisterOppositeEndPoint (oppositeEndPoint);

      Assert.That (_dataKeeper.ContainsOppositeEndPoint (oppositeEndPoint), Is.True);
    }

    [Test]
    public void RegisterOppositeEndPoint ()
    {
      Assert.That (_dataKeeper.OppositeEndPoints.ToArray(), Is.Empty);

      var endPointMock = MockRepository.GenerateStrictMock<IObjectEndPoint> ();
      endPointMock.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject2);
      endPointMock.Expect (mock => mock.MarkSynchronized ());
      endPointMock.Replay ();

      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Not.Contains (_domainObject2));

      _dataKeeper.RegisterOppositeEndPoint (endPointMock);

      endPointMock.VerifyAllExpectations();
      Assert.That (_dataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.False);
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray(), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OppositeEndPoints.ToArray(), Is.EqualTo (new[] { endPointMock }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has already been registered.")]
    [Ignore ("TODO 3771")]
    public void RegisterOppositeEndPoint_AlreadyRegistered ()
    {
      var oppositeEndPoint = CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1);
      _dataKeeper.RegisterOppositeEndPoint (oppositeEndPoint);
      _dataKeeper.RegisterOppositeEndPoint (oppositeEndPoint);
    }

    [Test]
    public void UnregisterOppositeEndPoint ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IObjectEndPoint> ();
      endPointMock.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject2);
      endPointMock.Stub (stub => stub.ObjectID).Return (_domainObject2.ID);
      endPointMock.Stub (stub => stub.MarkSynchronized());
      endPointMock.Expect (mock => mock.MarkUnsynchronized ());
      endPointMock.Replay ();

      _dataKeeper.RegisterOppositeEndPoint (endPointMock);

      Assert.That (_dataKeeper.OppositeEndPoints.Length, Is.EqualTo (1));
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Contains (_domainObject2));
      
      _dataKeeper.UnregisterOppositeEndPoint (endPointMock);

      endPointMock.VerifyAllExpectations();
      Assert.That (_dataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.False);
      Assert.That (_dataKeeper.CollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_dataKeeper.OppositeEndPoints.ToArray(), Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has not been registered.")]
    [Ignore ("TODO 3771")]
    public void UnregisterOppositeEndPoint_NotRegistered ()
    {
      var oppositeEndPoint = CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1);
      _dataKeeper.UnregisterOppositeEndPoint (oppositeEndPoint);
    }

    [Test]
    public void HasDataChanged ()
    {
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (Arg.Is (_dataKeeper.CollectionData), Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
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
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (Arg.Is (_dataKeeper.CollectionData), Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
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
      using (_changeDetectionStrategyMock.GetMockRepository ().Ordered ())
      {
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (Arg.Is (_dataKeeper.CollectionData), Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
            .Return (true);
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (Arg.Is (_dataKeeper.CollectionData), Arg<IDomainObjectCollectionData>.List.Equal (_dataKeeper.OriginalCollectionData)))
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
      var dataKeeper = new CollectionEndPointDataKeeper (
          _clientTransaction,
          _endPointID,
          null);

      dataKeeper.RegisterOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject3));
      dataKeeper.RegisterOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1));
      dataKeeper.RegisterOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject2));

      dataKeeper.SortCurrentAndOriginalData ();

      Assert.That (dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject3, _domainObject1, _domainObject2 }));
      Assert.That (dataKeeper.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject3, _domainObject1, _domainObject2 }));
    }

    [Test]
    public void SortCurrentAndOriginalData_WithComparer ()
    {
      var dataKeeper = new CollectionEndPointDataKeeper (
          _clientTransaction,
          _endPointID,
          _comparer123);

      dataKeeper.RegisterOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject3));
      dataKeeper.RegisterOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1));
      dataKeeper.RegisterOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject2));
      
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
    public void FlattenedSerializable ()
    {
      var comparer = Comparer<DomainObject>.Default;
      var data = new CollectionEndPointDataKeeper (ClientTransaction.CreateRootTransaction (), _endPointID, comparer);
      var endPointFake = new SerializableObjectEndPointFake (_domainObject1);
      data.RegisterOppositeEndPoint (endPointFake);
      
      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (data);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EndPointID, Is.EqualTo (_endPointID));
      Assert.That (deserializedInstance.SortExpressionBasedComparer, Is.Not.Null);

      Assert.That (deserializedInstance.CollectionData.Count, Is.EqualTo (1));
      Assert.That (deserializedInstance.OriginalCollectionData.Count, Is.EqualTo (1));
      Assert.That (deserializedInstance.OppositeEndPoints.Length, Is.EqualTo (1));
    }

    [Test]
    public void CommitOriginalContents_UpdatesOriginalContents ()
    {
      _dataKeeper.CollectionData.Insert (0, _domainObject1);
      Assert.That (_dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1 }));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray (), Is.Empty);

      _dataKeeper.CommitOriginalContents ();

      Assert.That (_dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1 }));
      Assert.That (_dataKeeper.OriginalCollectionData.ToArray(), Is.EqualTo (new[] { _domainObject1 }));
    }

    [Test]
    public void CommitOriginalContents_InvalidatesHasChangedCache ()
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

      _dataKeeper.CommitOriginalContents ();

      Assert.That (_dataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.False);
    }

    [Test]
    public void StateUpdates_RoutedToTransactionEventSink ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_clientTransaction);
      _dataKeeper.RegisterOppositeEndPoint (CollectionEndPointTestHelper.GetFakeOppositeEndPoint (_domainObject1));

      _dataKeeper.CollectionData.Clear();

      listenerMock.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_clientTransaction, _endPointID, null));
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