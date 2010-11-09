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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement
{
  [TestFixture]
  public class LazyLoadingCollectionEndPointDataKeeperTest : StandardMappingTest
  {
    private ClientTransaction _clientTransactionMock;
    private RelationEndPointID _endPointID;
    private ICollectionEndPointChangeDetectionStrategy _changeDetectionStrategyMock;
    
    private DomainObject _domainObject1;
    private DomainObject _domainObject2;
    private DomainObject _domainObject3;
    
    private LazyLoadingCollectionEndPointDataKeeper _loadedDataKeeper;
    private LazyLoadingCollectionEndPointDataKeeper _unloadedDataKeeper;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientTransactionMock = ClientTransactionObjectMother.CreateStrictMock ();
      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _changeDetectionStrategyMock = MockRepository.GenerateStrictMock<ICollectionEndPointChangeDetectionStrategy> ();

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject3 = DomainObjectMother.CreateFakeObject<Order> ();

      _loadedDataKeeper = new LazyLoadingCollectionEndPointDataKeeper (_clientTransactionMock, _endPointID, new[] { _domainObject1 });
      _unloadedDataKeeper = new LazyLoadingCollectionEndPointDataKeeper (_clientTransactionMock, _endPointID, null);
    }
    
    [Test]
    public void Initialization_Null ()
    {
      var data = new LazyLoadingCollectionEndPointDataKeeper (_clientTransactionMock, _endPointID, null);
      Assert.That (data.IsDataAvailable, Is.False);
    }

    [Test]
    public void Initialization_NotNull ()
    {
      var data = new LazyLoadingCollectionEndPointDataKeeper (_clientTransactionMock, _endPointID, new[] { _domainObject1, _domainObject2 });
      Assert.That (data.IsDataAvailable, Is.True);
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
      _clientTransactionMock.Replay ();
      Assert.That (_loadedDataKeeper.IsDataAvailable, Is.True);

      _loadedDataKeeper.EnsureDataAvailable ();

      Assert.That (_loadedDataKeeper.IsDataAvailable, Is.True);
      _clientTransactionMock.AssertWasNotCalled (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID));
    }

    [Test]
    public void EnsureDataAvailable_Unloaded ()
    {
      _clientTransactionMock
          .Expect (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID))
          .Return (new[] { _domainObject2, _domainObject3 });
      _clientTransactionMock.Replay ();
      
      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.False);

      _unloadedDataKeeper.EnsureDataAvailable ();

      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.True);
      _clientTransactionMock.VerifyAllExpectations ();
    }

    [Test]
    public void EnsureDataAvailable_Unloaded_CausesStateUpdate ()
    {
      _clientTransactionMock
          .Expect (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID))
          .Return (new[] { _domainObject2, _domainObject3 });
      _clientTransactionMock.Replay ();

      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.False);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_clientTransactionMock);
      _unloadedDataKeeper.EnsureDataAvailable ();

      listenerMock.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_clientTransactionMock, _endPointID, false));
      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.True);
    }

    [Test]
    public void DataStore_Loaded ()
    {
      _clientTransactionMock.Replay ();

      Assert.That (_loadedDataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1 }));
      _clientTransactionMock.AssertWasNotCalled (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID));
    }

    [Test]
    public void DataStore_Unloaded_LoadsData ()
    {
      _clientTransactionMock
          .Expect (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID))
          .Return (new[] { _domainObject2, _domainObject3 });
      _clientTransactionMock.Replay ();

      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.False);
      Assert.That (_unloadedDataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject2, _domainObject3 }));

      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.True);
      _clientTransactionMock.VerifyAllExpectations();
    }

    [Test]
    public void OriginalData_Loaded ()
    {
      _clientTransactionMock.Replay ();

      var originalData = _loadedDataKeeper.OriginalCollectionData;
      _clientTransactionMock.AssertWasNotCalled (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID));

      Assert.That (originalData.ToArray(), Is.EqualTo (new[] { _domainObject1 }));
      Assert.That (originalData.IsReadOnly, Is.True);
    }

    [Test]
    public void OriginalData_Unloaded_LoadsData ()
    {
      _clientTransactionMock
          .Expect (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID))
          .Return (new[] { _domainObject2, _domainObject3 });
      _clientTransactionMock.Replay ();

      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.False);
      var originalData = _unloadedDataKeeper.OriginalCollectionData;
      Assert.That (_unloadedDataKeeper.IsDataAvailable, Is.True);
      _clientTransactionMock.VerifyAllExpectations ();

      Assert.That (originalData.ToArray(), Is.EqualTo (new[] { _domainObject2, _domainObject3 }));
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
    public void Unload_CausesDataStoreToBeReloaded ()
    {
      Assert.That (_loadedDataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1 }));

      _loadedDataKeeper.Unload ();

      StubLoadRelatedObjects (_domainObject2);
      Assert.That (_loadedDataKeeper.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject2 }));
    }

    [Test]
    public void Unload_CausesOriginalObjectsToBeReloaded ()
    {
      Assert.That (_loadedDataKeeper.OriginalCollectionData.ToArray(), Is.EqualTo (new[] { _domainObject1 }));

      _loadedDataKeeper.Unload ();

      StubLoadRelatedObjects (_domainObject2);
      Assert.That (_loadedDataKeeper.OriginalCollectionData.ToArray(), Is.EqualTo (new[] { _domainObject2 }));
      Assert.That (_loadedDataKeeper.OriginalCollectionData.IsReadOnly, Is.True);
    }

    [Test]
    public void Serializable_Loaded ()
    {
      var data = new LazyLoadingCollectionEndPointDataKeeper (ClientTransaction.CreateRootTransaction (), _endPointID, new[] { _domainObject1 });
      
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
      var data = new LazyLoadingCollectionEndPointDataKeeper (ClientTransaction.CreateRootTransaction (), _endPointID, null);

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
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_clientTransactionMock);
      _loadedDataKeeper.CollectionData.Clear();

      listenerMock.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_clientTransactionMock, _endPointID, null));
    }

    private void StubLoadRelatedObjects (params DomainObject[] relatedObjects)
    {
      _clientTransactionMock
          .Stub (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID))
          .Return (relatedObjects);

      _clientTransactionMock.Replay ();
    }
  }
}