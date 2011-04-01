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

    private IRelationEndPointLazyLoader _lazyLoaderStub;
    private IRelationEndPointProvider _endPointProviderStub;
    private IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> _dataKeeperFactory;
    private IVirtualObjectEndPointLoadState _loadStateMock;
    
    private VirtualObjectEndPoint _endPoint;

    private IRealObjectEndPoint _oppositeEndPointStub;
    private OrderTicket _oppositeObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
    
      _lazyLoaderStub = MockRepository.GenerateStub<IRelationEndPointLazyLoader> ();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      _dataKeeperFactory = new VirtualObjectEndPointDataKeeperFactory (ClientTransactionMock);
      _loadStateMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPointLoadState> ();

      _endPoint = new VirtualObjectEndPoint (
          ClientTransaction.Current, 
          _endPointID, 
          _lazyLoaderStub, 
          _endPointProviderStub,
          _dataKeeperFactory);
      PrivateInvoke.SetNonPublicField (_endPoint, "_loadState", _loadStateMock);

      _oppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
      _oppositeObject = DomainObjectMother.CreateFakeObject<OrderTicket>();
    }

    [Test]
    public void Initialization ()
    {
      var endPoint = new VirtualObjectEndPoint (
          ClientTransaction.Current,
          _endPointID,
          _lazyLoaderStub,
          _endPointProviderStub,
          _dataKeeperFactory);

      Assert.That (endPoint.ID, Is.EqualTo (_endPointID));
      Assert.That (endPoint.ClientTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (endPoint.LazyLoader, Is.SameAs (_lazyLoaderStub));
      Assert.That (endPoint.EndPointProvider, Is.SameAs (_endPointProviderStub));
      Assert.That (endPoint.DataKeeperFactory, Is.SameAs (_dataKeeperFactory));
      Assert.That (endPoint.HasBeenTouched, Is.False);
      Assert.That (endPoint.IsDataComplete, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "End point ID must refer to a virtual end point.\r\nParameter name: id")]
    public void Initialization_NonVirtualDefinition ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      new VirtualObjectEndPoint (ClientTransactionMock, id, _lazyLoaderStub, _endPointProviderStub, _dataKeeperFactory);
    }

    [Test]
    public void OppositeObjectID ()
    {
      _loadStateMock.Expect (mock => mock.GetData (_endPoint)).Return (DomainObjectIDs.OrderTicket1);
      _loadStateMock.Replay();

      var result = _endPoint.OppositeObjectID;
      _loadStateMock.VerifyAllExpectations ();

      Assert.That (result, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void GetData ()
    {
      _loadStateMock.Expect (mock => mock.GetData (_endPoint)).Return (DomainObjectIDs.OrderTicket1);
      _loadStateMock.Replay ();

      var result = ((IVirtualEndPoint<ObjectID>) _endPoint).GetData ();
      Assert.That (result, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void OriginalOppositeObjectID ()
    {
      _loadStateMock.Expect (mock => mock.GetOriginalData (_endPoint)).Return (DomainObjectIDs.OrderTicket2);
      _loadStateMock.Replay ();

      var result = _endPoint.OriginalOppositeObjectID;
      _loadStateMock.VerifyAllExpectations ();

      Assert.That (result, Is.EqualTo (DomainObjectIDs.OrderTicket2));
    }

    [Test]
    public void GetOriginalData ()
    {
      _loadStateMock.Expect (mock => mock.GetOriginalData (_endPoint)).Return (DomainObjectIDs.OrderTicket2);
      _loadStateMock.Replay ();

      var result = ((IVirtualEndPoint<ObjectID>) _endPoint).GetOriginalData ();
      Assert.That (result, Is.EqualTo (DomainObjectIDs.OrderTicket2));
    }

    [Test]
    public void HasChanged ()
    {
      _loadStateMock.Expect (mock => mock.HasChanged()).Return (true);
      _loadStateMock.Replay ();

      var result = _endPoint.HasChanged;

      _loadStateMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
    }

    [Test]
    public void IsDataComplete ()
    {
      _loadStateMock.Expect (mock => mock.IsDataComplete()).Return (true);
      _loadStateMock.Replay ();

      var result = _endPoint.IsDataComplete;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
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
    public void EnsureDataComplete ()
    {
      _loadStateMock.Expect (mock => mock.EnsureDataComplete (_endPoint));
      _loadStateMock.Replay();

      _endPoint.EnsureDataComplete();

      _loadStateMock.VerifyAllExpectations();
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
      _loadStateMock.Expect (mock => mock.SynchronizeOppositeEndPoint (_oppositeEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.SynchronizeOppositeEndPoint (_oppositeEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete ()
    {
      Action<IVirtualObjectEndPointDataKeeper> stateSetter = null;

      _loadStateMock
          .Expect (mock => mock.MarkDataComplete (Arg.Is (_endPoint), Arg.Is (_oppositeObject), Arg<Action<IVirtualObjectEndPointDataKeeper>>.Is.Anything))
          .WhenCalled (mi => { stateSetter = (Action<IVirtualObjectEndPointDataKeeper>) mi.Arguments[2]; });
      _loadStateMock.Replay ();

      _endPoint.MarkDataComplete (_oppositeObject);

      _loadStateMock.VerifyAllExpectations ();

      Assert.That (VirtualObjectEndPointTestHelper.GetLoadState (_endPoint), Is.SameAs (_loadStateMock));

      var dataKeeperStub = MockRepository.GenerateStub<IVirtualObjectEndPointDataKeeper>();
      stateSetter (dataKeeperStub);
      
      var newLoadState = VirtualObjectEndPointTestHelper.GetLoadState (_endPoint);
      Assert.That (newLoadState, Is.Not.SameAs (_loadStateMock));
      Assert.That (newLoadState, Is.TypeOf (typeof (CompleteVirtualObjectEndPointLoadState)));

      Assert.That (((CompleteVirtualObjectEndPointLoadState) newLoadState).DataKeeper, Is.SameAs (dataKeeperStub));
      Assert.That (((CompleteVirtualObjectEndPointLoadState) newLoadState).ClientTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (((CompleteVirtualObjectEndPointLoadState) newLoadState).EndPointProvider, Is.SameAs (_endPointProviderStub));
    }

    [Test]
    public void MarkDataComplete_Null ()
    {
      _loadStateMock
          .Expect (
              mock => mock.MarkDataComplete (
                  Arg.Is (_endPoint),
                  Arg.Is ((DomainObject) null),
                  Arg<Action<IVirtualObjectEndPointDataKeeper>>.Is.Anything));
      _loadStateMock.Replay ();

      _endPoint.MarkDataComplete (null);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataIncomplete ()
    {
      Action<IVirtualObjectEndPointDataKeeper> stateSetter = null;

      _loadStateMock
          .Expect (mock => mock.MarkDataIncomplete (Arg.Is (_endPoint), Arg<Action<IVirtualObjectEndPointDataKeeper>>.Is.Anything))
          .WhenCalled (mi => { stateSetter = (Action<IVirtualObjectEndPointDataKeeper>) mi.Arguments[1]; });
      _loadStateMock.Replay ();

      _endPoint.MarkDataIncomplete ();

      _loadStateMock.VerifyAllExpectations ();

      Assert.That (VirtualObjectEndPointTestHelper.GetLoadState (_endPoint), Is.SameAs (_loadStateMock));

      var dataKeeperStub = MockRepository.GenerateStub<IVirtualObjectEndPointDataKeeper> ();
      stateSetter (dataKeeperStub);

      var newLoadState = VirtualObjectEndPointTestHelper.GetLoadState (_endPoint);
      Assert.That (newLoadState, Is.Not.SameAs (_loadStateMock));
      Assert.That (newLoadState, Is.TypeOf (typeof (IncompleteVirtualObjectEndPointLoadState)));

      Assert.That (((IncompleteVirtualObjectEndPointLoadState) newLoadState).DataKeeperFactory, Is.SameAs (_dataKeeperFactory));
      Assert.That (((IncompleteVirtualObjectEndPointLoadState) newLoadState).LazyLoader, Is.SameAs (_lazyLoaderStub));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (_endPoint, _oppositeEndPointStub));
      _loadStateMock.Replay();

      _endPoint.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      _loadStateMock.VerifyAllExpectations();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (_endPoint, _oppositeEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.UnregisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.RegisterCurrentOppositeEndPoint (_endPoint, _oppositeEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.UnregisterCurrentOppositeEndPoint (_endPoint, _oppositeEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateSetCommand ()
    {
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand>();
      _loadStateMock.Expect (mock => mock.CreateSetCommand (_endPoint, _oppositeObject)).Return (fakeCommand);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateSetCommand (_oppositeObject);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeCommand));
    }

    [Test]
    public void CreateSetCommand_Null ()
    {
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand> ();
      _loadStateMock.Expect (mock => mock.CreateSetCommand (_endPoint, null)).Return (fakeCommand);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateSetCommand (null);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeCommand));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand> ();
      _loadStateMock.Expect (mock => mock.CreateDeleteCommand (_endPoint)).Return (fakeCommand);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateDeleteCommand ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeCommand));
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
      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _loadStateMock.Expect (mock => mock.Commit());
      _loadStateMock.Replay ();
      
      _endPoint.Commit ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback ()
    {
      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _loadStateMock.Expect (mock => mock.Rollback ());
      _loadStateMock.Replay ();

      _endPoint.Rollback ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void SetOppositeObjectIDValueFrom ()
    {
      var oppositeEndPointStub = MockRepository.GenerateStub<IVirtualObjectEndPoint>();

      _loadStateMock.Expect (mock => mock.SetValueFrom (_endPoint, oppositeEndPointStub));
      _loadStateMock.Replay();

      PrivateInvoke.InvokeNonPublicMethod (_endPoint, "SetOppositeObjectIDValueFrom", oppositeEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }
  }
}