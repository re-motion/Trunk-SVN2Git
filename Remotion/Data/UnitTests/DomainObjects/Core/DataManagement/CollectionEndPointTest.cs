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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class CollectionEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _customerEndPointID;
    
    private IRelationEndPointLazyLoader _lazyLoaderMock;
    private CollectionEndPoint _customerEndPoint;

    private Order _order1; // Customer1
    private Order _orderWithoutOrderItem; // Customer1
    private Order _order2; // Customer3

    private ICollectionEndPointLoadState _loadStateMock;
    private CollectionEndPoint _endPointWithLoadStateMock;
    private IRealObjectEndPoint _relatedEndPointStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _customerEndPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);

      _lazyLoaderMock = MockRepository.GenerateMock<IRelationEndPointLazyLoader> ();

      var changeDetectionStrategy = new RootCollectionEndPointChangeDetectionStrategy();
      _customerEndPoint = new CollectionEndPoint (
          ClientTransactionMock,
          _customerEndPointID,
          _lazyLoaderMock,
          ClientTransactionMock.DataManager,
          new CollectionEndPointDataKeeperFactory (ClientTransactionMock, changeDetectionStrategy));

      CollectionEndPointTestHelper.FillCollectionEndPointWithInitialContents (_customerEndPoint, new[] { _order1, _orderWithoutOrderItem });

      _loadStateMock = MockRepository.GenerateStrictMock<ICollectionEndPointLoadState> ();
      _endPointWithLoadStateMock = CreateEndPointWithLoadStateMock (_loadStateMock);
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
    }

    [Test]
    public void Initialize ()
    {
      var lazyLoaderStub = MockRepository.GenerateStub<IRelationEndPointLazyLoader> ();
      var endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();

      var dataKeeperStub = MockRepository.GenerateStub<ICollectionEndPointDataKeeper>();
      dataKeeperStub.Stub (stub => stub.OriginalOppositeEndPoints).Return (new IRealObjectEndPoint[0]);

      var dataKeeperFactoryMock = MockRepository.GenerateMock<IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper>> ();
      dataKeeperFactoryMock
          .Expect (mock => mock.Create (Arg.Is (_customerEndPointID)))
          .Return (dataKeeperStub);
      dataKeeperFactoryMock.Replay();

      var endPoint = new CollectionEndPoint (
          ClientTransactionMock, 
          _customerEndPointID, 
          lazyLoaderStub, 
          endPointProviderStub,
          dataKeeperFactoryMock);

      dataKeeperFactoryMock.VerifyAllExpectations();

      Assert.That (endPoint.ID, Is.EqualTo (_customerEndPointID));
      Assert.That (endPoint.LazyLoader, Is.SameAs (lazyLoaderStub));
      Assert.That (endPoint.EndPointProvider, Is.SameAs (endPointProviderStub));
      Assert.That (endPoint.DataKeeperFactory, Is.SameAs (dataKeeperFactoryMock));

      var loadState = CollectionEndPointTestHelper.GetLoadState (endPoint);
      Assert.That (loadState, Is.TypeOf (typeof (IncompleteCollectionEndPointLoadState)));
      Assert.That (((IncompleteCollectionEndPointLoadState) loadState).DataKeeperFactory, Is.SameAs (dataKeeperFactoryMock));
      Assert.That (((IncompleteCollectionEndPointLoadState) loadState).DataKeeper, Is.SameAs (dataKeeperStub));
    }

    [Test]
    public void Initialize_UsesEndPointDelegatingData ()
    {
      var dataDecorator = DomainObjectCollectionDataTestHelper.GetDataStrategyAndCheckType<ModificationCheckingCollectionDataDecorator> (
          _customerEndPoint.Collection);
      Assert.That (dataDecorator.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
      
      DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<EndPointDelegatingCollectionData> (dataDecorator);
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
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainObjectWithCollectionMissingCtor));

      var endPointID = RelationEndPointID.Create(new ObjectID (classDefinition, Guid.NewGuid ()), 
                              typeof (DomainObjectWithCollectionMissingCtor) + ".OppositeObjects");
      RelationEndPointObjectMother.CreateCollectionEndPoint (endPointID, new DomainObject[0]);
    }

    [Test]
    public void HasChanged_True_CollectionChanged ()
    {
      _loadStateMock.Replay();
      CollectionEndPointTestHelper.SetCollection (_endPointWithLoadStateMock, new DomainObjectCollection());

      var result = _endPointWithLoadStateMock.HasChanged;

      _loadStateMock.AssertWasNotCalled (mock => mock.HasChanged ());
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChanged_True_LoadState ()
    {
      _loadStateMock.Expect (mock => mock.HasChanged ()).Return (true);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.HasChanged;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChanged_False ()
    {
      _loadStateMock.Expect (mock => mock.HasChanged ()).Return (false);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.HasChanged;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.False);
    }
    
    [Test]
    public void Touch ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Touch ();
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Touch_DoesNotLoadData ()
    {
      _customerEndPoint.MarkDataIncomplete ();
      Assert.That (_customerEndPoint.IsDataComplete, Is.False);

      _customerEndPoint.Touch ();

      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
      AssertDidNotLoadData (_customerEndPoint);
    }
    
    [Test]
    public void HasBeenTouchedAddAndRemove_LeavingSameElements ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection.Add (Order.NewObject ());
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
      _customerEndPoint.Collection.RemoveAt (_customerEndPoint.Collection.Count - 1);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedInsert ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection.Insert (0, Order.NewObject ());
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedRemove ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection.Remove (_customerEndPoint.Collection[0]);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedRemoveNonExisting ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection.Remove (Order.NewObject());
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedClear ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection.Clear ();
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedClearEmpty ()
    {
      _customerEndPoint.Collection.Clear ();
      _customerEndPoint.Commit ();

      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection.Clear ();
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedReplace ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection[0] = Order.NewObject();
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedReplaceWithSame ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection[0] = _customerEndPoint.Collection[0];
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouched_DoesNotLoadData ()
    {
      _customerEndPoint.MarkDataIncomplete ();
      Assert.That (_customerEndPoint.IsDataComplete, Is.False);

      var result = _customerEndPoint.HasBeenTouched;

      Assert.That (result, Is.EqualTo (false));
      AssertDidNotLoadData (_customerEndPoint);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _loadStateMock.Expect (mock => mock.EnsureDataComplete (_endPointWithLoadStateMock));
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.EnsureDataComplete();

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete ()
    {
      Action<ICollectionEndPointDataKeeper> stateSetter = null;

      var items = new DomainObject[0];

      _loadStateMock
          .Expect (mock => mock.MarkDataComplete (Arg.Is (_endPointWithLoadStateMock), Arg.Is (items), Arg<Action<ICollectionEndPointDataKeeper>>.Is.Anything))
          .WhenCalled (mi => { stateSetter = (Action<ICollectionEndPointDataKeeper>) mi.Arguments[2]; });
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.MarkDataComplete (items);

      _loadStateMock.VerifyAllExpectations ();

      Assert.That (CollectionEndPointTestHelper.GetLoadState (_endPointWithLoadStateMock), Is.SameAs (_loadStateMock));

      var dataKeeperStub = MockRepository.GenerateStub<ICollectionEndPointDataKeeper>();
      stateSetter (dataKeeperStub);
      
      var newLoadState = CollectionEndPointTestHelper.GetLoadState (_endPointWithLoadStateMock);
      Assert.That (newLoadState, Is.TypeOf (typeof (CompleteCollectionEndPointLoadState)));

      Assert.That (((CompleteCollectionEndPointLoadState) newLoadState).DataKeeper, Is.SameAs (dataKeeperStub));
      Assert.That (((CompleteCollectionEndPointLoadState) newLoadState).ClientTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (((CompleteCollectionEndPointLoadState) newLoadState).EndPointProvider, Is.SameAs (ClientTransactionMock.DataManager));
    }

    [Test]
    public void MarkDataIncomplete ()
    {
      Action<ICollectionEndPointDataKeeper> stateSetter = null;

      _loadStateMock
          .Expect (mock => mock.MarkDataIncomplete (Arg.Is (_endPointWithLoadStateMock), Arg<Action<ICollectionEndPointDataKeeper>>.Is.Anything))
          .WhenCalled (mi => { stateSetter = (Action<ICollectionEndPointDataKeeper>) mi.Arguments[1]; });
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.MarkDataIncomplete ();

      _loadStateMock.VerifyAllExpectations ();

      Assert.That (CollectionEndPointTestHelper.GetLoadState (_endPointWithLoadStateMock), Is.SameAs (_loadStateMock));

      var dataKeeperStub = MockRepository.GenerateStub<ICollectionEndPointDataKeeper> ();
      dataKeeperStub.Stub (stub => stub.OriginalOppositeEndPoints).Return (new IRealObjectEndPoint[0]);
      stateSetter (dataKeeperStub);
      
      var newLoadState = CollectionEndPointTestHelper.GetLoadState (_endPointWithLoadStateMock);
      Assert.That (newLoadState, Is.TypeOf (typeof (IncompleteCollectionEndPointLoadState)));

      Assert.That (((IncompleteCollectionEndPointLoadState) newLoadState).DataKeeper, Is.SameAs (dataKeeperStub));
      Assert.That (((IncompleteCollectionEndPointLoadState) newLoadState).LazyLoader, Is.SameAs (GetEndPointLazyLoader (_endPointWithLoadStateMock)));
    }

    [Test]
    public void PerformWithoutBegin ()
    {
      Assert.That (_customerEndPoint.Collection.Count, Is.EqualTo (_customerEndPoint.GetCollectionWithOriginalData().Count));

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_order1.ID, "Customer");
      IRelationEndPoint endPointOfObjectBeingRemoved = RelationEndPointObjectMother.CreateObjectEndPoint (endPointID, _customerEndPoint.ObjectID);

      var command = _customerEndPoint.CreateRemoveCommand (endPointOfObjectBeingRemoved.GetDomainObject());
      command.Perform();

      Assert.That (_customerEndPoint.GetCollectionWithOriginalData().Count != _customerEndPoint.Collection.Count, Is.True);
    }

    [Test]
    public void Commit_Unchanged ()
    {
      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.Touch ();
      Assert.That (_endPointWithLoadStateMock.HasBeenTouched, Is.True);
      Assert.That (_endPointWithLoadStateMock.HasChanged, Is.False);

      _endPointWithLoadStateMock.Commit();

      _loadStateMock.AssertWasNotCalled (mock => mock.Commit());
      Assert.That (_endPointWithLoadStateMock.HasBeenTouched, Is.False);
    }

    [Test]
    public void Commit_Changed ()
    {
      _loadStateMock.Expect (mock => mock.Commit ());
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.Touch ();
      var newCollection = new DomainObjectCollection ();
      CollectionEndPointTestHelper.SetCollection (_endPointWithLoadStateMock, newCollection);

      Assert.That (_endPointWithLoadStateMock.HasBeenTouched, Is.True);
      Assert.That (_endPointWithLoadStateMock.OriginalCollection, Is.Not.SameAs (newCollection));
      Assert.That (_endPointWithLoadStateMock.HasChanged, Is.True);

      _endPointWithLoadStateMock.Commit ();

      _loadStateMock.VerifyAllExpectations();
      Assert.That (_endPointWithLoadStateMock.HasBeenTouched, Is.False);
      Assert.That (_endPointWithLoadStateMock.OriginalCollection, Is.SameAs (newCollection));
    }

    [Test]
    public void Rollback ()
    {
      var fakeCurrentData = new DomainObjectCollectionData (new[] { _order1 });
      var fakeCollectionWithOriginalData = new DomainObjectCollectionData();

      _loadStateMock.Stub (stub => stub.HasChanged()).Return (true);
      _loadStateMock
          .Stub (stub => stub.GetData (_endPointWithLoadStateMock))
          .Return (new ReadOnlyCollectionDataDecorator (fakeCurrentData, true));
      _loadStateMock
          .Stub (stub => stub.GetOriginalData (_endPointWithLoadStateMock))
          .Return (new ReadOnlyCollectionDataDecorator (fakeCollectionWithOriginalData, true));
      _loadStateMock.Expect (mock => mock.Rollback());
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.Touch();
      Assert.That (_endPointWithLoadStateMock.HasBeenTouched, Is.True);

      var collectionBefore = _customerEndPoint.Collection;

      _endPointWithLoadStateMock.Rollback ();

      _loadStateMock.VerifyAllExpectations();
      Assert.That (_endPointWithLoadStateMock.HasBeenTouched, Is.False);
      Assert.That (_customerEndPoint.Collection, Is.SameAs (collectionBefore));
    }
    
    [Test]
    public void Rollback_TouchedUnchanged ()
    {
      _customerEndPoint.Collection[0] = _customerEndPoint.Collection[0];

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);

      _customerEndPoint.Rollback();

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback_AfterSetCollection ()
    {
      var oldCollection = _customerEndPoint.Collection;

      var newCollection = new OrderCollection { _order2 };
      _customerEndPoint.CreateSetCollectionCommand (newCollection).ExpandToAllRelatedObjects().NotifyAndPerform();

      Assert.That (_customerEndPoint.Collection, Is.SameAs (newCollection));
      Assert.That (newCollection.IsAssociatedWith (_customerEndPoint), Is.True);
      Assert.That (oldCollection.IsAssociatedWith (null), Is.True);

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.Collection, Is.SameAs (oldCollection));
      Assert.That (newCollection.IsAssociatedWith (null), Is.True);
      Assert.That (oldCollection.IsAssociatedWith (_customerEndPoint), Is.True);
    }

    [Test]
    public void Rollback_Unchanged_DoesNotLoadData ()
    {
      _customerEndPoint.MarkDataIncomplete ();
      Assert.That (_customerEndPoint.IsDataComplete, Is.False);

      _customerEndPoint.Rollback ();

      AssertDidNotLoadData (_customerEndPoint);
    }

    [Test]
    public void SetValueFrom ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order2 });

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Expect (mock => mock.SetValueFrom (_endPointWithLoadStateMock, source));
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.SetValueFrom (source);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void SetValueFrom_TouchesEndPoint_WhenSourceHasBeenTouched ()
    {
      var sourceEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      sourceEndPointStub.Stub (stub => stub.Definition).Return (_customerEndPointID.Definition);
      sourceEndPointStub.Stub (stub => stub.HasBeenTouched).Return (true);

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Stub (stub => stub.SetValueFrom (_endPointWithLoadStateMock, sourceEndPointStub));
      _loadStateMock.Replay ();

      Assert.That (_endPointWithLoadStateMock.HasBeenTouched, Is.False);

      _endPointWithLoadStateMock.SetValueFrom (sourceEndPointStub);

      Assert.That (_endPointWithLoadStateMock.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_TouchesEndPoint_WhenTargetHasChanged ()
    {
      var sourceEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      sourceEndPointStub.Stub (stub => stub.Definition).Return (_customerEndPointID.Definition);
      sourceEndPointStub.Stub (stub => stub.HasBeenTouched).Return (false);

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (true);
      _loadStateMock.Stub (stub => stub.SetValueFrom (_endPointWithLoadStateMock, sourceEndPointStub));
      _loadStateMock.Replay ();

      Assert.That (_endPointWithLoadStateMock.HasBeenTouched, Is.False);

      _endPointWithLoadStateMock.SetValueFrom (sourceEndPointStub);

      Assert.That (_endPointWithLoadStateMock.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_DoesNotTouchEndPoint_WhenSourceUntouched_AndTargetUnchanged ()
    {
      var sourceEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      sourceEndPointStub.Stub (stub => stub.Definition).Return (_customerEndPointID.Definition);
      sourceEndPointStub.Stub (stub => stub.HasBeenTouched).Return (false);

      _loadStateMock.Stub (stub => stub.HasChanged ()).Return (false);
      _loadStateMock.Stub (stub => stub.SetValueFrom (_endPointWithLoadStateMock, sourceEndPointStub));
      _loadStateMock.Replay();

      Assert.That (_endPointWithLoadStateMock.HasBeenTouched, Is.False);

      _endPointWithLoadStateMock.SetValueFrom (sourceEndPointStub);

      Assert.That (_endPointWithLoadStateMock.HasBeenTouched, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Cannot set this end point's value from "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'; the end points "
        + "do not have the same end point definition.\r\nParameter name: source")]
    public void SetValueFrom_InvalidDefinition ()
    {
      var otherID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (otherID, new DomainObject[0]);

      _customerEndPoint.SetValueFrom (source);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (_endPointWithLoadStateMock, _relatedEndPointStub));
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.RegisterOriginalOppositeEndPoint(_relatedEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (_endPointWithLoadStateMock, _relatedEndPointStub));
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.UnregisterOriginalOppositeEndPoint (_relatedEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.RegisterCurrentOppositeEndPoint (_endPointWithLoadStateMock, _relatedEndPointStub));
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.RegisterCurrentOppositeEndPoint (_relatedEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.UnregisterCurrentOppositeEndPoint (_endPointWithLoadStateMock, _relatedEndPointStub));
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.UnregisterCurrentOppositeEndPoint (_relatedEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void IsSynchronized ()
    {
      _loadStateMock.Expect (mock => mock.IsSynchronized (_endPointWithLoadStateMock)).Return (true);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.IsSynchronized;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Synchronize ()
    {
      _loadStateMock.Expect (mock => mock.Synchronize (_endPointWithLoadStateMock));
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.Synchronize();

      _loadStateMock.VerifyAllExpectations ();
    }
    
    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      var fakeEndPoint = _relatedEndPointStub;
      _loadStateMock.Expect (mock => mock.SynchronizeOppositeEndPoint(fakeEndPoint));
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.SynchronizeOppositeEndPoint (fakeEndPoint);

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
                  Arg.Is (_endPointWithLoadStateMock),
                  Arg.Is (oppositeDomainObjects),
                  Arg<Action<DomainObjectCollection>>.Is.Anything))
          .Return (fakeResult)
          .WhenCalled (mi => { collectionSetter = (Action<DomainObjectCollection>) mi.Arguments[2]; });
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.CreateSetCollectionCommand (oppositeDomainObjects);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));

      Assert.That (_endPointWithLoadStateMock.Collection, Is.Not.SameAs (oppositeDomainObjects));
      collectionSetter (oppositeDomainObjects);
      Assert.That (_endPointWithLoadStateMock.Collection, Is.SameAs (oppositeDomainObjects));
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateRemoveCommand (_endPointWithLoadStateMock, _order1)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.CreateRemoveCommand (_order1);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateDeleteCommand (_endPointWithLoadStateMock)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.CreateDeleteCommand ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateInsertCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateInsertCommand (_endPointWithLoadStateMock, _order1, 0)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.CreateInsertCommand (_order1, 0);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateAddCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateAddCommand (_endPointWithLoadStateMock, _order1)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.CreateAddCommand (_order1);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateReplaceCommand (_endPointWithLoadStateMock, 0, _order1)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.CreateReplaceCommand (0, _order1);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateDelegatingCollectionData ()
    {
      var data = _customerEndPoint.CreateDelegatingCollectionData ();

      Assert.That (data.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (data.Count, Is.EqualTo (2), "contains data of end point");
      _customerEndPoint.Collection.Insert (1, _order2);
      Assert.That (data.Count, Is.EqualTo (3), "represents end point");
    }

    [Test]
    public void CreateDelegatingCollectionData_DoesNotLoadData ()
    {
      _customerEndPoint.MarkDataIncomplete ();
      _customerEndPoint.CreateDelegatingCollectionData ();

      AssertDidNotLoadData (_customerEndPoint);
    }

    [Test]
    public void Collection_Get_DoesNotLoadData ()
    {
      var collection = _customerEndPoint.Collection;

      _customerEndPoint.MarkDataIncomplete ();
      Assert.That (_customerEndPoint.IsDataComplete, Is.False);

      var result = _customerEndPoint.Collection;

      Assert.That (result, Is.SameAs (collection));
      AssertDidNotLoadData (_customerEndPoint);
    }

    [Test]
    public void Collection_Set ()
    {
      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData ();
      var newCollection = new OrderCollection (delegatingData);

      CollectionEndPointTestHelper.SetCollection (_customerEndPoint, newCollection);

      Assert.That (_customerEndPoint.Collection, Is.SameAs (newCollection));
    }

    [Test]
    public void Collection_Set_DoesNotLoadData ()
    {
      _customerEndPoint.MarkDataIncomplete ();

      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData ();
      var newCollection = new OrderCollection (delegatingData);

      CollectionEndPointTestHelper.SetCollection (_customerEndPoint, newCollection);

      AssertDidNotLoadData (_customerEndPoint);
    }

    [Test]
    public void Collection_Set_TouchesEndPoint ()
    {
      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData ();
      var newCollection = new OrderCollection (delegatingData);

      CollectionEndPointTestHelper.SetCollection (_customerEndPoint, newCollection);

      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Collection_Set_CauseTransactionListenerNotification ()
    {
      var listener = ClientTransactionTestHelper.CreateAndAddListenerMock (_customerEndPoint.ClientTransaction);

      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData ();
      var newCollection = new OrderCollection (delegatingData);
      CollectionEndPointTestHelper.SetCollection (_customerEndPoint, newCollection);

      listener.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_customerEndPoint.ClientTransaction, _customerEndPoint.ID, true));

      CollectionEndPointTestHelper.SetCollection (_customerEndPoint, _customerEndPoint.OriginalCollection);

      listener.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_customerEndPoint.ClientTransaction, _customerEndPoint.ID, false));
    }

    [Test]
    public void OriginalCollection_Get_DoesNotLoadData ()
    {
      var originalReference = _customerEndPoint.OriginalCollection;

      _customerEndPoint.MarkDataIncomplete ();
      Assert.That (_customerEndPoint.IsDataComplete, Is.False);

      var result = _customerEndPoint.OriginalCollection;

      Assert.That (result, Is.SameAs (originalReference));
      AssertDidNotLoadData (_customerEndPoint);
    }

    [Test]
    public void GetData ()
    {
      var fakeResult = new ReadOnlyCollectionDataDecorator(new DomainObjectCollectionData (), true);
      _loadStateMock.Expect (mock => mock.GetData (_endPointWithLoadStateMock)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.GetData ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void GetOriginalData ()
    {
      var fakeResult = new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData(), false);
      _loadStateMock.Expect (mock => mock.GetOriginalData (_endPointWithLoadStateMock)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.GetOriginalData ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void GetCollectionWithOriginalData ()
    {
      var collectionDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData>();
      collectionDataStub.Stub (stub => stub.RequiredItemType).Return (typeof (Order));
      var readOnlyCollectionDataDecorator = new ReadOnlyCollectionDataDecorator (collectionDataStub, false);

      _loadStateMock.Stub (stub => stub.GetOriginalData (_endPointWithLoadStateMock)).Return (readOnlyCollectionDataDecorator);
      _loadStateMock.Replay();

      var result = _endPointWithLoadStateMock.GetCollectionWithOriginalData();

      Assert.That (result, Is.TypeOf (typeof (OrderCollection)));
      var actualCollectionData = DomainObjectCollectionDataTestHelper.GetDataStrategyAndCheckType<IDomainObjectCollectionData> (result);
      Assert.That (actualCollectionData, Is.SameAs (readOnlyCollectionDataDecorator));
    }

    [Test]
    public void GetOppositeRelationEndPointIDs ()
    {
      var relatedObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var relatedObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      var collectionData = new DomainObjectCollectionData (new[] { relatedObject1, relatedObject2 });

      _loadStateMock.Stub (stub => stub.GetData (_endPointWithLoadStateMock)).Return (new ReadOnlyCollectionDataDecorator (collectionData, false));
      _loadStateMock.Replay();

      var oppositeEndPoints = _endPointWithLoadStateMock.GetOppositeRelationEndPointIDs ().ToArray ();

      var expectedOppositeEndPointID1 = RelationEndPointID.Create (relatedObject1.ID, typeof (Order).FullName + ".Customer");
      var expectedOppositeEndPointID2 = RelationEndPointID.Create (relatedObject2.ID, typeof (Order).FullName + ".Customer");
      Assert.That (oppositeEndPoints, Is.EqualTo (new[] { expectedOppositeEndPointID1, expectedOppositeEndPointID2 }));
    }
    
    [Test]
    public void ChangesToDataState_CauseTransactionListenerNotifications ()
    {
      var listener = ClientTransactionTestHelper.CreateAndAddListenerMock (_customerEndPoint.ClientTransaction);

      _customerEndPoint.Collection.Add (_order2);

      listener.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_customerEndPoint.ClientTransaction, _customerEndPoint.ID, null));
    }

    [Test]
    public void CheckMandatory_WithItems_Succeeds ()
    {
      var domainObjectCollectionData = new DomainObjectCollectionData (new[] { DomainObjectMother.CreateFakeObject<Order> () });
      _loadStateMock
          .Stub (stub => stub.GetData (_endPointWithLoadStateMock))
          .Return (new ReadOnlyCollectionDataDecorator (domainObjectCollectionData, false));
      _loadStateMock.Replay();

      _endPointWithLoadStateMock.CheckMandatory ();
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException), ExpectedMessage =
        "Mandatory relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders' of domain object "
        + "'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' contains no items.")]
    public void CheckMandatory_WithNoItems_Throws ()
    {
      var domainObjectCollectionData = new DomainObjectCollectionData ();
      _loadStateMock
          .Stub (stub => stub.GetData (_endPointWithLoadStateMock))
          .Return (new ReadOnlyCollectionDataDecorator (domainObjectCollectionData, false));
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.CheckMandatory ();
    }

    private IRelationEndPointLazyLoader GetEndPointLazyLoader (CollectionEndPoint endPoint)
    {
      return (IRelationEndPointLazyLoader) PrivateInvoke.GetNonPublicField (endPoint, "_lazyLoader");
    }

    private void AssertDidNotLoadData (IVirtualEndPoint virtualEndPoint)
    {
      _lazyLoaderMock.AssertWasNotCalled (mock => mock.LoadLazyVirtualEndPoint (virtualEndPoint));
      Assert.That (virtualEndPoint.IsDataComplete, Is.False);
    }

    private CollectionEndPoint CreateEndPointWithLoadStateMock (ICollectionEndPointLoadState loadStateMock)
    {
      var changeDetectionStrategy = MockRepository.GenerateStub<ICollectionEndPointChangeDetectionStrategy> ();
      var collectionEndPoint = new CollectionEndPoint (
          ClientTransactionMock,
          _customerEndPointID,
          MockRepository.GenerateStub<IRelationEndPointLazyLoader> (),
          ClientTransactionMock.DataManager,
          new CollectionEndPointDataKeeperFactory (ClientTransactionMock, changeDetectionStrategy));
      PrivateInvoke.SetNonPublicField (collectionEndPoint, "_loadState", loadStateMock);
      return collectionEndPoint;
    }
  }
}
