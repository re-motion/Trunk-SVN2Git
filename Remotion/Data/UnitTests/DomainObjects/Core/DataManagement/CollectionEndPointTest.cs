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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;

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

      _customerEndPointID = new RelationEndPointID (DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
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
      Assert.That (_customerEndPoint.ID, Is.EqualTo (_customerEndPointID));

      var loadState = GetLoadState (_customerEndPoint);
      Assert.That (loadState, Is.TypeOf (typeof (CompleteCollectionEndPointLoadState)));
      Assert.That (((CompleteCollectionEndPointLoadState) loadState).CollectionEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (((CompleteCollectionEndPointLoadState) loadState).DataKeeper, Is.SameAs (GetEndPointDataKeeper (_customerEndPoint)));

      Assert.That (_customerEndPoint.IsDataComplete, Is.True);
      Assert.That (_customerEndPoint.Collection, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));

      Assert.That (_customerEndPoint.OriginalCollection, Is.SameAs (_customerEndPoint.Collection));
      Assert.That (_customerEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
      Assert.That (_customerEndPoint.GetCollectionWithOriginalData(), Is.Not.SameAs (_customerEndPoint.Collection));
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
    public void Initialize_WithNullInitialContents ()
    {
      var endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (
          _customerEndPointID,
          new RootCollectionEndPointChangeDetectionStrategy(),
          _lazyLoaderMock,
          ClientTransactionMock,
          null);

      var loadState = GetLoadState (endPoint);
      Assert.That (loadState, Is.TypeOf (typeof (IncompleteCollectionEndPointLoadState)));
      Assert.That (((IncompleteCollectionEndPointLoadState) loadState).CollectionEndPoint, Is.SameAs (endPoint));
      Assert.That (((IncompleteCollectionEndPointLoadState) loadState).LazyLoader, Is.SameAs (_lazyLoaderMock));

      Assert.That (endPoint.IsDataComplete, Is.False);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException))]
    public void Initialize_WithoutCollectionCtorTakingData_Throws ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainObjectWithCollectionMissingCtor));

      var endPointID = new RelationEndPointID (
          new ObjectID (classDefinition, Guid.NewGuid ()), 
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
    public void HasChanged_FastPathFalse ()
    {
      var strategyMock = new MockRepository().StrictMock<ICollectionEndPointChangeDetectionStrategy> ();
      strategyMock.Replay ();

      var endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (
          _customerEndPointID, 
          strategyMock, 
          ClientTransactionMock.DataManager, 
          ClientTransactionMock, 
          new[] { _order1 });

      var result = endPoint.HasChanged;

      strategyMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (false));
    }

    [Test]
    public void HasChanged_UsesStrategy_IfRequired ()
    {
      var strategyMock = new MockRepository ().StrictMock<ICollectionEndPointChangeDetectionStrategy> ();
      var endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (
          _customerEndPointID,
          strategyMock,
          ClientTransactionMock.DataManager,
          ClientTransactionMock,
          new[] { _order1 });

      strategyMock
          .Expect (mock => mock.HasDataChanged (
              Arg<IDomainObjectCollectionData>.List.Equal (endPoint.Collection),
              Arg<IDomainObjectCollectionData>.List.Equal (endPoint.GetCollectionWithOriginalData())))
          .Return (true);
      strategyMock.Replay ();

      endPoint.Collection.Add (_order2);

      var result = endPoint.HasChanged;

      strategyMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (true));
    }

    [Test]
    public void HasChanged_TrueWithoutStrategy_WhenCollectionChanged ()
    {
      var strategyMock = MockRepository.GenerateMock<ICollectionEndPointChangeDetectionStrategy> ();
      var endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (
          _customerEndPointID,
          strategyMock,
          ClientTransactionMock.DataManager,
          ClientTransactionMock,
          new[] { _order1 });

      endPoint.CreateSetOppositeCollectionCommand (new OrderCollection { _order1 }).ExpandToAllRelatedObjects().NotifyAndPerform();

      var result = endPoint.HasChanged;

      strategyMock.AssertWasNotCalled (mock => mock.HasDataChanged (Arg<IDomainObjectCollectionData>.Is.Anything, Arg<IDomainObjectCollectionData>.Is.Anything));
      Assert.That (result, Is.EqualTo (true));
    }

    [Test]
    public void HasChanged_DoesNotLoadData ()
    {
      _customerEndPoint.MarkDataIncomplete ();
      Assert.That (_customerEndPoint.IsDataComplete, Is.False);

      var result = _customerEndPoint.HasChanged;
      
      Assert.That (result, Is.EqualTo (false));
      AssertDidNotLoadData(_customerEndPoint);
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
    public void GetCollectionWithOriginalData ()
    {
      var fakeResult = new DomainObjectCollection();
      _loadStateMock.Expect (mock => mock.GetCollectionWithOriginalData()).Return (fakeResult);
      _loadStateMock.Replay();

      var result = _endPointWithLoadStateMock.GetCollectionWithOriginalData();

      _loadStateMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _loadStateMock.Expect (mock => mock.EnsureDataComplete());
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.EnsureDataComplete();

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete ()
    {
      var endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order1 });
      endPoint.MarkDataIncomplete ();
      Assert.That (GetLoadState (endPoint), Is.TypeOf (typeof (IncompleteCollectionEndPointLoadState)));
      Assert.That (endPoint.IsDataComplete, Is.False);

      endPoint.MarkDataComplete ();

      Assert.That (GetLoadState (endPoint), Is.TypeOf (typeof (CompleteCollectionEndPointLoadState)));
      Assert.That (endPoint.IsDataComplete, Is.True);
      Assert.That (endPoint.Collection, Is.EqualTo (new[] { _order1 }));
      Assert.That (endPoint.GetCollectionWithOriginalData(), Is.EqualTo (new[] { _order1 }));
      Assert.That (endPoint.HasChanged, Is.False);
    }

    [Test]
    public void MarkDataComplete_WithChangedEndPoint ()
    {
      var endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order1 });
      endPoint.Collection.Add (_order2);

      endPoint.MarkDataIncomplete ();
      Assert.That (endPoint.IsDataComplete, Is.False);

      endPoint.MarkDataComplete ();

      Assert.That (endPoint.IsDataComplete, Is.True);
      Assert.That (endPoint.Collection, Is.EqualTo (new[] { _order1, _order2 }));
      Assert.That (endPoint.GetCollectionWithOriginalData(), Is.EqualTo (new[] { _order1 }));
      Assert.That (endPoint.HasChanged, Is.True);
    }

    [Test]
    public void MarkDataIncomplete ()
    {
      Assert.That (GetLoadState (_customerEndPoint), Is.TypeOf (typeof (CompleteCollectionEndPointLoadState)));
      Assert.That (_customerEndPoint.IsDataComplete, Is.True);
      
      _customerEndPoint.MarkDataIncomplete ();

      Assert.That (GetLoadState (_customerEndPoint), Is.TypeOf (typeof (IncompleteCollectionEndPointLoadState)));
      Assert.That (_customerEndPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void MarkDataIncomplete_TriggersListener ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      listenerMock
          .Expect (mock => mock.RelationEndPointUnloading (ClientTransactionMock, _customerEndPoint))
          .WhenCalled (mi => Assert.That (_customerEndPoint.IsDataComplete, Is.True));
      listenerMock.Replay ();

      ClientTransactionMock.AddListener (listenerMock);

      _customerEndPoint.MarkDataIncomplete ();

      listenerMock.VerifyAllExpectations ();
      listenerMock.BackToRecord (); // For Discard
    }

    [Test]
    public void MarkDataIncomplete_NoListener_IfDataNotComplete ()
    {
      _customerEndPoint.MarkDataIncomplete ();
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      _customerEndPoint.MarkDataIncomplete ();
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
    public void Commit ()
    {
      var newOrder = Order.NewObject ();
      _customerEndPoint.Collection.Add (newOrder);

      Assert.That (_customerEndPoint.HasChanged, Is.True);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
      Assert.That (_customerEndPoint.Collection.ContainsObject (newOrder), Is.True);
      Assert.That (_customerEndPoint.GetCollectionWithOriginalData().ContainsObject (newOrder), Is.False);

      var collectionBefore = _customerEndPoint.Collection;

      _customerEndPoint.Commit ();

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      Assert.That (_customerEndPoint.Collection.ContainsObject (newOrder), Is.True);
      Assert.That (_customerEndPoint.GetCollectionWithOriginalData().ContainsObject (newOrder), Is.True);

      Assert.That (_customerEndPoint.Collection, Is.SameAs (collectionBefore));
      Assert.That (_customerEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (_customerEndPoint.Collection));
    }

    [Test]
    public void Commit_TouchedUnchanged ()
    {
      _customerEndPoint.Collection[0] = _customerEndPoint.Collection[0];

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);

      _customerEndPoint.Commit ();

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Commit_Unchanged_DoesNotLoadData ()
    {
      _customerEndPoint.Touch ();
      _customerEndPoint.MarkDataIncomplete ();
      Assert.That (_customerEndPoint.IsDataComplete, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);

      _customerEndPoint.Commit();

      AssertDidNotLoadData (_customerEndPoint);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
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
      _customerEndPoint.CreateSetOppositeCollectionCommand (newCollection).ExpandToAllRelatedObjects().NotifyAndPerform();

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

      _loadStateMock.Expect (mock => mock.SetValueFrom (source));
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
    public void RegisterOriginalObject ()
    {
      _customerEndPoint.RegisterOriginalObject (_order2);

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.Collection, List.Contains (_order2));
      Assert.That (_customerEndPoint.GetCollectionWithOriginalData(), List.Contains (_order2));
    }

    [Test]
    public void RegisterOriginalObject_DoesNotLoadData ()
    {
      _customerEndPoint.MarkDataIncomplete ();
      _customerEndPoint.RegisterOriginalObject (_order2);

      AssertDidNotLoadData (_customerEndPoint);
    }

    [Test]
    public void UnregisterOriginalObject ()
    {
      _customerEndPoint.UnregisterOriginalObject (_order1.ID);

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.Collection, List.Not.Contains (_order1));
      Assert.That (_customerEndPoint.GetCollectionWithOriginalData(), List.Not.Contains (_order1));
    }

    [Test]
    public void UnregisterOriginalObject_DoesNotLoadData ()
    {
      _customerEndPoint.MarkDataIncomplete ();
      _customerEndPoint.UnregisterOriginalObject (_order1.ID);

      AssertDidNotLoadData (_customerEndPoint);
    }

    [Test]
    public void CreateSetOppositeCollectionCommand ()
    {
      var oppositeDomainObjects = new DomainObjectCollection ();
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand>();

      _loadStateMock.Expect (mock => mock.CreateSetOppositeCollectionCommand (oppositeDomainObjects)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.CreateSetOppositeCollectionCommand (oppositeDomainObjects);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateRemoveCommand (_order1)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.CreateRemoveCommand (_order1);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateDeleteCommand ()).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.CreateDeleteCommand ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateInsertCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateInsertCommand (_order1, 0)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.CreateInsertCommand (_order1, 0);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateAddCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateAddCommand (_order1)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.CreateAddCommand (_order1);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      _loadStateMock.Expect (mock => mock.CreateReplaceCommand (0, _order1)).Return (fakeResult);
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
      var oppositeCollection = _customerEndPoint.Collection;

      _customerEndPoint.MarkDataIncomplete ();
      Assert.That (_customerEndPoint.IsDataComplete, Is.False);

      var result = _customerEndPoint.Collection;

      Assert.That (result, Is.SameAs (oppositeCollection));
      AssertDidNotLoadData (_customerEndPoint);
    }

    [Test]
    public void Collection_Set ()
    {
      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData ();
      var newOppositeCollection = new OrderCollection (delegatingData);

      _customerEndPoint.Collection = newOppositeCollection;

      Assert.That (_customerEndPoint.Collection, Is.SameAs (newOppositeCollection));
    }

    [Test]
    public void Collection_Set_DoesNotLoadData ()
    {
      _customerEndPoint.MarkDataIncomplete ();

      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData ();
      var newOppositeCollection = new OrderCollection (delegatingData);

      _customerEndPoint.Collection = newOppositeCollection;

      AssertDidNotLoadData (_customerEndPoint);
    }

    [Test]
    public void Collection_Set_TouchesEndPoint ()
    {
      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData ();
      var newOppositeCollection = new OrderCollection (delegatingData);

      _customerEndPoint.Collection = newOppositeCollection;

      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Collection_Set_CauseTransactionListenerNotification ()
    {
      var listener = ClientTransactionTestHelper.CreateAndAddListenerMock (_customerEndPoint.ClientTransaction);

      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData ();
      var newOppositeCollection = new OrderCollection (delegatingData);
      _customerEndPoint.Collection = newOppositeCollection;

      listener.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_customerEndPoint.ClientTransaction, _customerEndPoint.ID, true));

      _customerEndPoint.Collection = _customerEndPoint.OriginalCollection;

      listener.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_customerEndPoint.ClientTransaction, _customerEndPoint.ID, false));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The new opposite collection must have been prepared to delegate to this end point. Use SetOppositeCollectionAndNotify instead."
        + "\r\nParameter name: value")]
    public void Collection_Set_NotAssociated ()
    {
      var newOppositeCollection = new OrderCollection { _order1 };

      _customerEndPoint.Collection = newOppositeCollection;
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "Argument value has type Remotion.Data.DomainObjects.DomainObjectCollection when type "
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderCollection was expected.\r\nParameter name: value")]
    public void Collection_Set_OtherType ()
    {
      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData ();
      var newOppositeCollection = new DomainObjectCollection (delegatingData);

      _customerEndPoint.Collection = newOppositeCollection;
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "Argument value has type Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.DerivedOrderCollection when type "
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderCollection was expected.\r\nParameter name: value")]
    public void Collection_Set_DerivedType ()
    {
      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData ();
      var newOppositeCollection = new DerivedOrderCollection (delegatingData);

      _customerEndPoint.Collection = newOppositeCollection;
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
    public void GetOppositeRelationEndPoints ()
    {
      var fakeResult = new IRelationEndPoint[0];
      var dataManagerStub = MockRepository.GenerateStub<IDataManager>();

      _loadStateMock.Expect (mock => mock.GetOppositeRelationEndPoints (dataManagerStub)).Return (fakeResult);
      _loadStateMock.Replay ();

      var result = _endPointWithLoadStateMock.GetOppositeRelationEndPoints (dataManagerStub);

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
      _loadStateMock.Expect (mock => mock.CheckMandatory());
      _loadStateMock.Replay ();

      _endPointWithLoadStateMock.CheckMandatory ();

      _loadStateMock.VerifyAllExpectations ();
    }
    
    private LazyLoadingCollectionEndPointDataKeeper GetEndPointDataKeeper (CollectionEndPoint endPoint)
    {
      return (LazyLoadingCollectionEndPointDataKeeper) PrivateInvoke.GetNonPublicField (endPoint, "_dataKeeper");
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
          MockRepository.GenerateStub<IRelationEndPointLazyLoader> (),
          null);
      PrivateInvoke.SetNonPublicField (collectionEndPoint, "_loadState", loadStateMock);
      return collectionEndPoint;
    }

    private ICollectionEndPointLoadState GetLoadState (CollectionEndPoint collectionEndPoint)
    {
      return (ICollectionEndPointLoadState) PrivateInvoke.GetNonPublicField (collectionEndPoint, "_loadState");
    }
  }
}
