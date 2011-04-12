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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RealObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class RealObjectEndPointTest : ClientTransactionBaseTest
  {
    private DataContainer _foreignKeyDataContainer;
    private IRelationEndPointLazyLoader _lazyLoaderStub;
    private IRelationEndPointProvider _endPointProviderStub;
    private IRealObjectEndPointSyncState _syncStateMock;

    private RealObjectEndPoint _endPoint;
    private RelationEndPointID _endPointID;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      _foreignKeyDataContainer = DataContainer.CreateForExisting (_endPointID.ObjectID, null, pd => pd.DefaultValue);
      _lazyLoaderStub = MockRepository.GenerateStub<IRelationEndPointLazyLoader>();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();
      _syncStateMock = MockRepository.GenerateStrictMock<IRealObjectEndPointSyncState> ();
    
      _endPoint = new RealObjectEndPoint (ClientTransactionMock, _endPointID, _foreignKeyDataContainer, _lazyLoaderStub, _endPointProviderStub);
      PrivateInvoke.SetNonPublicField (_endPoint, "_syncState", _syncStateMock);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "End point ID must refer to a non-virtual end point.\r\nParameter name: id")]
    public void Initialize_VirtualDefinition ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var foreignKeyDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      new RealObjectEndPoint (ClientTransactionMock, id, foreignKeyDataContainer, _lazyLoaderStub, _endPointProviderStub);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The foreign key data container must be compatible with the end point definition.\r\nParameter name: foreignKeyDataContainer")]
    public void Initialize_InvalidDataContainer ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      new RealObjectEndPoint (ClientTransactionMock, id, foreignKeyDataContainer, _lazyLoaderStub, _endPointProviderStub);
    }

    [Test]
    public void Initialization_SyncState ()
    {
      var endPoint = new RealObjectEndPoint (ClientTransactionMock, _endPointID, _foreignKeyDataContainer, _lazyLoaderStub, _endPointProviderStub);

      var syncState = RealObjectEndPointTestHelper.GetSyncState (endPoint);
      Assert.That (syncState, Is.TypeOf (typeof (UnknownRealObjectEndPointSyncState)));
      Assert.That (((UnknownRealObjectEndPointSyncState) syncState).LazyLoader, Is.SameAs (_lazyLoaderStub));
    }

    [Test]
    public void IsDataComplete_True ()
    {
      Assert.That (_endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void IsSynchronized ()
    {
      _syncStateMock
          .Expect (mock => mock.IsSynchronized (_endPoint))
          .Return (true);
      _syncStateMock.Replay ();

      var result = _endPoint.IsSynchronized;

      _syncStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void OppositeObjectID_Get_FromProperty ()
    {
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));
      _endPoint.ForeignKeyProperty.Value = DomainObjectIDs.Order2;

      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void OppositeObjectID_Get_DoesNotRaisePropertyReadEvents ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      Dev.Null = _endPoint.OppositeObjectID;

      listenerMock.AssertWasNotCalled (mock => mock.PropertyValueReading (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<DataContainer>.Is.Anything, 
          Arg<PropertyValue>.Is.Anything, 
          Arg<ValueAccess>.Is.Anything));
      listenerMock.AssertWasNotCalled (mock => mock.PropertyValueRead (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<DataContainer>.Is.Anything,
          Arg<PropertyValue>.Is.Anything,
          Arg<object>.Is.Anything,
          Arg<ValueAccess>.Is.Anything));
    }

    [Test]
    public void OppositeObjectID_Set_ToProperty ()
    {
      Assert.That (_endPoint.ForeignKeyProperty.Value, Is.Not.EqualTo (DomainObjectIDs.Order2));

      RealObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.Order2);
      
      Assert.That (_endPoint.ForeignKeyProperty.Value, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void OriginalOppositeObjectID_Get_FromProperty ()
    {
      Assert.That (_endPoint.OriginalOppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));

      PrivateInvoke.SetNonPublicField (_endPoint.ForeignKeyProperty, "_originalValue", DomainObjectIDs.Order2);

      Assert.That (_endPoint.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void HasChanged_FromProperty ()
    {
      Assert.That (_endPoint.HasChanged, Is.False);

      _endPoint.ForeignKeyProperty.Value = DomainObjectIDs.Order2;

      Assert.That (_endPoint.HasChanged, Is.True);
    }

    [Test]
    public void HasBeenTouched_FromProperty ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.ForeignKeyProperty.Touch();

      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void EnsureDataComplete_DoesNothing ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      _endPoint.EnsureDataComplete ();
    }

    [Test]
    public void Synchronize ()
    {
      var oppositeEndPointStub = MockRepository.GenerateStub<IVirtualEndPoint> ();
      _endPointProviderStub
          .Stub (stub => stub.GetOppositeVirtualEndPointWithLazyLoad (_endPoint, _endPoint.OppositeObjectID))
          .Return (oppositeEndPointStub);

      _syncStateMock
          .Expect (mock => mock.Synchronize (_endPoint, oppositeEndPointStub));
      _syncStateMock.Replay ();

      _endPoint.Synchronize ();

      _syncStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkSynchronized ()
    {
      Assert.That (RealObjectEndPointTestHelper.GetSyncState (_endPoint), Is.SameAs (_syncStateMock));

      _endPoint.MarkSynchronized ();

      Assert.That (RealObjectEndPointTestHelper.GetSyncState (_endPoint), Is.TypeOf (typeof (SynchronizedRealObjectEndPointSyncState)));
      Assert.That (_endPoint.EndPointProvider, Is.SameAs (_endPointProviderStub));
    }

    [Test]
    public void MarkUnsynchronized ()
    {
      Assert.That (RealObjectEndPointTestHelper.GetSyncState (_endPoint), Is.SameAs (_syncStateMock));

      _endPoint.MarkUnsynchronized ();
      Assert.That (RealObjectEndPointTestHelper.GetSyncState (_endPoint), Is.TypeOf (typeof (UnsynchronizedRealObjectEndPointSyncState)));
    }

    [Test]
    public void ResetSyncState ()
    {
      Assert.That (RealObjectEndPointTestHelper.GetSyncState (_endPoint), Is.SameAs (_syncStateMock));

      _endPoint.ResetSyncState ();

      var syncState = RealObjectEndPointTestHelper.GetSyncState (_endPoint);
      Assert.That (syncState, Is.TypeOf (typeof (UnknownRealObjectEndPointSyncState)));
      Assert.That (((UnknownRealObjectEndPointSyncState) syncState).LazyLoader, Is.SameAs (_lazyLoaderStub));
    }

    [Test]
    public void CreateSetCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();
      var relatedObject = DomainObjectMother.CreateFakeObject<Order> ();

      Action<DomainObject> oppositeObjectSetter = null;

      _syncStateMock
          .Expect (mock => mock.CreateSetCommand (Arg.Is (_endPoint), Arg.Is (relatedObject), Arg<Action<DomainObject>>.Is.Anything))
          .Return (fakeResult)
          .WhenCalled (mi => { oppositeObjectSetter = (Action<DomainObject>) mi.Arguments[2]; });
      _syncStateMock.Replay ();

      var result = _endPoint.CreateSetCommand (relatedObject);

      _syncStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));

      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      oppositeObjectSetter (newRelatedObject);
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (newRelatedObject.ID));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      Action<DomainObject> oppositeObjectSetter = null;
      _syncStateMock
          .Expect (mock => mock.CreateDeleteCommand (Arg.Is (_endPoint), Arg<Action<DomainObject>>.Is.Anything))
          .Return (fakeResult)
          .WhenCalled (mi => { oppositeObjectSetter = (Action<DomainObject>) mi.Arguments[1]; });
      _syncStateMock.Replay ();

      var result = _endPoint.CreateDeleteCommand ();

      _syncStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));

      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Order>();
      oppositeObjectSetter (newRelatedObject);
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (newRelatedObject.ID));
    }

    [Test]
    public void Touch_ToProperty ()
    {
      Assert.That (_endPoint.ForeignKeyProperty.HasBeenTouched, Is.False);

      _endPoint.Touch ();

      Assert.That (_endPoint.ForeignKeyProperty.HasBeenTouched, Is.True);
    }

    [Test]
    public void Commit_ToProperty ()
    {
      Assert.That (_endPoint.ForeignKeyProperty.Value, Is.Null);

      _endPoint.ForeignKeyProperty.Value = DomainObjectIDs.Order2;
      Assert.That (_endPoint.ForeignKeyProperty.HasChanged, Is.True);

      _endPoint.Commit ();

      Assert.That (_endPoint.ForeignKeyProperty.HasChanged, Is.False);
      Assert.That (_endPoint.ForeignKeyProperty.Value, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void Rollback_ToProperty ()
    {
      Assert.That (_endPoint.ForeignKeyProperty.Value, Is.Null);

      _endPoint.ForeignKeyProperty.Value = DomainObjectIDs.Order2;
      Assert.That (_endPoint.ForeignKeyProperty.HasChanged, Is.True);

      _endPoint.Rollback ();

      Assert.That (_endPoint.ForeignKeyProperty.HasChanged, Is.False);
      Assert.That (_endPoint.ForeignKeyProperty.Value, Is.Null);
    }

    [Test]
    public void SetOppositeObjectValueFrom ()
    {
      var source = MockRepository.GenerateStub<IObjectEndPoint>();
      source.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.Order2);

      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));

      PrivateInvoke.InvokeNonPublicMethod (_endPoint, "SetOppositeObjectFrom", source);

      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
      Assert.That (_endPoint.HasChanged, Is.True);
    }
  }
}