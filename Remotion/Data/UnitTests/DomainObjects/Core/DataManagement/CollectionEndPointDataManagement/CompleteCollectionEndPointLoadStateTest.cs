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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement.SerializableFakes;
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
    private CompleteCollectionEndPointLoadState _loadState;

    private IRelationEndPointDefinition _definition;
    private Order _relatedObject;
    private Customer _owningObject;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      _dataKeeperMock = MockRepository.GenerateStrictMock<ICollectionEndPointDataKeeper>();
      _loadState = new CompleteCollectionEndPointLoadState (_collectionEndPointMock, _dataKeeperMock);

      _definition = Configuration.ClassDefinitions[typeof (Customer)].GetRelationEndPointDefinition (typeof (Customer).FullName + ".Orders");
      _relatedObject = DomainObjectMother.CreateFakeObject<Order>();
      _owningObject = DomainObjectMother.CreateFakeObject<Customer>();
    }

    [Test]
    public void EnsureDataComplete_DoesNothing ()
    {
      _collectionEndPointMock.Replay();
      _dataKeeperMock.Replay();

      _loadState.EnsureDataComplete();

      _collectionEndPointMock.VerifyAllExpectations();
      _dataKeeperMock.VerifyAllExpectations();
    }

    [Test]
    public void GetCollectionWithOriginalData_CreatesNewCollectionFromData ()
    {
      var collectionDataStub = MockRepository.GenerateStub<IDomainObjectCollectionData>();
      collectionDataStub.Stub (stub => stub.RequiredItemType).Return (typeof (Order));

      _collectionEndPointMock.Stub (stub => stub.Definition).Return (_definition);
      _dataKeeperMock.Stub (stub => stub.OriginalCollectionData).Return (collectionDataStub);

      var result = _loadState.GetCollectionWithOriginalData();

      Assert.That (result, Is.TypeOf (typeof (OrderCollection)));
      var actualCollectionData = DomainObjectCollectionDataTestHelper.GetDataStrategyAndCheckType<IDomainObjectCollectionData> (result);
      Assert.That (actualCollectionData, Is.SameAs (collectionDataStub));
    }

    [Test]
    public void GetOppositeRelationEndPoints ()
    {
      var relatedObject1 = DomainObjectMother.CreateFakeObject<Order> ();
      var relatedObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      var collectionData = new DomainObjectCollectionData (new[] { relatedObject1, relatedObject2 });

      _collectionEndPointMock.Stub (stub => stub.Definition).Return (_definition);
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (collectionData);

      var relationEndPointMapStub = MockRepository.GenerateStub<IRelationEndPointMapReadOnlyView>();

      var oppositeEndPointID1 = new RelationEndPointID (relatedObject1.ID, typeof (Order).FullName + ".Customer");
      var oppositeEndPointID2 = new RelationEndPointID (relatedObject2.ID, typeof (Order).FullName + ".Customer");

      var fakeEndPoint1 = MockRepository.GenerateStub<IRelationEndPoint> ();
      var fakeEndPoint2 = MockRepository.GenerateStub<IRelationEndPoint> ();

      relationEndPointMapStub.Stub (stub => stub.GetRelationEndPointWithLazyLoad (oppositeEndPointID1)).Return (fakeEndPoint1);
      relationEndPointMapStub.Stub (stub => stub.GetRelationEndPointWithLazyLoad (oppositeEndPointID2)).Return (fakeEndPoint2);

      var dataManagerStub = MockRepository.GenerateStub<IDataManager>();
      dataManagerStub.Stub (stub => stub.RelationEndPointMap).Return (relationEndPointMapStub);
      
      var oppositeEndPoints = _loadState.GetOppositeRelationEndPoints (dataManagerStub).ToArray ();

      Assert.That (oppositeEndPoints, Is.EqualTo (new[] { fakeEndPoint1, fakeEndPoint2 }));
    }

    [Test]
    public void CreateSetOppositeCollectionCommand ()
    {
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand>();

      var newOppositeCollectionMock = MockRepository.GenerateMock<IAssociatableDomainObjectCollection>();
      newOppositeCollectionMock.Expect (mock => mock.CreateAssociationCommand (_collectionEndPointMock)).Return (fakeCommand);
      newOppositeCollectionMock.Replay();

      var result = _loadState.CreateSetOppositeCollectionCommand (newOppositeCollectionMock);

      Assert.That (result, Is.SameAs (fakeCommand));
    }

    [Test]
    public void SetValueFrom_ReplacesCollectionData ()
    {
      var sourceItem1 = DomainObjectMother.CreateFakeObject<Order>();
      var sourceItem2 = DomainObjectMother.CreateFakeObject<Order>();

      var sourceCollection = new DomainObjectCollection (new DomainObject[] { sourceItem1, sourceItem2 }, null);
      var sourceEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
      sourceEndPointStub.Collection = sourceCollection;

      var targetItem1 = DomainObjectMother.CreateFakeObject<Order>();
      var targetCollectionData = new DomainObjectCollectionData (new DomainObject[] { targetItem1 });
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (targetCollectionData);
      _collectionEndPointMock.Stub (stub => stub.HasChanged).Return (false);

      _loadState.SetValueFrom (sourceEndPointStub);

      Assert.That (targetCollectionData.ToArray(), Is.EqualTo (new[] { sourceItem1, sourceItem2 }));
    }

    [Test]
    public void SetValueFrom_TouchesEndPoint_WhenSourceHasBeenTouched ()
    {
      var sourceCollection = new DomainObjectCollection();
      var sourceEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
      sourceEndPointStub.Collection = sourceCollection;
      sourceEndPointStub.Stub (stub => stub.HasBeenTouched).Return (true);

      _collectionEndPointMock.Stub (stub => stub.HasChanged).Return (false);
      _collectionEndPointMock.Expect (mock => mock.Touch());
      _collectionEndPointMock.Replay();

      var targetCollectionData = new DomainObjectCollectionData();
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (targetCollectionData);

      _loadState.SetValueFrom (sourceEndPointStub);

      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void SetValueFrom_TouchesEndPoint_WhenTargetHasChanged ()
    {
      var sourceCollection = new DomainObjectCollection();
      var sourceEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
      sourceEndPointStub.Collection = sourceCollection;
      sourceEndPointStub.Stub (stub => stub.HasBeenTouched).Return (false);

      _collectionEndPointMock.Stub (stub => stub.HasChanged).Return (true);
      _collectionEndPointMock.Expect (mock => mock.Touch());
      _collectionEndPointMock.Replay();

      var targetCollectionData = new DomainObjectCollectionData();
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (targetCollectionData);

      _loadState.SetValueFrom (sourceEndPointStub);

      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void SetValueFrom_DoesNotTouchEndPoint_WhenSourceUntouched_AndTargetUnchanged ()
    {
      var sourceCollection = new DomainObjectCollection();
      var sourceEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
      sourceEndPointStub.Collection = sourceCollection;
      sourceEndPointStub.Stub (stub => stub.HasBeenTouched).Return (false);

      _collectionEndPointMock.Stub (stub => stub.HasChanged).Return (false);
      _collectionEndPointMock.Replay();

      var targetCollectionData = new DomainObjectCollectionData();
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (targetCollectionData);

      _loadState.SetValueFrom (sourceEndPointStub);

      _collectionEndPointMock.AssertWasNotCalled (mock => mock.Touch());
      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void CheckMandatory_WithItems_Succeeds ()
    {
      _dataKeeperMock
          .Stub (stub => stub.CollectionData)
          .Return (new DomainObjectCollectionData (new[] { DomainObjectMother.CreateFakeObject<Order>() }));

      _loadState.CheckMandatory();
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

      _loadState.CheckMandatory();
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var fakeCollectionData = new DomainObjectCollectionData();
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);

      var fakeCollection = new DomainObjectCollection();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateRemoveCommand (_relatedObject);
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
      var fakeCollectionData = new DomainObjectCollectionData();
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);

      var fakeCollection = new DomainObjectCollection();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateDeleteCommand();
      Assert.That (command, Is.InstanceOfType (typeof (CollectionEndPointDeleteCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));

      Assert.That (((CollectionEndPointDeleteCommand) command).ModifiedCollectionData, Is.SameAs (fakeCollectionData));
      Assert.That (((CollectionEndPointDeleteCommand) command).ModifiedCollection, Is.SameAs (fakeCollection));
    }

    [Test]
    public void CreateInsertCommand ()
    {
      var fakeCollectionData = new DomainObjectCollectionData();
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);

      var fakeCollection = new DomainObjectCollection();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateInsertCommand (_relatedObject, 12);
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
          new DomainObjectCollectionData (new[] { DomainObjectMother.CreateFakeObject<Order>(), DomainObjectMother.CreateFakeObject<Order>() });
      _dataKeeperMock.Stub (stub => stub.CollectionData).Return (fakeCollectionData);

      var fakeCollection = new DomainObjectCollection();
      _collectionEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _collectionEndPointMock.Stub (mock => mock.Collection).Return (fakeCollection);
      _collectionEndPointMock.Stub (mock => mock.GetDomainObject()).Return (_owningObject);

      var command = (RelationEndPointModificationCommand) _loadState.CreateAddCommand (_relatedObject);
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

      var command = (RelationEndPointModificationCommand) _loadState.CreateReplaceCommand (0, _relatedObject);
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

      var command = (RelationEndPointModificationCommand) _loadState.CreateReplaceCommand (0, _relatedObject);
      Assert.That (command, Is.InstanceOfType (typeof (CollectionEndPointReplaceSameCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_collectionEndPointMock));
      Assert.That (command.OldRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (command.NewRelatedObject, Is.SameAs (_relatedObject));
    }

    [Test]
    public void Serializable ()
    {
      var endPoint = new SerializableCollectionEndPointFake();
      var dataKeeper = new SerializableCollectionEndPointDataKeeperFake();
      var state = new CompleteCollectionEndPointLoadState (endPoint, dataKeeper);

      var result = Serializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.CollectionEndPoint, Is.Null);
    }
  }
}