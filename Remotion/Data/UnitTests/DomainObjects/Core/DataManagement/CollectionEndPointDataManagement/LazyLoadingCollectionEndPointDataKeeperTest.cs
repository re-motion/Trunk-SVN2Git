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
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement
{
  [TestFixture]
  public class LazyLoadingCollectionEndPointDataKeeperTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private ICollectionEndPointChangeDetectionStrategy _changeDetectionStrategyMock;
    
    private DomainObject _domainObject1;
    private DomainObject _domainObject2;
    private DomainObject _domainObject3;

    private IObjectLoader _objectLoaderMock;
    private ClientTransaction _clientTransactionWithObjectLoaderMock;
    
    private LazyLoadingCollectionEndPointDataKeeper _completeDataKeeper;
    private LazyLoadingCollectionEndPointDataKeeper _incompleteDataKeeper;

    private DelegateBasedComparer<DomainObject> _comparer123;

    public override void SetUp ()
    {
      base.SetUp ();

      _objectLoaderMock = MockRepository.GenerateStrictMock<IObjectLoader> ();
      _clientTransactionWithObjectLoaderMock = ClientTransactionObjectMother.CreateTransactionWithObjectLoader<ClientTransaction> ((tx, cs, es) => _objectLoaderMock);
      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _changeDetectionStrategyMock = MockRepository.GenerateStrictMock<ICollectionEndPointChangeDetectionStrategy> ();

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject3 = DomainObjectMother.CreateFakeObject<Order> ();

      _completeDataKeeper = new LazyLoadingCollectionEndPointDataKeeper (_clientTransactionWithObjectLoaderMock, _endPointID, null, new[] { _domainObject1 });
      _incompleteDataKeeper = new LazyLoadingCollectionEndPointDataKeeper (_clientTransactionWithObjectLoaderMock, _endPointID, null, null);

      _comparer123 = new DelegateBasedComparer<DomainObject> (Compare123);
    }

    [Test]
    public void Initialization_NullContents ()
    {
      var data = new LazyLoadingCollectionEndPointDataKeeper (_clientTransactionWithObjectLoaderMock, _endPointID, null, null);
      Assert.That (data.IsDataComplete, Is.False);
    }

    [Test]
    public void Initialization_NonNullContents ()
    {
      var data = new LazyLoadingCollectionEndPointDataKeeper (_clientTransactionWithObjectLoaderMock, _endPointID, null, new[] { _domainObject1, _domainObject2 });
      Assert.That (data.IsDataComplete, Is.True);
    }
    
    [Test]
    public void RegisterOriginalObject ()
    {
      Assert.That (_completeDataKeeper.CollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_completeDataKeeper.OriginalCollectionData.ToArray (), List.Not.Contains (_domainObject2));

      _completeDataKeeper.RegisterOriginalObject (_domainObject2);

      Assert.That (_completeDataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.False);
      Assert.That (_completeDataKeeper.CollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_completeDataKeeper.OriginalCollectionData.ToArray(), List.Contains (_domainObject2));
    }

    [Test]
    public void UnregisterOriginalObject ()
    {
      Assert.That (_completeDataKeeper.CollectionData.ToArray (), List.Contains (_domainObject1));
      Assert.That (_completeDataKeeper.OriginalCollectionData.ToArray (), List.Contains (_domainObject1));
      
      _completeDataKeeper.UnregisterOriginalObject (_domainObject1.ID);

      Assert.That (_completeDataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.False);
      Assert.That (_completeDataKeeper.CollectionData.ToArray (), List.Not.Contains (_domainObject1));
      Assert.That (_completeDataKeeper.OriginalCollectionData.ToArray (), List.Not.Contains (_domainObject1));
    }

    [Test]
    public void HasDataChanged ()
    {
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (Arg.Is (_completeDataKeeper.CollectionData), Arg<IDomainObjectCollectionData>.List.Equal (_completeDataKeeper.OriginalCollectionData)))
          .Return (true);
      _changeDetectionStrategyMock.Replay ();

      _completeDataKeeper.CollectionData.Add (_domainObject2); // require use of strategy

      var result = _completeDataKeeper.HasDataChanged (_changeDetectionStrategyMock);

      _changeDetectionStrategyMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (true));
    }

    [Test]
    public void HasDataChanged_Cached ()
    {
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (Arg.Is (_completeDataKeeper.CollectionData), Arg<IDomainObjectCollectionData>.List.Equal (_completeDataKeeper.OriginalCollectionData)))
          .Return (true)
          .Repeat.Once();
      _changeDetectionStrategyMock.Replay ();

      _completeDataKeeper.CollectionData.Add (_domainObject2); // require use of strategy
 
      var result1 = _completeDataKeeper.HasDataChanged (_changeDetectionStrategyMock);
      var result2 = _completeDataKeeper.HasDataChanged (_changeDetectionStrategyMock);

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
            .Expect (mock => mock.HasDataChanged (Arg.Is (_completeDataKeeper.CollectionData), Arg<IDomainObjectCollectionData>.List.Equal (_completeDataKeeper.OriginalCollectionData)))
            .Return (true);
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (Arg.Is (_completeDataKeeper.CollectionData), Arg<IDomainObjectCollectionData>.List.Equal (_completeDataKeeper.OriginalCollectionData)))
            .Return (false);
      }
      _changeDetectionStrategyMock.Replay ();

      _completeDataKeeper.CollectionData.Add (_domainObject2); // require use of strategy

      var result1 = _completeDataKeeper.HasDataChanged (_changeDetectionStrategyMock);

      _completeDataKeeper.CollectionData.Clear ();

      var result2 = _completeDataKeeper.HasDataChanged (_changeDetectionStrategyMock);

      _changeDetectionStrategyMock.VerifyAllExpectations ();
      Assert.That (result1, Is.EqualTo (true));
      Assert.That (result2, Is.EqualTo (false));
    }

    [Test]
    public void HasDataChanged_Incomplete_DoesNotLoadData ()
    {
      _incompleteDataKeeper.HasDataChanged (_changeDetectionStrategyMock);

      Assert.That (_incompleteDataKeeper.IsDataComplete, Is.False);
    }

    [Test]
    public void MarkDataComplete_Loaded ()
    {
      _objectLoaderMock.Replay ();
      Assert.That (_completeDataKeeper.IsDataComplete, Is.True);

      _completeDataKeeper.MarkDataComplete ();

      Assert.That (_completeDataKeeper.IsDataComplete, Is.True);
      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));
    }

    [Test]
    public void MarkDataComplete_Incomplete ()
    {
      _objectLoaderMock.Replay ();

      Assert.That (_incompleteDataKeeper.IsDataComplete, Is.False);

      _incompleteDataKeeper.MarkDataComplete ();

      Assert.That (_incompleteDataKeeper.IsDataComplete, Is.True);
      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));
    }

    [Test]
    public void MarkDataComplete_Incomplete_WithSortComparer ()
    {
      _objectLoaderMock.Replay ();

      var dataKeeper = new LazyLoadingCollectionEndPointDataKeeper (
          _clientTransactionWithObjectLoaderMock, _endPointID, _comparer123, new[] { _domainObject3, _domainObject1, _domainObject2 });
      dataKeeper.MarkDataIncomplete();

      Assert.That (dataKeeper.IsDataComplete, Is.False);

      dataKeeper.MarkDataComplete ();

      Assert.That (dataKeeper.IsDataComplete, Is.True);
      Assert.That (dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
      Assert.That (dataKeeper.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));

      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));
    }

    [Test]
    public void DataStore_Loaded ()
    {
      _objectLoaderMock.Replay ();

      Assert.That (_completeDataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1 }));

      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));
    }

    [Test]
    public void DataStore_Incomplete_DoesNotLoadData ()
    {
      _objectLoaderMock.Replay ();

      Assert.That (_incompleteDataKeeper.IsDataComplete, Is.False);
      
      Assert.That (_incompleteDataKeeper.CollectionData.ToArray (), Is.Empty);

      Assert.That (_incompleteDataKeeper.IsDataComplete, Is.False);
      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));
    }

    [Test]
    public void OriginalData_Loaded ()
    {
      _objectLoaderMock.Replay ();

      var originalData = _completeDataKeeper.OriginalCollectionData;

      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));

      Assert.That (originalData.ToArray(), Is.EqualTo (new[] { _domainObject1 }));
      Assert.That (originalData.IsReadOnly, Is.True);
    }

    [Test]
    public void OriginalData_Incomplete_LoadsData ()
    {
      _objectLoaderMock.Replay ();
      Assert.That (_incompleteDataKeeper.IsDataComplete, Is.False);
      
      var originalData = _incompleteDataKeeper.OriginalCollectionData;

      Assert.That (_incompleteDataKeeper.IsDataComplete, Is.False);
      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));

      Assert.That (originalData.ToArray(), Is.Empty);
      Assert.That (originalData.IsReadOnly, Is.True);
    }
    
    [Test]
    public void MarkDataIncomplete ()
    {
      Assert.That (_completeDataKeeper.IsDataComplete, Is.True);

      _completeDataKeeper.MarkDataIncomplete ();

      Assert.That (_completeDataKeeper.IsDataComplete, Is.False);
    }

    [Test]
    public void Serializable_Complete ()
    {
      var data = new LazyLoadingCollectionEndPointDataKeeper (ClientTransaction.CreateRootTransaction (), _endPointID, null, new[] { _domainObject1 });
      
      var deserializedInstance = Serializer.SerializeAndDeserialize (data);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EndPointID, Is.EqualTo (_endPointID));

      Assert.That (deserializedInstance.IsDataComplete, Is.True);
      Assert.That (deserializedInstance.CollectionData.Count, Is.EqualTo (1));
      Assert.That (deserializedInstance.OriginalCollectionData.Count, Is.EqualTo (1));
    }

    [Test]
    public void Serializable_Incomplete ()
    {
      var data = new LazyLoadingCollectionEndPointDataKeeper (ClientTransaction.CreateRootTransaction (), _endPointID, null, null);

      var deserializedInstance = Serializer.SerializeAndDeserialize (data);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EndPointID, Is.EqualTo (_endPointID));

      Assert.That (deserializedInstance.IsDataComplete, Is.False);
    }

    [Test]
    public void CommitOriginalContents_Complete_UpdatesOriginalContents ()
    {
      _completeDataKeeper.CollectionData.Insert (1, _domainObject2);
      Assert.That (_completeDataKeeper.CollectionData.ToArray(), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_completeDataKeeper.OriginalCollectionData.ToArray(), Is.EqualTo (new[] { _domainObject1 }));

      _completeDataKeeper.CommitOriginalContents ();

      Assert.That (_completeDataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_completeDataKeeper.OriginalCollectionData.ToArray(), Is.EqualTo (new[] { _domainObject1 , _domainObject2}));
    }

    [Test]
    public void CommitOriginalContents_Complete_InvalidatesHasChangedCache ()
    {
      using (_changeDetectionStrategyMock.GetMockRepository ().Ordered ())
      {
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (
                Arg<IDomainObjectCollectionData>.List.Equal (_completeDataKeeper.CollectionData),
                Arg<IDomainObjectCollectionData>.List.Equal (_completeDataKeeper.OriginalCollectionData)))
            .Return (true);
      }
      _changeDetectionStrategyMock.Replay ();

      _completeDataKeeper.CollectionData.Add (_domainObject2); // require use of strategy

      Assert.That (_completeDataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.True);

      _completeDataKeeper.CommitOriginalContents ();

      Assert.That (_completeDataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.False);
    }

    [Test]
    public void CommitOriginalContents_Incomplete_DoesNotLoadData ()
    {
      _incompleteDataKeeper.CommitOriginalContents ();

      Assert.That (_incompleteDataKeeper.IsDataComplete, Is.False);
    }

    [Test]
    public void StateUpdates_RoutedToTransactionEventSink ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_clientTransactionWithObjectLoaderMock);
      _completeDataKeeper.CollectionData.Clear();

      listenerMock.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_clientTransactionWithObjectLoaderMock, _endPointID, null));
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