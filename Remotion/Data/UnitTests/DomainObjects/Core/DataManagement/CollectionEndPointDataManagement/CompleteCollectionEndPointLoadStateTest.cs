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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement
{
  [TestFixture]
  public class CompleteCollectionEndPointLoadStateTest : StandardMappingTest
  {
    private ICollectionEndPoint _collectionEndPointMock;
    private ICollectionEndPointDataKeeper _dataKeeperMock;
    private IRelationEndPointProvider _endPointProviderStub;
    private ClientTransaction _clientTransaction;

    private CompleteCollectionEndPointLoadState _loadState;

    private IRelationEndPointDefinition _definition;
    private Order _relatedObject;
    private IObjectEndPoint _relatedEndPointStub;
    private Customer _owningObject;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      _dataKeeperMock = MockRepository.GenerateStrictMock<ICollectionEndPointDataKeeper>();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();
      _clientTransaction = ClientTransaction.CreateRootTransaction ();

      _loadState = new CompleteCollectionEndPointLoadState (_dataKeeperMock, _endPointProviderStub, _clientTransaction);

      _definition = Configuration.ClassDefinitions[typeof (Customer)].GetRelationEndPointDefinition (typeof (Customer).FullName + ".Orders");
      _relatedObject = DomainObjectMother.CreateFakeObject<Order>();
      _relatedEndPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();
      _relatedEndPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_relatedObject);
      _owningObject = DomainObjectMother.CreateFakeObject<Customer>();
    }

    [Test]
    public void IsDataComplete ()
    {
      Assert.That (_loadState.IsDataComplete (), Is.True);
    }

    [Test]
    public void EnsureDataComplete_DoesNothing ()
    {
      _collectionEndPointMock.Replay();
      _dataKeeperMock.Replay();

      _loadState.EnsureDataComplete(_collectionEndPointMock);

      _collectionEndPointMock.VerifyAllExpectations();
      _dataKeeperMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The data is already complete.")]
    public void MarkDataComplete_ThrowsException ()
    {
      var items = new DomainObject[] { _relatedObject };
      _loadState.MarkDataComplete (_collectionEndPointMock, items, keeper => Assert.Fail ("Must not be called"));
    }

    [Test]
    public void MarkDataIncomplete_RaisesEvent ()
    {
      // The following is stubbed for NAnt's NUnit task, which enables logging (and the LoggingClientTransactionListener needs ID to be stubbed).
      _collectionEndPointMock
          .Stub (stub => stub.ID)
          .Return (RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems"));
      _collectionEndPointMock.Replay ();
      _dataKeeperMock.Replay ();

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_clientTransaction);

      _loadState.MarkDataIncomplete (_collectionEndPointMock, keeper => { });

      _collectionEndPointMock.VerifyAllExpectations ();
      _dataKeeperMock.VerifyAllExpectations ();

      listenerMock.AssertWasCalled (mock => mock.RelationEndPointUnloading (_clientTransaction, _collectionEndPointMock));
    }

    [Test]
    public void MarkDataIncomplete_ExecutesStateSetter_AndSynchronizesOppositeEndPoints ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IObjectEndPoint> ();
      PrivateInvoke.SetNonPublicField (_loadState, "_unsynchronizedOppositeEndPoints", new List<IObjectEndPoint> (new[] { endPointMock }));

      bool stateSetterCalled = false;

      // The following is stubbed for NAnt's NUnit task, which enables logging (and the LoggingClientTransactionListener needs ID to be stubbed).
      _collectionEndPointMock
          .Stub (stub => stub.ID)
          .Return (RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems"));
      _collectionEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock))
// ReSharper disable AccessToModifiedClosure
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
// ReSharper restore AccessToModifiedClosure
      _collectionEndPointMock.Replay ();

      endPointMock.Replay ();
      _dataKeeperMock.Replay ();

      _loadState.MarkDataIncomplete (
          _collectionEndPointMock,
          keeper =>
          {
            stateSetterCalled = true;
            Assert.That (keeper, Is.SameAs (_dataKeeperMock));
          });

      _collectionEndPointMock.VerifyAllExpectations ();
      endPointMock.VerifyAllExpectations ();
      _dataKeeperMock.VerifyAllExpectations ();

      Assert.That (stateSetterCalled, Is.True);
    }

    [Test]
    public void GetCollectionData ()
    {
      var collectionDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData> ();
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (collectionDataStub);

      var result = _loadState.GetCollectionData(_collectionEndPointMock);

      Assert.That (result, Is.SameAs (collectionDataStub));
    }

    [Test]
    public void GetCollectionWithOriginalData_CreatesNewCollectionFromData ()
    {
      var collectionDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData>();
      collectionDataStub.Stub (stub => stub.RequiredItemType).Return (typeof (Order));
      var readOnlyCollectionDataDecorator = new ReadOnlyCollectionDataDecorator (collectionDataStub, false);

      _collectionEndPointMock.Stub (stub => stub.Definition).Return (_definition);
      _dataKeeperMock.Stub (stub => stub.OriginalCollectionData).Return (readOnlyCollectionDataDecorator);

      var result = _loadState.GetCollectionWithOriginalData(_collectionEndPointMock);

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

      _collectionEndPointMock.Stub (stub => stub.Definition).Return (_definition);
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (collectionData);

      var oppositeEndPoints = _loadState.GetOppositeRelationEndPointIDs (_collectionEndPointMock).ToArray ();

      var expectedOppositeEndPointID1 = RelationEndPointID.Create (relatedObject1.ID, typeof (Order).FullName + ".Customer");
      var expectedOppositeEndPointID2 = RelationEndPointID.Create (relatedObject2.ID, typeof (Order).FullName + ".Customer");
      Assert.That (oppositeEndPoints, Is.EqualTo (new[] { expectedOppositeEndPointID1, expectedOppositeEndPointID2 }));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_WithoutExistingItem ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Expect (mock => mock.MarkUnsynchronized());
      endPointMock.Replay();

      var existingItems = new DomainObjectCollectionData ();
      _dataKeeperMock.Stub (stub => stub.OriginalCollectionData).Return (new ReadOnlyCollectionDataDecorator (existingItems, false));
      _dataKeeperMock.Replay();

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointMock);

      _dataKeeperMock.AssertWasNotCalled (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      endPointMock.VerifyAllExpectations();
      _dataKeeperMock.VerifyAllExpectations();

      Assert.That (_loadState.GetUnsynchronizedOppositeEndPoints(), List.Contains (endPointMock));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_WithExistingItem ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Expect (mock => mock.MarkSynchronized());
      endPointMock.Replay ();

      var existingItems = new DomainObjectCollectionData (new[] { DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1) });
      _dataKeeperMock.Stub (stub => stub.OriginalCollectionData).Return (new ReadOnlyCollectionDataDecorator (existingItems, false));
      _dataKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      _dataKeeperMock.Replay ();

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointMock);

      endPointMock.VerifyAllExpectations ();
      _dataKeeperMock.VerifyAllExpectations ();
      Assert.That (_loadState.GetUnsynchronizedOppositeEndPoints (), List.Not.Contains (endPointMock));
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();

      using (_collectionEndPointMock.GetMockRepository ().Ordered ())
      {
        _collectionEndPointMock.Expect (mock => mock.MarkDataIncomplete());
        _collectionEndPointMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (endPointStub));
      }
      _collectionEndPointMock.Replay();

      _loadState.UnregisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointStub);

      _dataKeeperMock.VerifyAllExpectations ();
      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void IsSynchronized_True ()
    {
      _dataKeeperMock.Stub (stub => stub.OriginalItemsWithoutEndPoints).Return (new DomainObject[0]);

      _collectionEndPointMock.Replay ();

      Assert.That (_loadState.IsSynchronized (_collectionEndPointMock), Is.True);
    }

    [Test]
    public void IsSynchronized_False ()
    {
      _dataKeeperMock.Stub (stub => stub.OriginalItemsWithoutEndPoints).Return (new DomainObject[] { _relatedObject });

      _collectionEndPointMock.Replay ();

      Assert.That (_loadState.IsSynchronized (_collectionEndPointMock), Is.False);
    }

    [Test]
    public void Synchronize_UnregistersItemsWithoutEndPoints ()
    {
      _dataKeeperMock
          .Stub (stub => stub.OriginalItemsWithoutEndPoints)
          .Return (new[] { _relatedObject });
      _dataKeeperMock.Expect (mock => mock.UnregisterOriginalItemWithoutEndPoint (_relatedObject));
      _dataKeeperMock.Replay();

      _loadState.Synchronize (_collectionEndPointMock);

      _dataKeeperMock.VerifyAllExpectations();
    }

    [Test]
    public void GetUnsynchronizedOppositeEndPoints_Empty ()
    {
      var result = _loadState.GetUnsynchronizedOppositeEndPoints();

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void SynchronizeOppositeEndPoint_InList ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Stub (mock => mock.MarkUnsynchronized ());
      endPointMock.Expect (mock => mock.MarkSynchronized ());
      endPointMock.Replay();

      _dataKeeperMock.Stub (stub => stub.OriginalCollectionData).Return (new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData(), false));
      _dataKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      _dataKeeperMock.Replay ();

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointMock);
      Assert.That (_loadState.GetUnsynchronizedOppositeEndPoints (), List.Contains (endPointMock));

      _loadState.SynchronizeOppositeEndPoint (endPointMock);

      _dataKeeperMock.VerifyAllExpectations();
      endPointMock.VerifyAllExpectations();
      Assert.That (_loadState.GetUnsynchronizedOppositeEndPoints (), List.Not.Contains (endPointMock));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot synchronize opposite end-point "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' - the "
        + "end-point is not in the list of unsynchronized end-points.")]
    public void SynchronizeOppositeEndPoint_NotInList ()
    {
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();
      endPointStub.Stub (stub => stub.ID).Return (RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order"));

      _loadState.SynchronizeOppositeEndPoint (endPointStub);
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);
      _collectionEndPointMock.Replay ();

      Action<DomainObjectCollection> fakeSetter = collection => { };
      var newCollection = new OrderCollection ();

      var command = (RelationEndPointModificationCommand) _loadState.CreateSetCollectionCommand (_collectionEndPointMock, newCollection, fakeSetter);

      Assert.That (command, Is.TypeOf (typeof (CollectionEndPointSetCollectionCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (((CollectionEndPointSetCollectionCommand) command).NewCollection, Is.SameAs (newCollection));
      Assert.That (((CollectionEndPointSetCollectionCommand) command).NewCollectionTransformer, Is.SameAs (newCollection));
      Assert.That (((CollectionEndPointSetCollectionCommand) command).OldCollectionTransformer, Is.SameAs (fakeCollection));
      Assert.That (PrivateInvoke.GetNonPublicField (command, "_collectionSetter"), Is.SameAs (fakeSetter));
    }
    
    [Test]
    public void CreateSetCollectionCommand_SelfReplace ()
    {
      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);
      _collectionEndPointMock.Replay ();
      
      Action<DomainObjectCollection> fakeSetter = collection => { };

      var command = (RelationEndPointModificationCommand) _loadState.CreateSetCollectionCommand (_collectionEndPointMock, fakeCollection, fakeSetter);

      Assert.That (command, Is.TypeOf (typeof (CollectionEndPointSetCollectionCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (((CollectionEndPointSetCollectionCommand) command).NewCollection, Is.SameAs (_collectionEndPointMock.Collection));
      Assert.That (((CollectionEndPointSetCollectionCommand) command).NewCollectionTransformer, Is.SameAs (fakeCollection));
      Assert.That (((CollectionEndPointSetCollectionCommand) command).OldCollectionTransformer, Is.SameAs (fakeCollection));
      Assert.That (PrivateInvoke.GetNonPublicField (command, "_collectionSetter"), Is.SameAs (fakeSetter));
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var fakeCollectionData = new DomainObjectCollectionData ();
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);

      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateRemoveCommand (_collectionEndPointMock, _relatedObject);
      Assert.That (command, Is.InstanceOfType (typeof (CollectionEndPointRemoveCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (command.DomainObject, Is.SameAs (_owningObject));
      Assert.That (command.OldRelatedObject, Is.SameAs (_relatedObject));

      Assert.That (((CollectionEndPointRemoveCommand) command).ModifiedCollection, Is.SameAs (fakeCollection));
      Assert.That (((CollectionEndPointRemoveCommand) command).ModifiedCollectionData, Is.SameAs (fakeCollectionData));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeCollectionData = new DomainObjectCollectionData ();
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);

      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateDeleteCommand (_collectionEndPointMock);
      Assert.That (command, Is.InstanceOfType (typeof (CollectionEndPointDeleteCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));

      Assert.That (((CollectionEndPointDeleteCommand) command).ModifiedCollectionData, Is.SameAs (fakeCollectionData));
      Assert.That (((CollectionEndPointDeleteCommand) command).ModifiedCollection, Is.SameAs (fakeCollection));
    }

    [Test]
    public void CreateInsertCommand ()
    {
      var fakeCollectionData = new DomainObjectCollectionData ();
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);

      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateInsertCommand (_collectionEndPointMock, _relatedObject, 12);
      Assert.That (command, Is.InstanceOfType (typeof (CollectionEndPointInsertCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (command.NewRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (((CollectionEndPointInsertCommand) command).Index, Is.EqualTo (12));

      Assert.That (((CollectionEndPointInsertCommand) command).ModifiedCollectionData, Is.SameAs (fakeCollectionData));
      Assert.That (((CollectionEndPointInsertCommand) command).ModifiedCollection, Is.SameAs (fakeCollection));
    }

    [Test]
    public void CreateAddCommand ()
    {
      var fakeCollectionData =
          new DomainObjectCollectionData (new[] { DomainObjectMother.CreateFakeObject<Order> (), DomainObjectMother.CreateFakeObject<Order> () });
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);

      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateAddCommand (_collectionEndPointMock, _relatedObject);
      Assert.That (command, Is.InstanceOfType (typeof (CollectionEndPointInsertCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (command.NewRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (((CollectionEndPointInsertCommand) command).Index, Is.EqualTo (2));

      Assert.That (((CollectionEndPointInsertCommand) command).ModifiedCollectionData, Is.SameAs (fakeCollectionData));
      Assert.That (((CollectionEndPointInsertCommand) command).ModifiedCollection, Is.SameAs (fakeCollection));
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      var fakeCollectionData = new DomainObjectCollectionData (new[] { oldRelatedObject });
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);

      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateReplaceCommand (_collectionEndPointMock, 0, _relatedObject);
      Assert.That (command, Is.InstanceOfType (typeof (CollectionEndPointReplaceCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (command.OldRelatedObject, Is.SameAs (oldRelatedObject));
      Assert.That (command.NewRelatedObject, Is.SameAs (_relatedObject));

      Assert.That (((CollectionEndPointReplaceCommand) command).ModifiedCollectionData, Is.SameAs (fakeCollectionData));
      Assert.That (((CollectionEndPointReplaceCommand) command).ModifiedCollection, Is.SameAs (fakeCollection));
    }

    [Test]
    public void CreateReplaceCommand_SelfReplace ()
    {
      var fakeCollectionData = new DomainObjectCollectionData (new[] { _relatedObject });
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);

      var fakeCollection = new DomainObjectCollection ();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateReplaceCommand (_collectionEndPointMock, 0, _relatedObject);
      Assert.That (command, Is.InstanceOfType (typeof (CollectionEndPointReplaceSameCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (command.OldRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (command.NewRelatedObject, Is.SameAs (_relatedObject));
    }

    [Test]
    public void SetValueFrom_ReplacesCollectionData ()
    {
      var sourceItem1 = DomainObjectMother.CreateFakeObject<Order>();
      var sourceItem2 = DomainObjectMother.CreateFakeObject<Order>();

      var sourceCollection = new DomainObjectCollection (new DomainObject[] { sourceItem1, sourceItem2 }, null);
      var sourceEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
      sourceEndPointStub.Stub (stub => stub.Collection).Return (sourceCollection);

      var targetItem1 = DomainObjectMother.CreateFakeObject<Order>();
      var targetCollectionData = new DomainObjectCollectionData (new DomainObject[] { targetItem1 });
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (targetCollectionData);
      _collectionEndPointMock.Stub (stub => stub.HasChanged).Return (false);

      _loadState.SetValueFrom (_collectionEndPointMock, sourceEndPointStub);

      Assert.That (targetCollectionData.ToArray(), Is.EqualTo (new[] { sourceItem1, sourceItem2 }));
    }

    [Test]
    public void SetValueFrom_TouchesEndPoint_WhenSourceHasBeenTouched ()
    {
      var sourceCollection = new DomainObjectCollection();
      var sourceEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
      sourceEndPointStub.Stub (stub => stub.Collection).Return (sourceCollection);
      sourceEndPointStub.Stub (stub => stub.HasBeenTouched).Return (true);

      _collectionEndPointMock.Stub (stub => stub.HasChanged).Return (false);
      _collectionEndPointMock.Expect (mock => mock.Touch());
      _collectionEndPointMock.Replay();

      var targetCollectionData = new DomainObjectCollectionData();
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (targetCollectionData);

      _loadState.SetValueFrom (_collectionEndPointMock, sourceEndPointStub);

      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void SetValueFrom_TouchesEndPoint_WhenTargetHasChanged ()
    {
      var sourceCollection = new DomainObjectCollection();
      var sourceEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
      sourceEndPointStub.Stub (stub => stub.Collection).Return (sourceCollection);
      sourceEndPointStub.Stub (stub => stub.HasBeenTouched).Return (false);

      _collectionEndPointMock.Stub (stub => stub.HasChanged).Return (true);
      _collectionEndPointMock.Expect (mock => mock.Touch());
      _collectionEndPointMock.Replay();

      var targetCollectionData = new DomainObjectCollectionData();
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (targetCollectionData);

      _loadState.SetValueFrom (_collectionEndPointMock, sourceEndPointStub);

      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void SetValueFrom_DoesNotTouchEndPoint_WhenSourceUntouched_AndTargetUnchanged ()
    {
      var sourceCollection = new DomainObjectCollection();
      var sourceEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
      sourceEndPointStub.Stub (stub => stub.Collection).Return (sourceCollection);
      sourceEndPointStub.Stub (stub => stub.HasBeenTouched).Return (false);

      _collectionEndPointMock.Stub (stub => stub.HasChanged).Return (false);
      _collectionEndPointMock.Replay();

      var targetCollectionData = new DomainObjectCollectionData();
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (targetCollectionData);

      _loadState.SetValueFrom (_collectionEndPointMock, sourceEndPointStub);

      _collectionEndPointMock.AssertWasNotCalled (mock => mock.Touch());
      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void CheckMandatory_WithItems_Succeeds ()
    {
      _dataKeeperMock
          .Stub (stub => stub.CollectionData)
          .Return (new DomainObjectCollectionData (new[] { DomainObjectMother.CreateFakeObject<Order>() }));

      _loadState.CheckMandatory(_collectionEndPointMock);
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException), ExpectedMessage =
        "Mandatory relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders' of domain object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' contains no items.")]
    public void CheckMandatory_WithNoItems_Throws ()
    {
      _dataKeeperMock
          .Stub (stub => stub.CollectionData)
          .Return (new DomainObjectCollectionData());
      _collectionEndPointMock
          .Stub (stub => stub.Definition)
          .Return (_definition);
      _collectionEndPointMock
          .Stub (stub => stub.GetDomainObjectReference())
          .Return (DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1));

      _loadState.CheckMandatory(_collectionEndPointMock);
    }

    [Test]
    public void HasChanged ()
    {
      _dataKeeperMock.Expect (mock => mock.HasDataChanged ()).Return (true);
      _dataKeeperMock.Replay ();

      var result = _loadState.HasChanged ();

      _dataKeeperMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Commit ()
    {
      _dataKeeperMock.Expect (mock => mock.Commit());
      _dataKeeperMock.Replay();

      _loadState.Commit();

      _dataKeeperMock.VerifyAllExpectations();
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var dataKeeper = new SerializableCollectionEndPointDataKeeperFake ();
      var endPointProvider = new SerializableRelationEndPointProviderFake();
      var state = new CompleteCollectionEndPointLoadState (dataKeeper, endPointProvider, _clientTransaction);

      var oppositeEndPoint = new SerializableObjectEndPointFake (null, _relatedObject);
      PrivateInvoke.SetNonPublicField (state, "_unsynchronizedOppositeEndPoints", new List<IObjectEndPoint> (new[] { oppositeEndPoint }));

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.DataKeeper, Is.Not.Null);
      Assert.That (result.ClientTransaction, Is.Not.Null);
      Assert.That (result.EndPointProvider, Is.Not.Null);
      Assert.That (result.GetUnsynchronizedOppositeEndPoints().Count, Is.EqualTo (1));

    }
  }
}