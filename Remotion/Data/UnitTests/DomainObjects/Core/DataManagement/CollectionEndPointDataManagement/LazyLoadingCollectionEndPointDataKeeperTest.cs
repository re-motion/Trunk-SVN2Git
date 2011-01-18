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
    
    private LazyLoadingCollectionEndPointDataKeeper _loadedDataKeeper;
    private LazyLoadingCollectionEndPointDataKeeper _unloadedDataKeeper;

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

      _loadedDataKeeper = new LazyLoadingCollectionEndPointDataKeeper (_clientTransactionWithObjectLoaderMock, _endPointID, null, new[] { _domainObject1 });
      _unloadedDataKeeper = new LazyLoadingCollectionEndPointDataKeeper (_clientTransactionWithObjectLoaderMock, _endPointID, null, null);

      _comparer123 = new DelegateBasedComparer<DomainObject> (Compare123);
    }

    [Test]
    public void Initialization_NullContents ()
    {
      var data = new LazyLoadingCollectionEndPointDataKeeper (_clientTransactionWithObjectLoaderMock, _endPointID, null, null);
      Assert.That (data.IsDataAvailable, Is.False);
    }

    [Test]
    public void Initialization_NonNullContents ()
    {
      var data = new LazyLoadingCollectionEndPointDataKeeper (_clientTransactionWithObjectLoaderMock, _endPointID, null, new[] { _domainObject1, _domainObject2 });
      Assert.That (data.IsDataAvailable, Is.True);
    }
    
    [Test]
    public void RegisterOriginalObject ()
    {
      Assert.That (_loadedDataKeeper.CollectionData.ToArray (), List.Not.Contains (_domainObject2));
      Assert.That (_loadedDataKeeper.OriginalCollectionData.ToArray (), List.Not.Contains (_domainObject2));

      _loadedDataKeeper.RegisterOriginalObject (_domainObject2);

      Assert.That (_loadedDataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.False);
      Assert.That (_loadedDataKeeper.CollectionData.ToArray (), List.Contains (_domainObject2));
      Assert.That (_loadedDataKeeper.OriginalCollectionData.ToArray(), List.Contains (_domainObject2));
    }

    [Test]
    public void UnregisterOriginalObject ()
    {
      Assert.That (_loadedDataKeeper.CollectionData.ToArray (), List.Contains (_domainObject1));
      Assert.That (_loadedDataKeeper.OriginalCollectionData.ToArray (), List.Contains (_domainObject1));
      
      _loadedDataKeeper.UnregisterOriginalObject (_domainObject1.ID);

      Assert.That (_loadedDataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.False);
      Assert.That (_loadedDataKeeper.CollectionData.ToArray (), List.Not.Contains (_domainObject1));
      Assert.That (_loadedDataKeeper.OriginalCollectionData.ToArray (), List.Not.Contains (_domainObject1));
    }

    [Test]
    public void HasDataChanged ()
    {
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (Arg.Is (_loadedDataKeeper.CollectionData), Arg<IDomainObjectCollectionData>.List.Equal (_loadedDataKeeper.OriginalCollectionData)))
          .Return (true);
      _changeDetectionStrategyMock.Replay ();

      _loadedDataKeeper.CollectionData.Add (_domainObject2); // require use of strategy

      var result = _loadedDataKeeper.HasDataChanged (_changeDetectionStrategyMock);

      _changeDetectionStrategyMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (true));
    }

    [Test]
    public void HasDataChanged_Cached ()
    {
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (Arg.Is (_loadedDataKeeper.CollectionData), Arg<IDomainObjectCollectionData>.List.Equal (_loadedDataKeeper.OriginalCollectionData)))
          .Return (true)
          .Repeat.Once();
      _changeDetectionStrategyMock.Replay ();

      _loadedDataKeeper.CollectionData.Add (_domainObject2); // require use of strategy
 
      var result1 = _loadedDataKeeper.HasDataChanged (_changeDetectionStrategyMock);
      var result2 = _loadedDataKeeper.HasDataChanged (_changeDetectionStrategyMock);

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
            .Expect (mock => mock.HasDataChanged (Arg.Is (_loadedDataKeeper.CollectionData), Arg<IDomainObjectCollectionData>.List.Equal (_loadedDataKeeper.OriginalCollectionData)))
            .Return (true);
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (Arg.Is (_loadedDataKeeper.CollectionData), Arg<IDomainObjectCollectionData>.List.Equal (_loadedDataKeeper.OriginalCollectionData)))
            .Return (false);
      }
      _changeDetectionStrategyMock.Replay ();

      _loadedDataKeeper.CollectionData.Add (_domainObject2); // require use of strategy

      var result1 = _loadedDataKeeper.HasDataChanged (_changeDetectionStrategyMock);

      _loadedDataKeeper.CollectionData.Clear ();

      var result2 = _loadedDataKeeper.HasDataChanged (_changeDetectionStrategyMock);

      _changeDetectionStrategyMock.VerifyAllExpectations ();
      Assert.That (result1, Is.EqualTo (true));
      Assert.That (result2, Is.EqualTo (false));
    }

    [Test]
    public void HasDataChanged_Unloaded_DoesNotLoadData ()
    {
      _unloadedDataKeeper.HasDataChanged (_changeDetectionStrategyMock);

      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.False);
    }

    [Test]
    public void EnsureDataAvailable_Loaded ()
    {
      _objectLoaderMock.Replay ();
      Assert.That (_loadedDataKeeper.IsDataAvailable, Is.True);

      _loadedDataKeeper.EnsureDataAvailable ();

      Assert.That (_loadedDataKeeper.IsDataAvailable, Is.True);
      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));
    }

    [Test]
    public void EnsureDataAvailable_Unloaded ()
    {
      _objectLoaderMock
          .Expect (mock => mock.LoadRelatedObjects (_endPointID))
          .Return (new[] { _domainObject2, _domainObject3 })
          .WhenCalled (mi => 
          {
            // Simulate what's usually done by RelationEndPointMap when new related objects are registered
            _unloadedDataKeeper.RegisterOriginalObject (_domainObject2);
            _unloadedDataKeeper.RegisterOriginalObject (_domainObject3);
          });
      _objectLoaderMock.Replay ();
      
      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.False);

      _unloadedDataKeeper.EnsureDataAvailable ();

      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.True);
      Assert.That (_unloadedDataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject2, _domainObject3 }));
      Assert.That (_unloadedDataKeeper.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject2, _domainObject3 }));
      _objectLoaderMock.VerifyAllExpectations ();
    }

    [Test]
    public void EnsureDataAvailable_Unloaded_WithSortComparer ()
    {
      var dataKeeper = new LazyLoadingCollectionEndPointDataKeeper (_clientTransactionWithObjectLoaderMock, _endPointID, _comparer123, null);

      _objectLoaderMock
          .Expect (mock => mock.LoadRelatedObjects (_endPointID))
          .Return (new[] { _domainObject3, _domainObject1, _domainObject2 })
          .WhenCalled (mi =>
          {
            // Simulate what's usually done by RelationEndPointMap when new related objects are registered
            dataKeeper.RegisterOriginalObject (_domainObject3);
            dataKeeper.RegisterOriginalObject (_domainObject1);
            dataKeeper.RegisterOriginalObject (_domainObject2);
          });
      _objectLoaderMock.Replay ();

      Assert.That (dataKeeper.IsDataAvailable, Is.False);

      dataKeeper.EnsureDataAvailable ();

      Assert.That (dataKeeper.IsDataAvailable, Is.True);
      Assert.That (dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
      Assert.That (dataKeeper.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
      _objectLoaderMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataAvailable_Loaded ()
    {
      _objectLoaderMock.Replay ();
      Assert.That (_loadedDataKeeper.IsDataAvailable, Is.True);

      _loadedDataKeeper.MarkDataAvailable ();

      Assert.That (_loadedDataKeeper.IsDataAvailable, Is.True);
      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));
    }

    [Test]
    public void MarkDataAvailable_Unloaded ()
    {
      _objectLoaderMock.Replay ();

      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.False);

      _unloadedDataKeeper.MarkDataAvailable ();

      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.True);
      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));
    }

    [Test]
    public void MarkDataAvailable_Unloaded_WithSortComparer ()
    {
      _objectLoaderMock.Replay ();

      var dataKeeper = new LazyLoadingCollectionEndPointDataKeeper (
          _clientTransactionWithObjectLoaderMock, _endPointID, _comparer123, new[] { _domainObject3, _domainObject1, _domainObject2 });
      dataKeeper.Unload();

      Assert.That (dataKeeper.IsDataAvailable, Is.False);

      dataKeeper.MarkDataAvailable ();

      Assert.That (dataKeeper.IsDataAvailable, Is.True);
      Assert.That (dataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));
      Assert.That (dataKeeper.OriginalCollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2, _domainObject3 }));

      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));
    }

    [Test]
    public void DataStore_Loaded ()
    {
      _objectLoaderMock.Replay ();

      Assert.That (_loadedDataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1 }));

      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));
    }

    [Test]
    public void DataStore_Unloaded_DoesNotLoadData ()
    {
      _objectLoaderMock.Replay ();

      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.False);
      
      Assert.That (_unloadedDataKeeper.CollectionData.ToArray (), Is.Empty);

      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.False);
      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));
    }

    [Test]
    public void OriginalData_Loaded ()
    {
      _objectLoaderMock.Replay ();

      var originalData = _loadedDataKeeper.OriginalCollectionData;

      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));

      Assert.That (originalData.ToArray(), Is.EqualTo (new[] { _domainObject1 }));
      Assert.That (originalData.IsReadOnly, Is.True);
    }

    [Test]
    public void OriginalData_Unloaded_LoadsData ()
    {
      _objectLoaderMock.Replay ();
      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.False);
      
      var originalData = _unloadedDataKeeper.OriginalCollectionData;

      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.False);
      _objectLoaderMock.AssertWasNotCalled (mock => mock.LoadRelatedObjects (_endPointID));

      Assert.That (originalData.ToArray(), Is.Empty);
      Assert.That (originalData.IsReadOnly, Is.True);
    }
    
    [Test]
    public void Unload ()
    {
      Assert.That (_loadedDataKeeper.IsDataAvailable, Is.True);

      _loadedDataKeeper.Unload ();

      Assert.That (_loadedDataKeeper.IsDataAvailable, Is.False);
    }

    [Test]
    public void Serializable_Loaded ()
    {
      var data = new LazyLoadingCollectionEndPointDataKeeper (ClientTransaction.CreateRootTransaction (), _endPointID, null, new[] { _domainObject1 });
      
      var deserializedInstance = Serializer.SerializeAndDeserialize (data);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EndPointID, Is.EqualTo (_endPointID));

      Assert.That (deserializedInstance.IsDataAvailable, Is.True);
      Assert.That (deserializedInstance.CollectionData.Count, Is.EqualTo (1));
      Assert.That (deserializedInstance.OriginalCollectionData.Count, Is.EqualTo (1));
    }

    [Test]
    public void Serializable_Unloaded ()
    {
      var data = new LazyLoadingCollectionEndPointDataKeeper (ClientTransaction.CreateRootTransaction (), _endPointID, null, null);

      var deserializedInstance = Serializer.SerializeAndDeserialize (data);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EndPointID, Is.EqualTo (_endPointID));

      Assert.That (deserializedInstance.IsDataAvailable, Is.False);
    }

    [Test]
    public void CommitOriginalContents_Loaded_UpdatesOriginalContents ()
    {
      _loadedDataKeeper.CollectionData.Insert (1, _domainObject2);
      Assert.That (_loadedDataKeeper.CollectionData.ToArray(), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_loadedDataKeeper.OriginalCollectionData.ToArray(), Is.EqualTo (new[] { _domainObject1 }));

      _loadedDataKeeper.CommitOriginalContents ();

      Assert.That (_loadedDataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_loadedDataKeeper.OriginalCollectionData.ToArray(), Is.EqualTo (new[] { _domainObject1 , _domainObject2}));
    }

    [Test]
    public void CommitOriginalContents_Loaded_InvalidatesHasChangedCache ()
    {
      using (_changeDetectionStrategyMock.GetMockRepository ().Ordered ())
      {
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (
                Arg<IDomainObjectCollectionData>.List.Equal (_loadedDataKeeper.CollectionData),
                Arg<IDomainObjectCollectionData>.List.Equal (_loadedDataKeeper.OriginalCollectionData)))
            .Return (true);
      }
      _changeDetectionStrategyMock.Replay ();

      _loadedDataKeeper.CollectionData.Add (_domainObject2); // require use of strategy

      Assert.That (_loadedDataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.True);

      _loadedDataKeeper.CommitOriginalContents ();

      Assert.That (_loadedDataKeeper.HasDataChanged (_changeDetectionStrategyMock), Is.False);
    }

    [Test]
    public void CommitOriginalContents_Unloaded_DoesNotLoadData ()
    {
      _unloadedDataKeeper.CommitOriginalContents ();

      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.False);
    }

    [Test]
    public void StateUpdates_RoutedToTransactionEventSink ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_clientTransactionWithObjectLoaderMock);
      _loadedDataKeeper.CollectionData.Clear();

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