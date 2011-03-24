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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class CompleteVirtualObjectEndPointLoadStateTest : StandardMappingTest
  {
    private IVirtualObjectEndPoint _virtualObjectEndPointMock;
    private IVirtualObjectEndPointDataKeeper _dataKeeperMock;
    private IRelationEndPointProvider _endPointProviderStub;
    private ClientTransaction _clientTransaction;

    private CompleteVirtualObjectEndPointLoadState _loadState;

    private IRelationEndPointDefinition _definition;
    private OrderTicket _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;
    private Order _owningObject;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _definition = Configuration.ClassDefinitions[typeof (Order)].GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");

      _virtualObjectEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      _dataKeeperMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPointDataKeeper> ();
      _dataKeeperMock.Stub (stub => stub.EndPointID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, _definition));
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      _clientTransaction = ClientTransaction.CreateRootTransaction ();

      _loadState = new CompleteVirtualObjectEndPointLoadState (_dataKeeperMock, _endPointProviderStub, _clientTransaction);

      _relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1);
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_relatedObject);
      _relatedEndPointStub.Stub (stub => stub.ObjectID).Return (_relatedObject.ID);
      _owningObject = DomainObjectMother.CreateFakeObject<Order> ();
    }

    [Test]
    public void GetData ()
    {
      _dataKeeperMock.Stub (stub => stub.CurrentOppositeObjectID).Return (_relatedObject.ID);
      _dataKeeperMock.Replay();

      var result = _loadState.GetData (_virtualObjectEndPointMock);

      Assert.That (result, Is.EqualTo (_relatedObject.ID));
    }

    [Test]
    public void GetOriginalData ()
    {
      _dataKeeperMock.Stub (stub => stub.OriginalOppositeObjectID).Return (_relatedObject.ID);
      _dataKeeperMock.Replay ();

      var result = _loadState.GetOriginalData (_virtualObjectEndPointMock);

      Assert.That (result, Is.EqualTo (_relatedObject.ID));
    }

    [Test]
    public void SetValueFrom ()
    {
      var sourceEndPointStub = MockRepository.GenerateStub<IVirtualObjectEndPoint>();
      sourceEndPointStub.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.OrderTicket3);

      _dataKeeperMock.Expect (mock => mock.CurrentOppositeObjectID = DomainObjectIDs.OrderTicket3);
      _dataKeeperMock.Replay();

      _loadState.SetValueFrom (_virtualObjectEndPointMock, sourceEndPointStub);

      _dataKeeperMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateSetCommand_Same ()
    {
      _dataKeeperMock.Stub (stub => stub.CurrentOppositeObjectID).Return (_relatedObject.ID);
      _dataKeeperMock.Replay();
      
      _virtualObjectEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);
      _virtualObjectEndPointMock.Stub (mock => mock.GetOppositeObject (true)).Return (_relatedObject);
      _virtualObjectEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _virtualObjectEndPointMock.Replay ();

      Action<ObjectID> fakeObjectIDSetter = id => { };

      var command = (RelationEndPointModificationCommand) _loadState.CreateSetCommand (_virtualObjectEndPointMock, _relatedObject, fakeObjectIDSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_owningObject));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_virtualObjectEndPointMock));
      Assert.That (command.OldRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (command.NewRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (fakeObjectIDSetter));
    }

    [Test]
    public void CreateSetCommand_Same_Null ()
    {
      _dataKeeperMock.Stub (stub => stub.CurrentOppositeObjectID).Return (null);
      _dataKeeperMock.Replay ();

      _virtualObjectEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);
      _virtualObjectEndPointMock.Stub (mock => mock.GetOppositeObject (true)).Return (null);
      _virtualObjectEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _virtualObjectEndPointMock.Replay ();

      Action<ObjectID> fakeObjectIDSetter = id => { };

      var command = (RelationEndPointModificationCommand) _loadState.CreateSetCommand (_virtualObjectEndPointMock, null, fakeObjectIDSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_owningObject));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_virtualObjectEndPointMock));
      Assert.That (command.OldRelatedObject, Is.Null);
      Assert.That (command.NewRelatedObject, Is.Null);
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (fakeObjectIDSetter));
    }

    [Test]
    public void CreateSetCommand_OneOne ()
    {
      _dataKeeperMock.Stub (stub => stub.CurrentOppositeObjectID).Return (_relatedObject.ID);
      _dataKeeperMock.Replay ();

      _virtualObjectEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);
      _virtualObjectEndPointMock.Stub (mock => mock.GetOppositeObject (true)).Return (_relatedObject);
      _virtualObjectEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _virtualObjectEndPointMock.Stub (mock => mock.Definition).Return (_definition);
      _virtualObjectEndPointMock.Replay ();

      Action<ObjectID> fakeObjectIDSetter = id => { };

      var newRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      var command = (RelationEndPointModificationCommand) _loadState.CreateSetCommand (_virtualObjectEndPointMock, newRelatedObject, fakeObjectIDSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetOneOneCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_virtualObjectEndPointMock));
      Assert.That (command.NewRelatedObject, Is.SameAs (newRelatedObject));
      Assert.That (command.OldRelatedObject, Is.SameAs (_relatedObject));
      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (fakeObjectIDSetter));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      _virtualObjectEndPointMock.Stub (mock => mock.GetDomainObject ()).Return (_owningObject);
      _virtualObjectEndPointMock.Stub (mock => mock.IsNull).Return (false);
      _virtualObjectEndPointMock.Replay ();

      Action<ObjectID> fakeObjectIDSetter = id => { };

      var command = (RelationEndPointModificationCommand) _loadState.CreateDeleteCommand (_virtualObjectEndPointMock, fakeObjectIDSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointDeleteCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_owningObject));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_virtualObjectEndPointMock));

      Assert.That (GetOppositeObjectIDSetter (command), Is.SameAs (fakeObjectIDSetter));
    }

    [Test]
    public void GetOriginalOppositeEndPoints ()
    {
      _dataKeeperMock.Stub (mock => mock.OriginalOppositeEndPoint).Return (_relatedEndPointStub);
      _dataKeeperMock.Replay ();

      var result = (IEnumerable<IRealObjectEndPoint>) PrivateInvoke.InvokeNonPublicMethod (_loadState, "GetOriginalOppositeEndPoints");

      Assert.That (result, Is.EqualTo (new[] { _relatedEndPointStub }));
    }

    [Test]
    public void GetOriginalOppositeEndPoints_None ()
    {
      _dataKeeperMock.Stub (mock => mock.OriginalOppositeEndPoint).Return (null);
      _dataKeeperMock.Replay ();

      var result = (IEnumerable<IRealObjectEndPoint>) PrivateInvoke.InvokeNonPublicMethod (_loadState, "GetOriginalOppositeEndPoints");

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetOriginalItemsWithoutEndPoints ()
    {
      _dataKeeperMock.Stub (mock => mock.OriginalItemWithoutEndPoint).Return (_relatedObject);
      _dataKeeperMock.Replay ();

      var result = (IEnumerable<DomainObject>) PrivateInvoke.InvokeNonPublicMethod (_loadState, "GetOriginalItemsWithoutEndPoints");

      Assert.That (result, Is.EqualTo (new[] { _relatedObject }));
    }

    [Test]
    public void GetOriginalItemsWithoutEndPoints_None ()
    {
      _dataKeeperMock.Stub (mock => mock.OriginalItemWithoutEndPoint).Return (null);
      _dataKeeperMock.Replay ();

      var result = (IEnumerable<DomainObject>) PrivateInvoke.InvokeNonPublicMethod (_loadState, "GetOriginalItemsWithoutEndPoints");

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void HasUnsynchronizedCurrentOppositeEndPoints_False_NoEndPoint ()
    {
      _dataKeeperMock.Stub (stub => stub.CurrentOppositeEndPoint).Return (null);
      _dataKeeperMock.Replay ();

      var result = (bool) PrivateInvoke.InvokeNonPublicMethod (_loadState, "HasUnsynchronizedCurrentOppositeEndPoints");

      Assert.That (result, Is.False);
    }

    [Test]
    public void HasUnsynchronizedCurrentOppositeEndPoints_False_OnlySynchronizedEndPoints ()
    {
      _relatedEndPointStub.Stub (stub => stub.IsSynchronized).Return (true);

      _dataKeeperMock.Stub (stub => stub.CurrentOppositeEndPoint).Return (_relatedEndPointStub);
      _dataKeeperMock.Replay ();

      var result = (bool) PrivateInvoke.InvokeNonPublicMethod (_loadState, "HasUnsynchronizedCurrentOppositeEndPoints");

      Assert.That (result, Is.False);
    }

    [Test]
    public void HasUnsynchronizedCurrentOppositeEndPoints_True ()
    {
      _relatedEndPointStub.Stub (stub => stub.IsSynchronized).Return (false);

      _dataKeeperMock.Stub (stub => stub.CurrentOppositeEndPoint).Return (_relatedEndPointStub);
      _dataKeeperMock.Replay ();

      var result = (bool) PrivateInvoke.InvokeNonPublicMethod (_loadState, "HasUnsynchronizedCurrentOppositeEndPoints");

      Assert.That (result, Is.True);
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var dataKeeper = new SerializableVirtualObjectEndPointDataKeeperFake ();
      var endPointProvider = new SerializableRelationEndPointProviderFake ();
      var state = new CompleteVirtualObjectEndPointLoadState (dataKeeper, endPointProvider, _clientTransaction);

      var oppositeEndPoint = new SerializableRealObjectEndPointFake (null, _relatedObject);
      AddUnsynchronizedOppositeEndPoint (state, oppositeEndPoint);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.DataKeeper, Is.Not.Null);
      Assert.That (result.ClientTransaction, Is.Not.Null);
      Assert.That (result.EndPointProvider, Is.Not.Null);
      Assert.That (result.UnsynchronizedOppositeEndPoints.Count, Is.EqualTo (1));
    }

    private Action<ObjectID> GetOppositeObjectIDSetter (RelationEndPointModificationCommand command)
    {
      return (Action<ObjectID>) PrivateInvoke.GetNonPublicField (command, "_oppositeObjectIDSetter");
    }

    private void AddUnsynchronizedOppositeEndPoint (CompleteVirtualObjectEndPointLoadState loadState, IRealObjectEndPoint oppositeEndPoint)
    {
      var dictionary = (Dictionary<ObjectID, IRealObjectEndPoint>) PrivateInvoke.GetNonPublicField (loadState, "_unsynchronizedOppositeEndPoints");
      dictionary.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
    }
  }
}