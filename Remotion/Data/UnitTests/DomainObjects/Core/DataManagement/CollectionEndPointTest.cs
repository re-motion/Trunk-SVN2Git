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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement;
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

    public override void SetUp ()
    {
      base.SetUp ();

      _customerEndPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);

      _lazyLoaderMock = MockRepository.GenerateMock<IRelationEndPointLazyLoader> ();
     
      _customerEndPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (
          _customerEndPointID,
          new RootCollectionEndPointChangeDetectionStrategy(),
          _lazyLoaderMock,
          ClientTransaction.Current,
          new[] { _order1, _orderWithoutOrderItem });

      _loadStateMock = MockRepository.GenerateStrictMock<ICollectionEndPointLoadState> ();
      _endPointWithLoadStateMock = CreateEndPointWithLoadStateMock (_loadStateMock);
    }

    [Test]
    public void Initialize ()
    {
      var changeDetectionStrategyStub = MockRepository.GenerateStub<ICollectionEndPointChangeDetectionStrategy>();
      var endPoint = new CollectionEndPoint (ClientTransactionMock, _customerEndPointID, changeDetectionStrategyStub, _lazyLoaderMock);

      Assert.That (endPoint.ID, Is.EqualTo (_customerEndPointID));

      var loadState = GetLoadState (endPoint);
      Assert.That (loadState, Is.TypeOf (typeof (IncompleteCollectionEndPointLoadState)));

      Assert.That (endPoint.IsDataComplete, Is.False);

      var dataKeeper = GetEndPointDataKeeper (endPoint);
      Assert.That (dataKeeper.CollectionData.ToArray (), Is.Empty);
      Assert.That (dataKeeper.OriginalOppositeEndPoints.ToArray (), Is.Empty);
      Assert.That (
          CollectionEndPointDataKeeperTestHelper.GetEndPointTracker (dataKeeper).EndPointProvider, 
          Is.SameAs (ClientTransactionMock.DataManager));
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
    public void Initialize_DataKeeper_Comparer_Null_NoSortExpression ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_order1.ID, "OrderItems");
      Assert.That (((VirtualRelationEndPointDefinition) endPointID.Definition).GetSortExpression (), Is.Null);

      var endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (endPointID, null);
      var dataKeeper = GetEndPointDataKeeper (endPoint);

      Assert.That (dataKeeper.SortExpressionBasedComparer, Is.Null);
    }

    [Test]
    public void Initialize_DataKeeper_Comparer_Null_NoRootTransaction ()
    {
      Assert.That (((VirtualRelationEndPointDefinition) _customerEndPointID.Definition).GetSortExpression (), Is.Not.Null);
      Assert.That (((VirtualRelationEndPointDefinition) _customerEndPointID.Definition).SortExpressionText, Is.EqualTo ("OrderNumber asc"));
      var endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (
          _customerEndPointID, 
          new SubCollectionEndPointChangeDetectionStrategy(), 
          ClientTransactionMock.DataManager, 
          ClientTransactionMock.CreateSubTransaction(), 
          null);
      
      var dataKeeper = GetEndPointDataKeeper (endPoint);
      Assert.That (dataKeeper.SortExpressionBasedComparer, Is.Null);
    }

    [Test]
    public void Initialize_DataKeeper_Comparer_NotNull_AccessesPropertyValueWithoutNotifications ()
    {
      Assert.That (((VirtualRelationEndPointDefinition) _customerEndPointID.Definition).GetSortExpression (), Is.Not.Null);
      Assert.That (((VirtualRelationEndPointDefinition) _customerEndPointID.Definition).SortExpressionText, Is.EqualTo ("OrderNumber asc"));
      var endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, null);
      var dataKeeper = GetEndPointDataKeeper (endPoint);

      Assert.That (dataKeeper.SortExpressionBasedComparer, Is.Not.Null);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      // _order1 has OrderNumber 1, _order2 has 3
      Assert.That (dataKeeper.SortExpressionBasedComparer.Compare (_order1, _order2), Is.EqualTo (-1));
      Assert.That (dataKeeper.SortExpressionBasedComparer.Compare (_order2, _order1), Is.EqualTo (1));
      Assert.That (dataKeeper.SortExpressionBasedComparer.Compare (_order2, _order2), Is.EqualTo (0));
    }

    [Test]
    public void HasChanged_True_CollectionChanged ()
    {
      _loadStateMock.Replay();
      CollectionEndPointTestHelper.SetCollection (_endPointWithLoadStateMock, new DomainObjectCollection());

      var result = _endPointWithLoadStateMock.HasChanged;

      _loadStateMock.AssertWasNotCalled (mock => mock.HasChanged (Arg<ICollectionEndPointChangeDetectionStrategy>.Is.Anything));
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChanged_True_LoadState ()
    {
      _loadStateMock.Expect (mock => mock.HasChanged (_endPointWithLoadStateMock.ChangeDetectionStrategy)).Return (true);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.HasChanged;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasChanged_False ()
    {
      _loadStateMock.Expect (mock => mock.HasChanged (_endPointWithLoadStateMock.ChangeDetectionStrategy)).Return (false);
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

      Assert.That (GetLoadState (_endPointWithLoadStateMock), Is.SameAs (_loadStateMock));

      var dataKeeperStub = MockRepository.GenerateStub<ICollectionEndPointDataKeeper>();
      stateSetter (dataKeeperStub);
      
      var newLoadState = GetLoadState (_endPointWithLoadStateMock);
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

      Assert.That (GetLoadState (_endPointWithLoadStateMock), Is.SameAs (_loadStateMock));

      var dataKeeperStub = MockRepository.GenerateStub<ICollectionEndPointDataKeeper> ();
      dataKeeperStub.Stub (stub => stub.OriginalOppositeEndPoints).Return (new IObjectEndPoint[0]);
      stateSetter (dataKeeperStub);
      
      var newLoadState = GetLoadState (_endPointWithLoadStateMock);
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
      _loadStateMock.Stub (stub => stub.HasChanged (_endPointWithLoadStateMock.ChangeDetectionStrategy)).Return (false);
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
      var newOrder = Order.NewObject ();
      _customerEndPoint.Collection.Add (newOrder);

      Assert.That (_customerEndPoint.HasChanged, Is.True);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
      Assert.That (_customerEndPoint.Collection.ContainsObject (newOrder), Is.True);
      Assert.That (_customerEndPoint.GetCollectionWithOriginalData().ContainsObject (newOrder), Is.False);

      var collectionBefore = _customerEndPoint.Collection;
      var originalCollectionBefore = _customerEndPoint.GetCollectionWithOriginalData();

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      Assert.That (_customerEndPoint.Collection.ContainsObject (newOrder), Is.False);
      Assert.That (_customerEndPoint.GetCollectionWithOriginalData().ContainsObject (newOrder), Is.False);

      Assert.That (_customerEndPoint.Collection, Is.SameAs (collectionBefore));
      Assert.That (_customerEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (originalCollectionBefore));
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
    public void Rollback_RestoresCollectionStrategies_AfterReplace ()
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

      _loadStateMock.Expect (mock => mock.SetValueFrom (_endPointWithLoadStateMock, source));
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.SetValueFrom (source);

      _loadStateMock.VerifyAllExpectations ();
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
      var oppositeEndPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();

      _loadStateMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (_endPointWithLoadStateMock, oppositeEndPointStub));
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.RegisterOriginalOppositeEndPoint(oppositeEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      var oppositeEndPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();

      _loadStateMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (_endPointWithLoadStateMock, oppositeEndPointStub));
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.UnregisterOriginalOppositeEndPoint (oppositeEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void IsSynchronized_True()
    {
      var fakeEndPoints = Array.AsReadOnly (new IObjectEndPoint[0]);
      _loadStateMock.Expect (mock => mock.GetUnsynchronizedOppositeEndPoints ()).Return (fakeEndPoints);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.IsSynchronized;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void IsSynchronized_False ()
    {
      var fakeEndPoints = Array.AsReadOnly (new[] { MockRepository.GenerateStub<IObjectEndPoint>()});
      _loadStateMock.Expect (mock => mock.GetUnsynchronizedOppositeEndPoints ()).Return (fakeEndPoints);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.IsSynchronized;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void GetUnsynchronizedOppositeEndPoints ()
    {
      var fakeEndPoints = Array.AsReadOnly (new IObjectEndPoint[0]);
      _loadStateMock.Expect (mock => mock.GetUnsynchronizedOppositeEndPoints ()).Return(fakeEndPoints);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.GetUnsynchronizedOppositeEndPoints();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeEndPoints));
    }

    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      var fakeEndPoint = MockRepository.GenerateStub<IObjectEndPoint>();
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
    public void GetCollectionData ()
    {
      var fakeResult = new DomainObjectCollectionData ();
      _loadStateMock.Expect (mock => mock.GetCollectionData (_endPointWithLoadStateMock)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.GetCollectionData ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void GetCollectionWithOriginalData ()
    {
      var fakeResult = new DomainObjectCollection ();
      _loadStateMock.Expect (mock => mock.GetCollectionWithOriginalData (_endPointWithLoadStateMock)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.GetCollectionWithOriginalData ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }
    
    [Test]
    public void GetOppositeRelationEndPointsIDs ()
    {
      var fakeResult = new RelationEndPointID[0];

      _loadStateMock.Expect (mock => mock.GetOppositeRelationEndPointIDs (_endPointWithLoadStateMock)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.GetOppositeRelationEndPointIDs ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }
    
    [Test]
    public void ChangesToDataState_CauseTransactionListenerNotifications ()
    {
      var listener = ClientTransactionTestHelper.CreateAndAddListenerMock (_customerEndPoint.ClientTransaction);

      _customerEndPoint.Collection.Add (_order2);

      listener.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_customerEndPoint.ClientTransaction, _customerEndPoint.ID, null));
    }

    [Test]
    public void CheckMandatory ()
    {
      _loadStateMock.Expect (mock => mock.CheckMandatory(_endPointWithLoadStateMock));
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.CheckMandatory ();

      _loadStateMock.VerifyAllExpectations ();
    }
    
    private CollectionEndPointDataKeeper GetEndPointDataKeeper (CollectionEndPoint endPoint)
    {
      var loadState = GetLoadState (endPoint);
      return (CollectionEndPointDataKeeper) ((IncompleteCollectionEndPointLoadState) loadState).DataKeeper;
    }

    private IRelationEndPointLazyLoader GetEndPointLazyLoader (CollectionEndPoint endPoint)
    {
      return (IRelationEndPointLazyLoader) PrivateInvoke.GetNonPublicField (endPoint, "_lazyLoader");
    }

    private void AssertDidNotLoadData (CollectionEndPoint collectionEndPoint)
    {
      _lazyLoaderMock.AssertWasNotCalled (mock => mock.LoadLazyCollectionEndPoint (collectionEndPoint));
      Assert.That (collectionEndPoint.IsDataComplete, Is.False);
    }

    private CollectionEndPoint CreateEndPointWithLoadStateMock (ICollectionEndPointLoadState loadStateMock)
    {
      var collectionEndPoint = new CollectionEndPoint (
          ClientTransactionMock,
          _customerEndPointID,
          MockRepository.GenerateStub<ICollectionEndPointChangeDetectionStrategy> (),
          MockRepository.GenerateStub<IRelationEndPointLazyLoader> ());
      PrivateInvoke.SetNonPublicField (collectionEndPoint, "_loadState", loadStateMock);
      return collectionEndPoint;
    }

    private ICollectionEndPointLoadState GetLoadState (CollectionEndPoint collectionEndPoint)
    {
      return (ICollectionEndPointLoadState) PrivateInvoke.GetNonPublicField (collectionEndPoint, "_loadState");
    }
  }
}
