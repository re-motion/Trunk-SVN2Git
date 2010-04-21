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
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class LazyLoadableCollectionEndPointDataTest : StandardMappingTest
  {
    private ClientTransaction _clientTransactionMock;
    private RelationEndPointID _endPointID;
    private ICollectionEndPointChangeDetectionStrategy _changeDetectionStrategyMock;
    
    private DomainObject _domainObject1;
    private DomainObject _domainObject2;
    private DomainObject _domainObject3;
    
    private LazyLoadableCollectionEndPointData _loadedData;
    private LazyLoadableCollectionEndPointData _unloadedData;
    
    public override void SetUp ()
    {
      base.SetUp ();

      _clientTransactionMock = ClientTransactionObjectMother.CreateStrictMock ();
      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _changeDetectionStrategyMock = new MockRepository().StrictMock<ICollectionEndPointChangeDetectionStrategy> ();

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      _domainObject3 = DomainObjectMother.CreateFakeObject<Order> ();

      _loadedData = new LazyLoadableCollectionEndPointData (_clientTransactionMock, _endPointID, new[] { _domainObject1 });
      _unloadedData = new LazyLoadableCollectionEndPointData (_clientTransactionMock, _endPointID, null);
    }
    
    [Test]
    public void Initialization_Null ()
    {
      var data = new LazyLoadableCollectionEndPointData (_clientTransactionMock, _endPointID, null);
      Assert.That (data.IsDataAvailable, Is.False);
    }

    [Test]
    public void Initialization_NotNull ()
    {
      var data = new LazyLoadableCollectionEndPointData (_clientTransactionMock, _endPointID, new[] { _domainObject1, _domainObject2 });
      Assert.That (data.IsDataAvailable, Is.True);
    }

    [Test]
    public void HasDataChanged_Loaded ()
    {
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (_loadedData.CollectionData, _loadedData.OriginalOppositeDomainObjectsContents))
          .Return (true);
      _changeDetectionStrategyMock.Replay ();

      var result = _loadedData.HasDataChanged (_changeDetectionStrategyMock);

      _changeDetectionStrategyMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (true));
    }

    [Test]
    public void HasDataChanged_Loaded_Cached ()
    {
      _changeDetectionStrategyMock
          .Expect (mock => mock.HasDataChanged (_loadedData.CollectionData, _loadedData.OriginalOppositeDomainObjectsContents))
          .Return (true)
          .Repeat.Once();
      _changeDetectionStrategyMock.Replay ();

      var result1 = _loadedData.HasDataChanged (_changeDetectionStrategyMock);
      var result2 = _loadedData.HasDataChanged (_changeDetectionStrategyMock);

      _changeDetectionStrategyMock.VerifyAllExpectations ();

      Assert.That (result1, Is.EqualTo (true));
      Assert.That (result2, Is.EqualTo (true));
    }

    [Test]
    public void HasDataChanged_Loaded_Cache_InvalidatedOnModifyingOperations ()
    {
      using (_changeDetectionStrategyMock.GetMockRepository ().Ordered ())
      {
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (_loadedData.CollectionData, _loadedData.OriginalOppositeDomainObjectsContents))
            .Return (true);
        _changeDetectionStrategyMock
            .Expect (mock => mock.HasDataChanged (_loadedData.CollectionData, _loadedData.OriginalOppositeDomainObjectsContents))
            .Return (false);
      }
      _changeDetectionStrategyMock.Replay ();

      var result1 = _loadedData.HasDataChanged (_changeDetectionStrategyMock);

      _loadedData.CollectionData.Clear ();

      var result2 = _loadedData.HasDataChanged (_changeDetectionStrategyMock);

      _changeDetectionStrategyMock.VerifyAllExpectations ();
      Assert.That (result1, Is.EqualTo (true));
      Assert.That (result2, Is.EqualTo (false));
    }

    [Test]
    public void HasDataChanged_Unloaded_AlwaysFalse ()
    {
      _changeDetectionStrategyMock.Replay ();

      var result = _unloadedData.HasDataChanged (_changeDetectionStrategyMock);

      _changeDetectionStrategyMock.AssertWasNotCalled (
          mock => mock.HasDataChanged (Arg<IDomainObjectCollectionData>.Is.Anything, Arg<DomainObjectCollection>.Is.Anything));
      Assert.That (result, Is.EqualTo (false));

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
    }

    [Test]
    public void EnsureDataAvailable_Loaded ()
    {
      _clientTransactionMock.Replay ();
      Assert.That (_loadedData.IsDataAvailable, Is.True);

      _loadedData.EnsureDataAvailable ();

      Assert.That (_loadedData.IsDataAvailable, Is.True);
      _clientTransactionMock.AssertWasNotCalled (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID));
    }

    [Test]
    public void EnsureDataAvailable_Unloaded ()
    {
      _clientTransactionMock
          .Expect (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID))
          .Return (new[] { _domainObject2, _domainObject3 });
      _clientTransactionMock.Replay ();
      
      Assert.That (_unloadedData.IsDataAvailable, Is.False);

      _unloadedData.EnsureDataAvailable ();

      Assert.That (_unloadedData.IsDataAvailable, Is.True);
      _clientTransactionMock.VerifyAllExpectations ();
    }

    [Test]
    public void DataStore_Loaded ()
    {
      _clientTransactionMock.Replay ();

      Assert.That (_loadedData.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1 }));
      _clientTransactionMock.AssertWasNotCalled (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID));
    }

    [Test]
    public void DataStore_Unloaded_LoadsData ()
    {
      _clientTransactionMock
          .Expect (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID))
          .Return (new[] { _domainObject2, _domainObject3 });
      _clientTransactionMock.Replay ();

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      Assert.That (_unloadedData.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject2, _domainObject3 }));

      Assert.That (_unloadedData.IsDataAvailable, Is.True);
      _clientTransactionMock.VerifyAllExpectations();
    }

    [Test]
    public void OriginalOppositeDomainObjectsContents_Loaded ()
    {
      _clientTransactionMock.Replay ();

      var domainObjectCollection = _loadedData.OriginalOppositeDomainObjectsContents;
      _clientTransactionMock.AssertWasNotCalled (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID));

      Assert.That (domainObjectCollection, Is.InstanceOfType (typeof (OrderCollection)));
      Assert.That (domainObjectCollection, Is.EqualTo (new[] { _domainObject1 }));
      Assert.That (domainObjectCollection.IsReadOnly, Is.True);
    }

    [Test]
    public void OriginalOppositeDomainObjectsContents_Unloaded_LoadsData ()
    {
      _clientTransactionMock
          .Expect (mock => ClientTransactionTestHelper.CallLoadRelatedObjects (mock, _endPointID))
          .Return (new[] { _domainObject2, _domainObject3 });
      _clientTransactionMock.Replay ();

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
      var domainObjectCollection = _unloadedData.OriginalOppositeDomainObjectsContents;
      Assert.That (_unloadedData.IsDataAvailable, Is.True);
      _clientTransactionMock.VerifyAllExpectations ();

      Assert.That (domainObjectCollection, Is.InstanceOfType (typeof (OrderCollection)));
      Assert.That (domainObjectCollection, Is.EqualTo (new[] { _domainObject2, _domainObject3 }));
      Assert.That (domainObjectCollection.IsReadOnly, Is.True);
    }
    
    [Test]
    public void Unload ()
    {
      Assert.That (_loadedData.IsDataAvailable, Is.True);

      _loadedData.Unload ();

      Assert.That (_loadedData.IsDataAvailable, Is.False);
    }

    [Test]
    public void Unload_CausesDataStoreToBeReloaded ()
    {
      Assert.That (_loadedData.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1 }));

      _loadedData.Unload ();

      StubLoadRelatedObjects (_domainObject2);
      Assert.That (_loadedData.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject2 }));
    }

    [Test]
    public void Unload_CausesOriginalObjectsToBeReloaded ()
    {
      Assert.That (_loadedData.OriginalOppositeDomainObjectsContents, Is.EqualTo (new[] { _domainObject1 }));

      _loadedData.Unload ();

      StubLoadRelatedObjects (_domainObject2);
      Assert.That (_loadedData.OriginalOppositeDomainObjectsContents, Is.EqualTo (new[] { _domainObject2 }));

      Assert.That (_loadedData.OriginalOppositeDomainObjectsContents, Is.InstanceOfType (typeof (OrderCollection)));
      Assert.That (_loadedData.OriginalOppositeDomainObjectsContents.IsReadOnly, Is.True);
    }

    [Test]
    public void FlattenedSerializable_Loaded ()
    {
      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (_loadedData);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EndPointID, Is.EqualTo (_endPointID));

      Assert.That (deserializedInstance.IsDataAvailable, Is.True);
      Assert.That (deserializedInstance.CollectionData.Count, Is.EqualTo (1));
      Assert.That (deserializedInstance.OriginalOppositeDomainObjectsContents.Count, Is.EqualTo (1));
    }

    [Test]
    public void FlattenedSerializable_Unloaded ()
    {
      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (_unloadedData);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EndPointID, Is.EqualTo (_endPointID));

      Assert.That (deserializedInstance.IsDataAvailable, Is.False);
      StubLoadRelatedObjects (_domainObject2, _domainObject3);

      Assert.That (deserializedInstance.CollectionData.ToArray(), Is.EqualTo (new[] { _domainObject2, _domainObject3 }));
      Assert.That (deserializedInstance.OriginalOppositeDomainObjectsContents, Is.EqualTo (new[] { _domainObject2, _domainObject3 }));
    }

    [Test]
    public void CommitOriginalContents_Loaded_UpdatesOriginalContents ()
    {
      _loadedData.CollectionData.Insert (1, _domainObject2);
      Assert.That (_loadedData.CollectionData.ToArray(), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_loadedData.OriginalOppositeDomainObjectsContents, Is.EqualTo (new[] { _domainObject1 }));

      _loadedData.CommitOriginalContents ();

      Assert.That (_loadedData.CollectionData.ToArray (), Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
      Assert.That (_loadedData.OriginalOppositeDomainObjectsContents, Is.EqualTo (new[] { _domainObject1 , _domainObject2}));
    }

    [Test]
    public void CommitOriginalContents_Loaded_InvalidatesHasChangedCache ()
    {
      using (_changeDetectionStrategyMock.GetMockRepository ().Ordered ())
      {
        _changeDetectionStrategyMock.Expect (mock => mock.HasDataChanged (_loadedData.CollectionData, _loadedData.OriginalOppositeDomainObjectsContents)).
            Return (true);
        _changeDetectionStrategyMock.Expect (mock => mock.HasDataChanged (_loadedData.CollectionData, _loadedData.OriginalOppositeDomainObjectsContents)).
            Return (false);
      }
      _changeDetectionStrategyMock.Replay ();

      Assert.That (_loadedData.HasDataChanged (_changeDetectionStrategyMock), Is.True);

      _loadedData.CommitOriginalContents ();

      Assert.That (_loadedData.HasDataChanged (_changeDetectionStrategyMock), Is.False);
    }

    [Test]
    public void CommitOriginalContents_Unloaded_DoesNothing ()
    {
      _unloadedData.CommitOriginalContents ();

      Assert.That (_unloadedData.IsDataAvailable, Is.False);
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