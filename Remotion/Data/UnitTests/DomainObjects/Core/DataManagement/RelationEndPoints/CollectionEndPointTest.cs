// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class CollectionEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _customerEndPointID;
    private Order _order1; // Customer1
    private Order _order2; // Customer3
    
    private ILazyLoader _lazyLoaderMock;
    private IRelationEndPointProvider _endPointProviderStub;

    private ICollectionEndPointLoadState _loadStateMock;
    private CollectionEndPoint _endPoint;
    private IRealObjectEndPoint _relatedEndPointStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _customerEndPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);

      _lazyLoaderMock = MockRepository.GenerateMock<ILazyLoader> ();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();
      
      _loadStateMock = MockRepository.GenerateStrictMock<ICollectionEndPointLoadState> ();
      var changeDetectionStrategy = MockRepository.GenerateStub<ICollectionEndPointChangeDetectionStrategy> ();
      _endPoint = new CollectionEndPoint (
          ClientTransactionMock,
          _customerEndPointID,
          MockRepository.GenerateStub<ILazyLoader> (),
          _endPointProviderStub,
          new CollectionEndPointDataKeeperFactory (ClientTransactionMock, changeDetectionStrategy));
      PrivateInvoke.SetNonPublicField (_endPoint, "_loadState", _loadStateMock);
      _endPointProviderStub.Stub (stub => stub.GetOrCreateVirtualEndPoint (_customerEndPointID)).Return (_endPoint);

      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
    }

    [Test]
    public void Initialize ()
    {
      var lazyLoaderStub = MockRepository.GenerateStub<ILazyLoader> ();
      var endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();

      var dataKeeperStub = MockRepository.GenerateStub<ICollectionEndPointDataKeeper>();
      dataKeeperStub.Stub (stub => stub.OriginalOppositeEndPoints).Return (new IRealObjectEndPoint[0]);

      var dataKeeperFactoryStub = MockRepository.GenerateStub<IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper>> ();

      var endPoint = new CollectionEndPoint (
          ClientTransactionMock, 
          _customerEndPointID, 
          lazyLoaderStub, 
          endPointProviderStub,
          dataKeeperFactoryStub);

      Assert.That (endPoint.ID, Is.EqualTo (_customerEndPointID));
      Assert.That (endPoint.LazyLoader, Is.SameAs (lazyLoaderStub));
      Assert.That (endPoint.EndPointProvider, Is.SameAs (endPointProviderStub));
      Assert.That (endPoint.DataKeeperFactory, Is.SameAs (dataKeeperFactoryStub));

      var loadState = CollectionEndPointTestHelper.GetLoadState (endPoint);
      Assert.That (loadState, Is.TypeOf (typeof (IncompleteCollectionEndPointLoadState)));
      Assert.That (((IncompleteCollectionEndPointLoadState) loadState).DataKeeperFactory, Is.SameAs (dataKeeperFactoryStub));
    }

    [Test]
    public void Initialize_UsesEndPointDelegatingData ()
    {
      var dataDecorator = DomainObjectCollectionDataTestHelper.GetDataStrategyAndCheckType<ModificationCheckingCollectionDataDecorator> (
          _endPoint.Collection);
      var wrappedData = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<EndPointDelegatingCollectionData> (dataDecorator);
      Assert.That (wrappedData.EndPointID, Is.EqualTo (_customerEndPointID));
      Assert.That (wrappedData.VirtualEndPointProvider, Is.SameAs (_endPointProviderStub));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Initialize_WithInvalidRelationEndPointID_Throws ()
    {
      RelationEndPointObjectMother.CreateCollectionEndPoint (null, new DomainObject[0]);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException))]
    public void Initialize_WithoutCollectionCtorTakingData_Throws ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (DomainObjectWithCollectionMissingCtor));

      var endPointID = RelationEndPointID.Create(new ObjectID (classDefinition, Guid.NewGuid ()), 
                              typeof (DomainObjectWithCollectionMissingCtor) + ".OppositeObjects");
      RelationEndPointObjectMother.CreateCollectionEndPoint (endPointID, new DomainObject[0]);
    }

    [Test]
    public void HasChanged_True_CollectionChanged ()
    {
      _loadStateMock.Replay();
      CollectionEndPointTestHelper.SetCollection (_endPoint, new DomainObjectCollection());

      var result = _endPoint.HasChanged;

      _loadStateMock.AssertWasNotCalled (mock => mock.HasChanged ());
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChanged_True_LoadState ()
    {
      _loadStateMock.Expect (mock => mock.HasChanged ()).Return (true);
      _loadStateMock.Replay ();

      var result = _endPoint.HasChanged;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChanged_False ()
    {
      _loadStateMock.Expect (mock => mock.HasChanged ()).Return (false);
      _loadStateMock.Replay ();

      var result = _endPoint.HasChanged;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.False);
    }
    
    [Test]
    public void Touch ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);
      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Touch_DoesNotLoadData ()
    {
      _endPoint.Touch ();

      AssertDidNotLoadData();
    }
   
    [Test]
    public void HasBeenTouched_DoesNotLoadData ()
    {
      Dev.Null = _endPoint.HasBeenTouched;

      AssertDidNotLoadData();
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _loadStateMock.Expect (mock => mock.EnsureDataComplete (_endPoint));
      _loadStateMock.Replay ();

      _endPoint.EnsureDataComplete();

      _loadStateMock.VerifyAllExpectations ();
    }

    
    [Test]
    public void MarkDataComplete ()
    {
      Action<ICollectionEndPointDataKeeper> stateSetter = null;

      var items = new DomainObject[0];

      _loadStateMock
          .Expect (mock => mock.MarkDataComplete (Arg.Is (_endPoint), Arg.Is (items), Arg<Action<ICollectionEndPointDataKeeper>>.Is.Anything))
          .WhenCalled (mi => { stateSetter = (Action<ICollectionEndPointDataKeeper>) mi.Arguments[2]; });
      _loadStateMock.Replay ();

      _endPoint.MarkDataComplete (items);

      _loadStateMock.VerifyAllExpectations ();

      Assert.That (CollectionEndPointTestHelper.GetLoadState (_endPoint), Is.SameAs (_loadStateMock));

      var dataKeeperStub = MockRepository.GenerateStub<ICollectionEndPointDataKeeper>();
      stateSetter (dataKeeperStub);
      
      var newLoadState = CollectionEndPointTestHelper.GetLoadState (_endPoint);
      Assert.That (newLoadState, Is.TypeOf (typeof (CompleteCollectionEndPointLoadState)));

      Assert.That (((CompleteCollectionEndPointLoadState) newLoadState).DataKeeper, Is.SameAs (dataKeeperStub));
      Assert.That (((CompleteCollectionEndPointLoadState) newLoadState).ClientTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (((CompleteCollectionEndPointLoadState) newLoadState).EndPointProvider, Is.SameAs (_endPointProviderStub));
    }

    [Test]
    public void MarkDataIncomplete ()
    {
      Action stateSetter = null;

      _loadStateMock
          .Expect (mock => mock.MarkDataIncomplete (Arg.Is (_endPoint), Arg<Action>.Is.Anything))
          .WhenCalled (mi => { stateSetter = (Action) mi.Arguments[1]; });
      _loadStateMock.Replay ();

      _endPoint.MarkDataIncomplete ();

      _loadStateMock.VerifyAllExpectations ();

      Assert.That (CollectionEndPointTestHelper.GetLoadState (_endPoint), Is.SameAs (_loadStateMock));

      stateSetter ();
      
      var newLoadState = CollectionEndPointTestHelper.GetLoadState (_endPoint);
      Assert.That (newLoadState, Is.TypeOf (typeof (IncompleteCollectionEndPointLoadState)));

      Assert.That (((IncompleteCollectionEndPointLoadState) newLoadState).LazyLoader, Is.SameAs (GetEndPointLazyLoader (_endPoint)));
    }

    [Test]
    public void Commit_Unchanged ()
    {
      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Replay ();

      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);
      Assert.That (_endPoint.HasChanged, Is.False);

      _endPoint.Commit();

      _loadStateMock.AssertWasNotCalled (mock => mock.Commit(_endPoint));
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Commit_Changed ()
    {
      _loadStateMock.Expect (mock => mock.Commit (_endPoint));
      _loadStateMock.Replay ();

      _endPoint.Touch ();
      var newCollection = new DomainObjectCollection ();
      CollectionEndPointTestHelper.SetCollection (_endPoint, newCollection);

      Assert.That (_endPoint.HasBeenTouched, Is.True);
      Assert.That (_endPoint.OriginalCollection, Is.Not.SameAs (newCollection));
      Assert.That (_endPoint.HasChanged, Is.True);

      _endPoint.Commit ();

      _loadStateMock.VerifyAllExpectations();
      Assert.That (_endPoint.HasBeenTouched, Is.False);
      Assert.That (_endPoint.OriginalCollection, Is.SameAs (newCollection));
    }

    [Test]
    public void Rollback ()
    {
      var fakeCurrentData = new DomainObjectCollectionData (new[] { _order1 });
      var fakeOriginalData = new DomainObjectCollectionData (new[] { _order2 });

      _loadStateMock.Stub (stub => stub.HasChanged()).Return (true);
      _loadStateMock
          .Stub (stub => stub.GetData (_endPoint))
          .Return (new ReadOnlyCollectionDataDecorator (fakeCurrentData, true));
      _loadStateMock
          .Stub (stub => stub.GetOriginalData (_endPoint))
          .Return (new ReadOnlyCollectionDataDecorator (fakeOriginalData, true));
      _loadStateMock.Expect (mock => mock.Rollback (_endPoint));
      _loadStateMock.Replay ();

      _endPoint.Touch();
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      var collectionBefore = (OrderCollection) _endPoint.Collection;

      _endPoint.Rollback ();

      _loadStateMock.VerifyAllExpectations();
      Assert.That (_endPoint.HasBeenTouched, Is.False);
      Assert.That (_endPoint.Collection, Is.SameAs (collectionBefore));
    }
    
    [Test]
    public void Rollback_TouchedUnchanged ()
    {
      _endPoint.Touch();

      _loadStateMock.Stub (stub => stub.HasChanged()).Return (false);

      Assert.That (_endPoint.HasChanged, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _endPoint.Rollback ();

      Assert.That (_endPoint.HasChanged, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback_AfterSetCollection ()
    {
      var fakeCurrentData = new DomainObjectCollectionData (new[] { _order1 });
      var fakeOriginalData = new DomainObjectCollectionData (new[] { _order2 });
      var oldCollection = _endPoint.Collection;

      var newCollection = new OrderCollection { _order2 };
      CollectionEndPointTestHelper.SetCollection (_endPoint, newCollection);
      Assert.That (_endPoint.Collection, Is.SameAs (newCollection));

      var commandMock = MockRepository.GenerateStrictMock<IDataManagementCommand>();
      _loadStateMock
          .Expect (
              mock => mock.CreateSetCollectionCommand (
                  Arg.Is (_endPoint),
                  Arg.Is (oldCollection),
                  Arg<Action<DomainObjectCollection>>.Is.Anything))
          .Return (commandMock)
          .WhenCalled (mi =>
          {
            Assert.That (_endPoint.Collection, Is.SameAs (newCollection));
            var collectionSetter = (Action<DomainObjectCollection>) mi.Arguments[2];
            collectionSetter (oldCollection);
            Assert.That (_endPoint.Collection, Is.SameAs (oldCollection));
          });
      _loadStateMock
          .Stub (stub => stub.GetData (_endPoint))
          .Return (new ReadOnlyCollectionDataDecorator (fakeCurrentData, true));
      _loadStateMock
          .Stub (stub => stub.GetOriginalData (_endPoint))
          .Return (new ReadOnlyCollectionDataDecorator (fakeOriginalData, true));
      _loadStateMock.Expect (mock => mock.Rollback (_endPoint));
      _loadStateMock.Replay ();

      commandMock.Expect (mock => mock.Perform());
      commandMock.Replay();

      _endPoint.Rollback ();

      _loadStateMock.VerifyAllExpectations();
      commandMock.VerifyAllExpectations();

      Assert.That (_endPoint.Collection, Is.SameAs (oldCollection));
    }

    [Test]
    public void Rollback_Unchanged_DoesNotLoadData ()
    {
      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);

      _endPoint.Rollback ();

      AssertDidNotLoadData ();
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order2 });

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Expect (mock => mock.SetDataFromSubTransaction (_endPoint, CollectionEndPointTestHelper.GetLoadState (source)));
      _loadStateMock.Replay ();

      _endPoint.SetDataFromSubTransaction (source);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void SetDataFromSubTransaction_TouchesEndPoint_WhenSourceHasBeenTouched ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order2 });
      source.Touch();

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Stub (stub => stub.SetDataFromSubTransaction (_endPoint, CollectionEndPointTestHelper.GetLoadState (source)));
      _loadStateMock.Replay ();

      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetDataFromSubTransaction (source);

      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_TouchesEndPoint_WhenTargetHasChanged ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order2 });

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (true);
      _loadStateMock.Stub (stub => stub.SetDataFromSubTransaction (_endPoint, CollectionEndPointTestHelper.GetLoadState (source)));
      _loadStateMock.Replay ();

      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetDataFromSubTransaction (source);

      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_DoesNotTouchEndPoint_WhenSourceUntouched_AndTargetUnchanged ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order2 });

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Stub (stub => stub.SetDataFromSubTransaction (_endPoint, CollectionEndPointTestHelper.GetLoadState (source)));
      _loadStateMock.Replay();

      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetDataFromSubTransaction (source);

      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Cannot set this end point's value from "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'; the end points "
        + "do not have the same end point definition.\r\nParameter name: source")]
    public void SetDataFromSubTransaction_InvalidDefinition ()
    {
      var otherID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (otherID, new DomainObject[0]);

      _endPoint.SetDataFromSubTransaction (source);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (_endPoint, _relatedEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.RegisterOriginalOppositeEndPoint(_relatedEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (_endPoint, _relatedEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.UnregisterOriginalOppositeEndPoint (_relatedEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.RegisterCurrentOppositeEndPoint (_endPoint, _relatedEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.RegisterCurrentOppositeEndPoint (_relatedEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.UnregisterCurrentOppositeEndPoint (_endPoint, _relatedEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.UnregisterCurrentOppositeEndPoint (_relatedEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void IsSynchronized ()
    {
      _loadStateMock.Expect (mock => mock.IsSynchronized (_endPoint)).Return (true);
      _loadStateMock.Replay ();

      var result = _endPoint.IsSynchronized;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Synchronize ()
    {
      _loadStateMock.Expect (mock => mock.Synchronize (_endPoint));
      _loadStateMock.Replay ();

      _endPoint.Synchronize();

      _loadStateMock.VerifyAllExpectations ();
    }
    
    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      var fakeEndPoint = _relatedEndPointStub;
      _loadStateMock.Expect (mock => mock.SynchronizeOppositeEndPoint (_endPoint, fakeEndPoint));
      _loadStateMock.Replay ();

      _endPoint.SynchronizeOppositeEndPoint (fakeEndPoint);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      var oppositeDomainObjects = new OrderCollection ();
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand>();

      Action<DomainObjectCollection> collectionSetter = null;
      _loadStateMock
          .Expect (
              mock => mock.CreateSetCollectionCommand (
                  Arg.Is (_endPoint),
                  Arg.Is (oppositeDomainObjects),
                  Arg<Action<DomainObjectCollection>>.Is.Anything))
          .Return (fakeResult)
          .WhenCalled (mi => { collectionSetter = (Action<DomainObjectCollection>) mi.Arguments[2]; });
      _loadStateMock.Replay ();

      var result = _endPoint.CreateSetCollectionCommand (oppositeDomainObjects);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));

      Assert.That (_endPoint.Collection, Is.Not.SameAs (oppositeDomainObjects));
      collectionSetter (oppositeDomainObjects);
      Assert.That (_endPoint.Collection, Is.SameAs (oppositeDomainObjects));
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateRemoveCommand (_endPoint, _order1)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateRemoveCommand (_order1);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateDeleteCommand (_endPoint)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateDeleteCommand ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateInsertCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateInsertCommand (_endPoint, _order1, 0)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateInsertCommand (_order1, 0);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateAddCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateAddCommand (_endPoint, _order1)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateAddCommand (_order1);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateReplaceCommand (_endPoint, 0, _order1)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateReplaceCommand (0, _order1);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void SortCurrentData ()
    {
      Comparison<DomainObject> comparison = (one, two) => 0;
      _loadStateMock.Expect (mock => mock.SortCurrentData (_endPoint, comparison));
      _loadStateMock.Replay();

      _endPoint.SortCurrentData (comparison);

      _loadStateMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateDelegatingCollectionData ()
    {
      var data = _endPoint.CreateDelegatingCollectionData ();

      Assert.That (data, Is.TypeOf<ModificationCheckingCollectionDataDecorator> ());
      var wrappedData =
          DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<EndPointDelegatingCollectionData> (
              (ModificationCheckingCollectionDataDecorator) data);
      Assert.That (wrappedData.EndPointID, Is.EqualTo (_endPoint.ID));
      Assert.That (wrappedData.VirtualEndPointProvider, Is.SameAs (_endPoint.EndPointProvider));
    }

    [Test]
    public void Collection_Get_DoesNotLoadData ()
    {
      Dev.Null = _endPoint.Collection;

      AssertDidNotLoadData ();
    }

    [Test]
    public void Collection_Set ()
    {
      var delegatingData = _endPoint.CreateDelegatingCollectionData ();
      var newCollection = new OrderCollection (delegatingData);

      CollectionEndPointTestHelper.SetCollection (_endPoint, newCollection);

      Assert.That (_endPoint.Collection, Is.SameAs (newCollection));
    }

    [Test]
    public void Collection_Set_DoesNotLoadData ()
    {
      var delegatingData = _endPoint.CreateDelegatingCollectionData ();
      var newCollection = new OrderCollection (delegatingData);

      CollectionEndPointTestHelper.SetCollection (_endPoint, newCollection);

      AssertDidNotLoadData();
    }

    [Test]
    public void Collection_Set_TouchesEndPoint ()
    {
      var delegatingData = _endPoint.CreateDelegatingCollectionData ();
      var newCollection = new OrderCollection (delegatingData);

      CollectionEndPointTestHelper.SetCollection (_endPoint, newCollection);

      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Collection_Set_CauseTransactionListenerNotification ()
    {
      var listener = ClientTransactionTestHelper.CreateAndAddListenerMock (_endPoint.ClientTransaction);

      var delegatingData = _endPoint.CreateDelegatingCollectionData ();
      var newCollection = new OrderCollection (delegatingData);
      CollectionEndPointTestHelper.SetCollection (_endPoint, newCollection);

      listener.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_endPoint.ClientTransaction, _endPoint.ID, null));
    }

    [Test]
    public void OriginalCollection_Get_DoesNotLoadData ()
    {
      Dev.Null = _endPoint.OriginalCollection;

      AssertDidNotLoadData ();
    }

    [Test]
    public void GetData ()
    {
      var fakeResult = new ReadOnlyCollectionDataDecorator(new DomainObjectCollectionData (), true);
      _loadStateMock.Expect (mock => mock.GetData (_endPoint)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.GetData ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void GetOriginalData ()
    {
      var fakeResult = new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData(), false);
      _loadStateMock.Expect (mock => mock.GetOriginalData (_endPoint)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPoint.GetOriginalData ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void GetCollectionEventRaiser ()
    {
      var result = _endPoint.GetCollectionEventRaiser();

      Assert.That (result, Is.SameAs (_endPoint.Collection));
    }

    [Test]
    public void GetCollectionWithOriginalData ()
    {
      var collectionDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData>();
      collectionDataStub.Stub (stub => stub.RequiredItemType).Return (typeof (Order));
      var readOnlyCollectionDataDecorator = new ReadOnlyCollectionDataDecorator (collectionDataStub, false);

      _loadStateMock.Stub (stub => stub.GetOriginalData (_endPoint)).Return (readOnlyCollectionDataDecorator);
      _loadStateMock.Replay();

      var result = _endPoint.GetCollectionWithOriginalData();

      Assert.That (result, Is.TypeOf (typeof (OrderCollection)));
      var actualCollectionData = DomainObjectCollectionDataTestHelper.GetDataStrategyAndCheckType<IDomainObjectCollectionData> (result);
      Assert.That (actualCollectionData, Is.SameAs (readOnlyCollectionDataDecorator));
    }

    [Test]
    public void CanBeCollected ()
    {
      _loadStateMock.Expect (mock => mock.CanEndPointBeCollected (_endPoint)).Return (true).Repeat.Once ();
      _loadStateMock.Expect (mock => mock.CanEndPointBeCollected (_endPoint)).Return (false).Repeat.Once ();
      _loadStateMock.Replay();

      var result1 = _endPoint.CanBeCollected;
      var result2 = _endPoint.CanBeCollected;

      _loadStateMock.VerifyAllExpectations();
      Assert.That (result1, Is.True);
      Assert.That (result2, Is.False);
    }

    [Test]
    public void CanBeMarkedIncomplete ()
    {
      _loadStateMock.Expect (mock => mock.CanDataBeMarkedIncomplete (_endPoint)).Return (true).Repeat.Once ();
      _loadStateMock.Expect (mock => mock.CanDataBeMarkedIncomplete (_endPoint)).Return (false).Repeat.Once ();
      _loadStateMock.Replay ();

      var result1 = _endPoint.CanBeMarkedIncomplete;
      var result2 = _endPoint.CanBeMarkedIncomplete;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result1, Is.True);
      Assert.That (result2, Is.False);
    }

    [Test]
    public void GetOppositeRelationEndPointIDs ()
    {
      var relatedObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var relatedObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      var collectionData = new DomainObjectCollectionData (new[] { relatedObject1, relatedObject2 });

      _loadStateMock.Stub (stub => stub.GetData (_endPoint)).Return (new ReadOnlyCollectionDataDecorator (collectionData, false));
      _loadStateMock.Replay();

      var oppositeEndPoints = _endPoint.GetOppositeRelationEndPointIDs ().ToArray ();

      var expectedOppositeEndPointID1 = RelationEndPointID.Create (relatedObject1.ID, typeof (Order).FullName + ".Customer");
      var expectedOppositeEndPointID2 = RelationEndPointID.Create (relatedObject2.ID, typeof (Order).FullName + ".Customer");
      Assert.That (oppositeEndPoints, Is.EqualTo (new[] { expectedOppositeEndPointID1, expectedOppositeEndPointID2 }));
    }
    

    [Test]
    public void ValidateMandatory_WithItems_Succeeds ()
    {
      var domainObjectCollectionData = new DomainObjectCollectionData (new[] { DomainObjectMother.CreateFakeObject<Order> () });
      _loadStateMock
          .Stub (stub => stub.GetData (_endPoint))
          .Return (new ReadOnlyCollectionDataDecorator (domainObjectCollectionData, false));
      _loadStateMock.Replay();

      _endPoint.ValidateMandatory ();
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException), ExpectedMessage =
        "Mandatory relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders' of domain object "
        + "'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' contains no items.")]
    public void ValidateMandatory_WithNoItems_Throws ()
    {
      var domainObjectCollectionData = new DomainObjectCollectionData ();
      _loadStateMock
          .Stub (stub => stub.GetData (_endPoint))
          .Return (new ReadOnlyCollectionDataDecorator (domainObjectCollectionData, false));
      _loadStateMock.Replay ();

      _endPoint.ValidateMandatory ();
    }

    private ILazyLoader GetEndPointLazyLoader (CollectionEndPoint endPoint)
    {
      return (ILazyLoader) PrivateInvoke.GetNonPublicField (endPoint, "_lazyLoader");
    }

    private void AssertDidNotLoadData ()
    {
      _lazyLoaderMock.AssertWasNotCalled (mock => mock.LoadLazyCollectionEndPoint (_endPoint));
    }
  }
}
