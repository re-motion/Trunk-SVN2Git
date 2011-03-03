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
using Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.ObjectEndPointDataManagement
{
  [TestFixture]
  public class VirtualObjectEndPointTest : ClientTransactionBaseTest
  {
    private VirtualObjectEndPoint _endPoint;
    private IRelationEndPointLazyLoader _lazyLoaderStub;
    private RelationEndPointID _endPointID;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _lazyLoaderStub = MockRepository.GenerateStub<IRelationEndPointLazyLoader> ();
      _endPoint = new VirtualObjectEndPoint (ClientTransaction.Current, _endPointID, DomainObjectIDs.OrderTicket1, _lazyLoaderStub);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "End point ID must refer to a virtual end point.\r\nParameter name: id")]
    public void Initialize_NonVirtualDefinition ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      new VirtualObjectEndPoint (ClientTransactionMock, id, null, _lazyLoaderStub);
    }

    [Test]
    public void InitializeWithNullObjectID ()
    {
      var endPoint = RelationEndPointObjectMother.CreateVirtualObjectEndPoint (_endPointID, null);

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
      var endPoint = RelationEndPointObjectMother.CreateVirtualObjectEndPoint (_endPointID, null);

      Assert.That (endPoint.HasChanged, Is.False);
    }

    [Test]
    public void HasChanged_WithOriginalNull_CurrentNotNull ()
    {
      var endPoint = RelationEndPointObjectMother.CreateVirtualObjectEndPoint (_endPointID, null);
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
  }
}