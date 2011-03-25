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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class VirtualObjectEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _endPointID;
    private Order _domainObject;

    private IRelationEndPointLazyLoader _lazyLoaderStub;
    private IRelationEndPointProvider _endPointProviderStub;
    private IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> _dataKeeperFactoryStub;
    
    private VirtualObjectEndPoint _endPoint;
    

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _domainObject = Order.GetObject (_endPointID.ObjectID);
    
      _lazyLoaderStub = MockRepository.GenerateStub<IRelationEndPointLazyLoader> ();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      _dataKeeperFactoryStub = MockRepository.GenerateStub<IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper>>();

      _endPoint = new VirtualObjectEndPoint (
          ClientTransaction.Current, 
          _endPointID, 
          DomainObjectIDs.OrderTicket1, 
          _lazyLoaderStub, 
          _endPointProviderStub,
          _dataKeeperFactoryStub);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "End point ID must refer to a virtual end point.\r\nParameter name: id")]
    public void Initialize_NonVirtualDefinition ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      new VirtualObjectEndPoint (ClientTransactionMock, id, null, _lazyLoaderStub, _endPointProviderStub, _dataKeeperFactoryStub);
    }

    [Test]
    public void InitializeWithNullObjectID ()
    {
      var endPoint = new VirtualObjectEndPoint (
          ClientTransaction.Current,
          _endPointID,
          null,
          _lazyLoaderStub,
          _endPointProviderStub,
          _dataKeeperFactoryStub);

      Assert.That (endPoint.OriginalOppositeObjectID, Is.Null);
      Assert.That (endPoint.OppositeObjectID, Is.Null);
    }

    [Test]
    public void OppositeObjectID_Get ()
    {
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void OppositeObjectID_Set ()
    {
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket2));
      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.OrderTicket2);
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket2));
    }

    [Test]
    public void OppositeObjectID_Set_TouchesEndPoint ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);
      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, _endPoint.OppositeObjectID);
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void OppositeObjectID_Set_RaisesStateNotification_Changed ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_endPoint.ClientTransaction);

      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.OrderTicket2);

      listenerMock.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_endPoint.ClientTransaction, _endPoint.ID, true));
    }

    [Test]
    public void OppositeObjectID_Set_RaisesStateNotification_Unchanged ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_endPoint.ClientTransaction);

      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, _endPoint.OppositeObjectID);

      listenerMock.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_endPoint.ClientTransaction, _endPoint.ID, false));
    }

    [Test]
    public void GetData ()
    {
      Assert.That (((IVirtualEndPoint<ObjectID>) _endPoint).GetData(), Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void GetOriginalData ()
    {
      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.OrderTicket2);

      Assert.That (((IVirtualEndPoint<ObjectID>) _endPoint).GetOriginalData (), Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void HasChanged ()
    {
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket2));
      Assert.That (_endPoint.HasChanged, Is.False);

      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.OrderTicket2);

      Assert.That (_endPoint.HasChanged, Is.True);
    }
    
    [Test]
    public void HasChanged_WithOriginalAndCurrentNull ()
    {
      var endPoint = new VirtualObjectEndPoint (
          ClientTransaction.Current,
          _endPointID,
          null,
          _lazyLoaderStub,
          _endPointProviderStub,
          _dataKeeperFactoryStub);

      Assert.That (endPoint.HasChanged, Is.False);
    }

    [Test]
    public void HasChanged_WithOriginalNull_CurrentNotNull ()
    {
      var endPoint = new VirtualObjectEndPoint (
          ClientTransaction.Current,
          _endPointID,
          null,
          _lazyLoaderStub,
          _endPointProviderStub,
          _dataKeeperFactoryStub);
      ObjectEndPointTestHelper.SetOppositeObjectID (endPoint, new ObjectID ("Order", Guid.NewGuid ()));

      Assert.That (endPoint.HasChanged, Is.True);
    }

    [Test]
    public void HasChangedWith_OriginalNonNull_CurrentNotNull ()
    {
      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, null);

      Assert.That (_endPoint.HasChanged, Is.True);
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.That (_endPoint.IsSynchronized, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "In the current implementation, ObjectEndPoints in a 1:1 relation should always be in-sync with each other.")]
    public void SynchronizeOppositeEndPoint ()
    {
      _endPoint.SynchronizeOppositeEndPoint (MockRepository.GenerateStub<IRealObjectEndPoint> ());
    }

    [Test]
    [Ignore ("TODO 3818")]
    public void MarkDataComplete ()
    {
      Assert.Fail ("TODO");
    }

    [Test]
    [Ignore ("TODO 3818")]
    public void MarkDataComplete_Null ()
    {
      Assert.Fail ("TODO");
    }

    [Test]
    public void CreateSetCommand_Same ()
    {
      var relatedObject = OrderTicket.GetObject(DomainObjectIDs.OrderTicket1);

      var command = (RelationEndPointModificationCommand) _endPoint.CreateSetCommand(relatedObject);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_domainObject));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPoint));
      Assert.That (command.OldRelatedObject, Is.SameAs (relatedObject));
      Assert.That (command.NewRelatedObject, Is.SameAs (relatedObject));
      CheckOppositeObjectIDSetter (command, _endPoint);
    }

    [Test]
    public void CreateSetCommand_Same_Null ()
    {
      var endPoint = new VirtualObjectEndPoint (
          ClientTransaction.Current,
          _endPointID,
          null,
          _lazyLoaderStub,
          _endPointProviderStub,
          _dataKeeperFactoryStub);

     var command = (RelationEndPointModificationCommand) endPoint.CreateSetCommand (null);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_domainObject));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (endPoint));
      Assert.That (command.OldRelatedObject, Is.Null);
      Assert.That (command.NewRelatedObject, Is.Null);
      CheckOppositeObjectIDSetter (command, endPoint);
    }

    [Test]
    public void CreateSetCommand_OneOne ()
    {
      var newRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      var command = (RelationEndPointModificationCommand) _endPoint.CreateSetCommand (newRelatedObject);

      Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetOneOneCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPoint));
      Assert.That (command.NewRelatedObject, Is.SameAs (newRelatedObject));
      Assert.That (command.OldRelatedObject, Is.SameAs (OrderTicket.GetObject(DomainObjectIDs.OrderTicket1)));
      CheckOppositeObjectIDSetter (command, _endPoint);
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var command = (RelationEndPointModificationCommand) _endPoint.CreateDeleteCommand ();

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointDeleteCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_domainObject));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPoint));
      
      CheckOppositeObjectIDSetter(command, _endPoint);
    }

    [Test]
    public void Touch ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.Touch ();
      
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Commit ()
    {
      Assert.That (_endPoint.OriginalOppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket2));
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket2));
      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.OrderTicket2);

      _endPoint.Commit ();

      Assert.That (_endPoint.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket2));
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket2));
    }

    [Test]
    public void Commit_ClearsTouchedFlag ()
    {
      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _endPoint.Commit ();

      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Commit_RaisesStateNotification ()
    {
      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.OrderTicket2);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_endPoint.ClientTransaction);

      _endPoint.Commit ();

      listenerMock.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_endPoint.ClientTransaction, _endPoint.ID, false));
    }

    [Test]
    public void Rollback ()
    {
      Assert.That (_endPoint.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.OrderTicket2);
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket1));

      _endPoint.Rollback ();

      Assert.That (_endPoint.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void Rollback_ClearsTouchedFlag ()
    {
      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _endPoint.Rollback ();

      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback_RaisesStateNotification ()
    {
      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.OrderTicket2);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_endPoint.ClientTransaction);

      _endPoint.Rollback ();

      listenerMock.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_endPoint.ClientTransaction, _endPoint.ID, false));
    }

    [Test]
    public void SetOppositeObjectIDValueFrom ()
    {
      var sourceID = RelationEndPointID.Create (DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, DomainObjectIDs.Order2);
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));

      PrivateInvoke.InvokeNonPublicMethod (_endPoint, "SetOppositeObjectIDValueFrom", source);

      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
      Assert.That (_endPoint.HasChanged, Is.True);
    }

    private void CheckOppositeObjectIDSetter (RelationEndPointModificationCommand command, VirtualObjectEndPoint endPoint)
    {
      var oppositeObjectIDSetter = GetOppositeObjectIDSetter (command);

      Assert.That (endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket3));
      oppositeObjectIDSetter (DomainObjectIDs.OrderTicket3);
      Assert.That (endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket3));
    }

    private Action<ObjectID> GetOppositeObjectIDSetter (RelationEndPointModificationCommand command)
    {
      return (Action<ObjectID>) PrivateInvoke.GetNonPublicField (command, "_oppositeObjectIDSetter");
    }
  }
}